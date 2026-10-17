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
    public class ExternalAgenciesController : NPFController
    {
        public UserManager<ApplicationUser> _userManager;
        public ExternalAgenciesService service;
        public AccessRightService Aservice;

        public ExternalAgenciesController(UserManager<ApplicationUser> userManager,
           SignInManager<ApplicationUser> signInManager, RoleManager<ApplicationRole> rolemanager) : base(userManager, signInManager, rolemanager)
        {
            service = new ExternalAgenciesService();
            _userManager = userManager;
            Aservice = new AccessRightService();
        }
        public async Task<ActionResult> List()
        {
            BaseResponseDTO<string> dt = new BaseResponseDTO<string>();
            dt = await Aservice.GetViewTitle("External Agency");
            ViewBag.Name = dt.Data;
            return View("~/Views/Modules/Administration/General/ExternalAgencies/ExternalAgenciesList.cshtml");
        }

        [HttpPost]
        public async Task<ActionResult<BaseResponseDTO<List<ExternalAgenciesListDTO>>>> GetList()
        {
            try
            {
                BaseResponseDTO<List<ExternalAgenciesListDTO>> CusList = new BaseResponseDTO<List<ExternalAgenciesListDTO>>();
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
                    ExternalAgenciesCRUDDTO dt = new ExternalAgenciesCRUDDTO();
                    return PartialView("~/Views/Modules/Administration/General/ExternalAgencies/ExternalAgenciesAddEdit.cshtml", dt);
                }
                else
                {
                    BaseResponseDTO<ExternalAgenciesCRUDDTO> dt = new BaseResponseDTO<ExternalAgenciesCRUDDTO>();
                    dt = service.GetById(dto.Data);
                    return PartialView("~/Views/Modules/Administration/General/ExternalAgencies/ExternalAgenciesAddEdit.cshtml", dt.Data);
                }

            }
            catch (Exception ex)
            {

                return BadRequest();

            }
        }

        [HttpPost]
        public async Task<ActionResult<BaseResponseDTO<ExternalAgenciesCRUDDTO>>> GetByIdDetails([FromBody] BaseRequestDTO<long> dto)
        {
            try
            {
                BaseResponseDTO<ExternalAgenciesCRUDDTO> CusList = new BaseResponseDTO<ExternalAgenciesCRUDDTO>();
                CusList = service.GetById(dto.Data);
                return Ok(CusList);


            }
            catch (Exception ex)
            {

                return BadRequest();

            }
        }


        [HttpPost]
        public async Task<ActionResult<BaseResponseDTO<bool>>> CreateUpdate([FromBody] BaseRequestDTO<ExternalAgenciesCRUDDTO> dto)
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
