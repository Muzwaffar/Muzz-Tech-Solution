using BL.Constants;
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
    public class PDAController : NPFController
    {
        public UserManager<ApplicationUser> _userManager;
        public PDAService service;
        public AccessRightService Aservice;
        public PDAController(Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> userManager,
          SignInManager<ApplicationUser> signInManager, Microsoft.AspNetCore.Identity.RoleManager<ApplicationRole> rolemanager) : base(userManager, signInManager, rolemanager)
        {
            service = new PDAService();
            _userManager = userManager;
            Aservice = new AccessRightService();
        }

        public async Task<ActionResult> List()
        {
            BaseResponseDTO<string> dt = new BaseResponseDTO<string>();
            dt = await Aservice.GetViewTitle("Pre-Determined Attandance");
            ViewBag.Name = dt.Data;
            return View("~/Views/Modules/Administration/General/PDA/PDAList.cshtml");
        }
        public async Task<ActionResult> GetById(long? Id, Guid MId)
        {
            try
            {
                if (Id == null)
                {
                    BaseResponseDTO<string> dt = new BaseResponseDTO<string>();
                    dt = await Aservice.GetViewTitle("Pre-Determined Attandance");
                    ViewBag.Name = dt.Data;
                    PDADTO VC = new PDADTO();
                    return View("~/Views/Modules/Administration/General/PDA/PDAAddEdit.cshtml", VC);
                }
                else
                {
                    BaseResponseDTO<PDADTO> VC = new BaseResponseDTO<PDADTO>();
                    VC = service.GetById(Id);
                    return View("~/Views/Modules/Administration/General/PDA/PDAAddEdit.cshtml", VC.Data);
                }

            }
            catch (Exception ex)
            {

                return BadRequest();

            }
        }

        [HttpPost]
        public async Task<ActionResult<BaseResponseDTO<List<PDADTO>>>> GetList()
        {
            try
            {
                BaseResponseDTO<List<PDADTO>> List = new BaseResponseDTO<List<PDADTO>>();
                List = service.GetAll();
                return Ok(List);


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
        public async Task<ActionResult<BaseResponseDTO<bool>>> CreateUpdate([FromBody] BaseRequestDTO<PDADTO> dto)
        {
            try
            {
                GenericConstant con = new GenericConstant();
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
        public async Task<ActionResult<BaseResponseDTO<PDADTO>>> getTableDetails([FromBody] BaseRequestDTO<long> dto)
        {
            try
            {
                BaseResponseDTO<PDADTO> ResourceList = new BaseResponseDTO<PDADTO>();
                ResourceList = service.getTableDetails(dto.Data);
                return Ok(ResourceList);
            }
            catch (Exception ex)
            {

                return BadRequest();

            }
        }


        //API Delete Category
        [HttpPost]
        public async Task<ActionResult<BaseResponseDTO<bool>>> Delete([FromBody] BaseRequestDTO<long> dto)
        {
            try
            {
                BaseResponseDTO<bool> BaseResponseDTO = new BaseResponseDTO<bool>();
                BaseResponseDTO = service.Delete(dto.Data);
                return Ok(BaseResponseDTO);
            }
            catch (Exception ex)
            {

                return BadRequest();

            }
        }
    }
}
