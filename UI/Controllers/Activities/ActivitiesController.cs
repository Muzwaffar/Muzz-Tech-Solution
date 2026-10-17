using BL.Constants;
using BL.Models.Activities;
using BL.Models.Common;
using BL.Models.Dashboard;
using BL.Services.Activities;
using DAL.Common;
using DAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UI.Helper;

namespace UI.Controllers.Activities
{
    public class ActivitiesController : NPFController
    {
        public UserManager<ApplicationUser> _userManager;
        public ActivitiesService service;
        public ActivitiesController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<ApplicationRole> rolemanager) : base(userManager, signInManager, rolemanager)
        {
            _userManager = userManager;
            service = new ActivitiesService();
        }
        
        #region View
        public async Task<ActionResult> Activities()
        {
            BaseResponseDTO<string> dt = new BaseResponseDTO<string>();
            return View("~/Views/Modules/Activities/Activities.cshtml");
        }
        public async Task<ActionResult> StationActivities()
        {
            BaseResponseDTO<string> dt = new BaseResponseDTO<string>();
            return View("~/Views/Modules/StationManagement/StationActivities/StationActivities.cshtml");
        }
        #endregion

        #region Graph
        [HttpPost]
        public async Task<ActionResult<BaseResponseDTO<MyActivitiesGraphDTO>>> ActivitiesData([FromBody] BaseRequestDTO<DateRangeModuleDTO> dt)
        {
            try
            {
                BaseResponseDTO<MyActivitiesGraphDTO> result = new BaseResponseDTO<MyActivitiesGraphDTO>();
                result = await service.GetAllGraph(dt);
                return result;
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }
        #endregion

        #region GetById - View and Details
        [HttpPost]
        public async Task<ActionResult> GetById([FromBody] BaseRequestDTO<long> dto)
        {
            try
            {
                if (dto == null)
                {
                    ActivitiesDTO dt = new ActivitiesDTO();
                    return PartialView("~/Views/Modules/Activities/ActivitiesAddEdit.cshtml", dt);
                }
                else
                {
                    BaseResponseDTO<ActivitiesDTO> Act = new BaseResponseDTO<ActivitiesDTO>();
                    Act =await  service.GetById(dto.Data);
                    return PartialView("~/Views/Modules/Activities/ActivitiesAddEdit.cshtml", Act.Data);
                }
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

         [HttpPost]
        public async Task<ActionResult> GetByIdStation([FromBody] BaseRequestDTO<long> dto)
        {
            try
            {
                if (dto == null)
                {
                    ActivitiesDTO dt = new ActivitiesDTO();
                    return PartialView("~/Views/Modules/StationManagement/StationActivities/StationActivitiesAddEdit.cshtml", dt);
                }
                else
                {
                    BaseResponseDTO<ActivitiesDTO> Act = new BaseResponseDTO<ActivitiesDTO>();
                    Act = await service.GetById(dto.Data);
                    return PartialView("~/Views/Modules/StationManagement/StationActivities/StationActivitiesAddEdit.cshtml", Act.Data);
                }
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }
        [HttpPost]
        public async Task<ActionResult<BaseResponseDTO<List<ActivitiesDetailsDTO>>>> GetByIdDetails([FromBody] BaseRequestDTO<long> dto)
        {
            try
            {
                BaseResponseDTO<List<ActivitiesDetailsDTO>> List = new BaseResponseDTO<List<ActivitiesDetailsDTO>>();
                List = await service.GetByIdDetails(dto.Data);
                return Ok(List);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }
        [HttpPost]
        public async Task<ActionResult<BaseResponseDTO<List<ActivitiesConfigDTO>>>> GetByIdConfig([FromBody] BaseRequestDTO<long> dto)
        {
            try
            {
                BaseResponseDTO<List<ActivitiesConfigDTO>> List = new BaseResponseDTO<List<ActivitiesConfigDTO>>();
                List = await service.GetByIdConfig(dto.Data);
                return Ok(List);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        #endregion

        #region Dropdowns
        [HttpPost]
        public async Task<ActionResult<BaseResponseDTO<List<DropDown>>>> GetAllDropDownValues([FromBody] BaseRequestDTO<ActivityModule> dt)
        {
            try
            {
                BaseResponseDTO<List<DropDown>> List = new BaseResponseDTO<List<DropDown>>();
                List = service.GetAllDropDownValues(dt.Data);
                return Ok(List);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpPost]
        public async Task<ActionResult<BaseResponseDTO<List<DropDown>>>> GetDrpdownValuesByLabel([FromBody] BaseRequestDTO<ActivitiesDropdownSearchDTO> dto)
        {
            try
            {
                BaseResponseDTO<List<DropDown>> List = new BaseResponseDTO<List<DropDown>>();
                List = service.GetDrpdownValuesByLabel(dto.Data);
                return Ok(List);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }
        #endregion

        #region Create - Update - Listing 
        [HttpPost]
        public async Task<ActionResult<BaseResponseDTO<bool>>> CreateUpdate([FromBody] BaseRequestDTO<ActivitiesDTO> dto)
        {
            try
            {
                BaseResponseDTO<bool> BaseResponseDTO = new BaseResponseDTO<bool>();
                if (dto.Data.Id == 0)
                {
                    BaseResponseDTO = await service.SaveAsync(dto, _userManager);
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
        public async Task<ActionResult<BaseResponseDTO<List<ActivitiesListDTO>>>> FilterDatatableByPeriod([FromBody] BaseRequestDTO<DateRangeModuleDTO> dto)
        {
            try
            {
                BaseResponseDTO<List<ActivitiesListDTO>> SRList = new BaseResponseDTO<List<ActivitiesListDTO>>();
                SRList = await service.FilterDatatableByPeriod(dto);
                return Ok(SRList);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }
        #endregion




        //[HttpPost]
        //public async Task<ActionResult<BaseResponseDTO<List<DropDown>>>> GetSchemeDropDownValues()
        //{
        //    try
        //    {
        //        BaseResponseDTO<List<DropDown>> List = new BaseResponseDTO<List<DropDown>>();
        //        List = service.GetSchemeDropDownValues();
        //        return Ok(List);


        //    }
        //    catch (Exception ex)
        //    {

        //        return BadRequest();

        //    }
        //}

        //[HttpPost]
        //public async Task<ActionResult<BaseResponseDTO<List<DropDown>>>> GetCropDropDownValues()
        //{
        //    try
        //    {
        //        BaseResponseDTO<List<DropDown>> List = new BaseResponseDTO<List<DropDown>>();
        //        List = service.GetCropDropDownValues();
        //        return Ok(List);


        //    }
        //    catch (Exception ex)
        //    {

        //        return BadRequest();

        //    }
        //}

        //[HttpPost]
        //public async Task<ActionResult<BaseResponseDTO<List<DropDown>>>> GetLivestockDropDownValues()
        //{
        //    try
        //    {
        //        BaseResponseDTO<List<DropDown>> List = new BaseResponseDTO<List<DropDown>>();
        //        List = service.GetLivestockDropDownValues();
        //        return Ok(List);


        //    }
        //    catch (Exception ex)
        //    {

        //        return BadRequest();

        //    }

        //}

        //[HttpPost]
        //public async Task<ActionResult<BaseResponseDTO<List<DropDown>>>> GetSchemeCodeDropdown([FromBody] BaseRequestDTO<long> dt)
        //{
        //    try
        //    {
        //        BaseResponseDTO<List<DropDown>> List = new BaseResponseDTO<List<DropDown>>();
        //        List = await service.GetSchemeCodeDropdown(dt.Data);
        //        return Ok(List);


        //    }
        //    catch (Exception ex)
        //    {

        //        return BadRequest();

        //    }
        //}

        //[HttpPost]
        //public async Task<ActionResult<BaseResponseDTO<List<DropDown>>>> GetSectionByIdDropdown([FromBody] BaseRequestDTO<long> dt)
        //{
        //    try
        //    {
        //        BaseResponseDTO<List<DropDown>> List = new BaseResponseDTO<List<DropDown>>();
        //        List = await service.GetSectionByIdDropdown(dt.Data);
        //        return Ok(List);


        //    }
        //    catch (Exception ex)
        //    {

        //        return BadRequest();

        //    }
        //}

    }
}
