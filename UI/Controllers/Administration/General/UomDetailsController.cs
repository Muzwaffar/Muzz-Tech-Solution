using BL.Constants;
using BL.Models.Access;
using BL.Models.Common;
using BL.Services.Access.AccessRights;
using BL.Services.Administration;
using BL.Services.Common;
using DAL.Models;
using DAL.Models.Administration;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UI.Helper;

namespace UI.Controllers.Administration.General
{
    public class UomDetailsController : NPFController
    {
        public UserManager<ApplicationUser> _userManager;
        public UomDetailsService service;
        public AccessRightService Aservice;
        public UomDetailsController(UserManager<ApplicationUser> userManager,
           SignInManager<ApplicationUser> signInManager, RoleManager<ApplicationRole> rolemanager) : base(userManager, signInManager, rolemanager)
        {
            service = new UomDetailsService();
            _userManager = userManager;
            Aservice = new AccessRightService();
        }
        public async Task<ActionResult> List()
        {
            BaseResponseDTO<string> dt = new BaseResponseDTO<string>();
            dt = await Aservice.GetViewTitle("Unit of Measure Details");
            ViewBag.Name = dt.Data;
            return View("~/Views/Modules/Administration/General/UomDetails/UomDetailsList.cshtml");
        }

        [HttpPost]
        public async Task<ActionResult<BaseResponseDTO<List<NPF_SYS_UomDetails>>>> GetList()
        {
            try
            {
                BaseResponseDTO<List<NPF_SYS_UomDetails>> CusList = new BaseResponseDTO<List<NPF_SYS_UomDetails>>();
                CusList = service.GetAll();
                return Ok(CusList);


            }
            catch (Exception ex)
            {

                return BadRequest();

            }
        }
        [HttpPost]
        public async Task<ActionResult> GetById([FromBody] BaseRequestDTO<int> dto)
        {
            try
            {
                if (dto == null)
                {
                    NPF_SYS_UomDetails dt = new NPF_SYS_UomDetails();
                    return PartialView("~/Views/Modules/Administration/General/UomDetails/UomDetailsAddEdit.cshtml", dt);
                }
                else
                {
                    BaseResponseDTO<NPF_SYS_UomDetails> dt = new BaseResponseDTO<NPF_SYS_UomDetails>();
                    dt = service.GetById(dto.Data);
                    return PartialView("~/Views/Modules/Administration/General/UomDetails/UomDetailsAddEdit.cshtml", dt.Data);
                }

            }
            catch (Exception ex)
            {

                return BadRequest();

            }
        }
        [HttpPost]
        public async Task<ActionResult<BaseResponseDTO<bool>>> CreateUpdate([FromBody] BaseRequestDTO<NPF_SYS_UomDetails> dto)
        {
            try
            {
                BaseResponseDTO<bool> BaseResponseDTO = new BaseResponseDTO<bool>();
                if (dto.Data.Id == 0)
                {
                    BaseResponseDTO = await service.SaveAsync(dto);
                }
                else
                {
                    BaseResponseDTO = await service.UpdateAsync(dto);
                }

                return Ok(BaseResponseDTO);
            }
            catch (Exception ex)
            {

                return BadRequest();

            }
        }

        [HttpPost]
        public async Task<ActionResult<BaseResponseDTO<bool>>> Delete([FromBody] BaseRequestDTO<long> dto)
        {
            try
            {
                BaseResponseDTO<bool> BaseResponseDTO = new BaseResponseDTO<bool>();
                BaseResponseDTO = service.Delete(dto);
                return Ok(BaseResponseDTO);
            }
            catch (Exception ex)
            {

                return BadRequest();

            }
        }

        [HttpPost]
        public async Task<ActionResult<BaseResponseDTO<List<DropDown>>>> GetAllDropDownValues()
        {
            try
            {
                BaseResponseDTO<List<DropDown>> List = new BaseResponseDTO<List<DropDown>>();
                List = service.GetAllDropDownValues();
                return Ok(List);


            }
            catch (Exception ex)
            {

                return BadRequest();

            }
        }
    }
}
