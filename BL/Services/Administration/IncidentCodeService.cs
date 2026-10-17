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
    public class IncidentCodeService
    {
        private NPFContext context;
        private QueryResult queryResult;

        public IncidentCodeService()
        {
            context = new NPFContext();
            queryResult = new QueryResult();
        }

        public BaseResponseDTO<List<IncidentCodeListDTO>> GetAll()
        {
            BaseResponseDTO<List<IncidentCodeListDTO>> dto = new BaseResponseDTO<List<IncidentCodeListDTO>>();
            List<IncidentCodeListDTO> result = new List<IncidentCodeListDTO>();
            string errorMsg = "No Data Found";
            try
            {
                result = (from a in context.NPF_SYS_IncidentCode
                          where a.DeleteStatus == false
                          select new IncidentCodeListDTO()
                          {
                              Id = a.Id,
                              Code = a.Code,
                              Name = a.Name,
                              Description = a.Description,
                              IncidentType = a.IncidentType,
                              IncidentSubType=a.IncidentSubType.Name
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
            BaseResponseDTO<List<DropDown>> dto = new BaseResponseDTO<List<DropDown>>();
            List<DropDown> Ddl = new List<DropDown>();
            DropDown Dd = new DropDown();
            string errorMsg = "No Data Found";
            try
            {

                List<NPF_SYS_IncidentSubType> Mk = context.NPF_SYS_IncidentSubType.Where(x => x.DeleteStatus == false).ToList();
                Dd.title = "IncidentSubType";
                Dd.items = new List<DropDownItem>();
                foreach (NPF_SYS_IncidentSubType mk in Mk)
                {
                    DropDownItem Ddi = new DropDownItem();
                    Ddi.Id = mk.Id;
                    Ddi.text = mk.Name;
                    Dd.items.Add(Ddi);
                }
                Ddl.Add(Dd);
                dto.Data = Ddl;
                dto.QryResult = queryResult.SUCEEDED;
            }
            catch (Exception ex)
            {
                dto.Data = Ddl;
                dto.AdditionalData = errorMsg;
                dto.QryResult = queryResult.FAILED;
            }
            return dto;
        }

        public BaseResponseDTO<IncidentCodeDTO> GetById(long Id)
        {
            BaseResponseDTO<IncidentCodeDTO> dto = new BaseResponseDTO<IncidentCodeDTO>();
            IncidentCodeDTO result = new IncidentCodeDTO();
            string errorMsg = "No Data Found";
            try
            {
                result = (from a in context.NPF_SYS_IncidentCode
                          where a.DeleteStatus == false && a.Id == Id
                          select new IncidentCodeDTO()
                          {
                              Id = a.Id,
                              Code = a.Code,
                              Name = a.Name,
                              Description = a.Description,
                              IncidentSubType = a.IncidentSubType.Id,
                              IncidentType= a.IncidentType,
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

        public BaseResponseDTO<IncidentSubTypeDTO> GetIncidentTypeByInidentSubTypeId(long Id)
        {
            BaseResponseDTO<IncidentSubTypeDTO> dto = new BaseResponseDTO<IncidentSubTypeDTO>();
            //BaseResponseDTO<IncidentTypeDTO> dtoMain = new BaseResponseDTO<IncidentTypeDTO>();
            IncidentSubTypeDTO result = new IncidentSubTypeDTO();
            string errorMsg = "No Data Found";
            try
            {
                result = (from a in context.NPF_SYS_IncidentSubType
                          where a.DeleteStatus == false && a.Id == Id
                          select new IncidentSubTypeDTO()
                          {
                              Id = a.IncidentType.Id,
                              Code = a.IncidentType.Code,
                              Name = a.IncidentType.Name,
                              Description = a.IncidentType.Description,
                              IncidentType=a.IncidentType.Id,
                              
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

        public async Task<BaseResponseDTO<bool>> SaveAsync(BaseRequestDTO<IncidentCodeDTO> dataToSave)
        {
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
            BaseResponseDTO<string> BaseDtoS = new BaseResponseDTO<string>();
            try
            {
                if (!context.NPF_SYS_IncidentCode.Any(x => x.Name.ToLower() == dataToSave.Data.Name.ToLower() && x.DeleteStatus != true))
                {
                    NPF_SYS_IncidentSubType incidentsubtype = context.NPF_SYS_IncidentSubType.Where(x => x.Id == dataToSave.Data.IncidentSubType).FirstOrDefault();
                    BaseDtoS = await new GenerateCode().GCodeMain(dataToSave.userToken, "NPF_SYS_IncidentCode");
                    NPF_SYS_IncidentCode IC = new NPF_SYS_IncidentCode()
                    {
                        Code = BaseDtoS.Data,
                        Name = dataToSave.Data.Name,
                        Description = dataToSave.Data.Description,
                        IncidentSubType = incidentsubtype,
                        IncidentType= dataToSave.Data.IncidentType,
                    };
                    context.NPF_SYS_IncidentCode.Add(IC);
                    context.SaveChanges();

                    BaseDto.Data = true;
                    BaseDto.QryResult = queryResult.SUCEEDED;
                }
                else
                {
                    BaseDto.Data = false;
                    BaseDto.AdditionalData = "Error Adding Record";
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

        public async Task<BaseResponseDTO<bool>> UpdateAsync(BaseRequestDTO<IncidentCodeDTO> dataToUpdate)
        {
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
            try
            {
                if (!context.NPF_SYS_IncidentSubType.Any(x => x.Name.ToLower() == dataToUpdate.Data.Name.ToLower() && x.DeleteStatus != true
                && x.Id != dataToUpdate.Data.Id))
                {
                    NPF_SYS_IncidentSubType incidentsubtype = context.NPF_SYS_IncidentSubType.Where(x => x.Id == dataToUpdate.Data.IncidentSubType).FirstOrDefault();
                    NPF_SYS_IncidentCode IncidentCode = context.NPF_SYS_IncidentCode.Where(x => x.Id == dataToUpdate.Data.Id).FirstOrDefault();
                    IncidentCode.Name = dataToUpdate.Data.Name;
                    IncidentCode.Description = dataToUpdate.Data.Description;
                    IncidentCode.IncidentSubType = incidentsubtype;
                    IncidentCode.IncidentType = dataToUpdate.Data.IncidentType;
                    context.NPF_SYS_IncidentCode.Update(IncidentCode);
                    context.SaveChanges();
                    BaseDto.Data = true;
                    BaseDto.QryResult = queryResult.SUCEEDED;
                }
                else
                {
                    BaseDto.Data = false;
                    BaseDto.AdditionalData = "Incident code Already Exist";
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
                NPF_SYS_IncidentCode upd = context.NPF_SYS_IncidentCode.Where(x => x.Id == dataToDelId).FirstOrDefault();
                upd.DeleteStatus = true;
                context.NPF_SYS_IncidentCode.Update(upd);
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
