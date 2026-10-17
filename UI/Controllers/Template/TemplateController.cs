using BL.Models.Common;
using BL.Models.Dashboard;
using BL.Services.Administration;
using BL.Services.Dashboard;
using DAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UI.Helper;

namespace UI.Controllers.Template
{
    public class TemplateController : NPFController
    {

        public UserManager<ApplicationUser> _userManager;
        //public DashboardService service;
        public TemplateController(Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> userManager,
           SignInManager<ApplicationUser> signInManager, Microsoft.AspNetCore.Identity.RoleManager<ApplicationRole> rolemanager) : base(userManager, signInManager, rolemanager)
        {
            //service = new DashboardService();
            _userManager = userManager;
        }
        public ActionResult Template()
        {
            try
            {
                return View("~/Views/Template/Template.cshtml");
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
            
        }
        public ActionResult TemplateInProgress()
        {
            try
            {
                return View("~/Views/Template/TemplateInProgress.cshtml");
            }
            catch (Exception ex)
            {
                return BadRequest();
            }

        }

        public ActionResult TemplateList()
        {
            try
            {
                return View("~/Views/Template/TemplateList.cshtml");
            }
            catch (Exception ex)
            {
                return BadRequest();
            }

        }

        public ActionResult TreeView()
        {
            try
            {
                return View("~/Views/Template/TreeView.cshtml");
            }
            catch (Exception ex)
            {
                return BadRequest();

            }

        }

        public ActionResult Test()
        {
            try
            {
                return View("~/Views/Modules/ResearchLifecycle/Crop/ProgramOfWork_Crop/ProgramOfWork_Crop.cshtml");
            }
            catch (Exception ex)
            {
                return BadRequest();

            }

        }
    }
}
