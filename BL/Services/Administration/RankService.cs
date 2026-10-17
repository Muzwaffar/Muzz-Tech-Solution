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
    public class RankService
    {
        private NPFContext context;
        private QueryResult queryResult;

        public RankService()
        {
            context = new NPFContext();
            queryResult = new QueryResult();
        }

        public BaseResponseDTO<List<RankListDTO>> GetAll()
        {
            BaseResponseDTO<List<RankListDTO>> dto = new BaseResponseDTO<List<RankListDTO>>();
            List<RankListDTO> result = new List<RankListDTO>();
            string errorMsg = "No Data Found";
            try
            {
                result = (from a in context.NPF_SYS_Rank
                          where a.DeleteStatus == false
                          select new RankListDTO()
                          {
                              Id = a.Id,
                              Code = a.Code,
                              Name = a.Name,
                              Description = a.Description,
                              ResourceCategory = context.NPF_SYS_ResourceType.Where(x => x.Id == a.ResourceCategory).FirstOrDefault().Name
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

                List<NPF_SYS_ResourceType> Mk = context.NPF_SYS_ResourceType.Where(x => x.DeleteStatus == false).ToList();
                Dd.title = "ResourceCategory";
                Dd.items = new List<DropDownItem>();
                foreach (NPF_SYS_ResourceType mk in Mk)
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

        public BaseResponseDTO<RankDTO> GetById(long Id)
        {
            BaseResponseDTO<RankDTO> dto = new BaseResponseDTO<RankDTO>();
            RankDTO result = new RankDTO();
            string errorMsg = "No Data Found";
            try
            {
                result = (from a in context.NPF_SYS_Rank
                          where a.DeleteStatus == false && a.Id == Id
                          select new RankDTO()
                          {
                              Id = a.Id,
                              Code = a.Code,
                              Name = a.Name,
                              Description = a.Description,
                              Order = a.Order,
                              ResourceCategory = a.ResourceCategory,
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

        public async Task<BaseResponseDTO<bool>> SaveAsync(BaseRequestDTO<RankDTO> dataToSave)
        {
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
            BaseResponseDTO<string> BaseDtoS = new BaseResponseDTO<string>();
            try
            {
                if (!context.NPF_SYS_Rank.Any(x => x.Name.ToLower() == dataToSave.Data.Name.ToLower() && x.DeleteStatus != true))
                {
                    BaseDtoS = await new GenerateCode().GCodeMain(dataToSave.userToken, "NPF_SYS_Rank");
                    NPF_SYS_Rank RNK = new NPF_SYS_Rank()
                    {
                        Code = BaseDtoS.Data,
                        Name = dataToSave.Data.Name,
                        Order = dataToSave.Data.Order,
                        Description = dataToSave.Data.Description,
                        ResourceCategory = dataToSave.Data.ResourceCategory
                    };
                    context.NPF_SYS_Rank.Add(RNK);
                    context.SaveChanges();

                    BaseDto.Data = true;
                    BaseDto.QryResult = queryResult.SUCEEDED;
                }
                else
                {
                    BaseDto.Data = false;
                    BaseDto.AdditionalData = "Rank Already Exist";
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

        public async Task<BaseResponseDTO<bool>> UpdateAsync(BaseRequestDTO<RankDTO> dataToUpdate)
        {
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
            try
            {
                if (!context.NPF_SYS_Rank.Any(x => x.Name.ToLower() == dataToUpdate.Data.Name.ToLower() && x.DeleteStatus != true
                && x.Id != dataToUpdate.Data.Id))
                {
                    NPF_SYS_Rank RNK = context.NPF_SYS_Rank.Where(x => x.Id == dataToUpdate.Data.Id).FirstOrDefault();
                    RNK.Name = dataToUpdate.Data.Name;
                    RNK.Order = dataToUpdate.Data.Order;
                    RNK.Description = dataToUpdate.Data.Description;
                    RNK.ResourceCategory = dataToUpdate.Data.ResourceCategory;
                    context.NPF_SYS_Rank.Update(RNK);
                    context.SaveChanges();
                    BaseDto.Data = true;
                    BaseDto.QryResult = queryResult.SUCEEDED;
                }
                else
                {
                    BaseDto.Data = false;
                    BaseDto.AdditionalData = "Rank Already Exist";
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
                NPF_SYS_Rank upd = context.NPF_SYS_Rank.Where(x => x.Id == dataToDelId).FirstOrDefault();
                upd.DeleteStatus = true;
                context.NPF_SYS_Rank.Update(upd);
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
