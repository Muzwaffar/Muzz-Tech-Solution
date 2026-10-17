using BL.Constants;
using BL.Models.Administration;
using BL.Models.Common;
using BL.Services.Access.AccessRights;
using BL.Services.Administration;
using DAL.Models;
using DAL.Models.Administration;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UI.Helper;

namespace UI.Controllers.Administration
{
    public class ZoneTypeController : NPFController
    {
        public UserManager<ApplicationUser> _userManager;
        public ZoneTypeService service;
        public AccessRightService Aservice;
        public ZoneTypeController(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager, RoleManager<ApplicationRole> rolemanager) : base(userManager, signInManager, rolemanager)
        {
            service = new ZoneTypeService();
            _userManager = userManager;
            Aservice = new AccessRightService();
        }
        public async Task<ActionResult> List()
        {
            BaseResponseDTO<string> dt = new BaseResponseDTO<string>();
            dt = await Aservice.GetViewTitle("Zone Type");
            ViewBag.Name = dt.Data;
            return View("~/Views/Modules/Administration/General/ZoneType/ZoneTypeList.cshtml");
        }

        [HttpPost]
        public async Task<ActionResult<BaseResponseDTO<List<ZoneTypeDTO>>>> GetList()
        {
            try
            {
                BaseResponseDTO<List<ZoneTypeDTO>> List = new BaseResponseDTO<List<ZoneTypeDTO>>();
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
                if (dto == null)
                {
                    ZoneTypeDTO VC = new ZoneTypeDTO();
                    return PartialView("~/Views/Modules/Administration/General/ZoneType/ZoneTypeAddEdit.cshtml", VC);
                }
                else
                {
                    BaseResponseDTO<ZoneTypeDTO> VC = new BaseResponseDTO<ZoneTypeDTO>();
                    VC = service.GetById(dto.Data);
                    return PartialView("~/Views/Modules/Administration/General/ZoneType/ZoneTypeAddEdit.cshtml", VC.Data);
                }

            }
            catch (Exception ex)
            {

                return BadRequest();

            }
        }
        [HttpPost]
        public async Task<ActionResult<BaseResponseDTO<bool>>> CreateUpdate([FromBody] BaseRequestDTO<ZoneTypeDTO> dto)
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


        //API Delete Category
        [HttpPost]
        public async Task<ActionResult<BaseResponseDTO<bool>>> Delete([FromBody] BaseRequestDTO<long> dto)
        {
            try
            {
                BaseResponseDTO<bool> BaseResponseDTO = new BaseResponseDTO<bool>();
                BaseResponseDTO = service.Delete(dto.Data);
                return Ok(BaseResponseDTO);
            }
            catch (Exception ex)
            {

                return BadRequest();

            }
        }
    }
}
