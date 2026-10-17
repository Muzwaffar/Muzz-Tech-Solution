using BL.Models.Common;
using BL.Services.Access.AccessRights;
using BL.Services.Administration;
using DAL.Models.Administration;
using DAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UI.Helper;
using BL.Models.Administration;

namespace UI.Controllers.Administration.General
{
    public class DepartmentController : NPFController
    {
        public UserManager<ApplicationUser> _userManager;
        public DepartmentService service;
        public AccessRightService Aservice;
        public DepartmentController(UserManager<ApplicationUser> userManager,
           SignInManager<ApplicationUser> signInManager, RoleManager<ApplicationRole> rolemanager) : base(userManager, signInManager, rolemanager)
        {
            service = new DepartmentService();
            _userManager = userManager;
            Aservice = new AccessRightService();
        }
        public async Task<ActionResult> List()
        {
            BaseResponseDTO<string> dt = new BaseResponseDTO<string>();
            dt = await Aservice.GetViewTitle("Department");
            ViewBag.Name = dt.Data;
            return View("~/Views/Modules/Administration/General/Department/DepartmentList.cshtml");
        }

        [HttpPost]
        public async Task<ActionResult<BaseResponseDTO<List<DepartmentListDTO>>>> GetList()
        {
            try
            {
                BaseResponseDTO<List<DepartmentListDTO>> List = new BaseResponseDTO<List<DepartmentListDTO>>();
                List = service.GetAll();
                return Ok(List);


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
                    DepartmentCRUDTO dt = new DepartmentCRUDTO();
                    return PartialView("~/Views/Modules/Administration/General/Department/DepartmentAddEdit.cshtml", dt);
                }
                else
                {
                    BaseResponseDTO<DepartmentCRUDTO> dt = new BaseResponseDTO<DepartmentCRUDTO>();
                    dt = service.GetById(dto.Data);
                    return PartialView("~/Views/Modules/Administration/General/Department/DepartmentAddEdit.cshtml", dt.Data);
                }

            }
            catch (Exception ex)
            {

                return BadRequest();

            }
        }
        [HttpPost]
        public async Task<ActionResult<BaseResponseDTO<bool>>> CreateUpdate([FromBody] BaseRequestDTO<DepartmentCRUDTO> dto)
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
        public async Task<ActionResult<BaseResponseDTO<bool>>> Delete([FromBody] BaseRequestDTO<int> dto)
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
