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

namespace BL.Services.Access
{
    public class MenuService
    {
        private NPFContext context;
        private QueryResult queryResult;
        public MenuService()
        {
            context = new NPFContext();
            queryResult = new QueryResult();
        }

        public BaseResponseDTO<List<SideMenuDTO>> GetAll()
        {
            BaseResponseDTO<List<SideMenuDTO>> dto = new BaseResponseDTO<List<SideMenuDTO>>();
            List<SideMenuDTO> result = new List<SideMenuDTO>();
            string errorMsg = "No Data Found";
            try
            {
                result = (from a in context.NPF_SYS_Modules
                          where a.DeleteStatus == false
                          select new SideMenuDTO()
                          {
                              Id = a.Id,
                              DisplayName = a.DisplayName,
                              Order = a.Order,
                              Icon = a.Icon,

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

        public BaseResponseDTO<SideMenuDTO> GetById(string Id)
        {
            BaseResponseDTO<SideMenuDTO> dto = new BaseResponseDTO<SideMenuDTO>();
            SideMenuDTO result = new SideMenuDTO();
            string errorMsg = "No Data Found";
            try
            {
                Guid Mid = Guid.Parse(Id);
                result = (from a in context.NPF_SYS_Modules
                          where a.Id == Mid
                          select new SideMenuDTO()
                          {
                              Id = a.Id,
                              DisplayName = a.DisplayName,
                              Order = a.Order,
                              Icon = a.Icon,
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

        public async Task<BaseResponseDTO<bool>> UpdateAsync(BaseRequestDTO<SideMenuDTO> dataToUpdate)
        {
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
            try
            {
                NPF_SYS_Modules Mod = context.NPF_SYS_Modules.Where(x => x.Id == dataToUpdate.Data.Id).FirstOrDefault();
                Mod.DisplayName = dataToUpdate.Data.DisplayName;
                Mod.Order = dataToUpdate.Data.Order;
                Mod.Icon = dataToUpdate.Data.Icon;
                context.NPF_SYS_Modules.Update(Mod);
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
