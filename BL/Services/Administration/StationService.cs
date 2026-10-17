using BL.Constants;
using BL.Models.Administration;
using BL.Models.Common;
using BL.Services.Common;
using DAL.DBContext;
using DAL.Models.Administration;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace BL.Services.Administration
{
    public class StationService
    {
        private NPFContext context;
        private QueryResult queryResult;

        public StationService()
        {
            context = new NPFContext();
            queryResult = new QueryResult();
        }

        public BaseResponseDTO<List<StationListDTO>> GetAll()
        {
            BaseResponseDTO<List<StationListDTO>> dto = new BaseResponseDTO<List<StationListDTO>>();
            List<StationListDTO> result = new List<StationListDTO>();
            string errorMsg = "No Data Found";
            try
            {
                result = (from a in context.NPF_SYS_Station
                          where a.DeleteStatus == false
                          select new StationListDTO()
                          {
                              Id = a.Id,
                              Code = a.Code,
                              Name = a.Name,
                              Description = a.Description,
                              StationType = a.StationType,
                              StationLocation = a.StationLocation,
                              StationCallSign = a.StationCallSign,
                              PhoneNumber = a.PhoneNumber,
                              ContactNumber = a.ContactNumber,
                              Fax = a.Fax,
                              Email = a.Email,
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

        public BaseResponseDTO<StationCRUDDTO> GetById(long Id)
        {
            BaseResponseDTO<StationCRUDDTO> dto = new BaseResponseDTO<StationCRUDDTO>();
            StationCRUDDTO result = new StationCRUDDTO();
            string errorMsg = "No Data Found";
            try
            {
                result = (from a in context.NPF_SYS_Station
                          where a.Id == Id
                          select new StationCRUDDTO()
                          {
                              Id = a.Id,
                              Code = a.Code,
                              Name = a.Name,
                              Description = a.Description,
                              StationType = a.StationType.Id,
                              StationLocation = a.StationLocation.Id,
                              StationCallSign = a.StationCallSign,
                              PhoneNumber = a.PhoneNumber,
                              ContactNumber = a.ContactNumber,
                              Fax = a.Fax,
                              Email = a.Email,
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

        public async Task<BaseResponseDTO<bool>> SaveAsync(BaseRequestDTO<StationCRUDDTO> dataToSave)
        {
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
            BaseResponseDTO<string> BaseDtoS = new BaseResponseDTO<string>();
            try
            {
                if (!context.NPF_SYS_Station.Any(x => x.Name == dataToSave.Data.Name && x.DeleteStatus != true))
                {
                    BaseDtoS = await new GenerateCode().GCodeMain(dataToSave.userToken, "NPF_SYS_Station");
                    NPF_SYS_Station dt = new NPF_SYS_Station()
                    {
                        Code = BaseDtoS.Data,
                        Name = dataToSave.Data.Name,
                        Description = dataToSave.Data.Description ?? string.Empty,

                        StationType = context.NPF_SYS_StationType.Where(x => x.Id == dataToSave.Data.StationType).FirstOrDefault(),
                        StationLocation = context.NPF_SYS_Zone.Where(x => x.Id == dataToSave.Data.StationLocation).FirstOrDefault(),

                        StationCallSign = dataToSave.Data.StationCallSign,
                        PhoneNumber = dataToSave.Data.PhoneNumber,
                        ContactNumber = dataToSave.Data.ContactNumber,
                        Fax = dataToSave.Data.Fax,
                        Email = dataToSave.Data.Email,

                    };
                    context.NPF_SYS_Station.Add(dt);
                    context.SaveChanges();
                    BaseDto.ExtData = dt.Id.ToString();
                    BaseDto.Data = true;
                    BaseDto.QryResult = queryResult.SUCEEDED;
                }
                else
                {
                    BaseDto.Data = false;
                    BaseDto.AdditionalData = "Station Already Exist";
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

        public async Task<BaseResponseDTO<bool>> UpdateAsync(BaseRequestDTO<StationCRUDDTO> dataToUpdate)
        {
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
            try
            {
                if (context.NPF_SYS_Station.Any(x => x.Name == dataToUpdate.Data.Name
               && x.DeleteStatus != true && x.Id != dataToUpdate.Data.Id))
                {
                    BaseDto.Data = false;
                    BaseDto.AdditionalData = "Station Already Exist";
                    BaseDto.QryResult = queryResult.FAILED;
                    return BaseDto;
                }

                NPF_SYS_Station Update = context.NPF_SYS_Station.Where(x => x.Id == dataToUpdate.Data.Id).FirstOrDefault();

                Update.Name = dataToUpdate.Data.Name;
                Update.Description = dataToUpdate.Data.Description ?? string.Empty;

                Update.StationType = context.NPF_SYS_StationType.Where(x => x.Id == dataToUpdate.Data.StationType).FirstOrDefault();
                Update.StationLocation = context.NPF_SYS_Zone.Where(x => x.Id == dataToUpdate.Data.StationLocation).FirstOrDefault();

                Update.StationCallSign = dataToUpdate.Data.StationCallSign;
                Update.PhoneNumber = dataToUpdate.Data.PhoneNumber;
                Update.ContactNumber = dataToUpdate.Data.ContactNumber;
                Update.Fax = dataToUpdate.Data.Fax;
                Update.Email = dataToUpdate.Data.Email;

                context.NPF_SYS_Station.Update(Update);
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
                NPF_SYS_Station upd = context.NPF_SYS_Station.Where(x => x.Id == dataToDelId.Data).FirstOrDefault();
                upd.DeleteStatus = true;
                context.NPF_SYS_Station.Update(upd);
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
                List<NPF_SYS_StationType> Ltl = context.NPF_SYS_StationType.Where(x => x.DeleteStatus == false).ToList();
                Dd.title = "StationType";
                Dd.items = new List<DropDownItem>();
                foreach (NPF_SYS_StationType lt in Ltl)
                {
                    DropDownItem Ddi = new DropDownItem();
                    Ddi.Id = lt.Id;
                    Ddi.text = lt.Name;
                    Dd.items.Add(Ddi);
                }
                Ddl.Add(Dd);

                Dd = new DropDown();

                List<NPF_SYS_Zone> Lt2 = context.NPF_SYS_Zone.Where(x => x.DeleteStatus == false).ToList();
                Dd.title = "Zone";
                Dd.items = new List<DropDownItem>();
                foreach (NPF_SYS_Zone lt in Lt2)
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
