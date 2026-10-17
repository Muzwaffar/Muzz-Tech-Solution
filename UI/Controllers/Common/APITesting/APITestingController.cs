using BL.Models.Common;
using BL.Models.Modules.Common;
using BL.Models.Modules.Research;
using BL.Models.Schedule;
using BL.Models.Workflow;
using BL.Services.Common;
using BL.Services.Dispatch;
using DAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UI.Helper;

namespace UI.Controllers.Common.APITesting
{
    public class APITestingController : NPFController
    {
        public UserManager<ApplicationUser> _userManager;
        public DispatchService service;
        public APITestingController(UserManager<ApplicationUser> userManager,
           SignInManager<ApplicationUser> signInManager, RoleManager<ApplicationRole> rolemanager) : base(userManager, signInManager, rolemanager)
        {
            _userManager = userManager;
            service = new DispatchService();
        }

        public async Task<ActionResult> APITesting()
        {
            return PartialView("~/Views/Common/APITesting/APITesting.cshtml");
        }

    }
}
