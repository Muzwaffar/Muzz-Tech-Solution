using BL.Models.Administration;
using BL.Models.Common;
using BL.Services.Access.AccessRights;
using BL.Services.Administration;
using DAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UI.Helper;

namespace UI.Controllers.Administration.General
{
    public class CallerController : NPFController
    {
        public UserManager<ApplicationUser> _userManager;
        public CallerService service;
        public AccessRightService Aservice;

        public CallerController(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager, RoleManager<ApplicationRole> rolemanager) : base(userManager, signInManager, rolemanager)
        {
            service = new CallerService();
            _userManager = userManager;
            Aservice = new AccessRightService();
        }

        public async Task<ActionResult> List()
        {
            BaseResponseDTO<string> dt = new BaseResponseDTO<string>();
            dt = await Aservice.GetViewTitle("Caller");
            ViewBag.Name = dt.Data;
            return View("~/Views/Modules/Administration/General/Caller/CallerList.cshtml");
        }

        public async Task<ActionResult> Caller(long? Id)
        {
            try
            {
                BaseResponseDTO<string> dt = new BaseResponseDTO<string>();
                dt = await Aservice.GetViewTitle("Caller");
                ViewBag.Name = dt.Data;

                BaseResponseDTO<CallerCRUDDTO> dtdata = new BaseResponseDTO<CallerCRUDDTO>();

                if (Id == null)
                {
                    return PartialView("~/Views/Modules/Administration/General/Caller/Caller.cshtml", dtdata.Data);
                }
                else
                {
                    dtdata = service.GetById(Id.Value);
                    return PartialView("~/Views/Modules/Administration/General/Caller/Caller.cshtml", dtdata.Data);
                }
            }
            catch (Exception ex)
            {
                return BadRequest();

            }
        }

        [HttpPost]
        public async Task<ActionResult<BaseResponseDTO<List<CallerListDTO>>>> GetList()
        {
            try
            {
                BaseResponseDTO<List<CallerListDTO>> CusList = new BaseResponseDTO<List<CallerListDTO>>();
                CusList = service.GetAll();
                return Ok(CusList);


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
        public async Task<ActionResult<BaseResponseDTO<bool>>> CreateUpdate([FromBody] BaseRequestDTO<CallerCRUDDTO> dto)
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
        public async Task<ActionResult<BaseResponseDTO<CallerCRUDDTO>>> GetCallerByMobile([FromBody] BaseRequestDTO<string> dto)
        {
            try
            {
                BaseResponseDTO<CallerCRUDDTO> BaseResponseDTO = new BaseResponseDTO<CallerCRUDDTO>();
                BaseResponseDTO = service.GetCallerByMobile(dto.Data);

                return Ok(BaseResponseDTO);
            }
            catch (Exception ex)
            {

                return BadRequest();

            }
        }

        [HttpPost]
        public async Task<ActionResult<BaseResponseDTO<CallerCRUDDTO>>> GetCallerById([FromBody] BaseRequestDTO<long> dto)
        {
            try
            {
                BaseResponseDTO<CallerCRUDDTO> BaseResponseDTO = new BaseResponseDTO<CallerCRUDDTO>();
                BaseResponseDTO = service.GetCallerById(dto.Data);

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
