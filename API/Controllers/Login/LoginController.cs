using BL.Constants;
using BL.Models.Administration;
using BL.Models.Common;
using BL.Services.Administration;
using BL.Services.Common;
using DAL.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NPOI.XWPF.UserModel;

namespace API.Controllers.Login
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {

        private readonly Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public LoginController(Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> userManager,
                              SignInManager<ApplicationUser> signInManager, RoleManager<ApplicationRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        [HttpPost("Login")]
        public async Task<ActionResult<BaseResponseDTO<ApplicationUser>>> Login([FromBody] BaseRequestDTO<LoginDTO> dto)
        {
            string message = string.Empty;
            BaseResponseDTO<ApplicationUser> ncb = new BaseResponseDTO<ApplicationUser>();
            try
            {
                string clientIps = String.Empty;
                var clientIp = Request.Host;
                ncb = await LoginService.Login(dto, _signInManager, _userManager, _roleManager, clientIp.Value);
            }
            catch (Exception ex)
            {
                ncb.Data = new ApplicationUser();
                ncb.QryResult = new QueryResult().FAILED;
            }
        
            return Ok(ncb);
        }


        [HttpPost("Logout")]
        public async Task<IActionResult> Logout([FromBody] BaseRequestDTO<LogoutDTO> dto)
        {
            BaseResponseDTO<dynamic> resp = new BaseResponseDTO<dynamic>();
            QueryResult queryResult = new QueryResult();

            try
            {
                await LogoutService.LogOutMobile(_signInManager, dto.Data);
                resp.Data = queryResult.SUCEEDED;
                resp.QryResult = queryResult.SUCEEDED;
            }
            catch (Exception)
            {
                resp.QryResult = queryResult.FAILED;
            }
            return Ok(resp);
        }
        [HttpPost("Forget")]
        public async Task<IActionResult> Forget()
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
        [HttpPost("GetData")]
        public async Task<IActionResult> GetData()
        {
            BaseResponseDTO<dynamic> resp = new BaseResponseDTO<dynamic>();
            QueryResult queryResult = new QueryResult();

            try
            {
               // await LogoutService.LogOut(_signInManager);
                resp.Data = queryResult.SUCEEDED;
                resp.QryResult = queryResult.SUCEEDED;
            }
            catch (Exception)
            {
                resp.QryResult = queryResult.FAILED;
            }
            return Ok(resp);
        }
    }
}
