using BL.Models.Common;
using BL.Models.Dashboard;
using BL.Models.Incident;
using BL.Services.Administration;
using BL.Services.Dashboard;
using DAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Reflection.PortableExecutable;
using System.Security.Policy;
using System.Text;
using UI.Helper;

namespace UI.Controllers.Dashboard
{
    public class ExternalController : NPFController
    {

        public UserManager<ApplicationUser> _userManager;
        public ExternalService service;
        public ExternalController(Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> userManager,
           SignInManager<ApplicationUser> signInManager, Microsoft.AspNetCore.Identity.RoleManager<ApplicationRole> rolemanager) : base(userManager, signInManager, rolemanager)
        {
            service = new ExternalService();
            _userManager = userManager;
        }

        [HttpPost]
        public async Task<ActionResult<BaseResponseDTO<Getparams>>> GetParams()
        {
            try
            {
                BaseResponseDTO<Getparams> Param = new BaseResponseDTO<Getparams>();
                Param = service.GetFleetParam();
                return Ok(Param);
            }
            catch (Exception ex)
            {

                return BadRequest();

            }
        }

    }
}
