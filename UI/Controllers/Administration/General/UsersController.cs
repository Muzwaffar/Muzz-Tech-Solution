using BL.Constants;
using BL.Models.Access;
using BL.Models.Administration;
using BL.Models.Common;
using BL.Services.Access.AccessRights;
using BL.Services.Administration;
using BL.Services.Common;
using DAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using System.Net;
using UI.Helper;

namespace UI.Controllers.Administration.General
{
    public class UsersController : NPFController
    {
        //public UserService service;
        #region Views

        public async Task<ActionResult> List()

        {
            BaseResponseDTO<string> dt = new BaseResponseDTO<string>();
            dt = await Aservice.GetViewTitle("User");
            return View("~/Views/Modules/Administration/Access/UserList.cshtml");
        }

        #endregion

        public async Task<ActionResult> GetUserPartialView([FromBody] BaseRequestDTO<string> dto)
        {
            try
            {

                if (dto.Data == "")
                {
                    UserDTO dt = new UserDTO();
                    return PartialView("~/Views/Modules/Administration/Access/UserAddEdit.cshtml", dt);
                }
                else
                {
                    BaseResponseDTO<UserDTO> dt = new BaseResponseDTO<UserDTO>();
                    dt = new UserService().GetById(dto.Data);
                    return PartialView("~/Views/Modules/Administration/Access/UserAddEdit.cshtml", dt.Data);
                }

            }
            catch (Exception ex)
            {

                return BadRequest();

            }
        }

        #region PartialViews
        public async Task<ActionResult> ChangePasswordPV()
        {
            try
            {

                ChangePasswordDTO changePasswordDTO = new ChangePasswordDTO();
                return PartialView("~/Views/Common/Layout/ChangePasswordLayout.cshtml", changePasswordDTO);


            }
            catch (Exception ex)
            {

                return BadRequest();

            }
        }

        public async Task<ActionResult> ChangePasswordLT()
        {
            try
            {

                ChangePasswordDTO changePasswordDTO = new ChangePasswordDTO();
                return PartialView("~/Views/Modules/Login/ChangePassword.cshtml", changePasswordDTO);


            }
            catch (Exception ex)
            {

                return BadRequest();

            }
        }
        #endregion

        #region ajax 
        public UserManager<ApplicationUser> _userManager;
        public RoleManager<ApplicationRole> _roleManager;
        public AccessRightService Aservice;
        private MatrixService Mservice;

        public UsersController(UserManager<ApplicationUser> userManager,
           SignInManager<ApplicationUser> signInManager, RoleManager<ApplicationRole> rolemanager) : base(userManager, signInManager, rolemanager)
        {

            _userManager = userManager;
            _roleManager = rolemanager;
            Aservice = new AccessRightService();
            Mservice = new MatrixService();
        }

        [HttpPost]
        public async Task<ActionResult<BaseResponseDTO<bool>>> ChangePassword([FromBody] BaseRequestDTO<ChangePasswordDTO> dto)
        {

            var user = dto.Data;
            BaseResponseDTO<bool> baseDTOSave = await UserService.ChangePassword(dto, _userManager);

            return Ok(baseDTOSave);
        }

        [HttpPost]
        public async Task<ActionResult<BaseResponseDTO<bool>>> ResetPassword([FromBody] BaseRequestDTO<string> dto)
        {

            var user = dto.Data;
            BaseResponseDTO<bool> baseDTOSave = await UserService.ResetPassword(dto, _userManager);


            return Ok(baseDTOSave);
        }

        public ActionResult<BaseResponseDTO<List<UsersDTO>>> GetUserList()
        {
            try
            {
                BaseResponseDTO<List<ApplicationUser>> UserList = new BaseResponseDTO<List<ApplicationUser>>();
                UserList = new UserService().GetAll();
                return Ok(UserList);


            }
            catch (Exception ex)
            {

                return BadRequest();

            }
        }


        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            BaseResponseDTO<dynamic> resp = new BaseResponseDTO<dynamic>();
            QueryResult queryResult = new QueryResult();

            try
            {
                await LogoutService.LogOut(_signInManager);
                resp.Data = queryResult.SUCEEDED;
                resp.QryResult = queryResult.SUCEEDED;
            }
            catch (Exception)
            {
                resp.QryResult = queryResult.FAILED;
            }
            return Ok(resp);
        }


        [HttpPost]
        public ActionResult<BaseResponseDTO<List<DropDownLogin>>> getUserDropDown()
        {
            try
            {
                BaseResponseDTO<List<DropDownLogin>> List = new BaseResponseDTO<List<DropDownLogin>>();
                List = new UserService().GetAllDropDownValues();
                return Ok(List);


            }
            catch (Exception ex)
            {

                return BadRequest();

            }
        }

        [HttpPost]
        public async Task<ActionResult<BaseResponseDTO<List<CRUDMatrix>>>> GetTree([FromBody] BaseRequestDTO<string?> dto)
        {
            try
            {
                BaseResponseDTO<List<CRUDMatrix>> tree = new BaseResponseDTO<List<CRUDMatrix>>();
                if(dto.Data == string.Empty)
                {
                    tree = await Mservice.GetTree();
                }
                else
                {
                    tree = await Mservice.GetTreeUser(dto.Data);
                }
                return Ok(tree);


            }
            catch (Exception ex)
            {

                return BadRequest();

            }
        }
        [HttpPost]
        public async Task<ActionResult<BaseResponseDTO<bool>>> CreateUpdate([FromBody] BaseRequestDTO<UserDTO> dto)
        {
            try
            {
                GenericConstant con = new GenericConstant();
                BaseResponseDTO<bool> BaseResponseDTO = new BaseResponseDTO<bool>();
                if (dto.Data.UserToken == con.Gcons)
                {

                    BaseResponseDTO = await new UserService().SaveUser(dto, _userManager, _roleManager);
                    if(BaseResponseDTO.Data == true)
                    {
                        BaseResponseDTO = await Mservice.SaveUserM(dto.Data.UserMatrix,BaseResponseDTO.ExtData);
                    }
                }
                else
                {
                    BaseResponseDTO = new UserService().UpdateUser(dto, _userManager, _roleManager);
                    if (BaseResponseDTO.Data == true)
                    {
                        BaseResponseDTO = await Mservice.UpdateUserM(dto.Data.UserMatrix, BaseResponseDTO.ExtData);
                    }
                }

                return Ok(BaseResponseDTO);
            }
            catch (Exception ex)
            {

                return BadRequest();

            }
        }
        [HttpPost]
        public async Task<ActionResult<BaseResponseDTO<SyncUserDTO>>> GetTunnelDT([FromBody] BaseRequestDTO<string> dto)
        {
            try
            {
                BaseResponseDTO<SyncUserDTO> dt = new BaseResponseDTO<SyncUserDTO>();
                dt  = await new UserService().GetTunnelDT(dto.userToken,dto.Data);
                return Ok(dt);
            }
            catch (Exception ex)
            {

                return BadRequest();

            }
        }


        [HttpPost]
        public async Task<ActionResult<BaseResponseDTO<bool>>> Delete([FromBody] BaseRequestDTO<string> dto)
        {
            try
            {
                BaseResponseDTO<bool> BaseResponseDTO = new BaseResponseDTO<bool>();
                BaseResponseDTO = new UserService().Delete(dto);
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
