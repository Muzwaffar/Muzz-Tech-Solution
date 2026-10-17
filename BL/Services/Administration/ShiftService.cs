using BL.Constants;
using BL.Models.Administration;
using BL.Models.Common;
using BL.Services.Common;
using DAL.DBContext;
using DAL.Models.Administration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Services.Administration
{
	public class ShiftService
	{
		private NPFContext context;
		private QueryResult queryResult;
		public ShiftService() {
			context = new NPFContext();
			queryResult = new QueryResult();
		}

		public BaseResponseDTO<List<ShiftDTO>> GetAll()
		{
			BaseResponseDTO<List<ShiftDTO>> dto = new BaseResponseDTO<List<ShiftDTO>>();
			List<ShiftDTO> result = new List<ShiftDTO>();
			string errorMsg = "No Data Found";
			try
			{
				result = (from a in context.NPF_SYS_Shift
						  where a.DeleteStatus == false
						  select new ShiftDTO()
						  {
							  Id = a.Id,
							  Code = a.Code,
							  Name = a.Name,
							  Description = a.Description,
							  TimeFrom = a.TimeFrom,
							  TimeTo = a.TimeTo,
						  }).ToList();
				dto.Data = result;
				dto.QryResult = queryResult.SUCEEDED;
			}
			catch (Exception ex)
			{
				dto.Data = result;
				dto.AdditionalData = errorMsg;
				dto.QryResult = queryResult.FAILED;
			}

			return dto;
		}

		public BaseResponseDTO<ShiftDTO> GetById(long Id)
		{
			BaseResponseDTO<ShiftDTO> dto = new BaseResponseDTO<ShiftDTO>();
			ShiftDTO result = new ShiftDTO();
			string errorMsg = "No Data Found";
			try
			{
				result = (from a in context.NPF_SYS_Shift
						  where a.DeleteStatus == false && a.Id == Id
						  select new ShiftDTO()
						  {
							  Id = a.Id,
							  Code = a.Code,
							  Name = a.Name,
							  Description = a.Description,
							  TimeFrom = a.TimeFrom,
							  TimeTo = a.TimeTo,
						  }).FirstOrDefault();
				if (result == null)
				{
					dto.Data = result;
					dto.QryResult = queryResult.FAILED;
				}
				else
				{
					dto.Data = result;
					dto.QryResult = queryResult.SUCEEDED;
				}
			}
			catch (Exception e)
			{
				dto.Data = result;
				dto.AdditionalData = errorMsg;
				dto.QryResult = queryResult.FAILED;
			}


			return dto;
		}

		public async Task<BaseResponseDTO<bool>> SaveAsync(BaseRequestDTO<ShiftDTO> dataToSave)
		{
			BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
			BaseResponseDTO<string> BaseDtoS = new BaseResponseDTO<string>();
			try
			{
				if (!context.NPF_SYS_Shift.Any(x => x.Name.ToLower() == dataToSave.Data.Name.ToLower() && x.DeleteStatus != true))
				{
					BaseDtoS = await new GenerateCode().GCodeMain(dataToSave.userToken, "NPF_SYS_Shift");
					NPF_SYS_Shift ZN = new NPF_SYS_Shift()
					{
						Code = BaseDtoS.Data,
						Name = dataToSave.Data.Name,
						Description = dataToSave.Data.Description,
						TimeFrom = dataToSave.Data.TimeFrom,
						TimeTo = dataToSave.Data.TimeTo,
					};
					context.NPF_SYS_Shift.Add(ZN);
					context.SaveChanges();

					BaseDto.Data = true;
					BaseDto.QryResult = queryResult.SUCEEDED;
				}
				else
				{
					BaseDto.Data = false;
					BaseDto.AdditionalData = "Shift Already Exist";
					BaseDto.QryResult = queryResult.FAILED;
				}
			}
			catch (Exception ex)
			{
				BaseDto.Data = false;
				BaseDto.AdditionalData = "Error Adding Record";
				BaseDto.QryResult = queryResult.FAILED;
			}
			return BaseDto;
		}

		public async Task<BaseResponseDTO<bool>> UpdateAsync(BaseRequestDTO<ShiftDTO> dataToUpdate)
		{
			BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
			try
			{
				if (!context.NPF_SYS_Shift.Any(x => x.Name.ToLower() == dataToUpdate.Data.Name.ToLower() && x.DeleteStatus != true
				&& x.Id != dataToUpdate.Data.Id))
				{
					NPF_SYS_Shift Shift = context.NPF_SYS_Shift.Where(x => x.Id == dataToUpdate.Data.Id).FirstOrDefault();
					Shift.Name = dataToUpdate.Data.Name;
					Shift.Description = dataToUpdate.Data.Description;
					Shift.TimeFrom = dataToUpdate.Data.TimeFrom;
					Shift.TimeTo = dataToUpdate.Data.TimeTo;
					context.NPF_SYS_Shift.Update(Shift);
					context.SaveChanges();
					BaseDto.Data = true;
					BaseDto.QryResult = queryResult.SUCEEDED;
				}
				else
				{
					BaseDto.Data = false;
					BaseDto.AdditionalData = "Shift Already Exist";
					BaseDto.QryResult = queryResult.FAILED;
				}
			}
			catch (Exception)
			{
				BaseDto.Data = false;
				BaseDto.AdditionalData = "Error Updating Record";
				BaseDto.QryResult = queryResult.FAILED;
			}
			return BaseDto;
		}

		public BaseResponseDTO<bool> Delete(long dataToDelId)
		{
			BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
			try
			{
				NPF_SYS_Shift upd = context.NPF_SYS_Shift.Where(x => x.Id == dataToDelId).FirstOrDefault();
				upd.DeleteStatus = true;
				context.NPF_SYS_Shift.Update(upd);
				context.SaveChanges();
				BaseDto.Data = true;
				BaseDto.QryResult = queryResult.SUCEEDED;

			}
			catch (Exception)
			{
				BaseDto.Data = false;
				BaseDto.AdditionalData = "Error Deleting Record";
				BaseDto.QryResult = queryResult.FAILED;
			}
			return BaseDto;
		}
	}
}
