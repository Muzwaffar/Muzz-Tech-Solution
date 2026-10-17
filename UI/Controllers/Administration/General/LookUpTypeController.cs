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
    public class LookUpTypeController : NPFController
    {
        public UserManager<ApplicationUser> _userManager;
        public LookUpTypeService service;
        public AccessRightService Aservice;
        public LookUpTypeController(UserManager<ApplicationUser> userManager,
           SignInManager<ApplicationUser> signInManager, RoleManager<ApplicationRole> rolemanager) : base(userManager, signInManager, rolemanager)
        {
            service = new LookUpTypeService();
            _userManager = userManager;
            Aservice = new AccessRightService();
        }
        public async Task<ActionResult> List()
        {
            BaseResponseDTO<string> dt = new BaseResponseDTO<string>();
            dt = await Aservice.GetViewTitle("LookUp Type");
            ViewBag.Name = dt.Data;
            return View("~/Views/Modules/Administration/General/LookUpType/LookUpTypeList.cshtml");
        }

        [HttpPost]
        public async Task<ActionResult<BaseResponseDTO<List<NPF_SYS_LookUpType>>>> GetList()
        {
            try
            {
                BaseResponseDTO<List<NPF_SYS_LookUpType>> List = new BaseResponseDTO<List<NPF_SYS_LookUpType>>();
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
        public async Task<ActionResult> GetById([FromBody] BaseRequestDTO<int> dto)
        {
            try
            {
                if (dto == null)
                {
                    NPF_SYS_LookUpType CP = new NPF_SYS_LookUpType();
                    return PartialView("~/Views/Modules/Administration/General/LookUpType/LookUpTypeAddEdit.cshtml", CP);
                }
                else
                {
                    BaseResponseDTO<NPF_SYS_LookUpType> CP = new BaseResponseDTO<NPF_SYS_LookUpType>();
                    CP = service.GetById(dto.Data);
                    return PartialView("~/Views/Modules/Administration/General/LookUpType/LookUpTypeAddEdit.cshtml", CP.Data);
                }

            }
            catch (Exception ex)
            {

                return BadRequest();

            }
        }
        [HttpPost]
        public async Task<ActionResult<BaseResponseDTO<bool>>> CreateUpdate([FromBody] BaseRequestDTO<NPF_SYS_LookUpType> dto)
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
        public async Task<ActionResult<BaseResponseDTO<bool>>> Delete([FromBody] BaseRequestDTO<int> dto)
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
