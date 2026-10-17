using BL.Models.Common;
using BL.Models.Form;
using BL.Models.Template;
using BL.Services.Access.AccessRights;
using BL.Services.Administration;
using BL.Services.Form.FormConfig;
using DAL.Common;
using DAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UI.Helper;

namespace UI.Controllers.Form.FormConfig
{
    public class FormSectionAndElementModuleMappingController : NPFController
    {
        public UserManager<ApplicationUser> _userManager;
        public FormSectionAndElementModuleMappingService service;
        public AccessRightService Aservice;
        public FormSectionAndElementModuleMappingController(UserManager<ApplicationUser> userManager,
           SignInManager<ApplicationUser> signInManager, RoleManager<ApplicationRole> rolemanager) : base(userManager, signInManager, rolemanager)
        {
            service = new FormSectionAndElementModuleMappingService();
            _userManager = userManager;
            Aservice = new AccessRightService();
        }
        public async Task<ActionResult> List()
        {
            BaseResponseDTO<string> dt = new BaseResponseDTO<string>();
            dt = await Aservice.GetViewTitle("Form Section And Element Module Mapping");
            ViewBag.Name = dt.Data;
            return View("~/Views/Modules/Form/FormConfig/FormSectionAndElementModuleMapping/FormSectionAndElementModuleMapping.cshtml");
        }
        public async Task<ActionResult> AddEditView(long ID, Guid MId)
        {
            try
            {
                if (ID == null || ID == 0)
                {
                    ViewBag.Name = "Form Section And Element Module Mapping";
                    BaseResponseDTO<FormSectionAndElementModuleMappingCRUDTO> FP = new BaseResponseDTO<FormSectionAndElementModuleMappingCRUDTO>();
                    return View("~/Views/Modules/Form/FormConfig/FormSectionAndElementModuleMapping/FormSectionAndElementModuleMappingAddEdit.cshtml", FP.Data);
                }
                else
                {
                    ViewBag.Name = "Form Section And Element Module Mapping";
                    BaseResponseDTO<FormSectionAndElementModuleMappingCRUDTO> FP = new BaseResponseDTO<FormSectionAndElementModuleMappingCRUDTO>();
                    FP = service.GetById(ID);
                    return View("~/Views/Modules/Form/FormConfig/FormSectionAndElementModuleMapping/FormSectionAndElementModuleMappingAddEdit.cshtml", FP.Data);
                }

            }
            catch (Exception ex)
            {

                return BadRequest();

            }
        }

        [HttpPost]
        public async Task<ActionResult<BaseResponseDTO<List<FormSectionAndElementModuleMappingListDTO>>>> GetList()
        {
            try
            {
                BaseResponseDTO<List<FormSectionAndElementModuleMappingListDTO>> SRList = new BaseResponseDTO<List<FormSectionAndElementModuleMappingListDTO>>();
                SRList = service.GetAll();
                return Ok(SRList);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpPost]
        public async Task<ActionResult<BaseResponseDTO<List<DropDownLogin>>>> GetAllDropdownValues()
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
        public async Task<ActionResult<BaseResponseDTO<bool>>> CreateUpdate([FromBody] BaseRequestDTO<FormSectionAndElementModuleMappingDTO> dto)
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
        public async Task<ActionResult<BaseResponseDTO<List<ElementMappingDTO>>>> GetSEElementTable([FromBody] BaseRequestDTO<FormSectionAndElementModuleMappingTableDTO> dt)
        {
            try
            {
                BaseResponseDTO<List<ElementMappingDTO>> List = new BaseResponseDTO<List<ElementMappingDTO>>();
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
