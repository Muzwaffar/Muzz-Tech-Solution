using BL.Models.Administration;
using BL.Models.Common;
using BL.Models.Store;
using BL.Services.Access.AccessRights;
using BL.Services.Administration;
using DAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UI.Helper;

namespace UI.Controllers.Administration.General
{
    public class VehicleController : NPFController
    {
        public UserManager<ApplicationUser> _userManager;
        public VehicleService service;
        public AccessRightService Aservice;

        public VehicleController(UserManager<ApplicationUser> userManager,
         SignInManager<ApplicationUser> signInManager, RoleManager<ApplicationRole> rolemanager) : base(userManager, signInManager, rolemanager)
        {
            service = new VehicleService();
            _userManager = userManager;
            Aservice = new AccessRightService();
        }


        #region Views
        public async Task<ActionResult> List()
        {
            BaseResponseDTO<string> dt = new BaseResponseDTO<string>();
            dt = await Aservice.GetViewTitle("Vehicle");
            ViewBag.Name = dt.Data;

            return PartialView("~/Views/Modules/Administration/AssetManagement/Vehicle/VehicleList.cshtml");
        }
        public async Task<ActionResult> Vehicle(long? Id)
        {
            try
            {

                BaseResponseDTO<string> dt = new BaseResponseDTO<string>();
                dt = await Aservice.GetViewTitle("Vehicle");
                ViewBag.Name = dt.Data;

                BaseResponseDTO<VehicleCRUDDTO> dtdata = new BaseResponseDTO<VehicleCRUDDTO>();

                if (Id == null)
                {
                    return PartialView("~/Views/Modules/Administration/AssetManagement/Vehicle/Vehicle.cshtml", dtdata.Data);
                }
                else
                {
                    dtdata = service.GetById(Id.Value);
                    return PartialView("~/Views/Modules/Administration/AssetManagement/Vehicle/Vehicle.cshtml", dtdata.Data);
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
        public async Task<ActionResult<BaseResponseDTO<List<VehicleListDTO>>>> GetList()
        {
            try
            {
                BaseResponseDTO<List<VehicleListDTO>> List = new BaseResponseDTO<List<VehicleListDTO>>();
                List = service.GetAll();
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
        public async Task<ActionResult<BaseResponseDTO<bool>>> CreateUpdate([FromBody] BaseRequestDTO<VehicleCRUDDTO> dto)
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
        #endregion
    }
}
