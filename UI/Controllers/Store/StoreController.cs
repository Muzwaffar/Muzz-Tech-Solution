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
    public class StoreController : NPFController
    {
        public UserManager<ApplicationUser> _userManager;
        public StoreService service;
        public AccessRightService Aservice;
        public StoreController(UserManager<ApplicationUser> userManager,
           SignInManager<ApplicationUser> signInManager, RoleManager<ApplicationRole> rolemanager) : base(userManager, signInManager, rolemanager)
        {
            service = new StoreService();
            _userManager = userManager;
            Aservice = new AccessRightService();
        }
        #region Views
        public async Task<ActionResult> List()
        {
            BaseResponseDTO<string> dt = new BaseResponseDTO<string>();
            dt = await Aservice.GetViewTitle("Store Location");
            ViewBag.Name = dt.Data;
            return View("~/Views/Modules/Store/Store/StoreList.cshtml");
        }
        public async Task<ActionResult> Store(long? Id, Guid MId)
        {
            try
            {
                BaseResponseDTO<string> dto = new BaseResponseDTO<string>();
                dto = await Aservice.GetViewTitle("Store Location");
                ViewBag.Name = dto.Data;
                BaseResponseDTO<StoreDTO> dt = new BaseResponseDTO<StoreDTO>();
                if (Id == null)
                {
                    return View("~/Views/Modules/Store/Store/Store.cshtml", dt.Data);
                }
                else
                {
                    dt = service.GetById(Id);
                    return View("/Views/Modules/Store/Store/Store.cshtml", dt.Data);
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
        public async Task<ActionResult<BaseResponseDTO<List<StoreListDTO>>>> GetList()
        {
            try
            {
                BaseResponseDTO<List<StoreListDTO>> List = new BaseResponseDTO<List<StoreListDTO>>();
                List = service.GetAllStore();
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
        public async Task<ActionResult<BaseResponseDTO<bool>>> SaveUpdateStore([FromBody] BaseRequestDTO<StoreDTO> dto)
        {
            try
            {
                BaseResponseDTO<bool> BaseResponseDTO = new BaseResponseDTO<bool>();
                if(dto.Data.Id == 0)
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
