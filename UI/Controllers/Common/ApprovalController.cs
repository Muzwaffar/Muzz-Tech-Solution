using BL.Models.Common;
using BL.Models.Modules.Common;
using BL.Models.Modules.Research;
using BL.Models.Workflow;
using BL.Services.Common;
using DAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UI.Helper;

namespace UI.Controllers.Common
{
    public class ApprovalController : NPFController
    {
        public UserManager<ApplicationUser> _userManager;
        private ApprovalService service;
        public ApprovalController(Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> userManager,
           SignInManager<ApplicationUser> signInManager, Microsoft.AspNetCore.Identity.RoleManager<ApplicationRole> rolemanager) : base(userManager, signInManager, rolemanager)
        {
            _userManager = userManager;
            service = new ApprovalService();
        }

        public async Task<ActionResult> Approval([FromBody] BaseRequestDTO<WorkFlowDTO> dto)
        {
            return PartialView("~/Views/Modules/Common/Approval/ApprovalService.cshtml", dto.Data);
        }
        public async Task<ActionResult<BaseResponseDTO<bool>>> SendForApproval([FromBody] BaseRequestDTO<ApprovalServiceDTO> dto)
        {
            try
            {
                BaseResponseDTO<bool> BaseResponseDTO = new BaseResponseDTO<bool>();
                BaseResponseDTO = await service.SendForApproval(dto.Data);
                return Ok(BaseResponseDTO);
            }
            catch (Exception ex)
            {

                return BadRequest();

            }
        }
        public async Task<ActionResult<BaseResponseDTO<ApprovalDTO>>> GetAssignToMe([FromBody] BaseRequestDTO<string> dto)
        {
            try
            {
                BaseResponseDTO<ApprovalDTO> BaseResponseDTO = new BaseResponseDTO<ApprovalDTO>();
                BaseResponseDTO = await service.GetAssignToMe(dto.userToken);
                return Ok(BaseResponseDTO);
            }
            catch (Exception ex)
            {

                return BadRequest();

            }
        }
        public async Task<ActionResult<BaseResponseDTO<List<DropDown>>>> GetDepartmentDropdownValues()
        {
            try
            {
                BaseResponseDTO<List<DropDown>> BaseResponseDTO = new BaseResponseDTO<List<DropDown>>();
                BaseResponseDTO = await service.GetDepartment();
                return Ok(BaseResponseDTO);
            }
            catch (Exception ex)
            {
                return BadRequest();

            }
        }
        public async Task<ActionResult<BaseResponseDTO<List<DropDown>>>> GetUsersDropdownValues([FromBody] BaseRequestDTO<ApprovalServiceDropdownDTO> dto)
        {
            try
            {
                BaseResponseDTO<List<DropDown>> BaseResponseDTO = new BaseResponseDTO<List<DropDown>>();
                BaseResponseDTO = await service.GetUsers(dto.Data);
                return Ok(BaseResponseDTO);
            }
            catch (Exception ex)
            {
                return BadRequest();

            }
        }
        public async Task<ActionResult<BaseResponseDTO<bool>>> ApproveDecline([FromBody] BaseRequestDTO<ApprovalServiceDTO> dto)
        {
            try
            {
                BaseResponseDTO<bool> BaseResponseDTO = new BaseResponseDTO<bool>();
                BaseResponseDTO = await service.ApproveDecline(dto.Data);
                return Ok(BaseResponseDTO);
            }
            catch (Exception ex)
            {

                return BadRequest();

            }
        }
    }
}
