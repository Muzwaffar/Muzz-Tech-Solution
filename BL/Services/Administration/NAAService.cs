using BL.Constants;
using BL.Models.Access;
using BL.Models.Administration;
using BL.Models.Common;
using BL.Services.Common;
using DAL.Common;
using DAL.DBContext;
using DAL.Models.Administration;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BL.Services.Administration
{
    public class NAAService
    {
        private NPFContext context;
        private QueryResult queryResult;
        public NAAService()
        {
            context = new NPFContext();
            queryResult = new QueryResult();
        }

        public BaseResponseDTO<List<NAAlistDTO>> GetAll()
        {
            BaseResponseDTO<List<NAAlistDTO>> dto = new BaseResponseDTO<List<NAAlistDTO>>();
            List<NAAlistDTO> result = new List<NAAlistDTO>();
            string errorMsg = "No Data Found";
            try
            {
                result = (from a in context.NPF_DIR_NAA
                          where a.DeleteStatus == false
                          select new NAAlistDTO()
                          {
                              Id = a.Id,
                              Code = a.Code,
                              FullName = a.FullNameM,
                              MobileNo = a.MobileNo,
                              Address = a.Address,
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

        public BaseResponseDTO<NAACRUDTO> GetById(long Id)
        {
            BaseResponseDTO<NAACRUDTO> dto = new BaseResponseDTO<NAACRUDTO>();
            NAACRUDTO result = new NAACRUDTO();
            string errorMsg = "No Data Found";
            try
            {
                result = (from a in context.NPF_DIR_NAA
                          where a.Id == Id
                          select new NAACRUDTO()
                          {
                              Id = a.Id,
                              Code = a.Code,
                              Title = (int)a.Title,
                              FirstName = a.FirstName,
                              MiddleName = a.MiddleName,
                              LastName = a.LastName,
                              Identifier = a.Identifier,
                              Gender = a.Gender,
                              DOB = a.DOB,
                              Email = a.Email,
                              TelNo = a.TelephoneNo,
                              MobileNo = a.MobileNo,
                              Occupation = a.Occupation,
                              Address = a.Address,
                              Street = a.Street,
                              Locality = a.Locality,
                              VillageTown = a.VillageTown,
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

        public async Task<BaseResponseDTO<bool>> SaveAsync(BaseRequestDTO<NAACRUDTO> dataToSave)
        {
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
            BaseResponseDTO<string> BaseDtoS = new BaseResponseDTO<string>();
            try
            {
                if (dataToSave.Data.Identifier.ToUpper() != "NA".ToUpper())
                {
                    if (!context.NPF_DIR_NAA.Any(x => x.Identifier == dataToSave.Data.Identifier /*&& dataToSave.Data.Identifier.ToUpper() != "NA"*/ && x.DeleteStatus != true))
                    {
                        BaseDtoS = await new GenerateCode().GCodeMain(dataToSave.userToken, "NPF_DIR_NAA");
                        NPF_DIR_NAA dt = new NPF_DIR_NAA()
                        {
                            Code = BaseDtoS.Data,
                            Title = dataToSave.Data.Title,
                            FirstName = dataToSave.Data.FirstName,
                            MiddleName = dataToSave.Data.MiddleName ?? string.Empty,
                            LastName = dataToSave.Data.LastName,
                            Identifier = dataToSave.Data.Identifier,
                            NICPASS = dataToSave.Data.NICPASS,
                            Gender = dataToSave.Data.Gender ?? null,
                            DOB = dataToSave.Data.DOB ?? DateTime.Now,
                            Email = dataToSave.Data.Email ?? string.Empty,
                            TelephoneNo = dataToSave.Data.TelNo ?? string.Empty,
                            MobileNo = dataToSave.Data.MobileNo,
                            Occupation = dataToSave.Data.Occupation == null ? string.Empty : dataToSave.Data.Occupation,
                            Address = dataToSave.Data.Address ?? string.Empty,
                            Street = dataToSave.Data.Street,
                            Locality = dataToSave.Data.Locality ?? string.Empty,
                            LocalityId = dataToSave.Data.LocalityId,
                            VillageTown = dataToSave.Data.VillageTown ?? string.Empty,
                            District = dataToSave.Data.District ?? string.Empty,
                            DistrictId = dataToSave.Data.DistrictId,
                        };
                         context.NPF_DIR_NAA.Add(dt);
                        context.SaveChanges();
                        BaseDto.ExtData = dt.Id.ToString();
                        BaseDto.Data = true;
                        BaseDto.QryResult = queryResult.SUCEEDED;
                    }
                    else
                    {
                        BaseDto.Data = false;
                        BaseDto.AdditionalData = "Name and Address Already Exist";
                        BaseDto.QryResult = queryResult.FAILED;
                    }
                }
                else
                {
                    BaseDtoS = await new GenerateCode().GCodeMain(dataToSave.userToken, "NPF_DIR_NAA");
                    NPF_DIR_NAA dt = new NPF_DIR_NAA()
                    {
                        Code = BaseDtoS.Data,
                        Title = dataToSave.Data.Title,
                        FirstName = dataToSave.Data.FirstName,
                        MiddleName = dataToSave.Data.MiddleName ?? string.Empty,
                        LastName = dataToSave.Data.LastName,
                        Identifier = dataToSave.Data.Identifier,
                        NICPASS = dataToSave.Data.NICPASS,
                        Gender = dataToSave.Data.Gender ?? null,
                        DOB = dataToSave.Data.DOB ?? DateTime.Now,
                        Email = dataToSave.Data.Email ?? string.Empty,
                        TelephoneNo = dataToSave.Data.TelNo ?? string.Empty,
                        MobileNo = dataToSave.Data.MobileNo,
                        Occupation = dataToSave.Data.Occupation == null ? string.Empty : dataToSave.Data.Occupation,
                        Address = dataToSave.Data.Address ?? string.Empty,
                        Street = dataToSave.Data.Street,
                        Locality = dataToSave.Data.Locality ?? string.Empty,
                        LocalityId = dataToSave.Data.LocalityId,
                        VillageTown = dataToSave.Data.VillageTown ?? string.Empty,
                        District = dataToSave.Data.District ?? string.Empty,
                        DistrictId = dataToSave.Data.DistrictId,
                    };
                    context.NPF_DIR_NAA.Add(dt);
                    context.SaveChanges();
                    BaseDto.ExtData = dt.Id.ToString();
                    BaseDto.Data = true;
                    BaseDto.QryResult = queryResult.SUCEEDED;
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

        public async Task<BaseResponseDTO<bool>> UpdateAsync(BaseRequestDTO<NAACRUDTO> dataToUpdate)
        {
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
            try
            {
                if (context.NPF_DIR_NAA.Any(x => x.Identifier == dataToUpdate.Data.Identifier
                && x.DeleteStatus != true && x.Id != dataToUpdate.Data.Id))
                {
                    BaseDto.Data = false;
                    BaseDto.AdditionalData = "Name and Address Already Exist";
                    BaseDto.QryResult = queryResult.FAILED;
                    return BaseDto;
                }
                NPF_DIR_NAA NAA = context.NPF_DIR_NAA.Where(x => x.Id == dataToUpdate.Data.Id).FirstOrDefault();
                NAA.Title = dataToUpdate.Data.Title;
                NAA.FirstName = dataToUpdate.Data.FirstName;
                NAA.MiddleName = dataToUpdate.Data.MiddleName;
                NAA.LastName = dataToUpdate.Data.LastName;
                NAA.Identifier = dataToUpdate.Data.Identifier;
                NAA.NICPASS = dataToUpdate.Data.NICPASS;
                NAA.Gender = dataToUpdate.Data.Gender;
                NAA.DOB = dataToUpdate.Data.DOB;
                NAA.Email = dataToUpdate.Data.Email;
                NAA.TelephoneNo = dataToUpdate.Data.TelNo;
                NAA.MobileNo = dataToUpdate.Data.MobileNo;
                NAA.Occupation = dataToUpdate.Data.Occupation;
                NAA.Address = dataToUpdate.Data.Address;
                NAA.Street = dataToUpdate.Data.Street;
                NAA.Locality = dataToUpdate.Data.Locality;
                NAA.VillageTown = dataToUpdate.Data.VillageTown;
                context.NPF_DIR_NAA.Update(NAA);
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
                NPF_DIR_NAA upd = context.NPF_DIR_NAA.Where(x => x.Id == dataToDelId.Data).FirstOrDefault();
                upd.DeleteStatus = true;
                context.NPF_DIR_NAA.Update(upd);
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
                string[] values = new string[] { "Title", "Gender" };
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
        public BaseResponseDTO<NAACRUDTO> GetByNic(string Nic)
        {
            BaseResponseDTO<NAACRUDTO> dto = new BaseResponseDTO<NAACRUDTO>();
            NAACRUDTO Naa = new NAACRUDTO();
            QueryResult queryResult = new QueryResult();
            string errorMsg = "No Data Found";
            string sQryResult = queryResult.FAILED;
            NAACRUDTO result = (from a in context.NPF_DIR_NAA
                                where a.DeleteStatus == false && a.Identifier == Nic
                                select new NAACRUDTO()
                                {
                                    Id = a.Id,
                                    Code = a.Code,
                                    Title = (int)a.Title,
                                    FirstName = a.FirstName,
                                    MiddleName = a.MiddleName,
                                    LastName = a.LastName,
                                    Identifier = a.Identifier,
                                    Gender = a.Gender,
                                    DOB = a.DOB,
                                    Email = a.Email,
                                    TelNo = a.TelephoneNo,
                                    MobileNo = a.MobileNo,
                                    Address = a.Address,
                                    Street = a.Street,
                                    Locality = a.Locality,
                                    VillageTown = a.VillageTown,
                                    VillageTownId = context.NPF_SYS_LookUpValue.Where(x => x.Name == a.VillageTown).FirstOrDefault().Id,
                                }).FirstOrDefault();

            if (result == null || Nic.ToUpper() == "NA")
            {
                dto.Data = Naa;
                dto.QryResult = queryResult.SUCEEDED;
            }
            else
            {
                dto.Data = result;
                dto.QryResult = queryResult.SUCEEDED;
            }

            return dto;
        }

        public BaseResponseDTO<NAACRUDTO> GetByMobileNo(string MobileNo)
        {
            BaseResponseDTO<NAACRUDTO> dto = new BaseResponseDTO<NAACRUDTO>();
            NAACRUDTO Naa = new NAACRUDTO();
            QueryResult queryResult = new QueryResult();
            string errorMsg = "No Data Found";
            string sQryResult = queryResult.FAILED;
            NAACRUDTO result = (from a in context.NPF_DIR_NAA
                                where a.DeleteStatus == false && a.MobileNo == MobileNo
                                select new NAACRUDTO()
                                {
                                    Id = a.Id,
                                    Code = a.Code,
                                    Title = (int)a.Title,
                                    FirstName = a.FirstName,
                                    MiddleName = a.MiddleName,
                                    LastName = a.LastName,
                                    Identifier = a.Identifier,
                                    Gender = a.Gender,
                                    DOB = a.DOB,
                                    Email = a.Email,
                                    TelNo = a.TelephoneNo,
                                    MobileNo = a.MobileNo,
                                    Address = a.Address,
                                    Street = a.Street,
                                    Locality = a.Locality,
                                    VillageTown = a.VillageTown,
                                    VillageTownId = context.NPF_SYS_LookUpValue.Where(x => x.Name == a.VillageTown).FirstOrDefault().Id,
                                }).FirstOrDefault();

            if (result == null)
            {
                dto.Data = Naa;
                dto.QryResult = queryResult.SUCEEDED;
            }
            else
            {
                dto.Data = result;
                dto.QryResult = queryResult.SUCEEDED;
            }

            return dto;
        }
        public BaseResponseDTO<NAACRUDTO> GetDetailsById(long Id)
        {
            BaseResponseDTO<NAACRUDTO> dto = new BaseResponseDTO<NAACRUDTO>();
            NAACRUDTO Naa = new NAACRUDTO();
            QueryResult queryResult = new QueryResult();
            string errorMsg = "No Data Found";
            string sQryResult = queryResult.FAILED;
            NAACRUDTO result = (from a in context.NPF_DIR_NAA
                                where a.DeleteStatus == false && a.Id == Id
                                select new NAACRUDTO()
                                {
                                    Id = a.Id,
                                    Code = a.Code,
                                    Title = (int)a.Title,
                                    FirstName = a.FirstName,
                                    MiddleName = a.MiddleName,
                                    LastName = a.LastName,
                                    Identifier = a.Identifier,
                                    Gender = a.Gender,
                                    DOB = a.DOB,
                                    Email = a.Email,
                                    TelNo = a.TelephoneNo,
                                    MobileNo = a.MobileNo,
                                    Address = a.Address,
                                    Street = a.Street,
                                    Locality = a.Locality,
                                    VillageTown = a.VillageTown,
                                }).FirstOrDefault();

            if (result == null)
            {
                dto.Data = Naa;
                dto.QryResult = queryResult.SUCEEDED;
            }
            else
            {
                dto.Data = result;
                dto.QryResult = queryResult.SUCEEDED;
            }

            return dto;
        }
        public async Task<BaseResponseDTO<long>> GetSaveNaa(NAACRUDTO dataToCheck, string userToken)
        {

            BaseResponseDTO<long> BaseDto = new BaseResponseDTO<long>();
            BaseRequestDTO<NAACRUDTO> dto = new BaseRequestDTO<NAACRUDTO>();
            dto.Data = dataToCheck;
            dto.userToken = userToken;
            BaseResponseDTO<bool> BaseResponseDTO = new BaseResponseDTO<bool>();
            try
            {
                if (dataToCheck.Id == 0) //!context.NPF_DIR_NAA.Any(x => x.Identifier == dataToCheck.Identifier)&&dataToCheck.Identifier.ToUpper() != "NA".ToUpper())
                {
                    dto.Data = dataToCheck;
                    try
                    {
                        BaseResponseDTO = await new NAAService().SaveAsync(dto);
                        BaseDto.Data = Convert.ToInt64(BaseResponseDTO.ExtData);
                    }
                    catch (Exception ex)
                    {
                        BaseDto.Data = 0;
                        BaseDto.AdditionalData = "Error Adding Record";
                        BaseDto.QryResult = new QueryResult().FAILED;
                        return BaseDto;
                    }

                }
                else
                {
                    NPF_DIR_NAA NAA = context.NPF_DIR_NAA.Where(x => x.Id == dataToCheck.Id).FirstOrDefault();
                    if (NAA != null)
                    {
                        NAA.Title = dataToCheck.Title;
                        NAA.FirstName = dataToCheck.FirstName;
                        NAA.MiddleName = dataToCheck.MiddleName ?? string.Empty;
                        NAA.LastName = dataToCheck.LastName;
                        NAA.Identifier = dataToCheck.Identifier;
                        NAA.Gender = dataToCheck.Gender ?? null;
                        NAA.DOB = dataToCheck.DOB ?? DateTime.Now;
                        NAA.Email = dataToCheck.Email ?? string.Empty;
                        NAA.TelephoneNo = dataToCheck.TelNo ?? string.Empty;
                        NAA.MobileNo = dataToCheck.MobileNo;
                        NAA.Occupation = dataToCheck.Occupation == null ? string.Empty : dataToCheck.Occupation;
                        NAA.Address = dataToCheck.Address ?? string.Empty;
                        NAA.Street = dataToCheck.Street;
                        NAA.Locality = dataToCheck.Locality ?? string.Empty;
                        NAA.LocalityId = dataToCheck.LocalityId;
                        NAA.VillageTown = dataToCheck.VillageTown ?? string.Empty;
                        NAA.District = dataToCheck.District ?? string.Empty;
                        NAA.DistrictId = dataToCheck.DistrictId;
                        context.NPF_DIR_NAA.Update(NAA);
                        context.SaveChanges();
                        BaseDto.Data = NAA.Id;
                    }
                    else
                    {
                        try
                        {
                            BaseResponseDTO = await new NAAService().SaveAsync(dto);
                            BaseDto.Data = Convert.ToInt64(BaseResponseDTO.ExtData);

                        }
                        catch (Exception ex)
                        {
                            //BaseDto.Data = null;
                            BaseDto.AdditionalData = "Error Adding Record";
                            BaseDto.QryResult = new QueryResult().FAILED;
                            return BaseDto;
                        }

                    }

                }
                //BaseDto.Data = dataToCheck;
                BaseDto.AdditionalData = "Record Added Successfully";
                BaseDto.QryResult = new QueryResult().SUCEEDED;
                return BaseDto;
            }
            catch (Exception ex)
            {
                // BaseDto.Data = null;
                BaseDto.AdditionalData = "Error Adding Record";
                BaseDto.QryResult = new QueryResult().FAILED;
                return BaseDto;
            }


        }
    }
}
