using BL.Constants;
using BL.Models.Form;
using BL.Models.Common;
using BL.Services.Common;
using DAL.DBContext;
using DAL.Models.Administration;
using DAL.Models.Form;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Services.Form
{
    public class FormTypeService
    {
        private NPFContext context;
        private QueryResult queryResult;
        public FormTypeService()
        {
            context = new NPFContext();
            queryResult = new QueryResult();
        }

        public BaseResponseDTO<List<FormTypeDTO>> GetAll()
        {
            BaseResponseDTO<List<FormTypeDTO>> dto = new BaseResponseDTO<List<FormTypeDTO>>();
            List<FormTypeDTO> result = new List<FormTypeDTO>();
            string errorMsg = "No Data Found";
            try
            {
                result = (from a in context.NPF_FRM_FormType
                          where a.DeleteStatus == false
                          select new FormTypeDTO()
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

        public BaseResponseDTO<FormTypeDTO> GetById(long Id)
        {
            BaseResponseDTO<FormTypeDTO> dto = new BaseResponseDTO<FormTypeDTO>();
            FormTypeDTO result = new FormTypeDTO();
            string errorMsg = "No Data Found";
            try
            {
                result = (from a in context.NPF_FRM_FormType
                          where a.Id == Id
                          select new FormTypeDTO()
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

        public async Task<BaseResponseDTO<bool>> SaveAsync(BaseRequestDTO<FormTypeDTO> dataToSave)
        {
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
            BaseResponseDTO<string> BaseDtoS = new BaseResponseDTO<string>();
            try
            {
                if (!context.NPF_FRM_FormType.Any(x => x.Name.ToLower() == dataToSave.Data.Name.ToLower() && x.DeleteStatus != true))
                {
                    BaseDtoS = await new GenerateCode().GCodeMain(dataToSave.userToken, "NPF_FRM_FormType");
                    NPF_FRM_FormType dt = new NPF_FRM_FormType()
                    {
                        Code = BaseDtoS.Data,
                        Name = dataToSave.Data.Name,
                        Description = dataToSave.Data.Description
                    };
                    context.NPF_FRM_FormType.Add(dt);
                    context.SaveChanges();
                    BaseDto.Data = true;
                    BaseDto.QryResult = queryResult.SUCEEDED;
                }
                else
                {
                    BaseDto.Data = false;
                    BaseDto.AdditionalData = "Form Type Already Exists";
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

        public async Task<BaseResponseDTO<bool>> UpdateAsync(BaseRequestDTO<FormTypeDTO> dataToUpdate)
        {
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
            try
            {
                if (context.NPF_FRM_FormType.Any(x => x.Name.ToLower() == dataToUpdate.Data.Name.ToLower() && x.DeleteStatus != true && x.Id != dataToUpdate.Data.Id))
                {
                    BaseDto.Data = false;
                    BaseDto.AdditionalData = "Form Type Already Exist";
                    BaseDto.QryResult = queryResult.FAILED;
                    return BaseDto;
                }
                NPF_FRM_FormType Form = context.NPF_FRM_FormType.Where(x => x.Id == dataToUpdate.Data.Id).FirstOrDefault();
                Form.Code = dataToUpdate.Data.Code;
                Form.Name = dataToUpdate.Data.Name;
                Form.Description = dataToUpdate.Data.Description;
                context.NPF_FRM_FormType.Update(Form);
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
                NPF_FRM_FormType upd = context.NPF_FRM_FormType.Where(x => x.Id == dataToDelId.Data).FirstOrDefault();
                upd.DeleteStatus = true;
                context.NPF_FRM_FormType.Update(upd);
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
