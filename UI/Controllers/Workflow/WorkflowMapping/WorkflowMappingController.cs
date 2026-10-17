using BL.Models.Common;
using BL.Models.Template;
using BL.Models.Workflow;
using BL.Services.Access.AccessRights;
using BL.Services.Administration;
using BL.Services.Workflow.WorkflowMapping;
using DAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UI.Helper;

namespace UI.Controllers.FormPage.Test
{
    public class WorkflowMappingController : NPFController
    {
        public UserManager<ApplicationUser> _userManager;
        public WorkflowMappingService service;
        public AccessRightService Aservice;
        public WorkflowMappingController(Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> userManager,
           SignInManager<ApplicationUser> signInManager, Microsoft.AspNetCore.Identity.RoleManager<ApplicationRole> rolemanager) : base(userManager, signInManager, rolemanager)
        {
            service = new WorkflowMappingService();
            _userManager = userManager;
            Aservice = new AccessRightService();
        }
        public async Task<ActionResult> List()
        {
            BaseResponseDTO<string> dt = new BaseResponseDTO<string>();
            dt = await Aservice.GetViewTitle("Workflow Mapping");
            ViewBag.Name = dt.Data;
            return View("~/Views/Modules/Workflow/WorkflowMapping/WorkflowMapping.cshtml");
        }
        public async Task<ActionResult> AddEditView(long ID, Guid MId)
        {
            try
            {
                if (ID == null || ID == 0)
                {
                    BaseResponseDTO<string> dt = new BaseResponseDTO<string>();
                    dt = await Aservice.GetViewTitle("Workflow Mapping");
                    ViewBag.Name = dt.Data;
                    WorkflowMappingCruDTO FP = new WorkflowMappingCruDTO();
                    return View("~/Views/Modules/Workflow/WorkflowMapping/WorkflowMappingAddEdit.cshtml", FP);
                }
                else
                {
                    ViewBag.Name = "Workflow Mapping";
                    BaseResponseDTO<WorkflowMappingCruDTO> FP = new BaseResponseDTO<WorkflowMappingCruDTO>();
                    FP = service.GetById(ID);
                    return View("~/Views/Modules/Workflow/WorkflowMapping/WorkflowMappingAddEdit.cshtml", FP.Data);
                }

            }
            catch (Exception ex)
            {

                return BadRequest();

            }
        }

        [HttpPost]
        public async Task<ActionResult<BaseResponseDTO<List<WorkflowMappingListDTO>>>> GetList()
        {
            try
            {
                BaseResponseDTO<List<WorkflowMappingListDTO>> SRList = new BaseResponseDTO<List<WorkflowMappingListDTO>>();
                SRList = service.GetAll();
                return Ok(SRList);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpPost]
        public async Task<ActionResult<BaseResponseDTO<List<DropDownLogin>>>> GetAllDropDownValues()
        {
            try
            {
                BaseResponseDTO<List<DropDownLogin>> List = new BaseResponseDTO<List<DropDownLogin>>();
                List = service.GetAllDropDownValues();
                return Ok(List);


            }
            catch (Exception ex)
            {

                return BadRequest();

            }
        }

        [HttpPost]
        public async Task<ActionResult<BaseResponseDTO<List<DropDown>>>> GetURLName([FromBody] BaseRequestDTO<string> dt)
        {
            try
            {
                BaseResponseDTO<string> UrlName = new BaseResponseDTO<string>();
                UrlName = service.GetURLName(dt.Data);
                return Ok(UrlName);


            }
            catch (Exception ex)
            {

                return BadRequest();

            }
        }

        [HttpPost]
        public async Task<ActionResult<BaseResponseDTO<bool>>> CreateUpdate([FromBody] BaseRequestDTO<WorkflowMappingDTO> dto)
        {
            try
            {
                BaseResponseDTO<bool> BaseResponseDTO = new BaseResponseDTO<bool>();
                if (dto.Data.Id == 0)
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
        public async Task<ActionResult<BaseResponseDTO<List<WorkflowMappingDetailsDTO>>>> GetWFDTable([FromBody] BaseRequestDTO<long> dt)
        {
            try
            {
                BaseResponseDTO<List<WorkflowMappingDetailsDTO>> List = new BaseResponseDTO<List<WorkflowMappingDetailsDTO>>();
                List = await service.GetTableById(dt.Data);
                return Ok(List);


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
    }
}
