using BL.Constants;
using BL.Models.Administration;
using BL.Models.Common;
using BL.Services.Administration;
using BL.Services.Common;
using DAL.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UI.Helper;

namespace UI.Controllers.Login
{
    public class LoginController : NPFController
    {
        #region constructor

        public LoginController(Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> userManager,SignInManager<ApplicationUser> signInManager, Microsoft.AspNetCore.Identity.RoleManager<ApplicationRole> rolemanager) 
        : base(userManager, signInManager, rolemanager)
        {
        

        }
        #endregion

        #region Views
        public IActionResult Login()
        {
            return View("~/Views/Modules/Login/Login.cshtml");
        }

        public IActionResult ForgotPassword()
        {
            return View("~/Views/Modules/Login/ForgotPassword.cshtml");
        }

        #endregion
        public void Service()
        {

        }

        #region Ajax call

        [HttpPost]
        public async Task<ActionResult<BaseResponseDTO<ApplicationUser>>> Login([FromBody] BaseRequestDTO<LoginDTO> dto)
        {
            try
            {
                LoginDTO logindto = new LoginDTO();

                if (dto != null)
                    logindto = dto.Data;

                string clientIps = String.Empty;
                 var clientIp = Request.Host;


                BaseResponseDTO<ApplicationUser> ncb = await LoginService.Login(dto, _signInManager, _userManager, _roleManager, clientIp.Value);
                if(ncb.QryResult != new QueryResult().FAILED)
                {
                    var key = "b14ca5898a4e4133bbce2ea2315a1916";
                    var encryptedString = AesOperation.EncryptString(key, dto.Data.Password);
                    ISession session = HttpContext.Session;
                    session.SetString("OTT", ncb.Data.OTT.ToString());
                    session.SetString("Username", ncb.Data.UserName.ToString());
                    session.SetString("Language", ncb.Data.Language.ToString());
                }
                return Ok(ncb);

          
            }
            catch (Exception ex)
            {
               
                return BadRequest();

            }


        }



        [HttpPost]
        public async Task<ActionResult<BaseResponseDTO<bool>>> ForgotPassword([FromBody] BaseRequestDTO<string> dto)
        {
            string email = dto.Data;

            BaseResponseDTO<Boolean> BaseResponseDTO = await UserService.ForgotPassword(email, _userManager);

            return Ok(BaseResponseDTO);
        }

        #endregion
    }
}

