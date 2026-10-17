using BL.Constants;
using BL.Models.Common;
using BL.Services.Common;
using DAL.DBContext;
using DAL.Models.Administration;

namespace BL.Services.Administration
{
    public class UomHeaderService : IRepositoryCus<NPF_SYS_UomHeader>
    {
        private NPFContext context;
        private QueryResult queryResult;
        public UomHeaderService()
        {
            context = new NPFContext();
            queryResult = new QueryResult();
        }
        public BaseResponseDTO<List<NPF_SYS_UomHeader>> GetAll()
        {
            BaseResponseDTO<List<NPF_SYS_UomHeader>> dto = new BaseResponseDTO<List<NPF_SYS_UomHeader>>();
            List<NPF_SYS_UomHeader> result = new List<NPF_SYS_UomHeader>();
            string errorMsg = "No Data Found";
            try
            {
                result = context.NPF_SYS_UomHeader.Where(x => x.DeleteStatus == false).ToList();
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
               
                List<NPF_SYS_UomHeader> Ltl= context.NPF_SYS_UomHeader.Where(x => x.DeleteStatus == false).ToList();
                Dd.title = "UomHeader";
                foreach(NPF_SYS_UomHeader lt in Ltl)
                {
                    DropDownItem Ddi = new DropDownItem();
                    Ddi.Id = lt.Id;
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

        public BaseResponseDTO<NPF_SYS_UomHeader> GetById(long Id)
        {
            
            BaseResponseDTO<NPF_SYS_UomHeader> dto = new BaseResponseDTO<NPF_SYS_UomHeader>();
            NPF_SYS_UomHeader result = new NPF_SYS_UomHeader();
            string errorMsg = "No Data Found";
            try
            {
                long id = Id;
                result = context.NPF_SYS_UomHeader.Where(x => x.Id == id).FirstOrDefault();
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

        public async Task<BaseResponseDTO<bool>> SaveAsync(BaseRequestDTO<NPF_SYS_UomHeader> dataToSave)
        {
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
            try
            {
                if (!context.NPF_SYS_UomHeader.Any(x => x.Name.ToLower() == dataToSave.Data.Name.ToLower() && x.DeleteStatus != true))
                {
                    dataToSave.Data.CreatedDate = DateTime.Now;
                    dataToSave.Data.CreatedBy = Guid.Parse(context.Users.Where(x => x.Email.ToLower() == "admin@naveo.mu").Select(x => x.Id).FirstOrDefault());
                    context.NPF_SYS_UomHeader.Add(dataToSave.Data);
                    context.SaveChanges();
                    BaseDto.Data = true;
                    BaseDto.QryResult = queryResult.SUCEEDED;
                }
                else
                {
                    BaseDto.Data = false;
                    BaseDto.AdditionalData = "Unit of Measure Header Already Exist";
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

        public async Task<BaseResponseDTO<bool>> UpdateAsync(BaseRequestDTO<NPF_SYS_UomHeader> dataToUpdate)
        {
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
            try
            {
                if (context.NPF_SYS_UomHeader.Any(x => x.Name.ToLower() == dataToUpdate.Data.Name.ToLower() && x.DeleteStatus != true
                && x.Id != dataToUpdate.Data.Id))
                {
                    BaseDto.Data = false;
                    BaseDto.AdditionalData = "Unit of Measure Header Already Exist";
                    BaseDto.QryResult = queryResult.FAILED;
                    return BaseDto;
                }
                dataToUpdate.Data.UpdatedDate = DateTime.Now;
                dataToUpdate.Data.UpdatedBy = Guid.Parse(context.Users.Where(x => x.Email.ToLower() == "admin@naveo.mu").Select(x => x.Id).FirstOrDefault());
                context.NPF_SYS_UomHeader.Update(dataToUpdate.Data);
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
        public BaseResponseDTO<bool> Delete(BaseRequestDTO<long> dataToDelId)
        {
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
            try
            {

                if (!context.NPF_SYS_UomDetails.Any(x => x.UomHeader.Id == dataToDelId.Data && x.DeleteStatus == false))
                {
                    NPF_SYS_UomHeader upd = context.NPF_SYS_UomHeader.Where(x => x.Id == dataToDelId.Data).FirstOrDefault();
                    upd.DeleteStatus = true;
                    context.NPF_SYS_UomHeader.Update(upd);
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
