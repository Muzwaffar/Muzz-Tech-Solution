using BL.Models.Access;
using BL.Models.Administration;
using BL.Models.Common;
using BL.Models.Form;
using BL.Models.Modules.Research;
using BL.Services.Access.AccessRights;
using BL.Services.Administration;
using BL.Services.Form;
using DAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UI.Helper;

namespace UI.Controllers.Form
{
    public class ElementFormConfigController : NPFController
    {
        public UserManager<ApplicationUser> _userManager;
        public ElementFormConfigService service;
        public AccessRightService Aservice;
        public ElementFormConfigController(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager, RoleManager<ApplicationRole> rolemanager) : base(userManager, signInManager, rolemanager)
        {
            service = new ElementFormConfigService();
            _userManager = userManager;
            Aservice = new AccessRightService();
        }
        public async Task<ActionResult> List()
        {
            BaseResponseDTO<string> dt = new BaseResponseDTO<string>();
            dt = await Aservice.GetViewTitle("Element Form Config");
            ViewBag.Name = dt.Data;
            return View("~/Views/Modules/Form/FormConfig/ElementFormConfig/ElementFormConfig.cshtml");
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
        public async Task<ActionResult<BaseResponseDTO<List<DropDownLogin>>>> GetAllDropdownValuesMenu()
        {
            try
            {
                BaseResponseDTO<List<DropDownLogin>> List = new BaseResponseDTO<List<DropDownLogin>>();
                List = service.GetAllDropDownValuesMenu();
                return Ok(List);
            }
            catch (Exception ex)
            {
                return BadRequest();

            }
        }

        [HttpPost]
        public async Task<ActionResult<BaseResponseDTO<ElementFormConfigDTO>>> GetElementFormConfig([FromBody] BaseRequestDTO<RetrieveElementFormConfigDTO> dt)
        {
            try
            {
                BaseResponseDTO<ElementFormConfigDTO> List = new BaseResponseDTO<ElementFormConfigDTO>();
                List = service.GetElementFormConfig(dt);
                return Ok(List);


            }
            catch (Exception ex)
            {

                return BadRequest();

            }
        }
        [HttpPost]
        public async Task<ActionResult<BaseResponseDTO<bool>>> CreateUpdate([FromBody] BaseRequestDTO<ElementFormConfigDTO> dto)
        {
            try
            {
               BaseResponseDTO<bool> BaseResponseDTO = new BaseResponseDTO<bool>();
               BaseResponseDTO = await service.SaveAsync(dto);
                return Ok(BaseResponseDTO);
            }
            catch (Exception ex)
            {

                return BadRequest();

            }
        }
    }
}
