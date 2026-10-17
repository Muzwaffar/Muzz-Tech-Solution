using BL.Models.Common;
using BL.Models.Notification;
using BL.Models.Transaction;
using BL.Services.Common;
using BL.Services.Request;
using BL.Services.Transaction;
using DAL.Models.Transaction;
using Microsoft.AspNetCore.Mvc;
using static BL.Models.Request.RequestDTO;

namespace API.Controllers.Request
{
    [Route("api/[controller]")]
    [ApiController]
    public class RequestController : ControllerBase
    {
        public RequestService service;
        public ChatService Cservice;

        public RequestController()
        {
            service = new RequestService();
            Cservice = new ChatService();
        }
        [HttpPost("screenData")]
        public async Task<ActionResult<BaseResponseDTO<ScreenDTO>>> screenData([FromBody] BaseRequestDTO<GetScreen> dto)
        {
            BaseResponseDTO<ScreenDTO> baseReponseDto = new BaseResponseDTO<ScreenDTO>();

            try
            {
                baseReponseDto = await service.GetScreenData(dto);
                return Ok(baseReponseDto);

            }
            catch (Exception ex)
            {
                return BadRequest();
            }

        }
        [HttpPost("insertRequest")]
        public async Task<ActionResult<BaseResponseDTO<bool>>> insertRequest([FromBody] BaseRequestDTO<InsertRequest> dto)
        {
            BaseResponseDTO<bool> baseReponseDto = new BaseResponseDTO<bool>();

            try
            {
                baseReponseDto = await service.InsertRequest(dto);
                return Ok(baseReponseDto);

            }
            catch (Exception ex)
            {
                return BadRequest();
            }

        }
        [HttpPost("deleteRequest")]
        public async Task<ActionResult<BaseResponseDTO<bool>>> deleteRequest([FromBody] BaseRequestDTO<long> dto)
        {
            BaseResponseDTO<bool> baseReponseDto = new BaseResponseDTO<bool>();

            try
            {
                baseReponseDto = await service.DeleteRequest(dto);
                return Ok(baseReponseDto);

            }
            catch (Exception ex)
            {
                return BadRequest();
            }

        }
        [HttpPost("saveUpdateDamage")]
        public async Task<ActionResult<BaseResponseDTO<bool>>> saveUpdateDamage([FromBody] BaseRequestDTO<NPF_TRN_IncidentDamage> dto)
        {
            BaseResponseDTO<bool> baseReponseDto = new BaseResponseDTO<bool>();

            try
            {
                baseReponseDto = await service.SaveUpdateDamage(dto);
                return Ok(baseReponseDto);

            }
            catch (Exception ex)
            {
                return BadRequest();
            }

        }
        [HttpPost("damageList")]
        public async Task<ActionResult<BaseResponseDTO<List<NPF_TRN_IncidentDamage>>>> damageList([FromBody] BaseRequestDTO<long> dto)
        {
            BaseResponseDTO<List<NPF_TRN_IncidentDamage>> baseReponseDto = new BaseResponseDTO<List<NPF_TRN_IncidentDamage>>();

            try
            {
                baseReponseDto = await service.GetDamageList(dto);
                return Ok(baseReponseDto);

            }
            catch (Exception ex)
            {
                return BadRequest();
            }

        }
        [HttpPost("fatalities")]
        public async Task<ActionResult<BaseResponseDTO<NPF_TRN_IncidentFatality>>> fatalities([FromBody] BaseRequestDTO<long> dto)
        {
            BaseResponseDTO<NPF_TRN_IncidentFatality> baseReponseDto = new BaseResponseDTO<NPF_TRN_IncidentFatality>();

            try
            {
                baseReponseDto = await service.GetFatalities(dto);
                return Ok(baseReponseDto);

            }
            catch (Exception ex)
            {
                return BadRequest();
            }

        }
        [HttpPost("updateFatalities")]
        public async Task<ActionResult<BaseResponseDTO<bool>>> updateFatalities([FromBody] BaseRequestDTO<NPF_TRN_IncidentFatality> dto)
        {
            BaseResponseDTO<bool> baseReponseDto = new BaseResponseDTO<bool>();

            try
            {
                baseReponseDto = await service.UpdateFatalities(dto);
                return Ok(baseReponseDto);

            }
            catch (Exception ex)
            {
                return BadRequest();
            }

        }
        [HttpPost("casualty")]
        public async Task<ActionResult<BaseResponseDTO<NPF_TRN_IncidentCasualty>>> casualty([FromBody] BaseRequestDTO<long> dto)
        {
            BaseResponseDTO<NPF_TRN_IncidentCasualty> baseReponseDto = new BaseResponseDTO<NPF_TRN_IncidentCasualty>();

            try
            {
                baseReponseDto = await service.GetCasualty(dto);
                return Ok(baseReponseDto);

            }
            catch (Exception ex)
            {
                return BadRequest();
            }

        }
        [HttpPost("updatecasualty")]
        public async Task<ActionResult<BaseResponseDTO<bool>>> updatecasualty([FromBody] BaseRequestDTO<NPF_TRN_IncidentCasualty> dto)
        {
            BaseResponseDTO<bool> baseReponseDto = new BaseResponseDTO<bool>();

            try
            {
                baseReponseDto = await service.UpdateCasualty(dto);
                return Ok(baseReponseDto);

            }
            catch (Exception ex)
            {
                return BadRequest();
            }

        }
        [HttpPost("chatlist")]
        public async Task<ActionResult<BaseResponseDTO<List<ChatBaseDTO>>>> chatlist([FromBody] BaseRequestDTO<long> dto)
        {
            BaseResponseDTO<List<ChatBaseDTO>> baseReponseDto = new BaseResponseDTO<List<ChatBaseDTO>>();

            try
            {
                baseReponseDto = await Cservice.GetChatById(dto);
                return Ok(baseReponseDto);

            }
            catch (Exception ex)
            {
                return BadRequest();
            }

        }
        [HttpPost("savechat")]
        public async Task<ActionResult<BaseResponseDTO<bool>>> savechat([FromBody] BaseRequestDTO<ChatSaveDTO> dto)
        {
            BaseResponseDTO<bool> baseReponseDto = new BaseResponseDTO<bool>();

            try
            {
                baseReponseDto = await Cservice.Save(dto,true);
                return Ok(baseReponseDto);

            }
            catch (Exception ex)
            {
                return BadRequest();
            }

        }
    }
}
