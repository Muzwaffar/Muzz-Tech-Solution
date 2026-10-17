using Microsoft.AspNetCore.Mvc;
using BL.Models.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BL.Models.Schedule;
using BL.Services.Schedule;
using BL.Services.Dispatch;
using BL.Models.Transaction;
using BL.Services.Store;
//using BL.Models.Source;

namespace API.Controllers.ServiceRequest
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScheduleController : ControllerBase
    {
        public DispatchService service;
        public ScheduleController()
        {
            service = new DispatchService();
        }

        [HttpPost("getCalender")]
        public async Task<ActionResult<BaseResponseDTO<CalenderMobileDTO>>> getCalenderAsync([FromBody] BaseRequestDTO<string> dto)
        {

            BaseResponseDTO<CalenderMobileDTO> ncb = new BaseResponseDTO<CalenderMobileDTO>();
            try
            {

                Guid dat = Guid.Parse(dto.Data);
                BaseResponseDTO<List<CalenderMobileDTO>> BaseResponseDTO = new BaseResponseDTO<List<CalenderMobileDTO>>();
                //BaseResponseDTO = await service.GetCalendarMobile(dat);

                return Ok(BaseResponseDTO);

            }
            catch (Exception ex)
            {
                return BadRequest();
            }

            return Ok(ncb);
        }
        [HttpPost("getAllTask")]
        public async Task<ActionResult<BaseResponseDTO<List<AllTaskMobileDTO>>>> GetAllTask([FromBody] BaseRequestDTO<DateRangeMobileDTO> dto)
        {

            try
            {
                BaseResponseDTO<List<AllTaskMobileDTO>> BaseResponseDTO = new BaseResponseDTO<List<AllTaskMobileDTO>>();
                BaseResponseDTO = await service.GetAllTaskMobile(dto.Data);
                return Ok(BaseResponseDTO);

            }
            catch (Exception ex)
            {
                return BadRequest();
            }

        }
        [HttpPost("getClientOverview")]
        public async Task<ActionResult<BaseResponseDTO<ClientSchSerReqDTO>>> getClientOverview([FromBody] BaseRequestDTO<string> dto)
        {

            try
            {
                BaseResponseDTO<List<ClientSchSerReqDTO>> BaseResponseDTO = new BaseResponseDTO<List<ClientSchSerReqDTO>>();
                BaseResponseDTO = await service.GetClientOverview(dto.Data);

                return Ok(BaseResponseDTO);

            }
            catch (Exception ex)
            {
                return BadRequest();
            }

        }
        [HttpPost("getTaskDetailMobile")]
        public async Task<ActionResult<BaseResponseDTO<GetTaskDetailMobileDTO>>> GetTaskDetailMobile([FromBody] BaseRequestDTO<GetTaskDetailMobile> dto)
        {

            try
            {

                long userId = Convert.ToInt64(dto.Data.id);
                BaseResponseDTO<GetTaskDetailMobileDTO> BaseResponseDTO = new BaseResponseDTO<GetTaskDetailMobileDTO>();
                BaseResponseDTO = await service.GetTaskDetailMobile(dto.Data);
                return Ok(BaseResponseDTO);

            }
            catch (Exception ex)
            {
                return BadRequest();
            }

        }
        [HttpPost("SaveRemarks")]
        public async Task<ActionResult<BaseResponseDTO<bool>>> SaveRemarks([FromBody] BaseRequestDTO<SaveFileRemarks> dto)
        {

            try
            {
                BaseResponseDTO<bool> BaseResponseDTO = new BaseResponseDTO<bool>();
                BaseResponseDTO = await service.SaveRemarks(dto);
                return Ok(BaseResponseDTO);

            }
            catch (Exception ex)
            {
                return BadRequest();
            }

        }
        [HttpPost("StatusUpdateTaskMobile")]
        public async Task<ActionResult<BaseResponseDTO<bool>>> StatusUpdateTaskMobile([FromBody] BaseRequestDTO<IncidentStatusChange> dto)
        {

            try
            {
                BaseResponseDTO<bool> BaseResponseDTO = new BaseResponseDTO<bool>();
                BaseResponseDTO = await service.StatusUpdateTaskMobile(dto);
                return Ok(BaseResponseDTO);

            }
            catch (Exception ex)
            {
                return BadRequest();
            }

        }
        [HttpPost("StatusUpdateTaskDetailMobile")]
        public async Task<ActionResult<BaseResponseDTO<bool>>> StatusUpdateTaskDetailMobile([FromBody] BaseRequestDTO<IncidentStatusChange> dto)
        {

            try
            {
                BaseResponseDTO<bool> BaseResponseDTO = new BaseResponseDTO<bool>();
                BaseResponseDTO = await service.StatusUpdateTaskDetailMobile(dto);
                return Ok(BaseResponseDTO);

            }
            catch (Exception ex)
            {
                return BadRequest();
            }

        }
        [HttpPost("SaveFile")]
        public async Task<ActionResult<BaseResponseDTO<bool>>> SaveFile([FromBody] BaseRequestDTO<SaveFileRemarks> dto)
        {

            try
            {
                BaseResponseDTO<bool> BaseResponseDTO = new BaseResponseDTO<bool>();
                BaseResponseDTO = await service.SaveFile(dto);
                return Ok(BaseResponseDTO);

            }
            catch (Exception ex)
            {
                return BadRequest();
            }

        }
        [HttpPost("DeleteFile")]
        public async Task<ActionResult<BaseResponseDTO<bool>>> DeleteFile([FromBody] BaseRequestDTO<long> dto)
        {

            try
            {

                BaseResponseDTO<bool> BaseResponseDTO = new BaseResponseDTO<bool>();
                BaseResponseDTO = await service.DeleteFile(dto);
                return Ok(BaseResponseDTO);

            }
            catch (Exception ex)
            {
                return BadRequest();
            }

        }
        [HttpPost("SaveZone")]
        public async Task<ActionResult<BaseResponseDTO<bool>>> SaveZone([FromBody] BaseRequestDTO<ScheZone> dto)
        {

            try
            {
                BaseResponseDTO<bool> BaseResponseDTO = new BaseResponseDTO<bool>();
                BaseResponseDTO = await service.SaveZone(dto);
                return Ok(BaseResponseDTO);

            }
            catch (Exception ex)
            {
                return BadRequest();
            }

        }
        [HttpPost("getFormList")]
        public async Task<ActionResult<BaseResponseDTO<List<DropDownApi>>>> getFormList([FromBody] BaseRequestDTO<long> dto)
        {

            try
            {

                BaseResponseDTO<List<DropDownApi>> BaseResponseDTO = new BaseResponseDTO<List<DropDownApi>>();
                //BaseResponseDTO = await service.GetFormList();
                return Ok(BaseResponseDTO);

            }
            catch (Exception ex)
            {
                return BadRequest();
            }

        }
        [HttpPost("AddFormFromMobile")]
        public async Task<ActionResult<BaseResponseDTO<long>>> AddFormFromMobile([FromBody] BaseRequestDTO<long> dto)
        {

            try
            {

                BaseResponseDTO<long> BaseResponseDTO = new BaseResponseDTO<long>();
               // BaseResponseDTO = await service.AddFormFromMobile(dto.Data,dto.userToken);
                return Ok(BaseResponseDTO);

            }
            catch (Exception ex)
            {
                return BadRequest();
            }

        }

    }
}
