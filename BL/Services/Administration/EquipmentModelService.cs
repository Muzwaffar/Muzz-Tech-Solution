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
    public class EquipmentModelService
    {
        private NPFContext context;
        private QueryResult queryResult;

        public EquipmentModelService()
        {
            context = new NPFContext();
            queryResult = new QueryResult();
        }

        public BaseResponseDTO<List<EquipmentModelListDTO>> GetAll()
        {
            BaseResponseDTO<List<EquipmentModelListDTO>> dto = new BaseResponseDTO<List<EquipmentModelListDTO>>();
            List<EquipmentModelListDTO> result = new List<EquipmentModelListDTO>();
            string errorMsg = "No Data Found";
            try
            {
                result = (from a in context.NPF_SYS_EquipmentModel
                          where a.DeleteStatus == false
                          select new EquipmentModelListDTO()
                          {
                              Id = a.Id,
                              Code = a.Code,
                              Name = a.Name,
                              Description = a.Description,
                              EquipmentMake = a.EquipmentMake,

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

        public BaseResponseDTO<EquipmentModelCRUDDTO> GetById(long Id)
        {
            BaseResponseDTO<EquipmentModelCRUDDTO> dto = new BaseResponseDTO<EquipmentModelCRUDDTO>();
            EquipmentModelCRUDDTO result = new EquipmentModelCRUDDTO();
            string errorMsg = "No Data Found";
            try
            {
                result = (from a in context.NPF_SYS_EquipmentModel
                          where a.Id == Id
                          select new EquipmentModelCRUDDTO()
                          {
                              Id = a.Id,
                              Code = a.Code,
                              Name = a.Name,
                              Description = a.Description,
                              EquipmentMake = a.EquipmentMake.Id,

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

        public async Task<BaseResponseDTO<bool>> SaveAsync(BaseRequestDTO<EquipmentModelCRUDDTO> dataToSave)
        {
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
            BaseResponseDTO<string> BaseDtoS = new BaseResponseDTO<string>();
            try
            {
                if (!context.NPF_SYS_EquipmentModel.Any(x => x.Name == dataToSave.Data.Name && x.DeleteStatus != true))
                {
                    BaseDtoS = await new GenerateCode().GCodeMain(dataToSave.userToken, "NPF_SYS_EquipmentModel");
                    NPF_SYS_EquipmentModel dt = new NPF_SYS_EquipmentModel()
                    {
                        Code = BaseDtoS.Data,
                        Name = dataToSave.Data.Name,
                        Description = dataToSave.Data.Description ?? string.Empty,
                        EquipmentMake = context.NPF_SYS_EquipmentMake.Where(x => x.Id == dataToSave.Data.EquipmentMake).FirstOrDefault(),
                    };
                    context.NPF_SYS_EquipmentModel.Add(dt);
                    context.SaveChanges();
                    BaseDto.ExtData = dt.Id.ToString();
                    BaseDto.Data = true;
                    BaseDto.QryResult = queryResult.SUCEEDED;
                }
                else
                {
                    BaseDto.Data = false;
                    BaseDto.AdditionalData = "Equipment Model Already Exist";
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

        public async Task<BaseResponseDTO<bool>> UpdateAsync(BaseRequestDTO<EquipmentModelCRUDDTO> dataToUpdate)
        {
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
            try
            {
                if (context.NPF_SYS_EquipmentModel.Any(x => x.Name == dataToUpdate.Data.Name
               && x.DeleteStatus != true && x.Id != dataToUpdate.Data.Id))
                {
                    BaseDto.Data = false;
                    BaseDto.AdditionalData = "Equipment Model Already Exist";
                    BaseDto.QryResult = queryResult.FAILED;
                    return BaseDto;
                }

                NPF_SYS_EquipmentModel Update = context.NPF_SYS_EquipmentModel.Where(x => x.Id == dataToUpdate.Data.Id).FirstOrDefault();

                Update.Name = dataToUpdate.Data.Name;
                Update.Description = dataToUpdate.Data.Description ?? string.Empty;

                Update.EquipmentMake = context.NPF_SYS_EquipmentMake.Where(x => x.Id == dataToUpdate.Data.EquipmentMake).FirstOrDefault();
                
                context.NPF_SYS_EquipmentModel.Update(Update);
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
                NPF_SYS_EquipmentModel upd = context.NPF_SYS_EquipmentModel.Where(x => x.Id == dataToDelId.Data).FirstOrDefault();
                upd.DeleteStatus = true;
                context.NPF_SYS_EquipmentModel.Update(upd);
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

        public BaseResponseDTO<List<DropDown>> GetAllDropDownValues()
        {
            BaseResponseDTO<List<DropDown>> dto = new BaseResponseDTO<List<DropDown>>();
            List<DropDown> Ddl = new List<DropDown>();
            DropDown Dd = new DropDown();
            string errorMsg = "No Data Found";
            try
            {
                Dd = new DropDown();
                List<NPF_SYS_EquipmentMake> Ltl = context.NPF_SYS_EquipmentMake.Where(x => x.DeleteStatus == false).ToList();
                Dd.title = "EquipmentMake";
                Dd.items = new List<DropDownItem>();
                foreach (NPF_SYS_EquipmentMake lt in Ltl)
                {
                    DropDownItem Ddi = new DropDownItem();
                    Ddi.Id = lt.Id;
                    Ddi.text = lt.Name;
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
    }
}
