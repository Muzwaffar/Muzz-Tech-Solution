using BL.Models.Form;
using BL.Models.Common;
using BL.Services.Access.AccessRights;
using BL.Services.Administration;
using BL.Services.Form;
using DAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UI.Helper;

namespace UI.Controllers.Form
{
    public class FormTypeController : NPFController
    {
        public UserManager<ApplicationUser> _userManager;
        public FormTypeService service;
        public AccessRightService Aservice;
        public FormTypeController(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager, RoleManager<ApplicationRole> rolemanager) : base(userManager, signInManager, rolemanager)
        {
            service = new FormTypeService();
            _userManager = userManager;
            Aservice = new AccessRightService();
        }
        public async Task<ActionResult> List()
        {
            BaseResponseDTO<string> dt = new BaseResponseDTO<string>();
            dt = await Aservice.GetViewTitle("Form Type");
            ViewBag.Name = dt.Data;
            return View("~/Views/Modules/Form/FormType/FormTypeList.cshtml");
        }

        [HttpPost]
        public async Task<ActionResult<BaseResponseDTO<List<FormTypeDTO>>>> GetList()
        {
            try
            {
                BaseResponseDTO<List<FormTypeDTO>> FormList = new BaseResponseDTO<List<FormTypeDTO>>();
                FormList = service.GetAll();
                return Ok(FormList);


            }
            catch (Exception ex)
            {

                return BadRequest();

            }
        }
        [HttpPost]
        public async Task<ActionResult> GetById([FromBody] BaseRequestDTO<int> dto)
        {
            try
            {
                if (dto == null)
                {
                    FormTypeDTO dt = new FormTypeDTO();
                    return PartialView("~/Views/Modules/Form/FormType/FormTypeAddEdit.cshtml", dt);
                }
                else
                {
                    BaseResponseDTO<FormTypeDTO> dt = new BaseResponseDTO<FormTypeDTO>();
                    dt = service.GetById(dto.Data);
                    return PartialView("~/Views/Modules/Form/FormType/FormTypeAddEdit.cshtml", dt.Data);
                }

            }
            catch (Exception ex)
            {

                return BadRequest();

            }
        }

        [HttpPost]
        public async Task<ActionResult<BaseResponseDTO<bool>>> CreateUpdate([FromBody] BaseRequestDTO<FormTypeDTO> dto)
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
