using BL.Models.Administration;
using BL.Models.Common;
using BL.Services.Access.AccessRights;
using BL.Services.Administration;
using DAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UI.Helper;

namespace UI.Controllers.Administration.General
{
    public class EquipmentMakeController : NPFController
    {
        public UserManager<ApplicationUser> _userManager;
        public EquipmentMakeService service;
        public AccessRightService Aservice;

        public EquipmentMakeController(UserManager<ApplicationUser> userManager,
           SignInManager<ApplicationUser> signInManager, RoleManager<ApplicationRole> rolemanager) : base(userManager, signInManager, rolemanager)
        {
            service = new EquipmentMakeService();
            _userManager = userManager;
            Aservice = new AccessRightService();
        }

        public async Task<ActionResult> List()
        {
            BaseResponseDTO<string> dt = new BaseResponseDTO<string>();
            dt = await Aservice.GetViewTitle("Equipment Make");
            ViewBag.Name = dt.Data;
            return View("~/Views/Modules/Administration/General/EquipmentMake/EquipmentMakeList.cshtml");
        }

        [HttpPost]
        public async Task<ActionResult<BaseResponseDTO<List<EquipmentMakeListDTO>>>> GetList()
        {
            try
            {
                BaseResponseDTO<List<EquipmentMakeListDTO>> CusList = new BaseResponseDTO<List<EquipmentMakeListDTO>>();
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
                    EquipmentMakeCRUDDTO dt = new EquipmentMakeCRUDDTO();
                    return PartialView("~/Views/Modules/Administration/General/EquipmentMake/EquipmentMakeAddEdit.cshtml", dt);
                }
                else
                {
                    BaseResponseDTO<EquipmentMakeCRUDDTO> dt = new BaseResponseDTO<EquipmentMakeCRUDDTO>();
                    dt = service.GetById(dto.Data);
                    return PartialView("~/Views/Modules/Administration/General/EquipmentMake/EquipmentMakeAddEdit.cshtml", dt.Data);
                }

            }
            catch (Exception ex)
            {

                return BadRequest();

            }
        }

        [HttpPost]
        public async Task<ActionResult<BaseResponseDTO<bool>>> CreateUpdate([FromBody] BaseRequestDTO<EquipmentMakeCRUDDTO> dto)
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

    }
}
