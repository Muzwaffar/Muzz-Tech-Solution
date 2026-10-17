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
    public class ResourceService
    {
        private NPFContext context;
        private QueryResult queryResult;
        public ResourceService()
        {
            context = new NPFContext();
            queryResult = new QueryResult();
        }

        public BaseResponseDTO<List<ResourceListDTO>> GetAll()
        {
            BaseResponseDTO<List<ResourceListDTO>> dto = new BaseResponseDTO<List<ResourceListDTO>>();
            List<ResourceListDTO> result = new List<ResourceListDTO>();
            string errorMsg = "No Data Found";
            try
            {
                result = (from a in context.NPF_SYS_Resource
                          where a.DeleteStatus == false
                          select new ResourceListDTO()
                          {
                              Id = a.Id,
                              Code = a.Code,
                              FullName = a.NAA.FullNameM,
                              Rank = a.Rank.Name,
                              PFNumber = a.PFNumber,
                              Station = a.StationUnitName.Name,
                              ResourceType = context.NPF_SYS_ResourceType.Where(x => x.Id == a.ResourceType.Id).FirstOrDefault().Name,
                              UserId = (from c in context.NPF_SYS_User
                                        join d in context.Users on c.AId equals d.Id
                                        where c.Id == a.UserId
                                        select d.FirstName).FirstOrDefault(),
                              MobileNo = a.NAA.MobileNo
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

        public BaseResponseDTO<ResourceCRUDTO> GetById(long Id)
        {
            BaseResponseDTO<ResourceCRUDTO> dto = new BaseResponseDTO<ResourceCRUDTO>();
            ResourceCRUDTO result = new ResourceCRUDTO();
            string errorMsg = "No Data Found";
            try
            {
                result = (from a in context.NPF_SYS_Resource
                          join b in context.NPF_DIR_NAA on a.NAA.Id equals b.Id
                          where a.Id == Id
                          select new ResourceCRUDTO()
                          {
                              Id = a.Id,
                              Code = a.Code,
                              ResourceType = context.NPF_SYS_ResourceType.Where(x => x.Id == a.ResourceType.Id).FirstOrDefault().Id,
                              Title = (int)b.Title,
                              NAAId = b.Id,
                              NAACode = b.Code,
                              FirstName = b.FirstName,
                              MiddleName = b.MiddleName,
                              LastName = b.LastName,
                              NICPassport = b.Identifier,
                              UserId = a.UserId,
                              Gender = b.Gender,
                              DOB = b.DOB,
                              Email = b.Email,
                              TelNo = b.TelephoneNo,
                              MobileNo = b.MobileNo,
                              Address = b.Address,
                              Street = b.Street,
                              Locality = b.Locality,
                              VillageTown = b.VillageTown,
                              Rank = a.Rank.Id,
                              DatejoinedMFRS = a.DatejoinedMFRS,
                              PFNumber = a.PFNumber,
                              StationUnitName = a.StationUnitName.Id,
                              PrivateLicenseNumber = a.PrivateLicenseNumber,
                              PrivateLicenseDriverCategoryId = a.PrivateLicenseDriverCategoryId,
                              PrivateLicenseIssueDate = a.PrivateLicenseIssueDate,
                              PrivateLicenseExpiryDate = a.PrivateLicenseExpiryDate,
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

        public async Task<BaseResponseDTO<bool>> SaveAsync(BaseRequestDTO<ResourceCRUDTO> dataToSave)
        {
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
            BaseResponseDTO<long> BaseDtoNaa = new BaseResponseDTO<long>();
            BaseResponseDTO<string> BaseDtoS = new BaseResponseDTO<string>();
            try
            {
                if (context.NPF_SYS_Resource.Any(x => x.NAA.Id == dataToSave.Data.NAAId && x.ResourceType.Id == dataToSave.Data.ResourceType && x.DeleteStatus != true))
                {
                    BaseDto.Data = false;
                    BaseDto.AdditionalData = "Resource Already Exist";
                    BaseDto.QryResult = queryResult.FAILED;
                    return BaseDto;
                }

                NAACRUDTO NAA = new NAACRUDTO()
                {
                    Id = dataToSave.Data.NAAId,
                    Code = dataToSave.Data.NAACode,
                    Title = (int)dataToSave.Data.Title,
                    Identifier = dataToSave.Data.NICPassport,
                    NICPASS = dataToSave.Data.NICPassport,
                    FirstName = dataToSave.Data.FirstName,
                    MiddleName = dataToSave.Data.MiddleName,
                    LastName = dataToSave.Data.LastName,
                    DOB = dataToSave.Data.DOB,
                    Gender = (int?)dataToSave.Data.Gender,
                    Email = dataToSave.Data.Email,
                    TelNo = dataToSave.Data.TelNo,
                    MobileNo = dataToSave.Data.MobileNo,
                    Address = dataToSave.Data.Address,
                    Street = dataToSave.Data.Street,
                    Locality = dataToSave.Data.Locality,
                    VillageTown = dataToSave.Data.VillageTown,
                };
                BaseDtoNaa = await new NAAService().GetSaveNaa(NAA, dataToSave.userToken);
                if (BaseDtoNaa.QryResult == new QueryResult().FAILED)
                {
                    BaseDto.Data = false;
                    BaseDto.AdditionalData = "Error Adding Record";
                    BaseDto.QryResult = new QueryResult().FAILED;
                    return BaseDto;
                }

                BaseDtoS = await new GenerateCode().GCodeMain(dataToSave.userToken, "NPF_SYS_Resource");
                NPF_SYS_ResourceType rt = context.NPF_SYS_ResourceType.Where(x => x.Id == dataToSave.Data.ResourceType).FirstOrDefault();
                NPF_SYS_Rank Rk = context.NPF_SYS_Rank.Where(x => x.Id == dataToSave.Data.Rank).FirstOrDefault();
                NPF_SYS_Station SN = context.NPF_SYS_Station.Where(x => x.Id == dataToSave.Data.StationUnitName).FirstOrDefault();
                NPF_SYS_Resource Resce = new NPF_SYS_Resource()
                {
                    Code = BaseDtoS.Data,
                    NAA = context.NPF_DIR_NAA.Where(x => x.Id == BaseDtoNaa.Data).FirstOrDefault(),
                    ResourceType = rt,
                    UserId = dataToSave.Data.UserId,
                    Rank = Rk,
                    DatejoinedMFRS = dataToSave.Data.DatejoinedMFRS,
                    PFNumber = dataToSave.Data.PFNumber,
                    StationUnitName = SN,
                    PrivateLicenseNumber = dataToSave.Data.PrivateLicenseNumber,
                    PrivateLicenseDriverCategoryId = dataToSave.Data.PrivateLicenseDriverCategoryId,
                    PrivateLicenseIssueDate = dataToSave.Data.PrivateLicenseIssueDate,
                    PrivateLicenseExpiryDate = dataToSave.Data.PrivateLicenseExpiryDate,

                };
                context.NPF_SYS_Resource.Add(Resce);
                context.SaveChanges();
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

        public async Task<BaseResponseDTO<bool>> UpdateAsync(BaseRequestDTO<ResourceCRUDTO> dataToUpdate)
        {
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
            BaseResponseDTO<long> BaseDtoNaa = new BaseResponseDTO<long>();
            try
            {
                if (context.NPF_SYS_Resource.Any(x => x.NAA.Id == dataToUpdate.Data.NAAId && x.ResourceType.Id == dataToUpdate.Data.ResourceType
                && x.Id != dataToUpdate.Data.Id && x.DeleteStatus != true))
                {
                    BaseDto.Data = false;
                    BaseDto.AdditionalData = "Resource Already Exist";
                    BaseDto.QryResult = queryResult.FAILED;
                    return BaseDto;
                }
                NAACRUDTO NAA = new NAACRUDTO()
                {
                    Id = dataToUpdate.Data.NAAId,
                    Code = dataToUpdate.Data.NAACode,
                    Title = (int)dataToUpdate.Data.Title,
                    Identifier = dataToUpdate.Data.NICPassport,
                    NICPASS = dataToUpdate.Data.NICPassport,
                    FirstName = dataToUpdate.Data.FirstName,
                    MiddleName = dataToUpdate.Data.MiddleName,
                    LastName = dataToUpdate.Data.LastName,
                    DOB = dataToUpdate.Data.DOB,
                    Gender = (int?)dataToUpdate.Data.Gender,
                    Email = dataToUpdate.Data.Email,
                    TelNo = dataToUpdate.Data.TelNo,
                    MobileNo = dataToUpdate.Data.MobileNo,
                    Address = dataToUpdate.Data.Address,
                    Street = dataToUpdate.Data.Street,
                    Locality = dataToUpdate.Data.Locality,
                    VillageTown = dataToUpdate.Data.VillageTown,
                };
                BaseDtoNaa = await new NAAService().GetSaveNaa(NAA, dataToUpdate.userToken);
                if (BaseDtoNaa.QryResult == new QueryResult().FAILED)
                {
                    BaseDto.Data = false;
                    BaseDto.AdditionalData = "Error Adding Record";
                    BaseDto.QryResult = new QueryResult().FAILED;
                    return BaseDto;
                }
                NPF_SYS_Resource Resce = context.NPF_SYS_Resource.Where(x => x.Id == dataToUpdate.Data.Id).FirstOrDefault();
                if (Resce.ResourceType.Id != dataToUpdate.Data.ResourceType)
                {
                    NPF_SYS_ResourceType rt = context.NPF_SYS_ResourceType.Where(x => x.Id == dataToUpdate.Data.ResourceType).FirstOrDefault();
                    Resce.ResourceType = rt;
                }

                Resce.NAA = context.NPF_DIR_NAA.Where(x => x.Id == BaseDtoNaa.Data).FirstOrDefault();
                Resce.UserId = dataToUpdate.Data.UserId;

                if (Resce.Rank.Id != dataToUpdate.Data.Rank)
                {
                    NPF_SYS_Rank Rk = context.NPF_SYS_Rank.Where(x => x.Id == dataToUpdate.Data.Rank).FirstOrDefault();
                    Resce.Rank = Rk;
                }

                if (Resce.StationUnitName.Id != dataToUpdate.Data.StationUnitName)
                {
                    NPF_SYS_Station SN = context.NPF_SYS_Station.Where(x => x.Id == dataToUpdate.Data.StationUnitName).FirstOrDefault();
                    Resce.StationUnitName = SN;
                }


                Resce.DatejoinedMFRS = dataToUpdate.Data.DatejoinedMFRS;
                Resce.PFNumber = dataToUpdate.Data.PFNumber;
                Resce.PrivateLicenseNumber = dataToUpdate.Data.PrivateLicenseNumber;
                Resce.PrivateLicenseDriverCategoryId = dataToUpdate.Data.PrivateLicenseDriverCategoryId;
                Resce.PrivateLicenseIssueDate = dataToUpdate.Data.PrivateLicenseIssueDate;
                Resce.PrivateLicenseExpiryDate = dataToUpdate.Data.PrivateLicenseExpiryDate;

                context.NPF_SYS_Resource.Update(Resce);
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
                NPF_SYS_Resource upd = context.NPF_SYS_Resource.Where(x => x.Id == dataToDelId.Data).FirstOrDefault();
                upd.DeleteStatus = true;
                context.NPF_SYS_Resource.Update(upd);
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
                string[] values = new string[] { "Title", "Gender", "License Category" };
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
                List<NPF_SYS_ResourceType> RT = context.NPF_SYS_ResourceType.Where(x => x.DeleteStatus == false).ToList();
                Dd = new DropDown();
                Dd.title = "ResearchType";
                Dd.items = new List<DropDownItem>();
                foreach (NPF_SYS_ResourceType R in RT)
                {
                    DropDownItem Ddi = new DropDownItem();
                    Ddi.Id = R.Id;
                    Ddi.text = R.Name;
                    Dd.items.Add(Ddi);
                }
                Ddl.Add(Dd);

                List<ResourceUserDTO> US = (from c in context.NPF_SYS_User
                                            join d in context.Users on c.AId equals d.Id
                                            where c.DeleteStatus == false
                                            select new ResourceUserDTO()
                                            {
                                                Name = d.FullName + " (" + d.Email + ")",
                                                Id = c.Id,
                                            }).ToList();
                Dd = new DropDown();
                Dd.title = "User";
                Dd.items = new List<DropDownItem>();
                foreach (ResourceUserDTO U in US)
                {
                    DropDownItem Ddi = new DropDownItem();
                    Ddi.Id = U.Id;
                    Ddi.text = U.Name;
                    Dd.items.Add(Ddi);
                }
                Ddl.Add(Dd);

                List<NPF_SYS_Rank> RK = context.NPF_SYS_Rank.Where(x => x.DeleteStatus == false).ToList();
                Dd = new DropDown();
                Dd.title = "Rank";
                Dd.items = new List<DropDownItem>();
                foreach (NPF_SYS_Rank R in RK)
                {
                    DropDownItem Ddi = new DropDownItem();
                    Ddi.Id = R.Id;
                    Ddi.text = R.Name;
                    Dd.items.Add(Ddi);
                }
                Ddl.Add(Dd);

                List<NPF_SYS_Station> stations = context.NPF_SYS_Station.Where(x => x.DeleteStatus == false).ToList();
                Dd = new DropDown();
                Dd.title = "Station";
                Dd.items = new List<DropDownItem>();
                foreach (NPF_SYS_Station R in stations)
                {
                    DropDownItem Ddi = new DropDownItem();
                    Ddi.Id = R.Id;
                    Ddi.text = R.Name;
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
