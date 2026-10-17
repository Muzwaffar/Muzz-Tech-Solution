using BL.Models.Form;
using BL.Models.Common;
using BL.Services.Access.AccessRights;
using BL.Services.Administration;
using BL.Services.Form;
using DAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UI.Helper;
using DAL.Models.Form;
using BL.Constants;

namespace UI.Controllers.Form
{
    public class SectionController : NPFController
    {
        public UserManager<ApplicationUser> _userManager;
        public SectionService service;
        public AccessRightService Aservice;
        public SectionController(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager, RoleManager<ApplicationRole> rolemanager) : base(userManager, signInManager, rolemanager)
        {
            service = new SectionService();
            _userManager = userManager;
            Aservice = new AccessRightService();
        }
        public async Task<ActionResult> List()
        {
            BaseResponseDTO<string> dt = new BaseResponseDTO<string>();
            dt = await Aservice.GetViewTitle("Section");
            ViewBag.Name = dt.Data;
            return View("~/Views/Modules/Form/Section/SectionList.cshtml");
        }

        [HttpPost]
        public async Task<ActionResult<BaseResponseDTO<List<NPF_FRM_Section>>>> GetList()
        {
            try
            {
                BaseResponseDTO<List<NPF_FRM_Section>> FormList = new BaseResponseDTO<List<NPF_FRM_Section>>();
                FormList = service.GetAll();
                return Ok(FormList);


            }
            catch (Exception ex)
            {

                return BadRequest();

            }
        }
        [HttpPost]
        public async Task<ActionResult<BaseResponseDTO<List<DropDown>>>> GetAllDropDownValues()
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
        public async Task<ActionResult<BaseResponseDTO<List<DropDown>>>> GetAllParameters([FromBody] BaseRequestDTO<long> dto)
        {
            try
            {
                BaseResponseDTO<List<DropDown>> List = new BaseResponseDTO<List<DropDown>>();
                if (dto != null) { 
                    List = service.GetAllParameters(dto.Data); 
                }

                return Ok(List);
            }
            catch (Exception ex)
            {

                return BadRequest();

            }
        }
        public async Task<ActionResult> GetById(long? Id)
        {
            try
            {
                BaseResponseDTO<NPF_FRM_Section> dt = new BaseResponseDTO<NPF_FRM_Section>();
                if (Id == null)
                {
                    return View("~/Views/Modules/Form/Section/SectionAddEdit.cshtml", dt.Data);
                }
                else
                {
                    dt = service.GetById(Convert.ToUInt32(Id));
                    return View("~/Views/Modules/Form/Section/SectionAddEdit.cshtml", dt.Data);
                }

            }
            catch (Exception ex)
            {

                return BadRequest();

            }
        }
        [HttpPost]
        public async Task<ActionResult<BaseResponseDTO<List<ElementDTO>>>> GetElementById([FromBody] BaseRequestDTO<long> dto)
        {
            try
            {
                BaseResponseDTO<List<ElementDTO>> ElementList = new BaseResponseDTO<List<ElementDTO>>();
                ElementList = service.GetElementById(dto.Data);
                return Ok(ElementList);
            }
            catch (Exception ex)
            {

                return BadRequest();

            }
        }
        [HttpPost]
        public async Task<ActionResult<BaseResponseDTO<bool>>> CreateUpdate([FromBody] BaseRequestDTO<SectionElementDTO> dto)
        {
            try
            {
                BaseResponseDTO<bool> BaseResponseDTO = new BaseResponseDTO<bool>();
                if (dto.Data.Sec.Id == 0)
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
