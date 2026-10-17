using BL.Constants;
using BL.Models.Common;
using DAL.DBContext;
using DAL.Models.Administration;
using NetTopologySuite.Geometries;
using NetTopologySuite.Operation.Union;

namespace BL.Services.Administration
{
    public class UomDetailsService : IRepositoryCus<NPF_SYS_UomDetails>
    {
        private NPFContext context;
        private QueryResult queryResult;
        public UomDetailsService()
        {
            context = new NPFContext();
            queryResult = new QueryResult();
        }

        public BaseResponseDTO<List<NPF_SYS_UomDetails>> GetAll()
        {
            BaseResponseDTO<List<NPF_SYS_UomDetails>> dto = new BaseResponseDTO<List<NPF_SYS_UomDetails>>();
            List<NPF_SYS_UomDetails> result = new List<NPF_SYS_UomDetails>();
            string errorMsg = "No Data Found";
            try
            {
                result = context.NPF_SYS_UomDetails.Where(x => x.DeleteStatus == false).ToList();
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

                List<NPF_SYS_UomHeader> Ltl = context.NPF_SYS_UomHeader.Where(x => x.DeleteStatus == false).ToList();
                Dd.title = "UomHeader";
                Dd.items = new List<DropDownItem>();
                foreach (NPF_SYS_UomHeader lt in Ltl)
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

        public BaseResponseDTO<NPF_SYS_UomDetails> GetById(long Id)
        {
            BaseResponseDTO<NPF_SYS_UomDetails> dto = new BaseResponseDTO<NPF_SYS_UomDetails>();
            NPF_SYS_UomDetails result = new NPF_SYS_UomDetails();
            string errorMsg = "No Data Found";
            try
            {
                long id = Id;
                result = context.NPF_SYS_UomDetails.Where(x => x.Id == id).FirstOrDefault();
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

        public async Task<BaseResponseDTO<bool>> SaveAsync(BaseRequestDTO<NPF_SYS_UomDetails> dataToSave)
        {
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
            try
            {
                if (!context.NPF_SYS_UomDetails.Any(x => x.Name.ToLower() == dataToSave.Data.Name.ToLower() && x.UomHeader.Id == dataToSave.Data.UomHeader.Id && x.DeleteStatus != true))
                {

                    NPF_SYS_UomHeader type = context.NPF_SYS_UomHeader.Where(x => x.Id == dataToSave.Data.UomHeader.Id).FirstOrDefault();
                    context.NPF_SYS_UomHeader.Update(type);
                    dataToSave.Data.UomHeader = type;
                    context.NPF_SYS_UomDetails.Add(dataToSave.Data);
                    context.SaveChanges();
                    BaseDto.Data = true;
                    BaseDto.QryResult = queryResult.SUCEEDED;
                }
                else
                {
                    BaseDto.Data = false;
                    BaseDto.AdditionalData = "Unit of Measure Details Already Exist";
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

        public async Task<BaseResponseDTO<bool>> UpdateAsync(BaseRequestDTO<NPF_SYS_UomDetails> dataToUpdate)
        {
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
            try
            {
                //NPF_SYS_GeoTest geo = new NPF_SYS_GeoTest()
                //{
                //    Description = "Test1",
                //    Geo = new Point(25.515406117058262 ,- 2.9748969291015044) { SRID = 4326 },
                //    Geom = new Point(35.515406117058262, -12.9748969291015044) { SRID = 4326 },
                //};
                //context.NPF_SYS_GeoTest.Add(geo);
                //context.SaveChanges();

                if (context.NPF_SYS_UomDetails.Any(x => x.Name.ToLower() == dataToUpdate.Data.Name.ToLower() 
                && x.UomHeader.Id == dataToUpdate.Data.UomHeader.Id && x.DeleteStatus != true && x.Id != dataToUpdate.Data.Id))
                {
                    BaseDto.Data = false;
                    BaseDto.AdditionalData = "Unit of Measure Already Exist";
                    BaseDto.QryResult = queryResult.FAILED;
                    return BaseDto;
                }
                NPF_SYS_UomDetails lv = context.NPF_SYS_UomDetails.Where(x => x.Id == dataToUpdate.Data.Id).FirstOrDefault();
                NPF_SYS_UomHeader lt = context.NPF_SYS_UomHeader.Where(x => x.Id == dataToUpdate.Data.UomHeader.Id).FirstOrDefault();
                lv.Name = dataToUpdate.Data.Name;
                lv.Unit = dataToUpdate.Data.Unit;
                lv.RelativeFactor = dataToUpdate.Data.RelativeFactor;
                lv.IsSystemInternational = dataToUpdate.Data.IsSystemInternational;
                lv.UomHeader = lt;
                context.NPF_SYS_UomDetails.Update(lv);
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
                NPF_SYS_UomDetails upd = context.NPF_SYS_UomDetails.Where(x => x.Id == dataToDelId.Data).FirstOrDefault();
                upd.DeleteStatus = true;
                context.NPF_SYS_UomDetails.Update(upd);
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
