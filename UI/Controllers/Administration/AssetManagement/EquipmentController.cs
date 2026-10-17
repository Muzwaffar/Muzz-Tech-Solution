using BL.Models.Administration;
using BL.Models.Common;
using BL.Models.Store;
using BL.Services.Access.AccessRights;
using BL.Services.Administration;
using BL.Services.Store;
using DAL.Common;
using DAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UI.Helper;

namespace UI.Controllers.Administration.General
{
    public class EquipmentController : NPFController
    {
        public UserManager<ApplicationUser> _userManager;
        public EquipmentService service;
        public AccessRightService Aservice;
        public EquipmentController(UserManager<ApplicationUser> userManager,
           SignInManager<ApplicationUser> signInManager, RoleManager<ApplicationRole> rolemanager) : base(userManager, signInManager, rolemanager)
        {
            service = new EquipmentService();
            _userManager = userManager;
            Aservice = new AccessRightService();
        }
        #region Views
        public async Task<ActionResult> List()
        {
            BaseResponseDTO<string> dt = new BaseResponseDTO<string>();
            ViewBag.Name = "EquipmentList";
            return View("~/Views/Modules/Administration/AssetManagement/Equipment/EquipmentList.cshtml");
        }

        public async Task<ActionResult> Equipment(long? Id)
        {
            try
            {
                ViewBag.Name = "Equipment";
                BaseResponseDTO<EquipmentDTO> dt = new BaseResponseDTO<EquipmentDTO>();
                if (Id == null)
                {
                    return View("~/Views/Modules/Administration/AssetManagement/Equipment/Equipment.cshtml", dt.Data);
                }
                else
                {
                    dt = service.GetById(Id);
                    return View("/Views/Modules/Administration/AssetManagement/Equipment/Equipment.cshtml", dt.Data);
                }
            }
            catch (Exception ex)
            {
                return BadRequest();

            }

        }
        #endregion
        #region CRUD
        [HttpPost]
        public async Task<ActionResult<BaseResponseDTO<List<EquipmentListDTO>>>> GetList()
        {
            try
            {
                BaseResponseDTO<List<EquipmentListDTO>> List = new BaseResponseDTO<List<EquipmentListDTO>>();
                List = await service.GetAllEquipmentItems();
                return Ok(List);
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
        public async Task<ActionResult<BaseResponseDTO<List<DropDown>>>> GetAllDropdownValuesModel([FromBody] BaseRequestDTO<long> dto)
        {
            try
            {
                BaseResponseDTO<List<DropDown>> List = new BaseResponseDTO<List<DropDown>>();
                List = service.GetAllDropdownValuesModel(dto.Data);
                return Ok(List);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpPost]
        public async Task<ActionResult<BaseResponseDTO<List<DropDown>>>> GetAllDropdownValuesDependent([FromBody] BaseRequestDTO<AssignmentType> dto)
        {
            try
            {
                BaseResponseDTO<List<DropDown>> List = new BaseResponseDTO<List<DropDown>>();
                List = await service.GetAllDropdownValuesDependent(dto.Data);
                return Ok(List);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpPost]
        public async Task<ActionResult<BaseResponseDTO<EquipmentListAssignmentDTO>>> SaveUpdateEquipment([FromBody] BaseRequestDTO<EquipmentDTO> dto)
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
                BaseResponseDTO = service.Delete(dto.Data);
                return Ok(BaseResponseDTO);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }
        #endregion
        #region Get Equipment By Assignment
        [HttpPost]
        public async Task<ActionResult<BaseResponseDTO<List<EquipmentListAssignmentDTO>>>> GetEquipmentByAssignment([FromBody] BaseRequestDTO<EquipemntAssignmentDTO> dto)
        {
            try
            {
                BaseResponseDTO<List<EquipmentListAssignmentDTO>> BaseResponseDTO = new BaseResponseDTO<List<EquipmentListAssignmentDTO>>();
                BaseResponseDTO = service.GetEquipmentByAssignment(dto.Data);
                return Ok(BaseResponseDTO);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }
        #endregion
    }
}
