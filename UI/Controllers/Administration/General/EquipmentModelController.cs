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
    public class EquipmentModelController : NPFController
    {

        public UserManager<ApplicationUser> _userManager;
        public EquipmentModelService service;
        public AccessRightService Aservice;

        public EquipmentModelController(UserManager<ApplicationUser> userManager,
       SignInManager<ApplicationUser> signInManager, RoleManager<ApplicationRole> rolemanager) : base(userManager, signInManager, rolemanager)
        {
            service = new EquipmentModelService();
            _userManager = userManager;
            Aservice = new AccessRightService();
        }

        public async Task<ActionResult> List()
        {
            BaseResponseDTO<string> dt = new BaseResponseDTO<string>();
            dt = await Aservice.GetViewTitle("Equipment Model");
            ViewBag.Name = dt.Data;
            return View("~/Views/Modules/Administration/General/EquipmentModel/EquipmentModelList.cshtml");
        }

        [HttpPost]
        public async Task<ActionResult<BaseResponseDTO<List<EquipmentModelListDTO>>>> GetList()
        {
            try
            {
                BaseResponseDTO<List<EquipmentModelListDTO>> CusList = new BaseResponseDTO<List<EquipmentModelListDTO>>();
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
                    EquipmentModelCRUDDTO dt = new EquipmentModelCRUDDTO();
                    return PartialView("~/Views/Modules/Administration/General/EquipmentModel/EquipmentModelAddEdit.cshtml", dt);
                }
                else
                {
                    BaseResponseDTO<EquipmentModelCRUDDTO> dt = new BaseResponseDTO<EquipmentModelCRUDDTO>();
                    dt = service.GetById(dto.Data);
                    return PartialView("~/Views/Modules/Administration/General/EquipmentModel/EquipmentModelAddEdit.cshtml", dt.Data);
                }

            }
            catch (Exception ex)
            {

                return BadRequest();

            }
        }

        [HttpPost]
        public async Task<ActionResult<BaseResponseDTO<List<DropDown>>>> GetAllDropdownValues()
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
        public async Task<ActionResult<BaseResponseDTO<bool>>> CreateUpdate([FromBody] BaseRequestDTO<EquipmentModelCRUDDTO> dto)
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
