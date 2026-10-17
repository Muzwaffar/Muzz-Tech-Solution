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
    public class IncidentSubTypeService
    {
        private NPFContext context;
        private QueryResult queryResult;

        public IncidentSubTypeService()
        {
            context = new NPFContext();
            queryResult = new QueryResult();
        }

        public BaseResponseDTO<List<IncidentListDTO>> GetAll()
        {
            BaseResponseDTO<List<IncidentListDTO>> dto = new BaseResponseDTO<List<IncidentListDTO>>();
            List<IncidentListDTO> result = new List<IncidentListDTO>();
            string errorMsg = "No Data Found";
            try
            {
                result = (from a in context.NPF_SYS_IncidentSubType
                          where a.DeleteStatus == false
                          select new IncidentListDTO()
                          {
                              Id = a.Id,
                              Code = a.Code,
                              Name = a.Name,
                              Description = a.Description,
                              IncidentType = a.IncidentType.Name,
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
            #pragma warning disable CS0168 // Variable is declared but never used
            try
            {

                List<NPF_SYS_IncidentType> Mk = context.NPF_SYS_IncidentType.Where(x => x.DeleteStatus == false).ToList();
                Dd.title = "IncidentType";
                Dd.items = new List<DropDownItem>();
                foreach (NPF_SYS_IncidentType mk in Mk)
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
#pragma warning restore CS0168 // Variable is declared but never used
            return dto;
        }

        public BaseResponseDTO<IncidentSubTypeDTO> GetById(long Id)
        {
            BaseResponseDTO<IncidentSubTypeDTO> dto = new BaseResponseDTO<IncidentSubTypeDTO>();
            IncidentSubTypeDTO result = new IncidentSubTypeDTO();
            string errorMsg = "No Data Found";
            try
            {
                result = (from a in context.NPF_SYS_IncidentSubType
                          where a.DeleteStatus == false && a.Id == Id
                          select new IncidentSubTypeDTO()
                          {
                              Id = a.Id,
                              Code = a.Code,
                              Name = a.Name,
                              Description = a.Description,
                              IncidentType = a.IncidentType.Id
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

        public async Task<BaseResponseDTO<bool>> SaveAsync(BaseRequestDTO<IncidentSubTypeDTO> dataToSave)
        {
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
            BaseResponseDTO<string> BaseDtoS = new BaseResponseDTO<string>();
            try
            {
                if (!context.NPF_SYS_IncidentSubType.Any(x => x.Name.ToLower() == dataToSave.Data.Name.ToLower() && x.DeleteStatus != true && x.IncidentType.Id == dataToSave.Data.IncidentType))
                {
                    NPF_SYS_IncidentType incidenttype = context.NPF_SYS_IncidentType.Where(x => x.Id == dataToSave.Data.IncidentType).FirstOrDefault();
                    BaseDtoS = await new GenerateCode().GCodeMain(dataToSave.userToken, "NPF_SYS_IncidentSubType");
                    NPF_SYS_IncidentSubType IN = new NPF_SYS_IncidentSubType()
                    {
                        Code = BaseDtoS.Data,
                        Name = dataToSave.Data.Name,
                        Description = dataToSave.Data.Description,
                        IncidentType = incidenttype,
                    };
                    context.NPF_SYS_IncidentSubType.Add(IN);
                    context.SaveChanges();

                    BaseDto.Data = true;
                    BaseDto.QryResult = queryResult.SUCEEDED;
                }
                else
                {
                    BaseDto.Data = false;
                    BaseDto.AdditionalData = "Incident Sub Type Already Exist";
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

        public async Task<BaseResponseDTO<bool>> UpdateAsync(BaseRequestDTO<IncidentSubTypeDTO> dataToUpdate)
        {
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
            try
            {
                if (!context.NPF_SYS_IncidentSubType.Any(x => x.Name.ToLower() == dataToUpdate.Data.Name.ToLower() 
                && x.DeleteStatus != true
                && x.IncidentType.Id == dataToUpdate.Data.IncidentType
                && x.Id != dataToUpdate.Data.Id))
                {
                    NPF_SYS_IncidentType incidenttype = context.NPF_SYS_IncidentType.Where(x => x.Id == dataToUpdate.Data.IncidentType).FirstOrDefault();
                    NPF_SYS_IncidentSubType IncidentSubType = context.NPF_SYS_IncidentSubType.Where(x => x.Id == dataToUpdate.Data.Id).FirstOrDefault();
                    IncidentSubType.Name = dataToUpdate.Data.Name;
                    IncidentSubType.Description = dataToUpdate.Data.Description;
                    IncidentSubType.IncidentType = incidenttype;
                    context.NPF_SYS_IncidentSubType.Update(IncidentSubType);
                    context.SaveChanges();
                    BaseDto.Data = true;
                    BaseDto.QryResult = queryResult.SUCEEDED;
                }
                else
                {
                    BaseDto.Data = false;
                    BaseDto.AdditionalData = "Incident Sub Type Already Exist";
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
                NPF_SYS_IncidentSubType upd = context.NPF_SYS_IncidentSubType.Where(x => x.Id == dataToDelId).FirstOrDefault();
                upd.DeleteStatus = true;
                context.NPF_SYS_IncidentSubType.Update(upd);
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
