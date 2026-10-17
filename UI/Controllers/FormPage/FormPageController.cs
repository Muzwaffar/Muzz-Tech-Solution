using BL.Constants;
using BL.Models.Common;
using BL.Models.FormPage;
using BL.Services.Access.AccessRights;
using BL.Services.Activities;
using BL.Services.Administration;
using BL.Services.ServiceRequest;
using DAL.Models;
using DAL.Models.Form;
using DAL.Models.FormPage;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Data;
using System.Text;
using UI.Helper;

namespace UI.Controllers.FormPage
{
    public class FormPageController : NPFController
    {
        public UserManager<ApplicationUser> _userManager;
        public FormPageService service;
        public WorkflowService WFservice;
        public AccessRightService Aservice;
        public FormPageController(Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> userManager,
           SignInManager<ApplicationUser> signInManager, Microsoft.AspNetCore.Identity.RoleManager<ApplicationRole> rolemanager) : base(userManager, signInManager, rolemanager)
        {
            service = new FormPageService();
            WFservice = new WorkflowService();
            _userManager = userManager;
            Aservice = new AccessRightService();
        }
        public async Task<ActionResult> List()
        {
            BaseResponseDTO<string> dt = new BaseResponseDTO<string>();
            dt = await Aservice.GetViewTitle("Form Page");
            ViewBag.Name = dt.Data;
            return View("~/Views/Modules/FormPage/FormPage.cshtml");
        }
        public async Task<ActionResult> AddEditView(long ID, Guid MId)
        {
            try
            {
                if (ID == null || ID == 0)
                {
                    BaseResponseDTO<FormPageDTO> FP = await service.GetDTO(MId);
                    ViewBag.Name = FP.AdditionalData;
                    return View("~/Views/Modules/FormPage/FormPageAddEdit.cshtml", FP.Data);
                }
                else
                {
                    BaseResponseDTO<FormPageDTO> FP = new BaseResponseDTO<FormPageDTO>();
                    //FP = await service.GetById(ID, MId);
                    FP = await WFservice.WFGetById(ID, MId);
                    ViewBag.Name = FP.AdditionalData;
                    return View("~/Views/Modules/FormPage/FormPageAddEdit.cshtml", FP.Data);
                }

            }
            catch (Exception ex)
            {

                return BadRequest();

            }
        }
        [HttpPost]
        public async Task<ActionResult<BaseResponseDTO<FormPageDTO>>> GetByIdApi([FromBody] BaseRequestDTO<long> dto)
        {
            try
            {
                if (dto.Data == null || dto.Data == 0)
                {
                    BaseResponseDTO<FormPageDTO> SSR = await service.GetDTO(Guid.Parse(dto.ModuleId.ToString()));
                    return Ok(SSR);

                }
                else
                {
                    BaseResponseDTO<FormPageDTO> SSR = new BaseResponseDTO<FormPageDTO>();
                    //  SSR = await service.GetById(dto.Data,Guid.Parse(dto.ModuleId.ToString()));
                    SSR = await WFservice.WFGetById(dto.Data, Guid.Parse(dto.ModuleId.ToString()));
                    return Ok(SSR);

                }


            }
            catch (Exception ex)
            {

                return BadRequest();

            }
        }
        [HttpPost]
        public async Task<ActionResult<BaseResponseDTO<List<NPF_FRM_FormPage>>>> GetList([FromBody] BaseRequestDTO<DateRangeDTO> dto)
        {
            try
            {
                BaseResponseDTO<List<NPF_FRM_FormPage>> SRList = new BaseResponseDTO<List<NPF_FRM_FormPage>>();
                SRList = await service.FilterDatatableByPeriod(dto);
                return Ok(SRList);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        //[HttpPost]
        //public async Task<ActionResult> GetById([FromBody] BaseRequestDTO<long> dto)
        //{
        //    try
        //    {
        //        if (dto.Data == null || dto.Data == 0)
        //        {
        //            FormPageDTO SSR = new FormPageDTO();
        //            SSR.Code = "AUTO-GENERATED";
        //            return View("~/Views/Modules/FormPage/FormPageAddEdit.cshtml", SSR);
        //        }
        //        else
        //        {
        //            BaseResponseDTO<FormPageDTO> SSR = new BaseResponseDTO<FormPageDTO>();
        //            SSR = await service.GetById(dto.Data);
        //            return View("~/Views/Modules/FormPage/FormPageAddEdit.cshtml", SSR.Data);
        //        }

        //    }
        //    catch (Exception ex)
        //    {

        //        return BadRequest();

        //    }
        //}
        [HttpPost]
        public async Task<ActionResult<BaseResponseDTO<bool>>> CreateUpdate([FromBody] BaseRequestDTO<FormPageDTO> dto)
        {
            try
            {
                BaseResponseDTO<bool> BaseResponseDTO = new BaseResponseDTO<bool>();
                if (dto.Data.FormPageId == 0)
                {
                    //  BaseResponseDTO = await service.SaveAsync(dto);
                    BaseResponseDTO = await WFservice.InitialiseProcessWorkflow(dto);
                }
                else
                {
                    BaseResponseDTO = await WFservice.UpdateProcessWorkflow(dto);
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
        public async Task<ActionResult<BaseResponseDTO<List<NPF_FRM_Section>>>> GetAllServiceListing([FromBody] BaseRequestDTO<List<long>> dto)
        {
            try
            {
                BaseResponseDTO<List<NPF_FRM_Section>> ServiceListin = new BaseResponseDTO<List<NPF_FRM_Section>>();
                if (dto != null)
                {
                    ServiceListin = await service.GetAllServiceListing(dto.Data);
                }
                return Ok(ServiceListin);
            }
            catch (Exception ex)
            {

                return BadRequest();

            }
        }
        [HttpPost]
        public async Task<ActionResult<BaseResponseDTO<List<DTSection>>>> GetSelectedServices([FromBody] BaseRequestDTO<List<long>> dto)
        {
            try
            {
                BaseResponseDTO<List<DTSection>> SelectedService = new BaseResponseDTO<List<DTSection>>();
                SelectedService = await service.GetSelectedServices(dto.Data);
                return Ok(SelectedService);
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
        public async Task<ActionResult<BaseResponseDTO<SectionElements>>> GetSectionElementsByForm([FromBody] BaseRequestDTO<long> dto)
        {
            try
            {
                BaseResponseDTO<SectionElements> SA = new BaseResponseDTO<SectionElements>();
                SA = await service.GetSectionElementsByForm(dto.Data, Guid.Parse(dto.ModuleId.ToString()));
                return Ok(SA);
            }
            catch (Exception ex)
            {

                return BadRequest();

            }
        }

        [HttpPost]
        public async Task<ActionResult<BaseResponseDTO<List<Values>>>> GetSourceValueByElementId([FromBody] BaseRequestDTO<long> dto)
        {
            try
            {
                BaseResponseDTO<List<Values>> SA = new BaseResponseDTO<List<Values>>();
                SA = await service.GetSourceValueByElementId(dto.Data);
                return Ok(SA);
            }
            catch (Exception ex)
            {

                return BadRequest();

            }
        }

        [HttpPost]
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
