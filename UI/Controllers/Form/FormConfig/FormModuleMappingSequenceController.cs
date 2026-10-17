using BL.Models.Administration;
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
    public class FormModuleMappingSequenceController : NPFController
    {
        public UserManager<ApplicationUser> _userManager;
        public FormModuleMappingSequenceService service;
        public AccessRightService Aservice;
        public FormModuleMappingSequenceController(UserManager<ApplicationUser> userManager,
           SignInManager<ApplicationUser> signInManager, RoleManager<ApplicationRole> rolemanager) : base(userManager, signInManager, rolemanager)
        {
            service = new FormModuleMappingSequenceService();
            _userManager = userManager;
            Aservice = new AccessRightService();
        }
        public async Task<ActionResult> List()
        {
            BaseResponseDTO<string> dt = new BaseResponseDTO<string>();
            dt = await Aservice.GetViewTitle("Form Module Mapping Sequence");
            ViewBag.Name = dt.Data;
            return View("~/Views/Modules/Form/FormConfig/FormModuleMappingSequence/FormModuleMappingSequence.cshtml");
        }
        [HttpPost]
        public async Task<ActionResult> GetById([FromBody] BaseRequestDTO<int> dto)
        {
            try
            {
                if (dto == null)
                {
                    FormModuleMappingSequenceDTO dt = new FormModuleMappingSequenceDTO();
                    return PartialView("~/Views/Modules/Form/FormConfig/FormModuleMappingSequence/FormModuleMappingSequenceAddEdit.cshtml", dt);
                }
                else
                {
                    BaseResponseDTO<FormModuleMappingSequenceDTO> dt = new BaseResponseDTO<FormModuleMappingSequenceDTO>();
                    dt = service.GetById(dto.Data);
                    return PartialView("~/Views/Modules/Form/FormConfig/FormModuleMappingSequence/FormModuleMappingSequenceAddEdit.cshtml", dt.Data);
                }

            }
            catch (Exception ex)
            {

                return BadRequest();

            }
        }

        [HttpPost]
        public async Task<ActionResult<BaseResponseDTO<List<FormModuleMappingSequenceListDTO>>>> GetList()
        {
            try
            {
                BaseResponseDTO<List<FormModuleMappingSequenceListDTO>> SRList = new BaseResponseDTO<List<FormModuleMappingSequenceListDTO>>();
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
        public async Task<ActionResult<BaseResponseDTO<bool>>> CreateUpdate([FromBody] BaseRequestDTO<FormModuleMappingSequenceDTO> dto)
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
