using BL.Models.Access;
using BL.Models.Common;
using BL.Services.Access.AccessRights;
using BL.Services.Administration;
using BL.Services.Common;
using DAL.Models;
using DAL.Models.Access;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UI.Helper;

namespace UI.Controllers.Access
{
    public class AccessRightsController : NPFController
    {
        private AccessRightService acessrightservice;
        private RowAccessService rowaccessservice;
        public AccessRightService Aservice;
        public Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> _userManager;
        public AccessRightsController(Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> userManager,
           SignInManager<ApplicationUser> signInManager, Microsoft.AspNetCore.Identity.RoleManager<ApplicationRole> rolemanager) : base(userManager, signInManager, rolemanager)
        {
            rowaccessservice = new RowAccessService();
            _userManager = userManager;
            Aservice = new AccessRightService();
        }


        #region views
        public async Task<ActionResult> List()
        {
            BaseResponseDTO<string> dt = new BaseResponseDTO<string>();
            dt = await Aservice.GetViewTitle("Access Rights");
            ViewBag.Name = dt.Data;
            return View("~/Views/Modules/Access/AccessRights/AccessRightsList.cshtml");
        }

        #endregion


        #region PartialViews
        [HttpPost]
        public async Task<ActionResult> GetById([FromBody] BaseRequestDTO<string> dto)
        {
            try
            {
                if (dto.Data == null || dto.Data == string.Empty)
                {
                    AccessRightsDTO dt = new AccessRightsDTO();
                    return PartialView("~/Views/Modules/Access/AccessRights/AccessRightsAddEdit.cshtml", dt);
                }
                else
                {
                    BaseResponseDTO<AccessRightsDTO> dt = new BaseResponseDTO<AccessRightsDTO>();
                    dt = new AccessRightService().GetById(dto.Data);
                    return PartialView("~/Views/Modules/Access/AccessRights/AccessRightsAddEdit.cshtml", dt.Data);
                }

            }
            catch (Exception ex)
            {

                return BadRequest();

            }
        }
        #endregion

        #region Ajax call 

        public async Task<ActionResult<BaseResponseDTO<List<AccessRightsDTO>>>> GetList()
        {
            try
            {
                BaseResponseDTO<List<AccessRightsDTO>> UserList = new BaseResponseDTO<List<AccessRightsDTO>>();
                UserList = new AccessRightService().GetAll();
                return Ok(UserList);


            }
            catch (Exception ex)
            {

                return BadRequest();

            }
        }
        public async Task<ActionResult<BaseResponseDTO<bool>>> GetRowAccess([FromBody] BaseRequestDTO<string> dto)
        {
            try
            {
                BaseResponseDTO<bool> dt = new BaseResponseDTO<bool>();
                dt.Access = rowaccessservice.GetMenuAccessByIds(dto.ModuleId);
                return Ok(dt);


            }
            catch (Exception ex)
            {

                return BadRequest();

            }
        }
        public async Task<ActionResult<BaseResponseDTO<bool>>> createUpdateAR([FromBody] BaseRequestDTO<MainARDTO> dataToSave)
        {
            try
            {
                return Ok(await new AccessRightService().SaveUpdateAR(dataToSave));
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }
        public async Task<ActionResult> GetModules()
        {
            try
            {
                BaseResponseDTO<MainARDTO> Ma = new BaseResponseDTO<MainARDTO>();
                Ma.Data = new MainARDTO();
                BaseResponseDTO<List<AccessRightDTO>> al = new AccessRightService().GetModules();
                Ma.Data.AccessRightDTO = al.Data;
                Ma.Data.RoleName = string.Empty;
                Ma.Data.RoleId = string.Empty;
                return Ok(Ma);


            }
            catch (Exception ex)
            {

                return BadRequest();

            }
        }
        public async Task<ActionResult<BaseResponseDTO<MainARDTO>>> GetARByRoleId([FromBody] BaseRequestDTO<string> dto)
        {
            try
            {
                if (dto.Data == null || dto.Data == string.Empty)
                {
                    BaseResponseDTO<AccessRightsDTO> dt = new BaseResponseDTO<AccessRightsDTO>();
                    return Ok(dt);
                }
                else
                {
                    return Ok(await new AccessRightService().GetARById(dto.Data));
                }
                

            }
            catch (Exception ex)
            {

                return BadRequest();

            }
        }
        public async Task<ActionResult<BaseResponseDTO<List<MainMenuDTO>>>> GetMenu([FromBody] BaseRequestDTO<AccessRightsDTO> dto)
        {
            try
            {
                return Ok(await new AccessRightService().getAccessRightByUserRoleId(dto.Data.Id, dto.Data.UserId));

            }
            catch (Exception ex)
            {

                return BadRequest();

            }
        }
        public ActionResult<BaseResponseDTO<bool>> Delete([FromBody] BaseRequestDTO<string> del)
        {
            try
            {
                return Ok(new AccessRightService().Delete(del));
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }


        #endregion
        public async Task<ActionResult<BaseResponseDTO<bool>>> createUpdateMenu([FromBody] BaseRequestDTO<MenuDTO> dataToSave)
        {
            try
            {
                return Ok(await new AccessRightService().createUpdateMenu(dataToSave));
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }
        public async Task<ActionResult<BaseResponseDTO<List<MenuDTO>>>> GetMenuList()
        {
            try
            {
                BaseResponseDTO<List<MenuDTO>> UserList = new BaseResponseDTO<List<MenuDTO>>();
                UserList = new AccessRightService().GetMenuList();
                return Ok(UserList);


            }
            catch (Exception ex)
            {

                return BadRequest();

            }
        }
        [HttpPost]
        public async Task<ActionResult> GetMenuById([FromBody] BaseRequestDTO<string> dto)
        {
            try
            {
                if (dto.Data == null || dto.Data == string.Empty)
                {
                    MenuDTO dt = new MenuDTO();
                    return PartialView("~/Views/Modules/Access/Menu/MenuAddEdit.cshtml", dt);
                }
                else
                {
                    BaseResponseDTO<MenuDTO> dt = new BaseResponseDTO<MenuDTO>();
                    dt = new AccessRightService().GetMenuById(dto.Data);
                    return PartialView("~/Views/Modules/Access/Menu/MenuAddEdit.cshtml", dt.Data);
                }

            }
            catch (Exception ex)
            {

                return BadRequest();

            }
        }

    }
}
