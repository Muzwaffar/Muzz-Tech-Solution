using BL.Constants;
using BL.Models.Access;
using BL.Models.Administration;
using BL.Models.Common;
using BL.Services.Common;
using DAL.Common;
using DAL.DBContext;
using DAL.Models.Access;
using DAL.Models.Administration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Services.Access.OfficeZones
{
    public class OfficeZonesService
    {
        private NPFContext context;
        private QueryResult queryResult;
        public OfficeZonesService()
        {
            context = new NPFContext();
            queryResult = new QueryResult();
        }

        public BaseResponseDTO<List<OfficeZonesListDTO>> GetAll()
        {
            BaseResponseDTO<List<OfficeZonesListDTO>> dto = new BaseResponseDTO<List<OfficeZonesListDTO>>();
            List<OfficeZonesListDTO> result = new List<OfficeZonesListDTO>();
            string errorMsg = "No Data Found";
            try
            {
                result = (from a in context.NPF_SYS_OfficeZones
                          where a.DeleteStatus == false
                          select new OfficeZonesListDTO()
                          {
                              Id = a.Id,
                              Code = a.Code,
                              OfficeName = a.OfficeName,
                              Department = context.NPF_SYS_Department.Where(x => x.Id == a.Department).FirstOrDefault().Name,
                              Zone = context.NPF_SYS_LookUpValue.Where(x => x.Id == a.Zone).FirstOrDefault().Name,
                              Locality = context.NPF_SYS_LookUpValue.Where(x => x.Id == a.Locality).FirstOrDefault().Name,

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

        public BaseResponseDTO<OfficeZonesCRUDTO> GetById(long Id)
        {
            BaseResponseDTO<OfficeZonesCRUDTO> dto = new BaseResponseDTO<OfficeZonesCRUDTO>();
            OfficeZonesCRUDTO result = new OfficeZonesCRUDTO();
            string errorMsg = "No Data Found";
            try
            {
                result = (from a in context.NPF_SYS_OfficeZones
                          where a.Id == Id
                          select new OfficeZonesCRUDTO()
                          {
                              Id = a.Id,
                              OfficeName = a.OfficeName,
                              Department = a.Department,
                              Locality = a.Locality,
                              Zone = a.Zone,
                              Code = a.Code,
                              Abbreviation = a.Abbrevation,
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

        public async Task<BaseResponseDTO<bool>> SaveAsync(BaseRequestDTO<OfficeZonesCRUDTO> dataToSave)
        {
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
            BaseResponseDTO<string> BaseDtoS = new BaseResponseDTO<string>();
            try
            {
                if (!context.NPF_SYS_OfficeZones.Any(x => x.OfficeName.ToLower() == dataToSave.Data.OfficeName.ToLower() && x.Locality == dataToSave.Data.Locality && x.DeleteStatus != true))
                {
                    BaseDtoS = await new GenerateCode().GCodeMain(dataToSave.userToken, "NPF_SYS_OfficeZones");
                    NPF_SYS_OfficeZones dt = new NPF_SYS_OfficeZones()
                    {
                        Code = BaseDtoS.Data,
                        OfficeName = dataToSave.Data.OfficeName,
                        Department = dataToSave.Data.Department,
                        Locality = dataToSave.Data.Locality,
                        Zone = dataToSave.Data.Zone,
                        Abbrevation = dataToSave.Data.Abbreviation

                    };
                    context.NPF_SYS_OfficeZones.Add(dt);
                    context.SaveChanges();
                    BaseDto.Data = true;
                    BaseDto.QryResult = queryResult.SUCEEDED;
                }
                else
                {
                    BaseDto.Data = false;
                    BaseDto.AdditionalData = "Office Zone Already Exists";
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

        public async Task<BaseResponseDTO<bool>> UpdateAsync(BaseRequestDTO<OfficeZonesCRUDTO> dataToUpdate)
        {
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
            try
            {
                if (context.NPF_SYS_OfficeZones.Any(x => x.OfficeName.ToLower() == dataToUpdate.Data.OfficeName.ToLower()
                && x.Locality == dataToUpdate.Data.Locality && x.DeleteStatus != true && x.Id != dataToUpdate.Data.Id))
                {
                    BaseDto.Data = false;
                    BaseDto.AdditionalData = "Office Zone Already Exist";
                    BaseDto.QryResult = queryResult.FAILED;
                    return BaseDto;
                }
                NPF_SYS_OfficeZones OFZ = context.NPF_SYS_OfficeZones.Where(x => x.Id == dataToUpdate.Data.Id).FirstOrDefault();
                OFZ.OfficeName = dataToUpdate.Data.OfficeName;
                OFZ.Department = dataToUpdate.Data.Department;
                OFZ.Locality = dataToUpdate.Data.Locality;
                OFZ.Zone = dataToUpdate.Data.Zone;
                OFZ.Abbrevation = dataToUpdate.Data.Abbreviation;
                context.NPF_SYS_OfficeZones.Update(OFZ);
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
                NPF_SYS_OfficeZones upd = context.NPF_SYS_OfficeZones.Where(x => x.Id == dataToDelId.Data).FirstOrDefault();
                upd.DeleteStatus = true;
                context.NPF_SYS_OfficeZones.Update(upd);
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
                string[] values = new string[] { "Locality", "Zone" };
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

                List<NPF_SYS_Department> DPT = context.NPF_SYS_Department.Where(x => x.DeleteStatus == false && (x.Name == ResearchType.Crop.ToString() || x.Name == ResearchType.LiveStock.ToString()
                || x.Name == ResearchType.Biofarming.ToString())).ToList();
                Dd = new DropDown();
                Dd.title = "Department";
                Dd.items = new List<DropDownItem>();
                foreach (NPF_SYS_Department D in DPT)
                {
                    DropDownItem Ddi = new DropDownItem();
                    Ddi.Id = D.Id;
                    Ddi.text = D.Name;
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
