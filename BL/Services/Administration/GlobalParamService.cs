using BL.Constants;
using BL.Models.Access;
using BL.Models.Administration;
using BL.Models.Common;
using BL.Services.Common;
using DAL.DBContext;
using DAL.Models.Administration;


namespace BL.Services.Administration
{
    public class GlobalParamService
    {
        private NPFContext context;
        private QueryResult queryResult;
        private MatrixService Mservice;
        public UserMatrixDTO UM = new UserMatrixDTO();

        public GlobalParamService()
        {
            context = new NPFContext();
            queryResult = new QueryResult();
            Mservice = new MatrixService();
            UM = Mservice.sGetMatrixUserByUserID();
        }

        public BaseResponseDTO<List<GlobalParamListDTO>> GetAll()
        {
            BaseResponseDTO<List<GlobalParamListDTO>> dto = new BaseResponseDTO<List<GlobalParamListDTO>>();
            List<GlobalParamListDTO> result = new List<GlobalParamListDTO>();
            string errorMsg = "No Data Found";
            try
            {
                result = (from a in context.NPF_SYS_GlobalParam
                          where a.DeleteStatus == false && a.AdminOnly == false
                          select new GlobalParamListDTO()
                          {
                              Id = a.Id,
                              Name = a.Name,
                              Value = a.Value,
                              AdditionalValue = a.AdditionalValue,
                              Comment = a.Comment,

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

        public BaseResponseDTO<GlobalParamCRUDDTO> GetById(Guid Id)
        {
            BaseResponseDTO<GlobalParamCRUDDTO> dto = new BaseResponseDTO<GlobalParamCRUDDTO>();
            GlobalParamCRUDDTO result = new GlobalParamCRUDDTO();
            string errorMsg = "No Data Found";
            try
            {
                result = (from a in context.NPF_SYS_GlobalParam
                          where a.Id == Id
                          select new GlobalParamCRUDDTO()
                          {
                              Id = a.Id,
                              Name = a.Name,
                              Value = a.Value,
                              AdditionalValue = a.AdditionalValue,
                              Comment = a.Comment,
                              AdminOnly = a.AdminOnly,

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

        public async Task<BaseResponseDTO<bool>> UpdateAsync(BaseRequestDTO<GlobalParamCRUDDTO> dataToUpdate)
        {
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
            try
            {
                if (context.NPF_SYS_GlobalParam.Any(x => x.Name.ToLower() == dataToUpdate.Data.Name.ToLower()
                && x.DeleteStatus != true && x.Id != dataToUpdate.Data.Id))
                {
                    BaseDto.Data = false;
                    BaseDto.AdditionalData = "Global Param Already Exist";
                    BaseDto.QryResult = queryResult.FAILED;
                    return BaseDto;
                }
                NPF_SYS_GlobalParam GP = context.NPF_SYS_GlobalParam.Where(x => x.Id == dataToUpdate.Data.Id).FirstOrDefault();
                GP.Value = dataToUpdate.Data.Value;
                GP.AdditionalValue = dataToUpdate.Data.AdditionalValue;
                GP.Comment = dataToUpdate.Data.Comment;
                GP.AdminOnly = dataToUpdate.Data.AdminOnly;

                context.NPF_SYS_GlobalParam.Update(GP);
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
    }
}

