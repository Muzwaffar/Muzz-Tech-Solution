using BL.Models.Form;
using BL.Models.Common;
using BL.Services.Access.AccessRights;
using BL.Services.Form;
using DAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UI.Helper;
using BL.Models.Schedule;
using DAL.Models.Form;
using Microsoft.AspNetCore.Authorization;
using BL.Constants;
using BL.Services.Schedule;
using BL.Models.FormPage;

namespace UI.Controllers.Schedule
{
    public class ScheduleController : NPFController
    {
        public UserManager<ApplicationUser> _userManager;
        public ScheduleService service;
        public AccessRightService Aservice;
        public ScheduleController(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager, RoleManager<ApplicationRole> rolemanager) : base(userManager, signInManager, rolemanager)
        {
            service = new ScheduleService();
            _userManager = userManager;
            Aservice = new AccessRightService();
        }
        public async Task<ActionResult> List()
        {
            BaseResponseDTO<string> dt = new BaseResponseDTO<string>();
            dt = await Aservice.GetViewTitle("Job List");
            ViewBag.Name = dt.Data;
            return View("~/Views/Modules/Schedule/ScheduleList/ScheduleList.cshtml");
        }
        public async Task<ActionResult> GetCalendarView()
        {
            BaseResponseDTO<string> dt = new BaseResponseDTO<string>();
            dt = await Aservice.GetViewTitle("Schedule Calendar");
            ViewBag.Name = dt.Data;
            return View("~/Views/Modules/Schedule/ScheduleCalendar/ScheduleCalendar.cshtml");
        }
        [HttpPost]
        public async Task<ActionResult<BaseResponseDTO<CalenderDTO>>> GetCalendarAPI()
        {
            try
            {
                BaseResponseDTO<List<CalenderDTO>> BaseResponseDTO = new BaseResponseDTO<List<CalenderDTO>>();
                BaseResponseDTO = await service.GetCalendar();

                return Ok(BaseResponseDTO);
            }
            catch (Exception ex)
            {

                return BadRequest();

            }
        }
        [HttpPost]
        public async Task<ActionResult<BaseResponseDTO<List<CalendarGrid>>>> GetCalendarGrid([FromBody] BaseRequestDTO<long> dto)
        {
            try
            {
                BaseResponseDTO<List<CalendarGrid>> BaseResponseDTO = new BaseResponseDTO<List<CalendarGrid>>();
                BaseResponseDTO = service.GetCalendarGrid(dto.Data);
                return Ok(BaseResponseDTO);
            }
            catch (Exception ex)
            {

                return BadRequest();

            }
        }
        [HttpPost]
        public async Task<ActionResult<BaseResponseDTO<List<FilesGrid>>>> GetCalendarFiles([FromBody] BaseRequestDTO<long> dto)
        {
            try
            {
                BaseResponseDTO<List<FilesGrid>> BaseResponseDTO = new BaseResponseDTO<List<FilesGrid>>();
                BaseResponseDTO = service.GetCalendarFiles(dto.Data);
                return Ok(BaseResponseDTO);
            }
            catch (Exception ex)
            {

                return BadRequest();

            }
        }
        public async Task<ActionResult> DownloadFile(long dto)
        {
            try
            {
                BaseResponseDTO<GetFile> BaseResponseDTO = new BaseResponseDTO<GetFile>();
                BaseResponseDTO = service.GetFileById(dto);
                byte[] bytes = Convert.FromBase64String(BaseResponseDTO.Data.File);
                string fileName = BaseResponseDTO.Data.FileName;
                return File(bytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);


            }
            catch (Exception ex)
            {
                return BadRequest();
            }

        }
        [HttpPost]
        public async Task<ActionResult> GetCalendarById([FromBody] BaseRequestDTO<long> dto)
        {
            try
            {

                BaseResponseDTO<CalenderViewDTO> dt = new BaseResponseDTO<CalenderViewDTO>();
                dt = service.GetCalendarById(dto.Data);
                return PartialView("~/Views/Modules/Schedule/ScheduleCalendar/ScheduleCalendarPopup.cshtml", dt.Data);


            }
            catch (Exception ex)
            {

                return BadRequest();

            }
        }

        [HttpPost]
        public async Task<ActionResult<BaseResponseDTO<List<DropDown>>>> GetDropDownByDate([FromBody] BaseRequestDTO<GetByDateDTO> dto)
        {
            try
            {
                BaseResponseDTO<List<DropDown>> List = new BaseResponseDTO<List<DropDown>>();
                List = await service.GetDropDownByDate(dto);
                return Ok(List);


            }
            catch (Exception ex)
            {

                return BadRequest();

            }
        }

        [HttpPost]
        public async Task<ActionResult<BaseResponseDTO<List<DropDownLogin>>>> GetSRlDropDownValues()
        {
            try
            {
                BaseResponseDTO<List<DropDownLogin>> List = new BaseResponseDTO<List<DropDownLogin>>();
                List = service.GetSRlDropDownValues();
                return Ok(List);


            }
            catch (Exception ex)
            {

                return BadRequest();

            }
        }
		[HttpPost]
		public async Task<ActionResult<BaseResponseDTO<List<DropDown>>>> GetScheduleItem([FromBody] BaseRequestDTO<string> dto)
		{
			try
			{
				BaseResponseDTO<List<DropDown>> List = new BaseResponseDTO<List<DropDown>>();
				List = service.GetScheduleItem(dto.Data);
				return Ok(List);


			}
			catch (Exception ex)
			{

				return BadRequest();

			}
		}
		[HttpPost]
        public async Task<ActionResult<BaseResponseDTO<List<DropDown>>>> GetSchDropDownDropDown([FromBody] BaseRequestDTO<long> dto)
        {
            try
            {
                BaseResponseDTO<List<DropDown>> List = new BaseResponseDTO<List<DropDown>>();
                List = service.GetSchDropDownValues(dto.Data);
                return Ok(List);


            }
            catch (Exception ex)
            {

                return BadRequest();

            }
        }


        [HttpPost]
        public async Task<ActionResult<BaseResponseDTO<bool>>> DeleteSchedule([FromBody] BaseRequestDTO<long> dto)
        {
            try
            {
                BaseResponseDTO<bool> BaseResponseDTO = new BaseResponseDTO<bool>();
                BaseResponseDTO = service.DeleteSchedule(dto);
                return Ok(BaseResponseDTO);
            }
            catch (Exception ex)
            {

                return BadRequest();

            }
        }

        [HttpPost]
        public async Task<ActionResult<BaseResponseDTO<bool>>> StatusUpdate([FromBody] BaseRequestDTO<StatusChange> dto)
        {
            try
            {
                BaseResponseDTO<bool> BaseResponseDTO = new BaseResponseDTO<bool>();
                BaseResponseDTO = await service.StatusUpdate(dto);
                return Ok(BaseResponseDTO);
            }
            catch (Exception ex)
            {

                return BadRequest();

            }
        }
        //[AllowAnonymous]
        //public async Task<ActionResult<bool>> RemapChecklist()
        //{
        //    try
        //    {
        //        BaseResponseDTO<bool> BaseResponseDTO = new BaseResponseDTO<bool>();
        //        BaseResponseDTO = await service.RemapChecklist();
        //        return Ok(BaseResponseDTO.Data);
        //    }
        //    catch (Exception ex)
        //    {

        //        return BadRequest();

        //    }
        //}        //[AllowAnonymous]
        //public async Task<ActionResult<bool>> RemapChecklist()
        //{
        //    try
        //    {
        //        BaseResponseDTO<bool> BaseResponseDTO = new BaseResponseDTO<bool>();
        //        BaseResponseDTO = await service.RemapChecklist();
        //        return Ok(BaseResponseDTO.Data);
        //    }
        //    catch (Exception ex)
        //    {

        //        return BadRequest();

        //    }
        //}

        [HttpPost]
        public async Task<ActionResult<BaseResponseDTO<HelpersTableDTO>>> GetHelpersTabeleById([FromBody] BaseRequestDTO<long> dto)
        {
            try
            {
                BaseResponseDTO<List<HelpersTableDTO>> BaseResponseDTO = new BaseResponseDTO<List<HelpersTableDTO>>();
                BaseResponseDTO = service.GetHelpersTabeleById(dto.Data);
                return Ok(BaseResponseDTO);
            }
            catch (Exception ex)
            {

                return BadRequest();

            }
        }
        [HttpPost]
        public async Task<ActionResult<BaseResponseDTO<AddEditSchduleDTO>>> GetByIdApi([FromBody] BaseRequestDTO<long> dto)
        {
            try
            {
                BaseResponseDTO<AddEditSchduleDTO> AES = new BaseResponseDTO<AddEditSchduleDTO>(); ;
                AES = await service.GetById(dto.Data);

                return Ok(AES);


            }
            catch (Exception ex)
            {

                return BadRequest();

            }
        }

        public async Task<ActionResult> AddEditView(long Id)
		{
            try
            {
				BaseResponseDTO<string> dto = new BaseResponseDTO<string>();
				dto = await Aservice.GetViewTitle("Job List");
				ViewBag.Name = dto.Data;
				BaseResponseDTO<AddEditSchduleDTO> dt = new BaseResponseDTO<AddEditSchduleDTO>();
				if (Id == null || Id == 0)
				{
                    return PartialView("~/Views/Modules/Schedule/ScheduleList/ScheduleAddEdit.cshtml", dt.Data);
                }
                else
                {
                    dt = await service.GetById(Id);
                    return PartialView("~/Views/Modules/Schedule/ScheduleList/ScheduleAddEdit.cshtml", dt.Data);
                }

            }
            catch (Exception ex)
            {

                return BadRequest();

            }
        }
        [HttpPost]
        public async Task<ActionResult<BaseResponseDTO<bool>>> CreateUpdate([FromBody] BaseRequestDTO<AddEditSchduleDTO> dto)
        {
            try
            {
                GenericConstant con = new GenericConstant();
                BaseResponseDTO<bool> BaseResponseDTO = new BaseResponseDTO<bool>();
                if (dto.Data.SchduleId == 0)
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
        public async Task<ActionResult<BaseResponseDTO<List<DTSection>>>> GetServicesByServiceId([FromBody] BaseRequestDTO<GetServicesDetails> dto)
        {
            try
            {
                GenericConstant con = new GenericConstant();
                BaseResponseDTO<List<DTSection>> BaseResponseDTO = new BaseResponseDTO<List<DTSection>>();
                BaseResponseDTO = await service.GetServicesByServiceId(dto);
                return Ok(BaseResponseDTO);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }
        //[HttpPost]
        //public async Task<ActionResult<BaseResponseDTO<FileDownloadDTO>>> ExportExcelSchedule([FromBody] BaseRequestDTO<List<ScheduleDTO>> dt)
        //{
        //    BaseResponseDTO<FileDownloadDTO> file = new BaseResponseDTO<FileDownloadDTO>();
        //    try
        //    {
        //        byte[] bytes = serviceExport.ExportExcelSchedule(dt.Data);
        //        string filename = "ScheduleReport_" + DateTime.Now.ToString("yyyy-MM-dd") + ".xlsx";
        //        file.Data = new FileDownloadDTO();
        //        file.Data.FileName = filename;
        //        file.Data.ByteArray = bytes;
        //        return Ok(file);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest();



        //    }
        //}
        [HttpPost]
        public async Task<ActionResult<BaseResponseDTO<List<ScheduleListDTO>>>> FilterDatatableByPeriod([FromBody] BaseRequestDTO<JobRange> dto)
        {
            try
            {
                BaseResponseDTO<List<ScheduleListDTO>> SchList = new BaseResponseDTO<List<ScheduleListDTO>>();
                SchList = service.FilterDatatableByPeriod(dto);
                return Ok(SchList);


            }
            catch (Exception ex)
            {

                return BadRequest();

            }
        }

		[HttpPost]
		public async Task<ActionResult<BaseResponseDTO<List<DropDown>>>> GetItemsByFormId([FromBody] BaseRequestDTO<string> dto)
		{
			try
			{
				BaseResponseDTO<List<DropDown>> List = new BaseResponseDTO<List<DropDown>>();
				List = service.GetItemsByFormId(dto.Data);
				return Ok(List);

			}
			catch (Exception ex)
			{

				return BadRequest();

			}
		}
	}
}
