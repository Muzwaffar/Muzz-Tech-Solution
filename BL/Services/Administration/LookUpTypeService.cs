using BL.Constants;
using BL.Models.Common;
using BL.Services.Common;
using DAL.DBContext;
using DAL.Models.Administration;

namespace BL.Services.Administration
{
    public class LookUpTypeService 
    {
        private NPFContext context;
        private QueryResult queryResult;
        public LookUpTypeService()
        {
            context = new NPFContext();
            queryResult = new QueryResult();
        }
        public BaseResponseDTO<List<NPF_SYS_LookUpType>> GetAll()
        {
            BaseResponseDTO<List<NPF_SYS_LookUpType>> dto = new BaseResponseDTO<List<NPF_SYS_LookUpType>>();
            List<NPF_SYS_LookUpType> result = new List<NPF_SYS_LookUpType>();
            string errorMsg = "No Data Found";
            try
            {
                result = context.NPF_SYS_LookUpType.Where(x => x.DeleteStatus == false).ToList();
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
            BaseResponseDTO <List<DropDown>> dto = new BaseResponseDTO<List<DropDown>>();
            List<DropDown> Ddl = new List<DropDown>();
            DropDown Dd = new DropDown();
            string errorMsg = "No Data Found";
            try
            {
               
                List<NPF_SYS_LookUpType> Ltl= context.NPF_SYS_LookUpType.Where(x => x.DeleteStatus == false).ToList();
                Dd.title = "LookUpType";
                foreach(NPF_SYS_LookUpType lt in Ltl)
                {
                    DropDownItem Ddi = new DropDownItem();
                    Ddi.Id = lt.Id;
                    Ddi.text = lt.Description;
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

        public BaseResponseDTO<NPF_SYS_LookUpType> GetById(int Id)
        {
            
            BaseResponseDTO<NPF_SYS_LookUpType> dto = new BaseResponseDTO<NPF_SYS_LookUpType>();
            NPF_SYS_LookUpType result = new NPF_SYS_LookUpType();
            string errorMsg = "No Data Found";
            try
            {
                int id = Id;
                result = context.NPF_SYS_LookUpType.Where(x => x.Id == id).FirstOrDefault();
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
            catch(Exception e)
            {
                dto.Data = result;
                dto.AdditionalData = errorMsg;
                dto.QryResult = queryResult.FAILED;
            }


            return dto;
        }

        public async Task<BaseResponseDTO<bool>> SaveAsync(BaseRequestDTO<NPF_SYS_LookUpType> dataToSave)
        {
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
            try
            {
                if (!context.NPF_SYS_LookUpType.Any(x => x.Name.ToLower() == dataToSave.Data.Name.ToLower() && x.DeleteStatus != true))
                {
                    dataToSave.Data.CreatedDate = DateTime.Now;
                    dataToSave.Data.CreatedBy = Guid.Parse(context.Users.Where(x => x.Email.ToLower() == "admin@naveo.mu").Select(x => x.Id).FirstOrDefault());
                    context.NPF_SYS_LookUpType.Add(dataToSave.Data);
                    context.SaveChanges();
                    BaseDto.Data = true;
                    BaseDto.QryResult = queryResult.SUCEEDED;
                }
                else
                {
                    BaseDto.Data = false;
                    BaseDto.AdditionalData = "LookUp Type Already Exist";
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

        public async Task<BaseResponseDTO<bool>> UpdateAsync(BaseRequestDTO<NPF_SYS_LookUpType> dataToUpdate)
        {
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
            try
            {
                if (context.NPF_SYS_LookUpType.Any(x => x.Name.ToLower() == dataToUpdate.Data.Name.ToLower() && x.DeleteStatus != true
                && x.Id != dataToUpdate.Data.Id))
                {
                    BaseDto.Data = false;
                    BaseDto.AdditionalData = "LookUp Type Already Exist";
                    BaseDto.QryResult = queryResult.FAILED;
                    return BaseDto;
                }
                dataToUpdate.Data.UpdatedDate = DateTime.Now;
                dataToUpdate.Data.UpdatedBy = Guid.Parse(context.Users.Where(x => x.Email.ToLower() == "admin@naveo.mu").Select(x => x.Id).FirstOrDefault());
                context.NPF_SYS_LookUpType.Update(dataToUpdate.Data);
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
        public BaseResponseDTO<bool> Delete(BaseRequestDTO<int> dataToDelId)
        {
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
            try
            {
                int Id = dataToDelId.Data;
                if (!context.NPF_SYS_LookUpValue.Any(x => x.LookUpType.Id == Id && x.DeleteStatus == false))
                {
                    NPF_SYS_LookUpType upd = context.NPF_SYS_LookUpType.Where(x => x.Id == Id).FirstOrDefault();
                    upd.DeleteStatus = true;
                    context.NPF_SYS_LookUpType.Update(upd);
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
