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
    public class ExternalAgenciesService
    {
        private NPFContext context;
        private QueryResult queryResult;

        public ExternalAgenciesService()
        {
            context = new NPFContext();
            queryResult = new QueryResult();
        }

        public BaseResponseDTO<List<ExternalAgenciesListDTO>> GetAll()
        {
            BaseResponseDTO<List<ExternalAgenciesListDTO>> dto = new BaseResponseDTO<List<ExternalAgenciesListDTO>>();
            List<ExternalAgenciesListDTO> result = new List<ExternalAgenciesListDTO>();
            string errorMsg = "No Data Found";
            try
            {
                result = (from a in context.NPF_SYS_ExternalAgencies
                          where a.DeleteStatus == false
                          select new ExternalAgenciesListDTO()
                          {
                              Id = a.Id,
                              Code = a.Code,
                              Name = a.Name,
                              ContactNumber = a.ContactNumber,
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

        public BaseResponseDTO<ExternalAgenciesCRUDDTO> GetById(long Id)
        {
            BaseResponseDTO<ExternalAgenciesCRUDDTO> dto = new BaseResponseDTO<ExternalAgenciesCRUDDTO>();
            ExternalAgenciesCRUDDTO result = new ExternalAgenciesCRUDDTO();
            string errorMsg = "No Data Found";
            try
            {
                result = (from a in context.NPF_SYS_ExternalAgencies
                          where a.Id == Id
                          select new ExternalAgenciesCRUDDTO()
                          {
                              Id = a.Id,
                              Code = a.Code,
                              Name = a.Name,
                              ContactNumber = a.ContactNumber,
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


        public async Task<BaseResponseDTO<bool>> SaveAsync(BaseRequestDTO<ExternalAgenciesCRUDDTO> dataToSave)
        {
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
            BaseResponseDTO<string> BaseDtoS = new BaseResponseDTO<string>();
            try
            {
                if (!context.NPF_SYS_ExternalAgencies.Any(x => x.Name == dataToSave.Data.Name && x.DeleteStatus != true))
                {
                    BaseDtoS = await new GenerateCode().GCodeMain(dataToSave.userToken, "NPF_SYS_ExternalAgencies");
                    NPF_SYS_ExternalAgencies dt = new NPF_SYS_ExternalAgencies()
                    {
                        Code = BaseDtoS.Data,
                        Name = dataToSave.Data.Name,
                        Description = dataToSave.Data.Description ?? string.Empty,
                        ContactNumber = dataToSave.Data.ContactNumber,
                    };
                    context.NPF_SYS_ExternalAgencies.Add(dt);
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

        public async Task<BaseResponseDTO<bool>> UpdateAsync(BaseRequestDTO<ExternalAgenciesCRUDDTO> dataToUpdate)
        {
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
            try
            {
                if (context.NPF_SYS_ExternalAgencies.Any(x => x.Name == dataToUpdate.Data.Name
               && x.DeleteStatus != true && x.Id != dataToUpdate.Data.Id))
                {
                    BaseDto.Data = false;
                    BaseDto.AdditionalData = "External Agency Already Exist";
                    BaseDto.QryResult = queryResult.FAILED;
                    return BaseDto;
                }

                NPF_SYS_ExternalAgencies ExternalAgencies = context.NPF_SYS_ExternalAgencies.Where(x => x.Id == dataToUpdate.Data.Id).FirstOrDefault();

                ExternalAgencies.Name = dataToUpdate.Data.Name;
                ExternalAgencies.Description = dataToUpdate.Data.Description;
                ExternalAgencies.ContactNumber = dataToUpdate.Data.ContactNumber;

                context.NPF_SYS_ExternalAgencies.Update(ExternalAgencies);
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
                NPF_SYS_ExternalAgencies upd = context.NPF_SYS_ExternalAgencies.Where(x => x.Id == dataToDelId.Data).FirstOrDefault();
                upd.DeleteStatus = true;
                context.NPF_SYS_ExternalAgencies.Update(upd);
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
