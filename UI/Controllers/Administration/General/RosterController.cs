using BL.Constants;
using BL.Models.Administration;
using BL.Models.Common;
using BL.Models.Store;
using BL.Services.Access.AccessRights;
using BL.Services.Administration;
using DAL.Models;
using DAL.Models.Administration;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UI.Helper;

namespace UI.Controllers.Resource
{
    public class RosterController : NPFController
    {
        public UserManager<ApplicationUser> _userManager;
        public RosterService service;
        public AccessRightService Aservice;
        public RosterController(Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> userManager,
           SignInManager<ApplicationUser> signInManager, Microsoft.AspNetCore.Identity.RoleManager<ApplicationRole> rolemanager) : base(userManager, signInManager, rolemanager)
        {
            service = new RosterService();
            _userManager = userManager;
            Aservice = new AccessRightService();
        }
        public async Task<ActionResult> List()
        {
            BaseResponseDTO<string> dt = new BaseResponseDTO<string>();
            dt = await Aservice.GetViewTitle("Roster");
            ViewBag.Name = dt.Data;
            return View("~/Views/Modules/Administration/General/Roster/RosterList.cshtml");
        }
        public async Task<ActionResult> RosterAddEdit(long? Id, Guid MId)
        {
            try
            {
                ViewBag.Name = "Roster";
                BaseResponseDTO<RosterShiftResourceMappingDTO> dt = new BaseResponseDTO<RosterShiftResourceMappingDTO>(); 
                if (Id == null)
                {
                    return View("/Views/Modules/Administration/General/Roster/RosterAddEdit.cshtml", dt.Data);
                }
                else
                {
                    dt = service.GetById(Id);
                    return View("/Views/Modules/Administration/General/Roster/RosterAddEdit.cshtml", dt.Data);
                }

                //if (Id == null || Id == 0)
                //{
                //    RosterShiftResourceMappingDTO dt = new RosterShiftResourceMappingDTO();
                //    dt.Roster.Code = "AUTO-GENERATED";
                //    return PartialView("~/Views/Modules/Administration/General/Roster/RosterAddEdit.cshtml", dt);
                //}
                //else
                //{
                //    BaseResponseDTO<RosterShiftResourceMappingDTO> dt = new BaseResponseDTO<RosterShiftResourceMappingDTO>();
                //    dt = service.GetById(Id);
                //    return PartialView("~/Views/Modules/Administration/General/Roster/RosterAddEdit.cshtml", dt.Data);
                //}

            }
            catch (Exception ex)
            {

                return BadRequest();

            }
        }
        [HttpPost]
        public async Task<ActionResult<BaseResponseDTO<List<NPF_SYS_RosterShiftResourceMapping>>>> GetList()
        {
            try
            {
                BaseResponseDTO<List<NPF_SYS_RosterShiftResourceMapping>> RosList = new BaseResponseDTO<List<NPF_SYS_RosterShiftResourceMapping>>();
                RosList = service.GetAll();
                return Ok(RosList);


            }
            catch (Exception ex)
            {

                return BadRequest();

            }
        }


        [HttpPost]
        public async Task<ActionResult<BaseResponseDTO<bool>>> CreateUpdate([FromBody] BaseRequestDTO<RosterShiftResourceMappingDTO> dto)
        {
            try
            {
                GenericConstant con = new GenericConstant();
                BaseResponseDTO<bool> BaseResponseDTO = new BaseResponseDTO<bool>();
                if (dto.Data.Roster.Id == 0)
                {
                    BaseResponseDTO = await service.SaveAsync(dto);
                }
                else
                {
                    BaseResponseDTO = await service.UpdateAsync(dto);
                }

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
                BaseResponseDTO = service.Delete(dto);
                return Ok(BaseResponseDTO);
            }
            catch (Exception ex)
            {

                return BadRequest();

            }
        }

        [HttpPost]
        public async Task<ActionResult<BaseResponseDTO<List<NPF_SYS_RosterResourceMapping>>>> GetDetailsById([FromBody] BaseRequestDTO<long> dto)
        {
            try
            {
                BaseResponseDTO<List<NPF_SYS_RosterResourceMapping>> mappingList = new BaseResponseDTO<List<NPF_SYS_RosterResourceMapping>>();
                mappingList = service.GetDetailsById(dto.Data);
                return Ok(mappingList);
            }
            catch (Exception ex)
            {

                return BadRequest();

            }
        }

        [HttpPost]
        public async Task<ActionResult<BaseResponseDTO<List<DropDown>>>> GetAllDropDownValues()
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
        public async Task<ActionResult<BaseResponseDTO<RosterShiftResourceMappingDTO>>> GetDaySelectionByRosterId([FromBody] BaseRequestDTO<long> dto)
        {
            try
            {
                return Ok(await service.GetDaySelectionRosterId(dto.Data));
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpPost]
        public async Task<ActionResult<BaseResponseDTO<RosterShiftResourceMappingDTO>>> GetIndividualShifts([FromBody] BaseRequestDTO<RosterShiftResourceMappingDTO> dto)
        {
            try
            {
                return Ok(await service.GetIndividualShifts(dto));
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpPost]
        public async Task<ActionResult<BaseResponseDTO<List<NPF_SYS_RosterShiftResourceMapping>>>> FilterDatatableByRosterPeriod([FromBody] BaseRequestDTO<NPF_SYS_Roster> dto)
        {
            try
            {
                BaseResponseDTO<List<NPF_SYS_RosterShiftResourceMapping>> RosList = new BaseResponseDTO<List<NPF_SYS_RosterShiftResourceMapping>>();
                RosList = service.filterDatatableByRosterPeriod(dto);
                return Ok(RosList);


            }
            catch (Exception ex)
            {

                return BadRequest();

            }
        }
    }
}
