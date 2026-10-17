using BL.Models.Access;
using BL.Constants;
using BL.Models.Common;
using BL.Models.Modules.Extension;
using BL.Models.Modules.Research;
using BL.Services.Access.AccessRights;
using BL.Services.ServiceRequest;
using DAL.Common;
using DAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Drawing;
using System.Reflection;
using System.Text.Json;
using UI.Helper;
using BL.Services.Store;
using BL.Models.Store;
using BL.Models.Template;
using BL.Models.Transaction;
using DAL.Models.Transaction;
using BL.Services.Transaction;

namespace UI.Controllers.StationManagement.Store
{
    public class IncidentHandlingController : NPFController
    {
        public UserManager<ApplicationUser> _userManager;
        public IncidentHandlingService service;
        public ChatService Cservice;
        public AccessRightService Aservice;
        public IncidentHandlingController(UserManager<ApplicationUser> userManager,
           SignInManager<ApplicationUser> signInManager, RoleManager<ApplicationRole> rolemanager) : base(userManager, signInManager, rolemanager)
        {
            _userManager = userManager;
            Aservice = new AccessRightService();
            service = new IncidentHandlingService(userManager);
            Cservice = new ChatService();
        }
        #region Views
        public async Task<ActionResult> List()
        {
            ViewBag.Name = "Incident Call Handling"; 
            return View("~/Views/Modules/Transaction/CallHandling/CallHandlingList.cshtml");
        }
        public async Task<ActionResult> AddEdit(long? Id,string UserToken)
        {
            try
            {
                ViewBag.Name = "Incident Call Handling";
                BaseResponseDTO<IncidentHandlingDTO> dt = new BaseResponseDTO<IncidentHandlingDTO>();
                if (Id == null)
                {
                    return View("~/Views/Modules/Transaction/CallHandling/CallHandling.cshtml", dt.Data);
                }
                else
                {
                    dt = service.GetById(Id, UserToken);
                    return View("/Views/Modules/Transaction/CallHandling/CallHandling.cshtml", dt.Data);
                }
            }
            catch (Exception ex)
            {
                return BadRequest();

            }

        }
        [HttpPost]
        public async Task<ActionResult<BaseResponseDTO<IncidentHandlingGetDetailsDTO>>> GetByIdDetails([FromBody] BaseRequestDTO<long?> dto)
        {
            try
            {
                BaseResponseDTO<IncidentHandlingGetDetailsDTO> BaseResponseDTO = new BaseResponseDTO<IncidentHandlingGetDetailsDTO>();
                BaseResponseDTO = await service.GetByIdDetails(dto.Data);
                return Ok(BaseResponseDTO);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }
        [HttpPost]
        public async Task<ActionResult<BaseResponseDTO<IncidentGetRefreshedListDTO>>> GetByIdRefreshed([FromBody] BaseRequestDTO<long> dto)
        {
            try
            {
                BaseResponseDTO<IncidentGetRefreshedListDTO> BaseResponseDTO = new BaseResponseDTO<IncidentGetRefreshedListDTO>();
                BaseResponseDTO = await service.GetByIdRefreshed(dto);
                return Ok(BaseResponseDTO);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
           
        }
        [HttpPost]
        public async Task<ActionResult<BaseResponseDTO<IncidentHandlingDTO>>> Initialise([FromBody] BaseRequestDTO<string> dto)
        {
            try
            {
                BaseResponseDTO<IncidentHandlingDTO> List = new BaseResponseDTO<IncidentHandlingDTO>();
                List = await service.Initialize(dto.Data);
                return Ok(List);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }
        #endregion
        #region CRUD
        [HttpPost]
        public async Task<ActionResult<BaseResponseDTO<List<IncidentHandlingListDTO>>>> GetList([FromBody] BaseRequestDTO<DateRangeDTO> dto)
        {
            try
            {
                BaseResponseDTO<List<IncidentHandlingListDTO>> List = new BaseResponseDTO<List<IncidentHandlingListDTO>>();
                List = service.GetAllCallHandling(dto);
                return Ok(List);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }
        [HttpPost]
        public async Task<ActionResult<BaseResponseDTO<List<DropDown>>>> GetAllDropdownValues()
        {
            try
            {
                BaseResponseDTO<List<DropDown>> List = new BaseResponseDTO<List<DropDown>>();
                List = service.GetAllDropDownValues();
                return Ok(List);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }
        [HttpPost]
        public async Task<ActionResult<BaseResponseDTO<List<DropDown>>>> GetCallhandlingIncidentSubType([FromForm] DropdownForm dto)
        {
            try
            {
                BaseResponseDTO<List<DropDown>> List = new BaseResponseDTO<List<DropDown>>();
                List = service.GetCallhandlingIncidentSubType(dto);
                return Ok(List);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }
        [HttpPost]
        public async Task<ActionResult<BaseResponseDTO<List<DropDown>>>> GetCallhandlingIncidentCode([FromForm] DropdownForm dto)
        {
            try
            {
                BaseResponseDTO<List<DropDown>> List = new BaseResponseDTO<List<DropDown>>();
                List = service.GetCallhandlingIncidentCode(dto);
                return Ok(List);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }
        [HttpPost]
        public async Task<ActionResult<BaseResponseDTO<bool>>> Save([FromBody] BaseRequestDTO<IncidentHandlingDTO> dto)
        {
            try
            {
                BaseResponseDTO<bool> BaseResponseDTO = new BaseResponseDTO<bool>();
                BaseResponseDTO = await service.Save(dto.Data,dto.userToken);
                return Ok(BaseResponseDTO);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }
        [HttpPost]
        public async Task<ActionResult<BaseResponseDTO<bool>>> Delete([FromBody] BaseRequestDTO<long> dto)
        {
            try
            {
                BaseResponseDTO<bool> BaseResponseDTO = new BaseResponseDTO<bool>();
                BaseResponseDTO = service.Delete(dto.Data);
                return Ok(BaseResponseDTO);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }
        [HttpPost]
        public async Task<ActionResult<BaseResponseDTO<List<IncidentHandlingCallerListDTO>>>> GetCallerLastTransactions([FromBody] BaseRequestDTO<long> dto)
        {
            try
            {
                BaseResponseDTO<List<IncidentHandlingCallerListDTO>> List = new BaseResponseDTO<List<IncidentHandlingCallerListDTO>>();
                List = service.GetCallerLastTransactions(dto.Data);
                return Ok(List);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }
        [HttpPost]
        public async Task<ActionResult<BaseResponseDTO<List<DropDown>>>> GetVehicleByCategory([FromBody] BaseRequestDTO<DropdownParameterDTO> dto)
        {
            try
            {
                BaseResponseDTO<List<DropDown>> List = new BaseResponseDTO<List<DropDown>>();
                List = service.GetVehicleByCategory(dto.Data);
                return Ok(List);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }
        [HttpPost]
        public async Task<ActionResult<BaseResponseDTO<List<DropDown>>>> GetEquipmentByCategory([FromBody] BaseRequestDTO<DropdownParameterDTO> dto)
        {
            try
            {
                BaseResponseDTO<List<DropDown>> List = new BaseResponseDTO<List<DropDown>>();
                List = service.GetEquipmentByCategory(dto.Data);
                return Ok(List);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }
        [HttpPost]
        public async Task<ActionResult<BaseResponseDTO<List<DropDown>>>> GetResourceByRank([FromBody] BaseRequestDTO<DropdownParameterDTO> dto)
        {
            try
            {
                BaseResponseDTO<List<DropDown>> List = new BaseResponseDTO<List<DropDown>>();
                List = await service.GetResourceByRank(dto.Data);
                return Ok(List);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }
        [HttpPost]
        public async Task<ActionResult<BaseResponseDTO<List<DropDown>>>> GetResourceByCategory([FromBody] BaseRequestDTO<DropdownParameterDTO> dto)
        {
            try
            {
                BaseResponseDTO<List<DropDown>> List = new BaseResponseDTO<List<DropDown>>();
                List = await    service.GetResourceByCategory(dto.Data);
                return Ok(List);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }
        [HttpPost]
        public async Task<ActionResult<BaseResponseDTO<bool>>> AddDeleteVehicleToTable([FromBody] BaseRequestDTO<NPF_TRN_IncidentVehicle> dto)
        {
            try
            {
                BaseResponseDTO<bool> List = new BaseResponseDTO<bool>();
                List = service.AddDeleteVehicleToTable(dto.Data,dto.userToken);
                return Ok(List);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }
        [HttpPost]
        public async Task<ActionResult<BaseResponseDTO<bool>>> AddDeleteEquipmentToTable([FromBody] BaseRequestDTO<NPF_TRN_IncidentEquipment> dto)
        {
            try
            {
                BaseResponseDTO<bool> List = new BaseResponseDTO<bool>();
                List = service.AddDeleteEquipmentToTable(dto.Data, dto.userToken);
                return Ok(List);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }
        [HttpPost]
        public async Task<ActionResult<BaseResponseDTO<bool>>> AddDeleteResourceToTable([FromBody] BaseRequestDTO<NPF_TRN_IncidentResource> dto)
        {
            try
            {
                BaseResponseDTO<bool> List = new BaseResponseDTO<bool>();
                List = service.AddDeleteResourceToTable(dto.Data, dto.userToken);
                return Ok(List);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }
        [HttpPost]
        public async Task<ActionResult<BaseResponseDTO<bool>>> AddDeleteExternalToTable([FromBody] BaseRequestDTO<NPF_TRN_IncidentExternalAgency> dto)
        {
            try
            {
                BaseResponseDTO<bool> List = new BaseResponseDTO<bool>();
                List = service.AddDeleteExternalToTable(dto.Data, dto.userToken);
                return Ok(List);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }
        [HttpPost]
        public async Task<ActionResult<BaseResponseDTO<bool>>> AddDeleteCasualtyToTable([FromBody] BaseRequestDTO<NPF_TRN_IncidentCasualtyDetails> dto)
        {
            try
            {
                BaseResponseDTO<bool> List = new BaseResponseDTO<bool>();
                List = service.AddDeleteCasualtyToTable(dto.Data, dto.userToken);
                return Ok(List);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }
        [HttpPost]
        public async Task<ActionResult<BaseResponseDTO<bool>>> AddDeleteFatalityToTable([FromBody] BaseRequestDTO<NPF_TRN_IncidentFatalityDetails> dto)
        {
            try
            {
                BaseResponseDTO<bool> List = new BaseResponseDTO<bool>();
                List = service.AddDeleteFatalityToTable(dto.Data, dto.userToken);
                return Ok(List);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }
        [HttpPost]
        public async Task<ActionResult<BaseResponseDTO<bool>>> AddDeleteOtherStationToTable([FromBody] BaseRequestDTO<NPF_TRN_IncidentOtherStation> dto)
        {
            try
            {
                BaseResponseDTO<bool> List = new BaseResponseDTO<bool>();
                List = service.AddDeleteOtherStationToTable(dto.Data, dto.userToken);
                return Ok(List);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }
        [HttpPost]
        public async Task<ActionResult<BaseResponseDTO<bool>>> AddDeleteDamageToTable([FromBody] BaseRequestDTO<NPF_TRN_IncidentDamage> dto)
        {
            try
            {
                BaseResponseDTO<bool> List = new BaseResponseDTO<bool>();
                List = service.AddDeleteDamageToTable(dto.Data, dto.userToken);
                return Ok(List);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }
        [HttpPost]
        public async Task<ActionResult<BaseResponseDTO<bool>>> AssigneResourcecrewCommander([FromBody] BaseRequestDTO<List<IncidentAssignmentDTO>> dto)
        {
            try
            {
                BaseResponseDTO<bool> BaseResponseDTO = new BaseResponseDTO<bool>();
                BaseResponseDTO = await service.AssignResource(dto.Data);
                return Ok(BaseResponseDTO);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }
        #endregion
        #region File
        [HttpPost]
        public async Task<ActionResult<BaseResponseDTO<bool>>> UploadFileIncident([FromForm] IncidentFilesDTO data)
        {
            try
            {
                BaseResponseDTO<bool> BaseResponseDTO = new BaseResponseDTO<bool>();
                BaseResponseDTO = service.UploadFileIncident(data);
                return Ok(BaseResponseDTO);
            }
            catch (Exception ex)
            {

                return BadRequest();

            }
        }
        [HttpPost]
        public async Task<ActionResult<BaseResponseDTO<bool>>> DeleteFileIncident([FromBody] BaseRequestDTO<long> dto)
        {
            try
            {
                BaseResponseDTO<bool> BaseResponseDTO = new BaseResponseDTO<bool>();
                BaseResponseDTO = service.DeleteFileIncident(dto.Data);
                return Ok(BaseResponseDTO);
            }
            catch (Exception ex)
            {

                return BadRequest();

            }
        }
        public async Task<ActionResult> DownloadIncidentFile(long Id)
        {
            try
            {
                BaseResponseDTO<AttachmentDTO> dt = new BaseResponseDTO<AttachmentDTO>();
                dt = await service.DownloadIncidentFile(Id);
                if (dt.QryResult == new QueryResult().SUCEEDED)
                {
                    byte[] bytes = Convert.FromBase64String(dt.Data.File);
                    string fileName = dt.Data.Filename;
                    return File(bytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                return BadRequest();
            }

        }
        #endregion
        #region Chat
        [HttpPost]
        public async Task<ActionResult<BaseResponseDTO<List<ChatBaseDTO>>>> Chatlist([FromBody] BaseRequestDTO<long> dto)
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
        [HttpPost]
        public async Task<ActionResult<BaseResponseDTO<bool>>> Savechat([FromBody] BaseRequestDTO<ChatSaveDTO> dto)
        {
            BaseResponseDTO<bool> baseReponseDto = new BaseResponseDTO<bool>();

            try
            {
                baseReponseDto = await Cservice.Save(dto);
                return Ok(baseReponseDto);

            }
            catch (Exception ex)
            {
                return BadRequest();
            }

        }
        [HttpPost]
        public async Task<ActionResult<BaseResponseDTO<bool>>> AbortCompleted([FromBody] BaseRequestDTO<AbortCompletedDTO> dto)
        {
            BaseResponseDTO<bool> baseReponseDto = new BaseResponseDTO<bool>();

            try
            {
                baseReponseDto = await service.AbortCompleted(dto);
                return Ok(baseReponseDto);

            }
            catch (Exception ex)
            {
                return BadRequest();
            }

        }
        [HttpPost]
        public async Task<ActionResult<BaseResponseDTO<bool>>> Acknowledge([FromBody] BaseRequestDTO<long> dto)
        {
            BaseResponseDTO<bool> baseReponseDto = new BaseResponseDTO<bool>();

            try
            {
                baseReponseDto = await service.Acknowledge(dto);
                return Ok(baseReponseDto);

            }
            catch (Exception ex)
            {
                return BadRequest();
            }

        }
        #endregion
        #region RequestCR
        [HttpPost]
        public async Task<ActionResult<BaseResponseDTO<List<CRRequestListDTO>>>> CRRequestList([FromBody] BaseRequestDTO<long> dto)
        {
            try
            {
                BaseResponseDTO<List<CRRequestListDTO>> List = new BaseResponseDTO<List<CRRequestListDTO>>();
                List = await service.GetCRRequestList(dto.Data);
                return Ok(List);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }
        [HttpPost]
        public async Task<ActionResult<BaseResponseDTO<bool>>> UpdateCRRequestDTO([FromBody] BaseRequestDTO<UpdateCRRequestDTO> dto)
        {
            try
            {
                BaseResponseDTO<bool> BaseResponseDTO = new BaseResponseDTO<bool>();
                BaseResponseDTO = await service.UpdateCRRequest(dto.Data,dto.userToken);
                return Ok(BaseResponseDTO);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }
        #endregion
        #region PDA
        [HttpPost]
        public async Task<ActionResult<BaseResponseDTO<IncidentPDADTO>>> GetIncidentPDA([FromBody] BaseRequestDTO<IncidentPDASearchDTO> dto)
        {
            try
            {
                BaseResponseDTO<IncidentPDADTO> BaseResponseDTO = new BaseResponseDTO<IncidentPDADTO>();
                BaseResponseDTO = await service.GetIncidentPDA(dto.Data);
                return Ok(BaseResponseDTO);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }
        #endregion
    }
}
