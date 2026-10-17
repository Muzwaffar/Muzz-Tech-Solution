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
    public class StationController : NPFController
    {
        public UserManager<ApplicationUser> _userManager;
        public StationService service;
        public AccessRightService Aservice;

        public StationController(UserManager<ApplicationUser> userManager,
         SignInManager<ApplicationUser> signInManager, RoleManager<ApplicationRole> rolemanager) : base(userManager, signInManager, rolemanager)
        {
            service = new StationService();
            _userManager = userManager;
            Aservice = new AccessRightService();
        }

        public async Task<ActionResult> List()
        {
            BaseResponseDTO<string> dt = new BaseResponseDTO<string>();
            dt = await Aservice.GetViewTitle("Station");
            ViewBag.Name = dt.Data;
            return View("~/Views/Modules/Administration/General/Station/StationList.cshtml");
        }

        [HttpPost]
        public async Task<ActionResult<BaseResponseDTO<List<StationListDTO>>>> GetList()
        {
            try
            {
                BaseResponseDTO<List<StationListDTO>> CusList = new BaseResponseDTO<List<StationListDTO>>();
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
                    StationCRUDDTO dt = new StationCRUDDTO();
                    return PartialView("~/Views/Modules/Administration/General/Station/StationAddEdit.cshtml", dt);
                }
                else
                {
                    BaseResponseDTO<StationCRUDDTO> dt = new BaseResponseDTO<StationCRUDDTO>();
                    dt = service.GetById(dto.Data);
                    return PartialView("~/Views/Modules/Administration/General/Station/StationAddEdit.cshtml", dt.Data);
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
        public async Task<ActionResult<BaseResponseDTO<bool>>> CreateUpdate([FromBody] BaseRequestDTO<StationCRUDDTO> dto)
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
