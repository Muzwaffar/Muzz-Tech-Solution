using BL.Models.Common;
using BL.Models.Template;
using BL.Services.Access.AccessRights;
using BL.Services.Common;
using DAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UI.Helper;

namespace UI.Controllers.Common
{
    public class ExportController : NPFController
    {
        public ISOExportService service;
        public ExportController(Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> userManager,
           SignInManager<ApplicationUser> signInManager, Microsoft.AspNetCore.Identity.RoleManager<ApplicationRole> rolemanager) : base(userManager, signInManager, rolemanager)
        {
            service = new ISOExportService();
        }
        [HttpPost]
        public async Task<ActionResult<BaseResponseDTO<string>>> ExportISO([FromBody] BaseRequestDTO<TemplateDownloadDTO> dto)
        {
            try
            {
                //long x = long.Parse(dto.Data);
                BaseResponseDTO<string> BaseResponseDTO = new BaseResponseDTO<string>();
                BaseResponseDTO = await service.ExportISO(dto);

                return Ok(BaseResponseDTO);
            }
            catch (Exception ex)
            {

                return BadRequest();

            }
        }
    }
}
