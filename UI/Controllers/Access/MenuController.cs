using BL.Models.Access;
using BL.Models.Administration;
using BL.Models.Common;
using BL.Services.Access;
using BL.Services.Access.AccessRights;
using BL.Services.Administration;
using DAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UI.Helper;

namespace UI.Controllers.Access
{
    public class MenuController : NPFController
    {
        public UserManager<ApplicationUser> _userManager;
        public MenuService service;
        public AccessRightService Aservice;
        public MenuController(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager, RoleManager<ApplicationRole> rolemanager) : base(userManager, signInManager, rolemanager)
        {
            service = new MenuService();
            _userManager = userManager;
            Aservice = new AccessRightService();
        }
        public async Task<ActionResult> List()
        {
            BaseResponseDTO<string> dt = new BaseResponseDTO<string>();
            dt = await Aservice.GetViewTitle("Menu");
            ViewBag.Name = dt.Data;
            return View("~/Views/Modules/Access/Menu/MenuList.cshtml");
        }

        [HttpPost]
        public async Task<ActionResult<BaseResponseDTO<List<SideMenuDTO>>>> GetList()
        {
            try
            {
                BaseResponseDTO<List<SideMenuDTO>> CusList = new BaseResponseDTO<List<SideMenuDTO>>();
                CusList = service.GetAll();
                return Ok(CusList);


            }
            catch (Exception ex)
            {

                return BadRequest();

            }
        }
        [HttpPost]
        public async Task<ActionResult> GetById([FromBody] BaseRequestDTO<string> dto)
        {
            try
            {
                if (dto == null)
                {
                    SideMenuDTO dt = new SideMenuDTO();
                    return PartialView("~/Views/Modules/Access/Menu/MenuAddEdit.cshtml", dt);
                }
                else
                {
                    BaseResponseDTO<SideMenuDTO> dt = new BaseResponseDTO<SideMenuDTO>();
                    dt = service.GetById(dto.Data);
                    return PartialView("~/Views/Modules/Access/Menu/MenuAddEdit.cshtml", dt.Data);
                }

            }
            catch (Exception ex)
            {

                return BadRequest();

            }
        }

        [HttpPost]
        public async Task<ActionResult<BaseResponseDTO<bool>>> Update([FromBody] BaseRequestDTO<SideMenuDTO> dto)
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
