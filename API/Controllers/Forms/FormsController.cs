using BL.Models.Common;
using BL.Models.FormPage;
using BL.Services.Activities;
using BL.Services.ServiceRequest;
using DAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Forms
{
    [Route("api/[controller]")]
    [ApiController]
    public class FormsController : ControllerBase
    {

        private readonly Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private FormPageService service;
        public WorkflowService WFservice;
        public FormsController(Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> userManager,
                              SignInManager<ApplicationUser> signInManager, RoleManager<ApplicationRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            service = new FormPageService();
            WFservice = new WorkflowService();
        }

        [HttpPost("getFormPageById")]
        public async Task<ActionResult<BaseResponseDTO<FormPageDTO>>> GetFormPageById([FromBody] BaseRequestDTO<long> dto)
        {
            try
            {
                if (dto.Data == null || dto.Data == 0)
                {
                    BaseResponseDTO<FormPageDTO> SSR = await service.GetDTO(Guid.NewGuid());
                    return Ok(SSR);

                }
                else
                {
                    BaseResponseDTO<FormPageDTO> SSR = new BaseResponseDTO<FormPageDTO>();
                    SSR = await WFservice.WFGetById(dto.Data, Guid.NewGuid(),true);
                    return Ok(SSR);

                }


            }
            catch (Exception ex)
            {

                return BadRequest();

            }
        }
        [HttpPost("UpdateForm")]
        public async Task<ActionResult<BaseResponseDTO<bool>>> UpdateForm([FromBody] BaseRequestDTO<FormPageDTO> dt)
        {

            try
            {
                BaseResponseDTO<bool> BaseResponseDTO = new BaseResponseDTO<bool>();
                BaseResponseDTO = await service.UpdateAsync(dt);
                return Ok(BaseResponseDTO);
            }
            catch (Exception ex)
            {

                return BadRequest();

            }
        }
        [HttpPost("getCustomDetailsByElementId")]
        public async Task<ActionResult<BaseResponseDTO<List<CustomDictionary>>>> GetCustomDetailsByElementId([FromBody] BaseRequestDTO<DetailsByIdSearchDTO> dto)
        {
            try
            {
                BaseResponseDTO<List<CustomDictionary>> SA = new BaseResponseDTO<List<CustomDictionary>>();
                SA = await service.GetCustomDetailsByElementId(dto.Data);
                return Ok(SA);
            }
            catch (Exception ex)
            {

                return BadRequest();

            }
        }

    }
}
