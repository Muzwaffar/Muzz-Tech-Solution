using BL.Constants;
using BL.Models.Administration;
using BL.Models.Common;
using BL.Models.Schedule;
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
    public class CallerService
    {
        private NPFContext context;
        private QueryResult queryResult;
        public CallerService()
        {
            context = new NPFContext();
            queryResult = new QueryResult();
        }

        public BaseResponseDTO<List<CallerListDTO>> GetAll()
        {
            BaseResponseDTO<List<CallerListDTO>> dto = new BaseResponseDTO<List<CallerListDTO>>();
            List<CallerListDTO> result = new List<CallerListDTO>();
            string errorMsg = "No Data Found";
            try
            {
                result = (from a in context.NPF_SYS_Caller
                          where a.DeleteStatus == false
                          select new CallerListDTO()
                          {
                              Id = a.Id,
                              Code = a.Code,
                              FullName = a.NAA.FullName,
                              NIC = a.NAA.Identifier,
                              MobileNo = a.NAA.MobileNo,
                              IsNuisance = a.IsNuisance,
                              Remarks = a.Remarks
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

        public BaseResponseDTO<CallerCRUDDTO> GetById(long Id)
        {
            BaseResponseDTO<CallerCRUDDTO> dto = new BaseResponseDTO<CallerCRUDDTO>();
            CallerCRUDDTO result = new CallerCRUDDTO();
            string errorMsg = "No Data Found";
            try
            {
                result = (from a in context.NPF_SYS_Caller
                          join b in context.NPF_DIR_NAA on a.NAA.Id equals b.Id
                          where a.Id == Id
                          select new CallerCRUDDTO()
                          {
                              Id = a.Id,
                              Code = a.Code,
                              NAAId = b.Id,
                              NAACode = b.Code,
                              FirstName = b.FirstName,
                              LastName = b.LastName,
                              NICPassport = b.Identifier,
                              MobileNo = b.MobileNo,
                              Address = b.Address,
                              Street = b.Street,
                              Locality = b.Locality,
                              LocalityId = b.LocalityId,
                              District = b.District,
                              DistrictId = b.DistrictId,
                              IsNuisance = a.IsNuisance,
                              Remarks = a.Remarks,
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
        public BaseResponseDTO<CallerCRUDDTO> GetCallerByMobile(string Number)
        {
            BaseResponseDTO<CallerCRUDDTO> dto = new BaseResponseDTO<CallerCRUDDTO>();
            CallerCRUDDTO result = new CallerCRUDDTO();
            string errorMsg = "No Data Found";
            try
            {
                result = (from a in context.NPF_SYS_Caller
                          join b in context.NPF_DIR_NAA on a.NAA.Id equals b.Id
                          where a.NAA.MobileNo == Number
                          select new CallerCRUDDTO()
                          {
                              Id = a.Id,
                              Code = a.Code,
                              NAAId = b.Id,
                              NAACode = b.Code,
                              FirstName = b.FirstName,
                              LastName = b.LastName,
                              NICPassport = b.Identifier,
                              MobileNo = b.MobileNo,
                              Address = b.Address,
                              Street = b.Street,
                              Locality = b.Locality,
                              LocalityId = b.LocalityId,
                              District = b.District,
                              DistrictId = b.DistrictId,
                              IsNuisance = a.IsNuisance,
                              Remarks = a.Remarks,
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

        public BaseResponseDTO<CallerCRUDDTO> GetCallerById(long Id)
        {
            BaseResponseDTO<CallerCRUDDTO> dto = new BaseResponseDTO<CallerCRUDDTO>();
            CallerCRUDDTO result = new CallerCRUDDTO();
            string errorMsg = "No Data Found";
            try
            {
                result = (from a in context.NPF_SYS_Caller
                          join b in context.NPF_DIR_NAA on a.NAA.Id equals b.Id
                          where a.Id == Id
                          select new CallerCRUDDTO()
                          {
                              Id = a.Id,
                              Code = a.Code,
                              NAAId = b.Id,
                              NAACode = b.Code,
                              FirstName = b.FirstName,
                              LastName = b.LastName,
                              NICPassport = b.Identifier,
                              MobileNo = b.MobileNo,
                              Address = b.Address,
                              Street = b.Street,
                              Locality = b.Locality,
                              LocalityId = b.LocalityId,
                              District = b.District,
                              DistrictId = b.DistrictId,
                              IsNuisance = a.IsNuisance,
                              Remarks = a.Remarks,
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
        public async Task<BaseResponseDTO<bool>> SaveAsync(BaseRequestDTO<CallerCRUDDTO> dataToSave)
        {
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
            BaseResponseDTO<long> BaseDtoNaa = new BaseResponseDTO<long>();
            BaseResponseDTO<string> BaseDtoS = new BaseResponseDTO<string>();
            try
            {

                NAACRUDTO NAA = new NAACRUDTO()
                {
                    Id = dataToSave.Data.NAAId,
                    Code = dataToSave.Data.NAACode,
                    Identifier = dataToSave.Data.MobileNo,
                    NICPASS = dataToSave.Data.NICPassport,
                    FirstName = dataToSave.Data.FirstName,
                    LastName = dataToSave.Data.LastName,
                    MobileNo = dataToSave.Data.MobileNo,
                    Address = dataToSave.Data.Address,
                    Street = dataToSave.Data.Street,
                    Locality = dataToSave.Data.Locality,
                    LocalityId = dataToSave.Data.LocalityId,
                    District = dataToSave.Data.District,
                    DistrictId = dataToSave.Data.DistrictId,
                };
                BaseDtoNaa = await new NAAService().GetSaveNaa(NAA, dataToSave.userToken);
                if (BaseDtoNaa.QryResult == new QueryResult().FAILED)
                {
                    BaseDto.Data = false;
                    BaseDto.AdditionalData = "Error Adding Record";
                    BaseDto.QryResult = new QueryResult().FAILED;
                    return BaseDto;
                }

                BaseDtoS = await new GenerateCode().GCodeMain(dataToSave.userToken, "NPF_SYS_Caller");
                NPF_DIR_NAA Naa = context.NPF_DIR_NAA.Where(x => x.Id == BaseDtoNaa.Data).FirstOrDefault();
                context.NPF_DIR_NAA.Update(Naa);
                NPF_SYS_Caller caller = new NPF_SYS_Caller()
                {
                    Code = BaseDtoS.Data,
                    NAA = Naa,
                    IsNuisance = dataToSave.Data.IsNuisance,
                    Remarks = dataToSave.Data.Remarks

                };

                context.NPF_SYS_Caller.Add(caller);
                context.SaveChanges();
                BaseDto.Id = caller.Id;
                BaseDto.Data = true;
                BaseDto.QryResult = queryResult.SUCEEDED;

            }
            catch (Exception ex)
            {
                BaseDto.Data = false;
                BaseDto.AdditionalData = "Error Adding Record";
                BaseDto.QryResult = queryResult.FAILED;
            }
            return BaseDto;
        }

        public async Task<BaseResponseDTO<bool>> UpdateAsync(BaseRequestDTO<CallerCRUDDTO> dataToUpdate)
        {
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
            BaseResponseDTO<long> BaseDtoNaa = new BaseResponseDTO<long>();
            try
            {
                NAACRUDTO NAA = new NAACRUDTO()
                {
                    Id = dataToUpdate.Data.NAAId,
                    Code = dataToUpdate.Data.NAACode,
                    Identifier = dataToUpdate.Data.MobileNo,
                    NICPASS = dataToUpdate.Data.NICPassport,
                    FirstName = dataToUpdate.Data.FirstName,
                    LastName = dataToUpdate.Data.LastName,
                    MobileNo = dataToUpdate.Data.MobileNo,
                    Address = dataToUpdate.Data.Address,
                    Street = dataToUpdate.Data.Street,
                    Locality = dataToUpdate.Data.Locality,
                    LocalityId = dataToUpdate.Data.LocalityId,
                    District = dataToUpdate.Data.District,
                    DistrictId = dataToUpdate.Data.DistrictId,
                };
                BaseDtoNaa = await new NAAService().GetSaveNaa(NAA, dataToUpdate.userToken);
                if (BaseDtoNaa.QryResult == new QueryResult().FAILED)
                {
                    BaseDto.Data = false;
                    BaseDto.AdditionalData = "Error Adding Record";
                    BaseDto.QryResult = new QueryResult().FAILED;
                    return BaseDto;
                }
                NPF_SYS_Caller caller = context.NPF_SYS_Caller.Where(x => x.Id == dataToUpdate.Data.Id).FirstOrDefault();

                caller.NAA = context.NPF_DIR_NAA.Where(x => x.Id == BaseDtoNaa.Data).FirstOrDefault();

                caller.IsNuisance = dataToUpdate.Data.IsNuisance;
                caller.Remarks = dataToUpdate.Data.Remarks;

                context.NPF_SYS_Caller.Update(caller);
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
                NPF_SYS_Caller upd = context.NPF_SYS_Caller.Where(x => x.Id == dataToDelId.Data).FirstOrDefault();
                upd.DeleteStatus = true;
                context.NPF_SYS_Caller.Update(upd);
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
                BaseRequestDTO<string> dt = new BaseRequestDTO<string>();
                string[] values = new string[] { "Locality", "District" };
                List<NPF_SYS_LookUpType> Ltl = context.NPF_SYS_LookUpType.Where(x => values.Contains(x.Name) && x.DeleteStatus == false).ToList();
                foreach (NPF_SYS_LookUpType lt in Ltl)
                {
                    Dd = new DropDown();
                    Dd.title = lt.Name;
                    List<NPF_SYS_LookUpValue> Lvl = context.NPF_SYS_LookUpValue.Where(x => x.LookUpType.Id == lt.Id && x.DeleteStatus == false).ToList();
                    Dd.items = new List<DropDownItem>();
                    foreach (NPF_SYS_LookUpValue lv in Lvl)
                    {
                        DropDownItem Ddi = new DropDownItem();
                        Ddi.Id = lv.Id;
                        Ddi.text = lv.Name;
                        Dd.items.Add(Ddi);
                    }

                    Ddl.Add(Dd);
                }

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
