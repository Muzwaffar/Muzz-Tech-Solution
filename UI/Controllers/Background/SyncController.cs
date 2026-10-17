using BL.Models.Administration;
using BL.Models.Common;
using BL.Services.Access.AccessRights;
using BL.Services.Administration;
using BL.Services.Sync;
using DAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace UI.Controllers.Background
{
    public class SyncController : Controller
    {
        public SyncService service;
        public SyncController()
        {
            service = new SyncService();
        }
        [System.Web.Mvc.AllowAnonymous]
        public void SyncService()
        {
                service.Sync();
        }
    }
}
