using BL.Models.Administration;
using BL.Models.Common;
using BL.Services.Access.AccessRights;
using BL.Services.Administration;
using DAL.Models;
using DAL.Models.Administration;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UI.Helper;

namespace UI.Controllers.Administration.General
{
    public class ResourceController : NPFController
    {
        public UserManager<ApplicationUser> _userManager;
        public ResourceService service;
        public AccessRightService Aservice;
        public ResourceController(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager, RoleManager<ApplicationRole> rolemanager) : base(userManager, signInManager, rolemanager)
        {
            service = new ResourceService();
            _userManager = userManager;
            Aservice = new AccessRightService();
        }
        public async Task<ActionResult> List()
        {
            BaseResponseDTO<string> dt = new BaseResponseDTO<string>();
            dt = await Aservice.GetViewTitle("Resource");
            ViewBag.Name = dt.Data;
            return View("~/Views/Modules/Administration/General/Resource/ResourceList.cshtml");
        }

        public async Task<ActionResult> Resource(long? Id)
        {
            try
            {

                BaseResponseDTO<string> dt = new BaseResponseDTO<string>();
                dt = await Aservice.GetViewTitle("Resource");
                ViewBag.Name = dt.Data;

                BaseResponseDTO<ResourceCRUDTO> dtdata = new BaseResponseDTO<ResourceCRUDTO>();

                if (Id == null)
                {
                    return PartialView("~/Views/Modules/Administration/General/Resource/ResourceAddEdit.cshtml", dtdata.Data);
                }
                else
                {
                    dtdata = service.GetById(Id.Value);
                    return PartialView("~/Views/Modules/Administration/General/Resource/ResourceAddEdit.cshtml", dtdata.Data);
                }
            }
            catch (Exception ex)
            {
                return BadRequest();

            }

        }
        [HttpPost]
        public async Task<ActionResult<BaseResponseDTO<List<ResourceListDTO>>>> GetList()
        {
            try
            {
                BaseResponseDTO<List<ResourceListDTO>> CusList = new BaseResponseDTO<List<ResourceListDTO>>();
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
                    ResourceCRUDTO dt = new ResourceCRUDTO();
                    return PartialView("~/Views/Modules/Administration/General/Resource/ResourceAddEdit.cshtml", dt);
                }
                else
                {
                    BaseResponseDTO<ResourceCRUDTO> dt = new BaseResponseDTO<ResourceCRUDTO>();
                    dt = service.GetById(dto.Data);
                    return PartialView("~/Views/Modules/Administration/General/Resource/ResourceAddEdit.cshtml", dt.Data);
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
        public async Task<ActionResult<BaseResponseDTO<bool>>> CreateUpdate([FromBody] BaseRequestDTO<ResourceCRUDTO> dto)
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
