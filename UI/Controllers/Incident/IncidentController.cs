using BL.Constants;
using BL.Models.Common;
using BL.Models.Incident;
using BL.Services.Access.AccessRights;
using BL.Services.Incident;
using DAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using UI.Helper;

namespace UI.Controllers.Incident
{
    public class IncidentController : NPFController
    {
        public UserManager<ApplicationUser> _userManager;
        public IncidentService service;
        public AccessRightService Aservice;
        public IncidentController(Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> userManager,
           SignInManager<ApplicationUser> signInManager, Microsoft.AspNetCore.Identity.RoleManager<ApplicationRole> rolemanager) : base(userManager, signInManager, rolemanager)
        {
            service = new IncidentService();
            _userManager = userManager;
            Aservice = new AccessRightService();
        }
        public async Task<ActionResult> List()
        {
            BaseResponseDTO<string> dt = new BaseResponseDTO<string>();
            dt = await Aservice.GetViewTitle("Dashboard");
            ViewBag.Name = dt.Data;
            return View("~/Views/Modules/Incident/Incident.cshtml");
        }

        [HttpPost]
        public async Task<ActionResult<BaseResponseDTO<List<IncidentDTO>>>> GetList()
        {
            try
            {
                BaseResponseDTO<List<IncidentDTO>> List = new BaseResponseDTO<List<IncidentDTO>>();
                List = service.GetAll();
                return Ok(List);


            }
            catch (Exception ex)
            {

                return BadRequest();

            }
        }
        [HttpPost]
        public async Task<ActionResult<BaseResponseDTO<List<IncidentDTO>>>> GetAllByTypeName([FromBody] BaseRequestDTO<string?> dto)
        {
            try
            {
                BaseResponseDTO<List<IncidentDTO>> List = new BaseResponseDTO<List<IncidentDTO>>();
                List = service.GetAllByTypeName(dto.Data);
                return Ok(List);


            }
            catch (Exception ex)
            {

                return BadRequest();

            }
        }

    }
}
