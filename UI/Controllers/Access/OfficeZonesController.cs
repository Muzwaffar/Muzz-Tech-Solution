using BL.Models.Access;
using BL.Models.Administration;
using BL.Models.Common;
using BL.Services.Access.AccessRights;
using BL.Services.Access.OfficeZones;
using BL.Services.Administration;
using DAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UI.Helper;

namespace UI.Controllers.Access
{
    public class OfficeZonesController : NPFController
    {
        public UserManager<ApplicationUser> _userManager;
        public OfficeZonesService service;
        public AccessRightService Aservice;
        public OfficeZonesController(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager, RoleManager<ApplicationRole> rolemanager) : base(userManager, signInManager, rolemanager)
        {
            service = new OfficeZonesService();
            _userManager = userManager;
            Aservice = new AccessRightService();
        }
        public async Task<ActionResult> List()
        {
            BaseResponseDTO<string> dt = new BaseResponseDTO<string>();
            dt = await Aservice.GetViewTitle("Office Zones");
            ViewBag.Name = dt.Data;
            return View("~/Views/Modules/Access/OfficeZones/OfficeZonesList.cshtml");
        }

        [HttpPost]
        public async Task<ActionResult<BaseResponseDTO<List<OfficeZonesListDTO>>>> GetList()
        {
            try
            {
                BaseResponseDTO<List<OfficeZonesListDTO>> CusList = new BaseResponseDTO<List<OfficeZonesListDTO>>();
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
                    OfficeZonesCRUDTO dt = new OfficeZonesCRUDTO();
                    return PartialView("~/Views/Modules/Access/OfficeZones/OfficeZonesAddEdit.cshtml", dt);
                }
                else
                {
                    BaseResponseDTO<OfficeZonesCRUDTO> dt = new BaseResponseDTO<OfficeZonesCRUDTO>();
                    dt = service.GetById(dto.Data);
                    return PartialView("~/Views/Modules/Access/OfficeZones/OfficeZonesAddEdit.cshtml", dt.Data);
                }

            }
            catch (Exception ex)
            {

                return BadRequest();

            }
        }

        [HttpPost]
        public async Task<ActionResult<BaseResponseDTO<bool>>> CreateUpdate([FromBody] BaseRequestDTO<OfficeZonesCRUDTO> dto)
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
    }
}
