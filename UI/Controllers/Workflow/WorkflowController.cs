using BL.Constants;
using BL.Models.Activities;
using BL.Models.Common;
using BL.Models.Dashboard;
using BL.Models.Modules.Common;
using BL.Models.Workflow;
using BL.Services.Activities;
using DAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UI.Helper;

namespace UI.Controllers.Workflow.WorkflowMapping
{
    public class WorkflowController : NPFController
    {
        public UserManager<ApplicationUser> _userManager;
        public WorkflowService service;
        public WorkflowController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<ApplicationRole> rolemanager) : base(userManager, signInManager, rolemanager)
        {
            _userManager = userManager;
            service = new WorkflowService();
        }

        public async Task<ActionResult> List()
        {
            BaseResponseDTO<string> dt = new BaseResponseDTO<string>();
            return View("~/Views/Modules/Workflow/WorkflowMapping/WorkflowMapping.cshtml");
        }
        public async Task<ActionResult<BaseResponseDTO<bool>>> WorkflowSave([FromBody] BaseRequestDTO<WorkFlowDTO> dto)
        {
            try
            {
                BaseResponseDTO<bool> BaseResponseDTO = new BaseResponseDTO<bool>();
                BaseResponseDTO = await service.WorkflowSave(dto, _userManager);
                return Ok(BaseResponseDTO);
            }
            catch (Exception ex)
            {

                return BadRequest();

            }
        }
        public async Task<ActionResult<BaseResponseDTO<bool>>> GetWorkflowSeq([FromBody] BaseRequestDTO<Guid> dto)
        {
            try
            {
                BaseResponseDTO<bool> BaseResponseDTO = new BaseResponseDTO<bool>();
                BaseResponseDTO = await service.GetWorkflowSeq(dto);
                return Ok(BaseResponseDTO);
            }
            catch (Exception ex)
            {

                return BadRequest();

            }
        }
    }
}
