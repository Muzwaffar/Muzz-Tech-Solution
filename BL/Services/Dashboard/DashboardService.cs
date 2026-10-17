using BL.Constants;
using BL.Models.Common;
using BL.Models.Dashboard;
using DAL.Common;
using DAL.DBContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Services.Dashboard
{
    public class DashboardService 
    {
        private NPFContext context;
        private QueryResult queryResult;
      //  private Guid _UserId;

        public DashboardService()
        {
            context = new NPFContext();
            queryResult = new QueryResult();
         //   _UserId = context.GetUser();
        }
        
        public BaseResponseDTO<GetDashboardDTO> GetAll()
        {
            BaseResponseDTO<GetDashboardDTO> dto = new BaseResponseDTO<GetDashboardDTO>();
            GetDashboardDTO result = new GetDashboardDTO();
            string errorMsg = "No Data Found";
        //    Guid asd = _UserId;
            try
            {
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


    }
}
