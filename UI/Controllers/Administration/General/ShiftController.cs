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
	public class ShiftController : NPFController
    {
		public UserManager<ApplicationUser> _userManager;
		public ShiftService service;
		public AccessRightService Aservice;
		public ShiftController(Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> userManager,
		  SignInManager<ApplicationUser> signInManager, Microsoft.AspNetCore.Identity.RoleManager<ApplicationRole> rolemanager) : base(userManager, signInManager, rolemanager)
		{
			service = new ShiftService();
			_userManager = userManager;
			Aservice = new AccessRightService();
		}

		public async Task<ActionResult> List()
		{
			BaseResponseDTO<string> dt = new BaseResponseDTO<string>();
			dt = await Aservice.GetViewTitle("Shift");
			ViewBag.Name = dt.Data;
			return View("~/Views/Modules/Administration/General/Shift/ShiftList.cshtml");
		}

		[HttpPost]
		public async Task<ActionResult<BaseResponseDTO<List<ShiftDTO>>>> GetList()
		{
			try
			{
				BaseResponseDTO<List<ShiftDTO>> List = new BaseResponseDTO<List<ShiftDTO>>();
				List = service.GetAll();
				return Ok(List);


			}
			catch (Exception ex)
			{

				return BadRequest();

			}
		}

		//[HttpPost]
		//public async Task<ActionResult<BaseResponseDTO<List<DropDown>>>> GetAllDropDownValues()
		//{
		//	try
		//	{
		//		BaseResponseDTO<List<DropDown>> List = new BaseResponseDTO<List<DropDown>>();
		//		List = service.GetAllDropDownValues();
		//		return Ok(List);


		//	}
		//	catch (Exception ex)
		//	{

		//		return BadRequest();

		//	}
		//}
		[HttpPost]
		public async Task<ActionResult> GetById([FromBody] BaseRequestDTO<long> dto)
		{
			try
			{
				if (dto == null)
				{
					ShiftDTO VC = new ShiftDTO();
					return PartialView("~/Views/Modules/Administration/General/Shift/ShiftAddEdit.cshtml", VC);
				}
				else
				{
					BaseResponseDTO<ShiftDTO> VC = new BaseResponseDTO<ShiftDTO>();
					VC = service.GetById(dto.Data);
					return PartialView("~/Views/Modules/Administration/General/Shift/ShiftAddEdit.cshtml", VC.Data);
				}

			}
			catch (Exception ex)
			{

				return BadRequest();

			}
		}
		[HttpPost]
		public async Task<ActionResult<BaseResponseDTO<bool>>> CreateUpdate([FromBody] BaseRequestDTO<ShiftDTO> dto)
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
