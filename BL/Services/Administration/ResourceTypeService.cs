using BL.Constants;
using BL.Models.Access;
using BL.Models.Administration;
using BL.Models.Common;
using BL.Services.Common;
using DAL.Common;
using DAL.DBContext;
using DAL.Models.Administration;
using OfficeOpenXml.Drawing.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Services.Administration
{
    public class ResourceTypeService
    {
        private NPFContext context;
        private QueryResult queryResult;
        private MatrixService Mservice;
        public UserMatrixDTO UM = new UserMatrixDTO();
        public ResourceTypeService()
        {
            context = new NPFContext();
            queryResult = new QueryResult();
            Mservice = new MatrixService();
            UM = Mservice.sGetMatrixUserByUserID();
        }

        public BaseResponseDTO<List<ResourceTypeListDTO>> GetAll()
        {
            BaseResponseDTO<List<ResourceTypeListDTO>> dto = new BaseResponseDTO<List<ResourceTypeListDTO>>();
            List<ResourceTypeListDTO> result = new List<ResourceTypeListDTO>();
            string errorMsg = "No Data Found";
            try
            {
                result = (from a in context.NPF_SYS_ResourceType
                          where a.DeleteStatus == false
                          select new ResourceTypeListDTO()
                          {
                              Id = a.Id,
                              Code = a.Code,
                              Name = a.Name,
                              Description = a.Description,
                              IsSystem = a.IsSystem,
                              IgnoreStation = a.IgnoreStation,

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

        public BaseResponseDTO<ResourceTypeCRUDDTO> GetById(long Id)
        {
            BaseResponseDTO<ResourceTypeCRUDDTO> dto = new BaseResponseDTO<ResourceTypeCRUDDTO>();
            ResourceTypeCRUDDTO result = new ResourceTypeCRUDDTO();
            string errorMsg = "No Data Found";
            try
            {
                result = (from a in context.NPF_SYS_ResourceType
                          where a.Id == Id
                          select new ResourceTypeCRUDDTO()
                          {
                              Id = a.Id,
                              Name = a.Name,
                              Description = a.Description,
                              Code = a.Code,
                              IsSystem = a.IsSystem,
                              IgnoreStation = a.IgnoreStation

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

        public async Task<BaseResponseDTO<bool>> SaveAsync(BaseRequestDTO<ResourceTypeCRUDDTO> dataToSave)
        {
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
            BaseResponseDTO<string> BaseDtoS = new BaseResponseDTO<string>();
            try
            {
                if (!context.NPF_SYS_ResourceType.Any(x => x.Name.ToLower() == dataToSave.Data.Name.ToLower() && x.DeleteStatus != true))
                {
                    BaseDtoS = await new GenerateCode().GCodeMain(dataToSave.userToken, "NPF_SYS_ResourceType");
                    NPF_SYS_ResourceType dt = new NPF_SYS_ResourceType()
                    {
                        Code = BaseDtoS.Data,
                        Name = dataToSave.Data.Name,
                        Description = dataToSave.Data.Description,
                        IsSystem = dataToSave.Data.IsSystem,
                        IgnoreStation = dataToSave.Data.IgnoreStation,

                    };
                    context.NPF_SYS_ResourceType.Add(dt);
                    context.SaveChanges();
                    BaseDto.Data = true;
                    BaseDto.QryResult = queryResult.SUCEEDED;
                }
                else
                {
                    BaseDto.Data = false;
                    BaseDto.AdditionalData = "Resource Type Already Exists";
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

        public async Task<BaseResponseDTO<bool>> UpdateAsync(BaseRequestDTO<ResourceTypeCRUDDTO> dataToUpdate)
        {
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
            try
            {
                if (context.NPF_SYS_ResourceType.Any(x => x.Name.ToLower() == dataToUpdate.Data.Name.ToLower()
                && x.DeleteStatus != true && x.Id != dataToUpdate.Data.Id))
                {
                    BaseDto.Data = false;
                    BaseDto.AdditionalData = "Resource Type Already Exist";
                    BaseDto.QryResult = queryResult.FAILED;
                    return BaseDto;
                }
                NPF_SYS_ResourceType RT = context.NPF_SYS_ResourceType.Where(x => x.Id == dataToUpdate.Data.Id).FirstOrDefault();
                if (RT.IsSystem == true)
                {
                    BaseDto.Data = false;
                    BaseDto.AdditionalData = "System Variables Cannot Be Updated";
                    BaseDto.QryResult = queryResult.FAILED;
                    return BaseDto;
                }
                RT.Name = dataToUpdate.Data.Name;
                RT.Description = dataToUpdate.Data.Description; 
                RT.IgnoreStation = dataToUpdate.Data.IgnoreStation;
                context.NPF_SYS_ResourceType.Update(RT);
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
                NPF_SYS_ResourceType upd = context.NPF_SYS_ResourceType.Where(x => x.Id == dataToDelId.Data).FirstOrDefault();
                if (upd.IsSystem == true)
                {
                    BaseDto.Data = false;
                    BaseDto.AdditionalData = "System Variables Cannot Be Deleted";
                    BaseDto.QryResult = queryResult.FAILED;
                    return BaseDto;
                }
                else
                {
                    upd.DeleteStatus = true;
                    context.NPF_SYS_ResourceType.Update(upd);
                    context.SaveChanges();
                    BaseDto.Data = true;
                    BaseDto.QryResult = queryResult.SUCEEDED;
                }

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
