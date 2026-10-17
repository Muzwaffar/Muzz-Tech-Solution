using BL.Models.Common;
using BL.Models.Notification;
using BL.Models.Schedule;
using BL.Services.Common;
using BL.Services.Schedule;
using DAL.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Notification
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {

        public NotificationService service;

        public NotificationController()
        {
            service = new NotificationService();
        }




        [HttpPost("saveUpdateUserMobile")]
        public ActionResult<BaseResponseDTO<bool>> SaveUpdateUserMobile([FromBody] BaseRequestDTO<UserMobileDTO> dto)
        {
            BaseResponseDTO<bool> baseReponseDto = new BaseResponseDTO<bool>();

            try
            {
                baseReponseDto = service.NotificationUserMobile(dto);
                return Ok(baseReponseDto);

            }
            catch (Exception ex)
            {
                return BadRequest();
            }

        }


        [HttpPost("getNotifications")]
        public ActionResult<BaseResponseDTO<List<NotificationDTO>>> getNotifications([FromBody] BaseRequestDTO<NotificationReportDTO> dto)
        {
            BaseResponseDTO<List<NotificationDTO>> baseReponseDto = new BaseResponseDTO<List<NotificationDTO>>();

            try
            {
                baseReponseDto = service.getNotification(dto.Data);
                return Ok(baseReponseDto);

            }
            catch (Exception ex)
            {
                return BadRequest();
            }

        }




    }
}
