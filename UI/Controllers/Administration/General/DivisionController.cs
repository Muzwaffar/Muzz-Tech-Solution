using BL.Models.Common;
using BL.Services.Access.AccessRights;
using BL.Services.Administration;
using DAL.Models.Administration;
using DAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UI.Helper;
using BL.Models.Administration;
using DAL.Common;

namespace UI.Controllers.Administration.General
{
    public class DivisionController : NPFController
    {
        public UserManager<ApplicationUser> _userManager;
        public DivisionService service;
        public AccessRightService Aservice;
        public DivisionController(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager, RoleManager<ApplicationRole> rolemanager) : base(userManager, signInManager, rolemanager)
        {
            service = new DivisionService();
            _userManager = userManager;
            Aservice = new AccessRightService();
        }
        public async Task<ActionResult> List()
        {
            BaseResponseDTO<string> dt = new BaseResponseDTO<string>();
            dt = await Aservice.GetViewTitle("Division");
            ViewBag.Name = dt.Data;
            return View("~/Views/Modules/Administration/General/Division/DivisionList.cshtml");
        }

        [HttpPost]
        public async Task<ActionResult<BaseResponseDTO<List<DivisionListDTO>>>> GetList()
        {
            try
            {
                BaseResponseDTO<List<DivisionListDTO>> CusList = new BaseResponseDTO<List<DivisionListDTO>>();
                CusList = service.GetAll();
                return Ok(CusList);


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
                    DivisionCRUDDTO dt = new DivisionCRUDDTO();
                    return PartialView("~/Views/Modules/Administration/General/Division/DivisionAddEdit.cshtml", dt);
                }
                else
                {
                    BaseResponseDTO<DivisionCRUDDTO> dt = new BaseResponseDTO<DivisionCRUDDTO>();
                    dt = service.GetById(dto.Data);
                    return PartialView("~/Views/Modules/Administration/General/Division/DivisionAddEdit.cshtml", dt.Data);
                }

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
        public async Task<ActionResult<BaseResponseDTO<bool>>> CreateUpdate([FromBody] BaseRequestDTO<DivisionCRUDDTO> dto)
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
