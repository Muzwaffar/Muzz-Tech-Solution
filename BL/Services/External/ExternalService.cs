using BL.Constants;
using BL.Models.Common;
using BL.Models.Dashboard;
using BL.Models.Incident;
using BL.Services.Common;
using DAL.Common;
using DAL.DBContext;
using DAL.Models;
using Newtonsoft.Json;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Services.Dashboard
{
    public class ExternalService
    {
        private NPFContext context;
        private QueryResult queryResult;

        public ExternalService()
        {
            context = new NPFContext();
            queryResult = new QueryResult();
        }

        public BaseResponseDTO<Getparams> GetFleetParam()
        {
            BaseResponseDTO<Getparams> dto = new BaseResponseDTO<Getparams>();
            Getparams dt = new Getparams();
            QueryResult queryResult = new QueryResult();

            string sQryResult = queryResult.SUCEEDED;

            try
            {
                dt = new Getparams
                {
                    FleetDataURL = context.NPF_SYS_GlobalParam.Where(x => x.Name == "FleetDataURL").FirstOrDefault().Value,
                    FleetLoginPayload = context.NPF_SYS_GlobalParam.Where(x => x.Name == "FleetLoginPayload").FirstOrDefault().Value,
                    FleetMapPayload = context.NPF_SYS_GlobalParam.Where(x => x.Name == "FleetMapPayload").FirstOrDefault().Value,

                };
                dto.Data = dt;
                dto.QryResult = queryResult.SUCEEDED;
                dto.AdditionalData = "";
            }
            catch (Exception es)
            {

                dto.Data = dt;
                dto.QryResult = queryResult.SUCEEDED;
                dto.AdditionalData = "An Error Occurred";
            }
            

            return dto;
        }



    }
}
