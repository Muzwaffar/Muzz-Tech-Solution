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
    public class IncidentTypeService
    {
        private NPFContext context;
        private QueryResult queryResult;

        public IncidentTypeService()
        {
            context = new NPFContext();
            queryResult = new QueryResult();
        }

        public BaseResponseDTO<List<IncidentTypeDTO>> GetAll()
        {
            BaseResponseDTO<List<IncidentTypeDTO>> dto = new BaseResponseDTO<List<IncidentTypeDTO>>();
            List<IncidentTypeDTO> result = new List<IncidentTypeDTO>();
            string errorMsg = "No Data Found";
            try
            {
                result = (from a in context.NPF_SYS_IncidentType
                          where a.DeleteStatus == false
                          select new IncidentTypeDTO()
                          {
                              Id = a.Id,
                              Code = a.Code,
                              Name = a.Name,
                              Description = a.Description
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

        public BaseResponseDTO<List<DropDown>> GetAllDropDownValues()
        {
            throw new NotImplementedException();
        }

        public BaseResponseDTO<IncidentTypeDTO> GetById(long Id)
        {
            BaseResponseDTO<IncidentTypeDTO> dto = new BaseResponseDTO<IncidentTypeDTO>();
            IncidentTypeDTO result = new IncidentTypeDTO();
            string errorMsg = "No Data Found";
            try
            {
                result = (from a in context.NPF_SYS_IncidentType
                          where a.DeleteStatus == false && a.Id == Id
                          select new IncidentTypeDTO()
                          {
                              Id = a.Id,
                              Code = a.Code,
                              Name = a.Name,
                              Description = a.Description
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

        public async Task<BaseResponseDTO<bool>> SaveAsync(BaseRequestDTO<IncidentTypeDTO> dataToSave)
        {
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
            BaseResponseDTO<string> BaseDtoS = new BaseResponseDTO<string>();
            try
            {
                if (!context.NPF_SYS_IncidentType.Any(x => x.Name.ToLower() == dataToSave.Data.Name.ToLower() && x.DeleteStatus != true))
                {
                    BaseDtoS = await new GenerateCode().GCodeMain(dataToSave.userToken, "NPF_SYS_IncidentType");
                    NPF_SYS_IncidentType ITT = new NPF_SYS_IncidentType()
                    {
                        Code = BaseDtoS.Data,
                        Name = dataToSave.Data.Name,
                        Description = dataToSave.Data.Description
                    };
                    context.NPF_SYS_IncidentType.Add(ITT);
                    context.SaveChanges();

                    BaseDto.Data = true;
                    BaseDto.QryResult = queryResult.SUCEEDED;
                }
                else
                {
                    BaseDto.Data = false;
                    BaseDto.AdditionalData = "Zone Type Already Exist";
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

        public async Task<BaseResponseDTO<bool>> UpdateAsync(BaseRequestDTO<IncidentTypeDTO> dataToUpdate)
        {
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
            try
            {
                if (!context.NPF_SYS_IncidentType.Any(x => x.Name.ToLower() == dataToUpdate.Data.Name.ToLower() && x.DeleteStatus != true
                && x.Id != dataToUpdate.Data.Id))
                {
                    NPF_SYS_IncidentType ITT = context.NPF_SYS_IncidentType.Where(x => x.Id == dataToUpdate.Data.Id).FirstOrDefault();
                    ITT.Name = dataToUpdate.Data.Name;
                    ITT.Description = dataToUpdate.Data.Description;
                    context.NPF_SYS_IncidentType.Update(ITT);
                    context.SaveChanges();
                    BaseDto.Data = true;
                    BaseDto.QryResult = queryResult.SUCEEDED;
                }
                else
                {
                    BaseDto.Data = false;
                    BaseDto.AdditionalData = "Zone Type Already Exist";
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
                NPF_SYS_IncidentType upd = context.NPF_SYS_IncidentType.Where(x => x.Id == dataToDelId).FirstOrDefault();
                upd.DeleteStatus = true;
                context.NPF_SYS_IncidentType.Update(upd);
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
