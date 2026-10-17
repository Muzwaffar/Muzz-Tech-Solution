using BL.Constants;
using BL.Models.Access;
using BL.Models.Administration;
using BL.Models.Common;
using BL.Services.Access.AccessRights;
using BL.Services.Common;
using DAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UI.Helper;

namespace UI.Controllers.Access
{
    public class MatrixController : NPFController
    {
        public UserManager<ApplicationUser> _userManager;
        private MatrixService service;
        public AccessRightService Aservice;
        public MatrixController(UserManager<ApplicationUser> userManager,SignInManager<ApplicationUser> signInManager, RoleManager<ApplicationRole> rolemanager) : base(userManager, signInManager, rolemanager)
        {
            _userManager = userManager;
            service = new MatrixService();
            Aservice = new AccessRightService();
        }
        public async Task<ActionResult> Matrix()
        {
            try
            {
                BaseResponseDTO<string> dt = new BaseResponseDTO<string>();
                dt = await Aservice.GetViewTitle("Matrix");
                ViewBag.Name = dt.Data;
                return View("~/Views/Modules/Access/Matrix/Matrix.cshtml");
            }
            catch (Exception ex)
            {
                return BadRequest();

            }

        }
        [HttpPost]
        public async Task<ActionResult<BaseResponseDTO<List<CRUDMatrix>>>> GetTree()
        {
            try
            {
                BaseResponseDTO<List<CRUDMatrix>> tree = new BaseResponseDTO<List<CRUDMatrix>>();
                tree = await service.GetTree();
                return Ok(tree);


            }
            catch (Exception ex)
            {

                return BadRequest();

            }
        }
        [HttpPost]
        public async Task<ActionResult<BaseResponseDTO<bool>>> CRUDMatrix([FromBody] BaseRequestDTO<List<CRUDMatrix>> dto)
        {
            try
            {
                BaseResponseDTO<bool> BaseResponseDTO = new BaseResponseDTO<bool>();
                BaseResponseDTO = await service.CRUDM(dto.Data);
                return Ok(BaseResponseDTO);
            }
            catch (Exception ex)
            {

                return BadRequest();

            }
        }
    }
}
