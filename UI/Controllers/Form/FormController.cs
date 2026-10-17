using BL.Models.Form;
using BL.Models.Common;
using BL.Services.Access.AccessRights;
using BL.Services.Form;
using DAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UI.Helper;
using DAL.Models.Form;

namespace UI.Controllers.Form
{
    public class FormController : NPFController
    {
        public UserManager<ApplicationUser> _userManager;
        public FormService service;
        public AccessRightService Aservice;
        public FormController(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager, RoleManager<ApplicationRole> rolemanager) : base(userManager, signInManager, rolemanager)
        {
            service = new FormService();
            _userManager = userManager;
            Aservice = new AccessRightService();
        }
        public async Task<ActionResult> List()
        {
            BaseResponseDTO<string> dt = new BaseResponseDTO<string>();
            dt = await Aservice.GetViewTitle("Form");
            ViewBag.Name = dt.Data;
            return View("~/Views/Modules/Form/Form/FormList.cshtml");
        }

        [HttpPost]
        public async Task<ActionResult<BaseResponseDTO<List<FormListDTO>>>> GetList()
        {
            try
            {
                BaseResponseDTO<List<FormListDTO>> FormList = new BaseResponseDTO<List<FormListDTO>>();
                FormList = service.GetAll();
                return Ok(FormList);


            }
            catch (Exception ex)
            {

                return BadRequest();

            }
        }
        [HttpPost]
        public async Task<ActionResult> GetById([FromBody] BaseRequestDTO<long> dto)
        {
            try
            {
                if (dto.Data == 0)
                {
                    FormDTO dt = new FormDTO();
                    return PartialView("~/Views/Modules/Form/Form/FormAddEdit.cshtml", dt);
                }
                else
                {
                    BaseResponseDTO<FormDTO> dt = new BaseResponseDTO<FormDTO>();
                    dt = service.GetById(dto.Data);
                    return PartialView("~/Views/Modules/Form/Form/FormAddEdit.cshtml", dt.Data);
                }

            }
            catch (Exception ex)
            {

                return BadRequest();

            }
        }

        [HttpPost]
        public async Task<ActionResult<BaseResponseDTO<bool>>> CreateUpdate([FromBody] BaseRequestDTO<FormSectionDTO> dto)
        {
            try
            {
                BaseResponseDTO<bool> BaseResponseDTO = new BaseResponseDTO<bool>();
                if (dto.Data.Form.Id == 0)
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
        public async Task<ActionResult<BaseResponseDTO<List<DropDown>>>> GetAllDropdownValues()
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
        public async Task<ActionResult<BaseResponseDTO<List<NPF_FRM_Section>>>> GetAllSection([FromBody] BaseRequestDTO<long> dto)
        {
            try
            {
                BaseResponseDTO<List<NPF_FRM_Section>> List = new BaseResponseDTO<List<NPF_FRM_Section>>();
                List = service.GetAllSection(dto.Data);
                return Ok(List);


            }
            catch (Exception ex)
            {

                return BadRequest();

            }
        }
    }
}
