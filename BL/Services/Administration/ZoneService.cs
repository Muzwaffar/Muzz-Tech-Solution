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
    public class ZoneService
    {
        private NPFContext context;
        private QueryResult queryResult;

        public ZoneService()
        {
            context = new NPFContext();
            queryResult = new QueryResult();
        }

        public BaseResponseDTO<List<ZoneListDTO>> GetAll()
        {
            BaseResponseDTO<List<ZoneListDTO>> dto = new BaseResponseDTO<List<ZoneListDTO>>();
            List<ZoneListDTO> result = new List<ZoneListDTO>();
            string errorMsg = "No Data Found";
            try
            {
                result = (from a in context.NPF_SYS_Zone
                          where a.DeleteStatus == false
                          select new ZoneListDTO()
                          {
                              Id = a.Id,
                              Code = a.Code,
                              Name = a.Name,
                              Description = a.Description,
                              ZoneType = a.ZoneType.Name,
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

                List<NPF_SYS_ZoneType> Mk = context.NPF_SYS_ZoneType.Where(x => x.DeleteStatus == false).ToList();
                Dd.title = "ZoneType";
                Dd.items = new List<DropDownItem>();
                foreach (NPF_SYS_ZoneType mk in Mk)
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

        public BaseResponseDTO<ZoneDTO> GetById(long Id)
        {
            BaseResponseDTO<ZoneDTO> dto = new BaseResponseDTO<ZoneDTO>();
            ZoneDTO result = new ZoneDTO();
            string errorMsg = "No Data Found";
            try
            {
                result = (from a in context.NPF_SYS_Zone
                          where a.DeleteStatus == false && a.Id == Id
                          select new ZoneDTO()
                          {
                              Id = a.Id,
                              Code = a.Code,
                              Name = a.Name,
                              Description = a.Description,
                              ZoneType = a.ZoneType.Id,
                              GeomDataText = a.GeomDataText,
                              Type = a.Type,
                              CenterPointLat = a.CenterPointLat,
                              CenterPointLong = a.CenterPointLong
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

        public async Task<BaseResponseDTO<bool>> SaveAsync(BaseRequestDTO<ZoneDTO> dataToSave)
        {
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
            BaseResponseDTO<string> BaseDtoS = new BaseResponseDTO<string>();
            try
            {
                if (!context.NPF_SYS_Zone.Any(x => x.Name.ToLower() == dataToSave.Data.Name.ToLower() && x.DeleteStatus != true))
                {
                    NPF_SYS_ZoneType zonetype = context.NPF_SYS_ZoneType.Where(x => x.Id == dataToSave.Data.ZoneType).FirstOrDefault();
                    BaseDtoS = await new GenerateCode().GCodeMain(dataToSave.userToken, "NPF_SYS_Zone");
                    NPF_SYS_Zone ZN = new NPF_SYS_Zone()
                    {
                        Code = BaseDtoS.Data,
                        Name = dataToSave.Data.Name,
                        Description = dataToSave.Data.Description,
                        ZoneType = zonetype,
                        GeomDataText = dataToSave.Data.GeomDataText,
                        Type = dataToSave.Data.Type,
                        CenterPointLong = dataToSave.Data.CenterPointLong,
                        CenterPointLat = dataToSave.Data.CenterPointLat,
                    };
                    context.NPF_SYS_Zone.Add(ZN);
                    context.SaveChanges();

                    BaseDto.Data = true;
                    BaseDto.QryResult = queryResult.SUCEEDED;
                }
                else
                {
                    BaseDto.Data = false;
                    BaseDto.AdditionalData = "Zone Already Exist";
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

        public async Task<BaseResponseDTO<bool>> UpdateAsync(BaseRequestDTO<ZoneDTO> dataToUpdate)
        {
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
            try
            {
                if (!context.NPF_SYS_Zone.Any(x => x.Name.ToLower() == dataToUpdate.Data.Name.ToLower() && x.DeleteStatus != true
                && x.Id != dataToUpdate.Data.Id))
                {
                    NPF_SYS_ZoneType zonetype = context.NPF_SYS_ZoneType.Where(x => x.Id == dataToUpdate.Data.ZoneType).FirstOrDefault();
                    NPF_SYS_Zone Zone = context.NPF_SYS_Zone.Where(x => x.Id == dataToUpdate.Data.Id).FirstOrDefault();
                    Zone.Name = dataToUpdate.Data.Name;
                    Zone.Description = dataToUpdate.Data.Description;
                    Zone.ZoneType = zonetype;
                    Zone.GeomDataText = dataToUpdate.Data.GeomDataText;
                    Zone.Type = dataToUpdate.Data.Type;
                    Zone.CenterPointLat = dataToUpdate.Data.CenterPointLat;
                    Zone.CenterPointLong = dataToUpdate.Data.CenterPointLong;
                    context.NPF_SYS_Zone.Update(Zone);
                    context.SaveChanges();
                    BaseDto.Data = true;
                    BaseDto.QryResult = queryResult.SUCEEDED;
                }
                else
                {
                    BaseDto.Data = false;
                    BaseDto.AdditionalData = "Zone Already Exist";
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
                NPF_SYS_Zone upd = context.NPF_SYS_Zone.Where(x => x.Id == dataToDelId).FirstOrDefault();
                upd.DeleteStatus = true;
                context.NPF_SYS_Zone.Update(upd);
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
