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
    public class NatureOfIncidentService
    {
        private NPFContext context;
        private QueryResult queryResult;

        public NatureOfIncidentService()
        {
            context = new NPFContext();
            queryResult = new QueryResult();
        }


        public BaseResponseDTO<List<NatureOfIncidentListDTO>> GetAll()
        {
            BaseResponseDTO<List<NatureOfIncidentListDTO>> dto = new BaseResponseDTO<List<NatureOfIncidentListDTO>>();
            List<NatureOfIncidentListDTO> result = new List<NatureOfIncidentListDTO>();
            string errorMsg = "No Data Found";
            try
            {
                result = (from a in context.NPF_SYS_NatureOfIncident
                          where a.DeleteStatus == false
                          select new NatureOfIncidentListDTO()
                          {
                              Id = a.Id,
                              Code = a.Code,
                              Name = a.Name,
                              Description = a.Description,
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

        public BaseResponseDTO<NatureOfIncidentCRUDDTO> GetById(long Id)
        {
            BaseResponseDTO<NatureOfIncidentCRUDDTO> dto = new BaseResponseDTO<NatureOfIncidentCRUDDTO>();
            NatureOfIncidentCRUDDTO result = new NatureOfIncidentCRUDDTO();
            string errorMsg = "No Data Found";
            try
            {
                result = (from a in context.NPF_SYS_NatureOfIncident
                          where a.Id == Id
                          select new NatureOfIncidentCRUDDTO()
                          {
                              Id = a.Id,
                              Code = a.Code,
                              Name = a.Name,
                              Description = a.Description,
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


        public async Task<BaseResponseDTO<bool>> SaveAsync(BaseRequestDTO<NatureOfIncidentCRUDDTO> dataToSave)
        {
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
            BaseResponseDTO<string> BaseDtoS = new BaseResponseDTO<string>();
            try
            {
                if (!context.NPF_SYS_NatureOfIncident.Any(x => x.Name == dataToSave.Data.Name && x.DeleteStatus != true))
                {
                    BaseDtoS = await new GenerateCode().GCodeMain(dataToSave.userToken, "NPF_SYS_NatureOfIncident");
                    NPF_SYS_NatureOfIncident dt = new NPF_SYS_NatureOfIncident()
                    {
                        Code = BaseDtoS.Data,
                        Name = dataToSave.Data.Name,
                        Description = dataToSave.Data.Description ?? string.Empty,
                    };
                    context.NPF_SYS_NatureOfIncident.Add(dt);
                    context.SaveChanges();
                    BaseDto.ExtData = dt.Id.ToString();
                    BaseDto.Data = true;
                    BaseDto.QryResult = queryResult.SUCEEDED;
                }
                else
                {
                    BaseDto.Data = false;
                    BaseDto.AdditionalData = "External Agency Already Exist";
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

        public async Task<BaseResponseDTO<bool>> UpdateAsync(BaseRequestDTO<NatureOfIncidentCRUDDTO> dataToUpdate)
        {
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
            try
            {
                if (context.NPF_SYS_NatureOfIncident.Any(x => x.Name == dataToUpdate.Data.Name
               && x.DeleteStatus != true && x.Id != dataToUpdate.Data.Id))
                {
                    BaseDto.Data = false;
                    BaseDto.AdditionalData = "External Agency Already Exist";
                    BaseDto.QryResult = queryResult.FAILED;
                    return BaseDto;
                }

                NPF_SYS_NatureOfIncident Update = context.NPF_SYS_NatureOfIncident.Where(x => x.Id == dataToUpdate.Data.Id).FirstOrDefault();

                Update.Name = dataToUpdate.Data.Name;
                Update.Description = dataToUpdate.Data.Description;

                context.NPF_SYS_NatureOfIncident.Update(Update);
                context.SaveChanges();
                BaseDto.Data = true;
                BaseDto.QryResult = queryResult.SUCEEDED;

            }
            catch (Exception ex)
            {
                BaseDto.Data = false;
                BaseDto.AdditionalData = "Error Updating Record";
                BaseDto.QryResult = queryResult.FAILED;
            }
            return BaseDto;
        }

        public BaseResponseDTO<bool> Delete(BaseRequestDTO<long> dataToDelId)
        {
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
            try
            {
                NPF_SYS_NatureOfIncident upd = context.NPF_SYS_NatureOfIncident.Where(x => x.Id == dataToDelId.Data).FirstOrDefault();
                upd.DeleteStatus = true;
                context.NPF_SYS_NatureOfIncident.Update(upd);
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
