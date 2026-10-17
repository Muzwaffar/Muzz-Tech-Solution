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
    public class AssetModelService
    {
        private NPFContext context;
        private QueryResult queryResult;

        public AssetModelService()
        {
            context = new NPFContext();
            queryResult = new QueryResult();
        }

        public BaseResponseDTO<List<AssetModelListDTO>> GetAll()
        {
            BaseResponseDTO<List<AssetModelListDTO>> dto = new BaseResponseDTO<List<AssetModelListDTO>>();
            List<AssetModelListDTO> result = new List<AssetModelListDTO>();
            string errorMsg = "No Data Found";
            try
            {
                result = (from a in context.NPF_SYS_AssetModel
                          where a.DeleteStatus == false
                          select new AssetModelListDTO()
                          {
                              Id = a.Id,
                              Code = a.Code,
                              Name = a.Name,
                              Description = a.Description,
                              AssetMake = a.AssetMake,

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

        public BaseResponseDTO<AssetModelCRUDDTO> GetById(long Id)
        {
            BaseResponseDTO<AssetModelCRUDDTO> dto = new BaseResponseDTO<AssetModelCRUDDTO>();
            AssetModelCRUDDTO result = new AssetModelCRUDDTO();
            string errorMsg = "No Data Found";
            try
            {
                result = (from a in context.NPF_SYS_AssetModel
                          where a.Id == Id
                          select new AssetModelCRUDDTO()
                          {
                              Id = a.Id,
                              Code = a.Code,
                              Name = a.Name,
                              Description = a.Description,
                              AssetMake = a.AssetMake.Id,

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

        public async Task<BaseResponseDTO<bool>> SaveAsync(BaseRequestDTO<AssetModelCRUDDTO> dataToSave)
        {
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
            BaseResponseDTO<string> BaseDtoS = new BaseResponseDTO<string>();
            try
            {
                if (!context.NPF_SYS_AssetModel.Any(x => x.Name == dataToSave.Data.Name && x.DeleteStatus != true))
                {
                    BaseDtoS = await new GenerateCode().GCodeMain(dataToSave.userToken, "NPF_SYS_AssetModel");
                    NPF_SYS_AssetModel dt = new NPF_SYS_AssetModel()
                    {
                        Code = BaseDtoS.Data,
                        Name = dataToSave.Data.Name,
                        Description = dataToSave.Data.Description ?? string.Empty,
                        AssetMake = context.NPF_SYS_AssetMake.Where(x => x.Id == dataToSave.Data.AssetMake).FirstOrDefault(),
                    };
                    context.NPF_SYS_AssetModel.Add(dt);
                    context.SaveChanges();
                    BaseDto.ExtData = dt.Id.ToString();
                    BaseDto.Data = true;
                    BaseDto.QryResult = queryResult.SUCEEDED;
                }
                else
                {
                    BaseDto.Data = false;
                    BaseDto.AdditionalData = "Vehicle Model Already Exist";
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

        public async Task<BaseResponseDTO<bool>> UpdateAsync(BaseRequestDTO<AssetModelCRUDDTO> dataToUpdate)
        {
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
            try
            {
                if (context.NPF_SYS_AssetModel.Any(x => x.Name == dataToUpdate.Data.Name
               && x.DeleteStatus != true && x.Id != dataToUpdate.Data.Id))
                {
                    BaseDto.Data = false;
                    BaseDto.AdditionalData = "Vehicle Model Already Exist";
                    BaseDto.QryResult = queryResult.FAILED;
                    return BaseDto;
                }

                NPF_SYS_AssetModel Update = context.NPF_SYS_AssetModel.Where(x => x.Id == dataToUpdate.Data.Id).FirstOrDefault();

                Update.Name = dataToUpdate.Data.Name;
                Update.Description = dataToUpdate.Data.Description ?? string.Empty;

                Update.AssetMake = context.NPF_SYS_AssetMake.Where(x => x.Id == dataToUpdate.Data.AssetMake).FirstOrDefault();
                
                context.NPF_SYS_AssetModel.Update(Update);
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
                NPF_SYS_AssetModel upd = context.NPF_SYS_AssetModel.Where(x => x.Id == dataToDelId.Data).FirstOrDefault();
                upd.DeleteStatus = true;
                context.NPF_SYS_AssetModel.Update(upd);
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
                List<NPF_SYS_AssetMake> Ltl = context.NPF_SYS_AssetMake.Where(x => x.DeleteStatus == false).ToList();
                Dd.title = "AssetMake";
                Dd.items = new List<DropDownItem>();
                foreach (NPF_SYS_AssetMake lt in Ltl)
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
