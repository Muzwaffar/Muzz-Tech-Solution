using BL.Constants;
using BL.Models.Common;
using BL.Models.Dashboard;
using BL.Models.Modules.Research;
using BL.Services.Common;
using BL.Services.Dashboard;
using DAL.Common;
using DAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using UI.Helper;

namespace UI.Controllers.Common
{
    public class FileUploadController : NPFController
    {
        public UserManager<ApplicationUser> _userManager;
        public FileuploadService service;
        public FileUploadController(Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> userManager,
           SignInManager<ApplicationUser> signInManager, Microsoft.AspNetCore.Identity.RoleManager<ApplicationRole> rolemanager) : base(userManager, signInManager, rolemanager)
        {
            service = new FileuploadService();
            _userManager = userManager;
        }
        [HttpPost]
        public async Task<IActionResult> UploadFiles([FromForm] FileUploadDTO data)
        {
            try
            {

                await service.SaveFile(data);

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
        [HttpPost]
        public async Task<ActionResult<BaseResponseDTO<bool>>> UploadSingleFile([FromForm] SingleFilesDTO data)
        {
            try
            {
                BaseResponseDTO<bool> BaseResponseDTO = new BaseResponseDTO<bool>();
                BaseResponseDTO = await service.SaveSingleFile(data);
                return Ok(BaseResponseDTO);


            }
            catch (Exception ex)
            {

                return BadRequest();

            }
        }
        [HttpPost]
        public async Task<ActionResult<BaseResponseDTO<List<AttachmentListDTO>>>> GetAttachementList([FromBody] BaseRequestDTO<GetAttachmentDTO> dto)
        {
            try
            {

                BaseResponseDTO<List<AttachmentListDTO>> BaseResponseDTO = new BaseResponseDTO<List<AttachmentListDTO>>();
                BaseResponseDTO = await service.GetAttachementList(dto.Data);
                return Ok(BaseResponseDTO);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
        public async Task<ActionResult> DownloadFile(long Id)
        {
            try
            {
                BaseResponseDTO<AttachmentDTO> dt = new BaseResponseDTO<AttachmentDTO>();
                dt = await service.DownloadFile(Id);
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

    }
}
