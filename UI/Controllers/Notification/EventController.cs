using BL.Models.Administration;
using BL.Models.Common;
using BL.Models.Dashboard;
using BL.Models.Notification;
using BL.Services.Administration;
using BL.Services.Common;
using BL.Services.Dashboard;
using DAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UI.Helper;

namespace UI.Controllers.Dashboard
{
    public class EventController : NPFController
    {

        public UserManager<ApplicationUser> _userManager;
        public EventService service;
        public EventController(Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> userManager,
           SignInManager<ApplicationUser> signInManager, Microsoft.AspNetCore.Identity.RoleManager<ApplicationRole> rolemanager) : base(userManager, signInManager, rolemanager)
        {
            service = new EventService();
            _userManager = userManager;
        }
        [HttpPost]
        public async Task<ActionResult<BaseResponseDTO<List<EventCenter>>>> GetEventList([FromBody] BaseRequestDTO<DateRangeDTO> dto)
        {
            try
            {
                BaseResponseDTO<List<EventCenter>> result = new BaseResponseDTO<List<EventCenter>>();
                result = service.GetList(dto.Data);
                return Ok(result);
            }
            catch (Exception ex)
            {

                return BadRequest();

            }
            
        }
        [HttpPost]
        public async Task<ActionResult<BaseResponseDTO<List<DropDown>>>> HeartBeat()
        {
            try
            {
                BaseResponseDTO<GetEvent> Event = new BaseResponseDTO<GetEvent>();
                Event = await service.HeartBeat();
                return Ok(Event);


            }
            catch (Exception ex)
            {

                return BadRequest();

            }
        }
    }
}
