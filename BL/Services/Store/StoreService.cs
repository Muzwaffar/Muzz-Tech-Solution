using BL.Constants;
using BL.Models.Access;
using BL.Models.Common;
using BL.Models.Store;
using BL.Services.Common;
using DAL.DBContext;
using DAL.Models.Administration;
using DAL.Models.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Services.Store
{
    public class StoreService
    {
        private NPFContext context;
        private QueryResult queryResult;
        private MatrixService Mservice;
        public UserMatrixDTO UM = new UserMatrixDTO();

        public StoreService()
        {
            context = new NPFContext();
            queryResult = new QueryResult();
            Mservice = new MatrixService();
            UM = Mservice.sGetMatrixUserByUserID();
        }
        public BaseResponseDTO<List<StoreListDTO>> GetAllStore()
        {
            BaseResponseDTO<List<StoreListDTO>> dto = new BaseResponseDTO<List<StoreListDTO>>();
            List<StoreListDTO> result = new List<StoreListDTO>();
            string errorMsg = "No Data Found";
            try
            {
                result =(from a in context.NPF_STR_Store where a.DeleteStatus == false
                         select new StoreListDTO
                         {
                             Id = a.Id,
                             Name = a.Name,
                             Code = a.Code,
                             StationName=a.Station.Name,
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
            try
            {

                Dd = new DropDown();
                Dd.title = "Station";
                Dd.items = new List<DropDownItem>();
                List<NPF_SYS_Station> station = context.NPF_SYS_Station.Where(x => x.DeleteStatus == false).ToList();
                foreach (NPF_SYS_Station st in station)
                {
                    DropDownItem Ddi = new DropDownItem();
                    Ddi.Id = st.Id;
                    Ddi.text = st.Name;
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
        public BaseResponseDTO<StoreDTO> GetById(long? Id)
        {

            BaseResponseDTO<StoreDTO> dto = new BaseResponseDTO<StoreDTO>();
            StoreDTO result = new StoreDTO();
            string errorMsg = "No Data Found";
            try
            {
                NPF_STR_Store store = context.NPF_STR_Store.Where(x => x.Id == Id).FirstOrDefault();
                result = (from a in context.NPF_STR_Store
                          where a.DeleteStatus == false
                          select new StoreDTO
                          {
                              Id = a.Id,
                              Name = a.Name,
                              Code = a.Code,
                              Description = a.Description,
                              StationId = a.Station.Id
                          }).FirstOrDefault();

                if (result != null)
                {
                    result.Id = store.Id;
                    result.Code = store.Code;
                    result.Name = store.Name;
                    result.Description = store.Description;
                    result.StationId = store.Station.Id;

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
        public async Task<BaseResponseDTO<bool>> SaveAsync(BaseRequestDTO<StoreDTO> dataToSave)
        {
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
            try
            {
                BaseResponseDTO<string> BaseDtoS = new BaseResponseDTO<string>();
                BaseDtoS = await new GenerateCode().GCodeMain(dataToSave.userToken, "NPF_STR_Store");
                NPF_STR_Store store = new NPF_STR_Store()
                {
                    Code = BaseDtoS.Data,
                    Name = dataToSave.Data.Name,
                    Description = dataToSave.Data.Description,  
                    Station = context.NPF_SYS_Station.Where(x => x.Id == dataToSave.Data.StationId).FirstOrDefault()
                };

                context.NPF_STR_Store.Add(store);
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
        public async Task<BaseResponseDTO<bool>> UpdateAsync(BaseRequestDTO<StoreDTO> dataToUpdate)
        {
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
            try
            {
                NPF_STR_Store store = context.NPF_STR_Store.Where(x => x.Id == dataToUpdate.Data.Id).FirstOrDefault();
                store.Name = dataToUpdate.Data.Name;
                store.Description = dataToUpdate.Data.Description;
                store.Station = context.NPF_SYS_Station.Where(x => x.Id == dataToUpdate.Data.StationId).FirstOrDefault();

                context.NPF_STR_Store.Update(store);
                context.SaveChanges();
                BaseDto.Data = true;
                BaseDto.QryResult = queryResult.SUCEEDED;

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
                NPF_STR_Store storeToDel = context.NPF_STR_Store.Where(x => x.Id == dataToDelId).FirstOrDefault();
                if(storeToDel != null) {
                    storeToDel.DeleteStatus = true;
                    context.NPF_STR_Store.Update(storeToDel);
                    context.SaveChanges();
                    BaseDto.Data = true;
                    BaseDto.QryResult = queryResult.SUCEEDED;
                }
                else
                {

                    BaseDto.Data = false;
                    BaseDto.AdditionalData = "Cannot Delete Record. Record Currently In Use";
                    BaseDto.QryResult = queryResult.FAILED;
                }
                

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
