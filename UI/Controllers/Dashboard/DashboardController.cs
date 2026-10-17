using BL.Models.Common;
using BL.Models.Dashboard;
using BL.Services.Access.AccessRights;
using BL.Services.Administration;
using BL.Services.Dashboard;
using DAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UI.Helper;

namespace UI.Controllers.Dashboard
{
    public class DashboardController : NPFController
    {

        public UserManager<ApplicationUser> _userManager;
        public DashboardService service;
        public AccessRightService Aservice;
        public DashboardController(Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> userManager,
           SignInManager<ApplicationUser> signInManager, Microsoft.AspNetCore.Identity.RoleManager<ApplicationRole> rolemanager) : base(userManager, signInManager, rolemanager)
        {
            service = new DashboardService();
            _userManager = userManager;
            Aservice = new AccessRightService();
        }
        public async Task<ActionResult<BaseResponseDTO<GetDashboardDTO>>> Dashboard()
        {
            try
            {
                BaseResponseDTO<GetDashboardDTO> result = new BaseResponseDTO<GetDashboardDTO>();
                BaseResponseDTO<string> dt = new BaseResponseDTO<string>();
                dt = await Aservice.GetViewTitle("Dashboard");
                ViewBag.Name = dt.Data;

                result = service.GetAll();
                return View("~/Views/Modules/Dashboard/Dashboard.cshtml", result.Data);
            }
            catch (Exception ex)
            {

                return BadRequest();

            }
            
        }

    }
}
