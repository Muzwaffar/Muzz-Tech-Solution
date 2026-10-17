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
    public class NAAController : NPFController
    {
        public UserManager<ApplicationUser> _userManager;
        public NAAService service;
        public AccessRightService Aservice;
        public NAAController(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager, RoleManager<ApplicationRole> rolemanager) : base(userManager, signInManager, rolemanager)
        {
            service = new NAAService();
            _userManager = userManager;
            Aservice = new AccessRightService();
        }
        public async Task<ActionResult> List()
        {
            BaseResponseDTO<string> dt = new BaseResponseDTO<string>();
            dt = await Aservice.GetViewTitle("Name and Address");
            ViewBag.Name = dt.Data;
            return View("~/Views/Modules/Administration/General/NAA/NAAList.cshtml");
        }

        [HttpPost]
        public async Task<ActionResult<BaseResponseDTO<List<NAAlistDTO>>>> GetList()
        {
            try
            {
                BaseResponseDTO<List<NAAlistDTO>> CusList = new BaseResponseDTO<List<NAAlistDTO>>();
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
                    NAACRUDTO dt = new NAACRUDTO();
                    return PartialView("~/Views/Modules/Administration/General/NAA/NAAAddEdit.cshtml", dt);
                }
                else
                {
                    BaseResponseDTO<NAACRUDTO> dt = new BaseResponseDTO<NAACRUDTO>();
                    dt = service.GetById(dto.Data);
                    return PartialView("~/Views/Modules/Administration/General/NAA/NAAAddEdit.cshtml", dt.Data);
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
        public async Task<ActionResult<BaseResponseDTO<bool>>> CreateUpdate([FromBody] BaseRequestDTO<NAACRUDTO> dto)
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
        public async Task<ActionResult<BaseResponseDTO<NAACRUDTO>>> GetByNic([FromBody] BaseRequestDTO<string> dto)
        {
            try
            {
                BaseResponseDTO<NAACRUDTO> BaseResponseDTO = new BaseResponseDTO<NAACRUDTO>();
                BaseResponseDTO = service.GetByNic(dto.Data);
                return Ok(BaseResponseDTO);
            }
            catch (Exception ex)
            {

                return BadRequest();

            }
        }

        [HttpPost]
        public async Task<ActionResult<BaseResponseDTO<NAACRUDTO>>> GetDetailsById([FromBody] BaseRequestDTO<long> dto)
        {
            try
            {
                BaseResponseDTO<NAACRUDTO> BaseResponseDTO = new BaseResponseDTO<NAACRUDTO>();
                BaseResponseDTO = service.GetDetailsById(dto.Data);
                return Ok(BaseResponseDTO);
            }
            catch (Exception ex)
            {

                return BadRequest();

            }
        }
        [HttpPost]
        public async Task<ActionResult<BaseResponseDTO<NAACRUDTO>>> GetByMobileNo([FromBody] BaseRequestDTO<string> dto)
        {
            try
            {
                BaseResponseDTO<NAACRUDTO> BaseResponseDTO = new BaseResponseDTO<NAACRUDTO>();
                BaseResponseDTO = service.GetByMobileNo(dto.Data);
                return Ok(BaseResponseDTO);
            }
            catch (Exception ex)
            {

                return BadRequest();

            }
        }
    }
}
