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
    public class GlobalParamController : NPFController
    {
        public UserManager<ApplicationUser> _userManager;
        public GlobalParamService service;
        public AccessRightService Aservice;
        public GlobalParamController(UserManager<ApplicationUser> userManager,
          SignInManager<ApplicationUser> signInManager, RoleManager<ApplicationRole> rolemanager) : base(userManager, signInManager, rolemanager)
        {
            service = new GlobalParamService();
            _userManager = userManager;
            Aservice = new AccessRightService();
        }
        public async Task<ActionResult> List()
        {
            BaseResponseDTO<string> dt = new BaseResponseDTO<string>();
            dt = await Aservice.GetViewTitle("Global Param");
            ViewBag.Name = dt.Data;
            return View("~/Views/Modules/Administration/General/GlobalParam/GlobalParamList.cshtml");
        }

        [HttpPost]
        public async Task<ActionResult<BaseResponseDTO<List<GlobalParamListDTO>>>> GetList()
        {
            try
            {
                BaseResponseDTO<List<GlobalParamListDTO>> CusList = new BaseResponseDTO<List<GlobalParamListDTO>>();
                CusList = service.GetAll();
                return Ok(CusList);


            }
            catch (Exception ex)
            {

                return BadRequest();

            }
        }
        [HttpPost]
        public async Task<ActionResult> GetById([FromBody] BaseRequestDTO<Guid> dto)
        {
            try
            {
                if (dto == null)
                {
                    GlobalParamCRUDDTO dt = new GlobalParamCRUDDTO();
                    return PartialView("~/Views/Modules/Administration/General/GlobalParam/GlobalParamAddEdit.cshtml", dt);
                }
                else
                {
                    BaseResponseDTO<GlobalParamCRUDDTO> dt = new BaseResponseDTO<GlobalParamCRUDDTO>();
                    dt = service.GetById(dto.Data);
                    return PartialView("~/Views/Modules/Administration/General/GlobalParam/GlobalParamAddEdit.cshtml", dt.Data);
                }

            }
            catch (Exception ex)
            {

                return BadRequest();

            }
        }
        [HttpPost]
        public async Task<ActionResult<BaseResponseDTO<bool>>> CreateUpdate([FromBody] BaseRequestDTO<GlobalParamCRUDDTO> dto)
        {
            try
            {
                BaseResponseDTO<bool> BaseResponseDTO = new BaseResponseDTO<bool>();
                BaseResponseDTO = await service.UpdateAsync(dto);

                return Ok(BaseResponseDTO);
            }
            catch (Exception ex)
            {

                return BadRequest();

            }
        }
    }
}
