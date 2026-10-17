using BL.Constants;
using BL.Models.Common;
using DAL.DBContext;
using DAL.Models.Administration;
using NetTopologySuite.Geometries;
using NetTopologySuite.Operation.Union;

namespace BL.Services.Administration
{
    public class LookUpValueService
    {
        private NPFContext context;
        private QueryResult queryResult;
        public LookUpValueService()
        {
            context = new NPFContext();
            queryResult = new QueryResult();
        }

        public BaseResponseDTO<List<NPF_SYS_LookUpValue>> GetAll()
        {
            BaseResponseDTO<List<NPF_SYS_LookUpValue>> dto = new BaseResponseDTO<List<NPF_SYS_LookUpValue>>();
            List<NPF_SYS_LookUpValue> result = new List<NPF_SYS_LookUpValue>();
            string errorMsg = "No Data Found";
            try
            {
                result = context.NPF_SYS_LookUpValue.Where(x => x.DeleteStatus == false).ToList();
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

                List<NPF_SYS_LookUpType> Ltl = context.NPF_SYS_LookUpType.Where(x => x.DeleteStatus == false).ToList();
                Dd.title = "LookUpType";
                Dd.items = new List<DropDownItem>();
                foreach (NPF_SYS_LookUpType lt in Ltl)
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

        public BaseResponseDTO<NPF_SYS_LookUpValue> GetById(int Id)
        {
            BaseResponseDTO<NPF_SYS_LookUpValue> dto = new BaseResponseDTO<NPF_SYS_LookUpValue>();
            NPF_SYS_LookUpValue result = new NPF_SYS_LookUpValue();
            string errorMsg = "No Data Found";
            try
            {
                int id = Id;
                result = context.NPF_SYS_LookUpValue.Where(x => x.Id == id).FirstOrDefault();
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

        public async Task<BaseResponseDTO<bool>> SaveAsync(BaseRequestDTO<NPF_SYS_LookUpValue> dataToSave)
        {
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
            try
            {
                if (!context.NPF_SYS_LookUpValue.Any(x => x.Name.ToLower() == dataToSave.Data.Name.ToLower() && x.LookUpType.Id == dataToSave.Data.LookUpType.Id && x.DeleteStatus != true))
                {

                    NPF_SYS_LookUpType type = context.NPF_SYS_LookUpType.Where(x => x.Id == dataToSave.Data.LookUpType.Id).FirstOrDefault();
                    context.NPF_SYS_LookUpType.Update(type);
                    dataToSave.Data.LookUpType = type;
                    dataToSave.Data.CreatedDate = DateTime.Now;
                    dataToSave.Data.CreatedBy = Guid.Parse(context.Users.Where(x => x.Email.ToLower() == "admin@naveo.mu").Select(x => x.Id).FirstOrDefault());
                    context.NPF_SYS_LookUpValue.Add(dataToSave.Data);
                    context.SaveChanges();
                    BaseDto.Data = true;
                    BaseDto.QryResult = queryResult.SUCEEDED;
                }
                else
                {
                    BaseDto.Data = false;
                    BaseDto.AdditionalData = "LookUp Value Already Exist";
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

        public async Task<BaseResponseDTO<bool>> UpdateAsync(BaseRequestDTO<NPF_SYS_LookUpValue> dataToUpdate)
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

                if (context.NPF_SYS_LookUpValue.Any(x => x.Name.ToLower() == dataToUpdate.Data.Name.ToLower() 
                && x.LookUpType.Id == dataToUpdate.Data.LookUpType.Id && x.DeleteStatus != true && x.Id != dataToUpdate.Data.Id))
                {
                    BaseDto.Data = false;
                    BaseDto.AdditionalData = "LookUp Value Already Exist";
                    BaseDto.QryResult = queryResult.FAILED;
                    return BaseDto;
                }
                NPF_SYS_LookUpValue lv = context.NPF_SYS_LookUpValue.Where(x => x.Id == dataToUpdate.Data.Id).FirstOrDefault();
                NPF_SYS_LookUpType lt = context.NPF_SYS_LookUpType.Where(x => x.Id == dataToUpdate.Data.LookUpType.Id).FirstOrDefault();
                lv.Name = dataToUpdate.Data.Name;
                lv.Description = dataToUpdate.Data.Description;
                lv.LookUpType = lt;
                lv.UpdatedDate = DateTime.Now;
                lv.UpdatedBy = Guid.Parse(context.Users.Where(x => x.Email.ToLower() == "admin@naveo.mu").Select(x => x.Id).FirstOrDefault());
                context.NPF_SYS_LookUpValue.Update(lv);
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

        public BaseResponseDTO<bool> Delete(BaseRequestDTO<int> dataToDelId)
        {
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
            try
            {
                int Id = dataToDelId.Data;
                NPF_SYS_LookUpValue upd = context.NPF_SYS_LookUpValue.Where(x => x.Id == Id).FirstOrDefault();
                upd.DeleteStatus = true;
                context.NPF_SYS_LookUpValue.Update(upd);
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
