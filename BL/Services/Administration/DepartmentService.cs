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
    public class DepartmentService
    {
        private NPFContext context;
        private QueryResult queryResult;
        public DepartmentService()
        {
            context = new NPFContext();
            queryResult = new QueryResult();
        }

        public BaseResponseDTO<List<DepartmentListDTO>> GetAll()
        {
            BaseResponseDTO<List<DepartmentListDTO>> dto = new BaseResponseDTO<List<DepartmentListDTO>>();
            List<DepartmentListDTO> result = new List<DepartmentListDTO>();
            string errorMsg = "No Data Found";
            try
            {
                result = (from a in context.NPF_SYS_Department
                          where a.DeleteStatus == false
                          select new DepartmentListDTO()
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

        public BaseResponseDTO<DepartmentCRUDTO> GetById(long Id)
        {
            BaseResponseDTO<DepartmentCRUDTO> dto = new BaseResponseDTO<DepartmentCRUDTO>();
            DepartmentCRUDTO result = new DepartmentCRUDTO();
            string errorMsg = "No Data Found";
            try
            {
                result = (from a in context.NPF_SYS_Department
                          where a.Id == Id
                          select new DepartmentCRUDTO()
                          {
                              Id = a.Id,
                              Name = a.Name,
                              Description = a.Description,
                              Code = a.Code
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

        public async Task<BaseResponseDTO<bool>> SaveAsync(BaseRequestDTO<DepartmentCRUDTO> dataToSave)
        {
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
            BaseResponseDTO<string> BaseDtoS = new BaseResponseDTO<string>();
            try
            {
                if (!context.NPF_SYS_Department.Any(x => x.Name.ToLower() == dataToSave.Data.Name.ToLower() && x.DeleteStatus != true))
                {
                    BaseDtoS = await new GenerateCode().GCodeMain(dataToSave.userToken, "NPF_SYS_Department");
                    NPF_SYS_Department dt = new NPF_SYS_Department()
                    {
                        Code = BaseDtoS.Data,
                        Name = dataToSave.Data.Name,
                        Description = dataToSave.Data.Description
                    };
                        context.NPF_SYS_Department.Add(dt);
                        context.SaveChanges();
                        BaseDto.Data = true;
                        BaseDto.QryResult = queryResult.SUCEEDED;
                }

                else
                {
                    BaseDto.Data = false;
                    BaseDto.AdditionalData = "Department Already Exists";
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

        public async Task<BaseResponseDTO<bool>> UpdateAsync(BaseRequestDTO<DepartmentCRUDTO> dataToUpdate)
        {
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
            try
            {
                if (context.NPF_SYS_Department.Any(x => x.Name.ToLower() == dataToUpdate.Data.Name.ToLower()
                && x.DeleteStatus != true && x.Id != dataToUpdate.Data.Id ))
                {
                    BaseDto.Data = false;
                    BaseDto.AdditionalData = "Department Already Exist";
                    BaseDto.QryResult = queryResult.FAILED;
                    return BaseDto;
                }
                NPF_SYS_Department PT = context.NPF_SYS_Department.Where(x => x.Id == dataToUpdate.Data.Id).FirstOrDefault();
                PT.Name = dataToUpdate.Data.Name;
                PT.Description = dataToUpdate.Data.Description;
                context.NPF_SYS_Department.Update(PT);
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

        public BaseResponseDTO<bool> Delete(BaseRequestDTO<int> dataToDelId)
        {
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
            try
            {
                int Id = dataToDelId.Data;
                NPF_SYS_Department upd = context.NPF_SYS_Department.Where(x => x.Id == Id).FirstOrDefault();
                upd.DeleteStatus = true;
                context.NPF_SYS_Department.Update(upd);
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
