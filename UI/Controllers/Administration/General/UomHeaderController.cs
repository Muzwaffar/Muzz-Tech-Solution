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
    public class UomHeaderController : NPFController
    {
        public UserManager<ApplicationUser> _userManager;
        public UomHeaderService service;
        public AccessRightService Aservice;
        public UomHeaderController(UserManager<ApplicationUser> userManager,
           SignInManager<ApplicationUser> signInManager, RoleManager<ApplicationRole> rolemanager) : base(userManager, signInManager, rolemanager)
        {
            service = new UomHeaderService();
            _userManager = userManager;
            Aservice = new AccessRightService();
        }
        public async Task<ActionResult> List()
        {
            BaseResponseDTO<string> dt = new BaseResponseDTO<string>();
            dt = await Aservice.GetViewTitle("Unit of Measure Header");
            ViewBag.Name = dt.Data;
            return View("~/Views/Modules/Administration/General/UomHeader/UomHeaderList.cshtml");
        }

        [HttpPost]
        public async Task<ActionResult<BaseResponseDTO<List<NPF_SYS_UomHeader>>>> GetList()
        {
            try
            {
                BaseResponseDTO<List<NPF_SYS_UomHeader>> List = new BaseResponseDTO<List<NPF_SYS_UomHeader>>();
                List = service.GetAll();
                return Ok(List);


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
        [HttpPost]
        public async Task<ActionResult> GetById([FromBody] BaseRequestDTO<long> dto)
        {
            try
            {
                if (dto.Data == 0)
                {
                    NPF_SYS_UomHeader CP = new NPF_SYS_UomHeader();
                    return PartialView("~/Views/Modules/Administration/General/UomHeader/UomHeaderAddEdit.cshtml", CP);
                }
                else
                {
                    BaseResponseDTO<NPF_SYS_UomHeader> CP = new BaseResponseDTO<NPF_SYS_UomHeader>();
                    CP = service.GetById(dto.Data);
                    return PartialView("~/Views/Modules/Administration/General/UomHeader/UomHeaderAddEdit.cshtml", CP.Data);
                }

            }
            catch (Exception ex)
            {

                return BadRequest();

            }
        }
        [HttpPost]
        public async Task<ActionResult<BaseResponseDTO<bool>>> CreateUpdate([FromBody] BaseRequestDTO<NPF_SYS_UomHeader> dto)
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


        //API Delete 
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

    }
}
