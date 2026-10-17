using BL.Models.Common;
using BL.Models.FormPage;
using BL.Models.Modules.Research;
using BL.Models.Template;
using BL.Services.Access.AccessRights;
using BL.Services.Activities;
using BL.Services.Administration;
using BL.Services.Form.FormConfig;
using BL.Services.ServiceRequest;
using DAL.Common;
using DAL.Models;
using DAL.Models.FormPage;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UI.Helper;

namespace UI.Controllers.Form.FormConfig
{
    public class FormPagePDFMappingController : NPFController
    {
        public UserManager<ApplicationUser> _userManager;
        public FormPagePDFMappingService service;
        public AccessRightService Aservice;
        public FormPagePDFMappingController(UserManager<ApplicationUser> userManager,
           SignInManager<ApplicationUser> signInManager, RoleManager<ApplicationRole> rolemanager) : base(userManager, signInManager, rolemanager)
        {
            service = new FormPagePDFMappingService();
            _userManager = userManager;
            Aservice = new AccessRightService();
        }
        public async Task<ActionResult> List()
        {
            BaseResponseDTO<string> dt = new BaseResponseDTO<string>();
            dt = await Aservice.GetViewTitle("Form Page PDF Mapping");
            ViewBag.Name = dt.Data;
            return View("~/Views/Modules/Form/FormConfig/FormPagePDFMapping/FormPagePDFMapping.cshtml");
        }
        public async Task<ActionResult> AddEditView(long ID, Guid MId)
        {
            try
            {
                if (ID == null || ID == 0)
                {
                    ViewBag.Name = "Form Page PDF Mapping";
                    BaseResponseDTO<TemplateFormMappingCRUDTO> FP = new BaseResponseDTO<TemplateFormMappingCRUDTO>();
                    return View("~/Views/Modules/Form/FormConfig/FormPagePDFMapping/FormPagePDFMappingAddEdit.cshtml", FP.Data);
                }
                else
                {
                    ViewBag.Name = "Form Page PDF Mapping";
                    BaseResponseDTO<TemplateFormMappingCRUDTO> FP = new BaseResponseDTO<TemplateFormMappingCRUDTO>();
                    FP = service.GetById(ID);
                    return View("~/Views/Modules/Form/FormConfig/FormPagePDFMapping/FormPagePDFMappingAddEdit.cshtml", FP.Data);
                }

            }
            catch (Exception ex)
            {

                return BadRequest();

            }
        }

        [HttpPost]
        public async Task<ActionResult<BaseResponseDTO<List<TemplateFormMappingListDTO>>>> GetList()
        {
            try
            {
                BaseResponseDTO<List<TemplateFormMappingListDTO>> SRList = new BaseResponseDTO<List<TemplateFormMappingListDTO>>();
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
        public async Task<ActionResult<BaseResponseDTO<List<DropDown>>>> GetSectionById([FromBody] BaseRequestDTO<long> dt)
        {
            try
            {
                BaseResponseDTO<List<DropDown>> List = new BaseResponseDTO<List<DropDown>>();
                List = service.GetSectionById(dt.Data);
                return Ok(List);


            }
            catch (Exception ex)
            {

                return BadRequest();

            }
        }

        [HttpPost]
        public async Task<ActionResult<BaseResponseDTO<List<DropDown>>>> GetElementById([FromBody] BaseRequestDTO<long> dt)
        {
            try
            {
                BaseResponseDTO<List<DropDown>> List = new BaseResponseDTO<List<DropDown>>();
                List = service.GetElementById(dt.Data);
                return Ok(List);


            }
            catch (Exception ex)
            {

                return BadRequest();

            }
        }

        [HttpPost]
        public async Task<ActionResult<BaseResponseDTO<List<DropDown>>>> GetTemplateTextById([FromBody] BaseRequestDTO<string> dt)
        {
            try
            {
                BaseResponseDTO<List<DropDown>> List = new BaseResponseDTO<List<DropDown>>();
                List = service.GetTemplateTextById(dt.Data);
                return Ok(List);


            }
            catch (Exception ex)
            {

                return BadRequest();

            }
        }

        [HttpPost]
        public async Task<ActionResult<BaseResponseDTO<bool>>> CreateUpdate([FromBody] BaseRequestDTO<TemplateFormMappingDTO> dto)
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
        public async Task<ActionResult<BaseResponseDTO<List<TemplateElementMappingDTO>>>> GetPDFElementTable([FromBody] BaseRequestDTO<long> dt)
        {
            try
            {
                BaseResponseDTO<List<TemplateElementMappingDTO>> List = new BaseResponseDTO<List<TemplateElementMappingDTO>>();
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
