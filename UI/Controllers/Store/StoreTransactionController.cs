using BL.Models.Common;
using BL.Models.Store;
using BL.Services.Access.AccessRights;
using BL.Services.Store;
using DAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UI.Helper;

namespace UI.Controllers.Store
{
    public class StoreTransactionController : NPFController
    {
        public UserManager<ApplicationUser> _userManager;
        public StoreTransactionService service;
        public AccessRightService Aservice;
        public StoreTransactionController(UserManager<ApplicationUser> userManager,
           SignInManager<ApplicationUser> signInManager, RoleManager<ApplicationRole> rolemanager) : base(userManager, signInManager, rolemanager)
        {
            service = new StoreTransactionService();
            _userManager = userManager;
            Aservice = new AccessRightService();
        }
        public async Task<ActionResult> View()
        {
            BaseResponseDTO<string> dt = new BaseResponseDTO<string>();
            dt = await Aservice.GetViewTitle("Store Transaction");
            ViewBag.Name = dt.Data;
            return View("~/Views/Modules/Store/StoreTransaction/StoreTransaction.cshtml");
        }

        [HttpPost]
        public async Task<ActionResult<BaseResponseDTO<List<StockRequestListDTO>>>> GetStockRequestList()
        {
            try
            {
                BaseResponseDTO<List<StockRequestListDTO>> List = new BaseResponseDTO<List<StockRequestListDTO>>();
                List = service.GetStockRequestList();
                return Ok(List);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpPost]
        public async Task<ActionResult<BaseResponseDTO<List<StoreTransactionListDTO>>>> GetStoreTransactionList()
        {
            try
            {
                BaseResponseDTO<List<StoreTransactionListDTO>> List = new BaseResponseDTO<List<StoreTransactionListDTO>>();
                List = service.GetStoreTransactionList();
                return Ok(List);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpPost]
        public async Task<ActionResult<BaseResponseDTO<List<StoreItemTableDTO>>>> GetStoreItemList()
        {
            try
            {
                BaseResponseDTO<List<StoreItemTableDTO>> List = new BaseResponseDTO<List<StoreItemTableDTO>>();
                List = service.GetStoreItemList();
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
                List = await service.GetAllDropDownValues();
                return Ok(List);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpPost]
        public async Task<ActionResult<BaseResponseDTO<bool>>> Save([FromBody] BaseRequestDTO<StoreTransactionDTO> dto)
        {
            try
            {
                BaseResponseDTO<bool> BaseResponseDTO = new BaseResponseDTO<bool>();
                BaseResponseDTO = await service.SaveAsync(dto);
                return Ok(BaseResponseDTO);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpPost]
        public async Task<ActionResult<BaseResponseDTO<bool>>> AproveDecline([FromBody] BaseRequestDTO<StockRequestADDTO> dto)
        {
            try
            {
                BaseResponseDTO<bool> BaseResponseDTO = new BaseResponseDTO<bool>();
                BaseResponseDTO = await service.AproveDecline(dto);
                return Ok(BaseResponseDTO);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

    }
}
