using BL.Models.Access;
using BL.Constants;
using BL.Models.Common;
using BL.Models.Modules.Extension;
using BL.Models.Modules.Research;
using BL.Services.Access.AccessRights;
using BL.Services.ServiceRequest;
using DAL.Common;
using DAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Drawing;
using System.Reflection;
using System.Text.Json;
using UI.Helper;
using BL.Services.Store;
using BL.Models.Store;

namespace UI.Controllers.StationManagement.Store
{
    public class StoreItemController : NPFController
    {
        public UserManager<ApplicationUser> _userManager;
        public StoreItemService service;
        public AccessRightService Aservice;
        public StoreItemController(UserManager<ApplicationUser> userManager,
           SignInManager<ApplicationUser> signInManager, RoleManager<ApplicationRole> rolemanager) : base(userManager, signInManager, rolemanager)
        {
            service = new StoreItemService();
            _userManager = userManager;
            Aservice = new AccessRightService();
        }
        #region Views
        public async Task<ActionResult> List()
        {
            BaseResponseDTO<string> dt = new BaseResponseDTO<string>();
            dt = await Aservice.GetViewTitle("Store Item");
            ViewBag.Name = dt.Data;
            return View("~/Views/Modules/Store/StoreItem/StoreItemList.cshtml");
        }
        public async Task<ActionResult> StoreItem(long? Id, Guid MId)
        {
            try
            {
                BaseResponseDTO<string> dto = new BaseResponseDTO<string>();
                dto = await Aservice.GetViewTitle("Store Item");
                ViewBag.Name = dto.Data;
                BaseResponseDTO<StoreItemDTO> dt = new BaseResponseDTO<StoreItemDTO>();
                if (Id == null)
                {
                    return View("~/Views/Modules/Store/StoreItem/StoreItem.cshtml", dt.Data);
                }
                else
                {
                    dt = service.GetById(Id);
                    return View("/Views/Modules/Store/StoreItem/StoreItem.cshtml", dt.Data);
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
        public async Task<ActionResult<BaseResponseDTO<List<StoreItemListDTO>>>> GetList()
        {
            try
            {
                BaseResponseDTO<List<StoreItemListDTO>> List = new BaseResponseDTO<List<StoreItemListDTO>>();
                List = service.GetAllStoreItems();
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
        public async Task<ActionResult<BaseResponseDTO<List<DropDown>>>> GetAllDropdownValuesUOM([FromBody] BaseRequestDTO<long> dto)
        {
            try
            {
                BaseResponseDTO<List<DropDown>> List = new BaseResponseDTO<List<DropDown>>();
                List = service.GetAllDropdownValuesUOM(dto.Data);
                return Ok(List);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }
        [HttpPost]
        public async Task<ActionResult<BaseResponseDTO<bool>>> SaveUpdateStoreItem([FromBody] BaseRequestDTO<StoreItemDTO> dto)
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
    }
}
