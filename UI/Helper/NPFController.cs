using BL.Models.Access;
using BL.Models.Common;
using BL.Services.Common;
using DAL.DBContext;
using DAL.Models;
using DAL.Models.Administration;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace UI.Helper
{
    public abstract class NPFController : Controller
    {

        public NPFContext context;
        public readonly UserManager<ApplicationUser> _userManager;
        public readonly SignInManager<ApplicationUser> _signInManager;
        public readonly RoleManager<ApplicationRole> _roleManager;
        public readonly List<MatrixDTO> _LM;

        public NPFController(UserManager<ApplicationUser> userManager,
                      SignInManager<ApplicationUser> signInManager, RoleManager<ApplicationRole> rolemanager)
        {
            context = new NPFContext();
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = rolemanager;
        }


        public override void OnActionExecuting(ActionExecutingContext context)
        {
            try
            {
                var httpContext = context.HttpContext;
                bool auth = httpContext.User.Identity.IsAuthenticated;
                var User = httpContext.User.Identity.Name;
                ISession session = HttpContext.Session;
                string OTT = session.GetString("OTT");
                string Username = session.GetString("Username");
                ApplicationUser usr = new NPFContext().Users.Where(x => x.UserName == User).FirstOrDefault();
                if (usr != null)
                {
                    if (usr.OTT.ToString() != OTT)
                    {
                        auth = false;
                    }
                    else
                    {
                        //TODO: ADD PopUP Loggin from another browser
                    }
                }
                else
                {
                    auth = false;
                }
                if (!auth)
                {
                    if (httpContext.Request.Path.Value != "/" && !httpContext.Request.Path.Value.ToLower().Contains("login"))
                    {
                        context.Result = RedirectToRoute(new { controller = "Login", action = "Login" });
                        return;
                    }
                }
                else
                {
                    if (httpContext.Request.Path.Value == "/" || (httpContext.Request.Path.Value.ToLower().Contains("login") && !httpContext.Request.QueryString.HasValue))
                    {
                        {
                            context.Result = RedirectToRoute(new { controller = "Dashboard", action = "Dashboard" });
                            return;
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                //System.IO.File.AppendAllText("c:\\errors.txt", ex.ToString());
            }
        }
    }
}
