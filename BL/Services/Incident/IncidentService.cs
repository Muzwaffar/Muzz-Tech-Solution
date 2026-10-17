using BL.Constants;
using BL.Models.Administration;
using BL.Models.Common;
using BL.Models.Incident;
using BL.Services.Administration;
using BL.Services.Common;
using BL.Services.Source;
using DAL.Common;
using DAL.DBContext;
using DAL.Models;
using DAL.Models.Administration;
using DAL.Models.Transaction;
using System;
using System.Text.RegularExpressions;

namespace BL.Services.Incident
{
    public class IncidentService
    {
        private NPFContext context;
        private QueryResult queryResult;
        public SourceControlService Sservice;
        public IncidentService()
        {
            context = new NPFContext();
            queryResult = new QueryResult();
            Sservice = new SourceControlService();
        }

        public BaseResponseDTO<List<IncidentDTO>> GetAll()
        {
            BaseResponseDTO<List<IncidentDTO>> dto = new BaseResponseDTO<List<IncidentDTO>>();
            List<IncidentDTO> result = new List<IncidentDTO>();
            string errorMsg = "No Data Found";
            try
            {
                List<NPF_TRN_Incident> List = context.NPF_TRN_Incident.Where(x => x.DeleteStatus == false && x.Status == TransactionStatus.Submitted && x.DateTimeSubmitted != null && x.DateTimeIncidentCompleted == null && x.DateTimeIncidentAbort == null).OrderByDescending(y => y.CreatedDate).ToList();
                foreach (NPF_TRN_Incident item in List)
                {
                    try
                    {
                        IncidentDTO incident = new IncidentDTO();

                        TimeSpan span = (DateTime.Now - item.CreatedDate);
                        incident.Id = item.Id;
                        incident.Timelapse = String.Format(" {0}day(s) {1}hr {2}min", span.Days, span.Hours, span.Minutes);
                        incident.Location =context.NPF_TRN_IncidentLocation.Where(x => x.IncidentId == item.Id).Select(p => p.Address).FirstOrDefault();
                        var coordinatesLong = context.NPF_TRN_IncidentLocation.Where(x => x.IncidentId == item.Id).Select(p => (p.Longitude).ToString()).FirstOrDefault();
                        var coordinatesLat = context.NPF_TRN_IncidentLocation.Where(x => x.IncidentId == item.Id).Select(p => (p.Latitude).ToString()).FirstOrDefault();
                        incident.Coordinates = coordinatesLat + "," + coordinatesLong;
                        if (coordinatesLat == "0" && coordinatesLong == "0") {
                            incident.Coordinates = null;
                        }
                        IncidentStationDTO station = (from a in context.NPF_TRN_IncidentStation
                                                where a.IncidentId == item.Id
                                                select new IncidentStationDTO
                                                {
                                                    Station = context.NPF_SYS_Station.Where(x => x.Id == a.StationId).Select(p => p.Name).FirstOrDefault(),
                                                    NatureOfIncident = context.NPF_SYS_NatureOfIncident.Where(x => x.Id == a.NatureOfIncidentId).Select(p => p.Name).FirstOrDefault(),
                                                }).FirstOrDefault();
                        incident.Station = station.Station;
                        incident.NatureOfIncident = station.NatureOfIncident;
                        incident.IncidentNo = item.IncidentNo;
                        //CallerListDTO caller = (from a in context.NPF_TRN_IncidentCaller
                        //                        join b in context.NPF_SYS_Caller on a.CallerId equals b.Id
                        //                        where a.IncidentId == item.Id
                        //                        select new CallerListDTO
                        //                        {
                        //                            MobileNo = b.NAA.MobileNo,
                        //                            FullName = b.NAA.FullName,
                        //                        }).FirstOrDefault();
                        //if (caller != null)
                        //{
                        //    incident.Phone = caller.MobileNo;
                        //    incident.ConatactName = caller.FullName == " " ? "No Name" : caller.FullName;
                        //}
                        //else
                        //{
                        //    incident.Phone = "No Contact Phone";
                        //    incident.ConatactName = "No Contact";
                        //}
                        var incidentCode = context.NPF_SYS_IncidentCode.Where(x => x.Id == item.IncidentCode).Select(p => p.Name).FirstOrDefault();
                        var type = context.NPF_SYS_IncidentType.Where(x => x.Id == item.IncidentType).Select(p => p.Name).FirstOrDefault();
                        var Subtype = context.NPF_SYS_IncidentSubType.Where(x => x.Id == item.IncidentSubType).Select(p => p.Name).FirstOrDefault();
                        incident.Type = type ?? "";
                        incident.SubType = Subtype ?? "";
                        incident.IncidentCode = incidentCode ?? "";

                        result.Add(incident);
                    }
                    catch (Exception ex)
                    {

                    }

                }
                //result.RemoveAll(s => s.Location == null && s.Coordinates == ",");
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
        public BaseResponseDTO<List<IncidentDTO>> GetAllByTypeName(string TypeName)
        {
            BaseResponseDTO<List<IncidentDTO>> dto = new BaseResponseDTO<List<IncidentDTO>>();
            List<IncidentDTO> result = new List<IncidentDTO>();
            string errorMsg = "No Data Found";
            try
            {
                DateTime From = DateTime.Today.AddDays(-1).StartOfDay();
                DateTime To = DateTime.Now.EndOfDay();
                long IncidentTypeId = context.NPF_SYS_IncidentType.Where(x => x.Name.ToLower().Trim(' ') == TypeName.ToLower().Trim(' ')).FirstOrDefault().Id;
                List<NPF_TRN_Incident> List = context.NPF_TRN_Incident.Where(x => x.DeleteStatus == false && x.DateTimeSubmitted != null && x.DateTimeIncidentCompleted == null && x.IncidentType == IncidentTypeId).OrderByDescending(y => y.CreatedDate).ToList();
                foreach (NPF_TRN_Incident item in List)
                {
                    try
                    {
                        IncidentDTO incident = new IncidentDTO();

                        TimeSpan span = (DateTime.Now - item.CreatedDate);
                        incident.Id = item.Id;
                        incident.Timelapse = String.Format(" {0}h {1}min", span.Hours, span.Minutes);
                        incident.Location = context.NPF_TRN_IncidentLocation.Where(x => x.IncidentId == item.Id).Select(p => p.Address).FirstOrDefault();
                        var coordinatesLong = context.NPF_TRN_IncidentLocation.Where(x => x.IncidentId == item.Id).Select(p => (p.Longitude).ToString()).FirstOrDefault();
                        var coordinatesLat = context.NPF_TRN_IncidentLocation.Where(x => x.IncidentId == item.Id).Select(p => (p.Latitude).ToString()).FirstOrDefault();
                        incident.Coordinates = coordinatesLat + "," + coordinatesLong;
                        if (coordinatesLat == "0" && coordinatesLong == "0")
                        {
                            incident.Coordinates = null;
                        }
                        IncidentStationDTO station = (from a in context.NPF_TRN_IncidentStation
                                                      where a.IncidentId == item.Id
                                                      select new IncidentStationDTO
                                                      {
                                                          Station = context.NPF_SYS_Station.Where(x => x.Id == a.StationId).Select(p => p.Name).FirstOrDefault(),
                                                          NatureOfIncident = context.NPF_SYS_NatureOfIncident.Where(x => x.Id == a.NatureOfIncidentId).Select(p => p.Name).FirstOrDefault(),
                                                      }).FirstOrDefault();
                        incident.Station = station.Station;
                        incident.NatureOfIncident = station.NatureOfIncident;
                        incident.IncidentNo = item.IncidentNo;
                        //CallerListDTO caller = (from a in context.NPF_TRN_IncidentCaller
                        //                        join b in context.NPF_SYS_Caller on a.CallerId equals b.Id
                        //                        where a.IncidentId == item.Id
                        //                        select new CallerListDTO
                        //                        {
                        //                            MobileNo = b.NAA.MobileNo,
                        //                            FullName = b.NAA.FullName,
                        //                        }).FirstOrDefault();
                        //if (caller != null)
                        //{
                        //    incident.Phone = caller.MobileNo;
                        //    incident.ConatactName = caller.FullName == " " ? "No Name" : caller.FullName;
                        //}
                        //else
                        //{
                        //    incident.Phone = "No Contact Phone";
                        //    incident.ConatactName = "No Contact";
                        //}
                        var incidentCode = context.NPF_SYS_IncidentCode.Where(x => x.Id == item.IncidentCode).Select(p => p.Name).FirstOrDefault();
                        var type = context.NPF_SYS_IncidentType.Where(x => x.Id == item.IncidentType).Select(p => p.Name).FirstOrDefault();
                        var Subtype = context.NPF_SYS_IncidentSubType.Where(x => x.Id == item.IncidentSubType).Select(p => p.Name).FirstOrDefault();
                        incident.Type = type ?? "";
                        incident.SubType = Subtype ?? "";
                        incident.IncidentCode = incidentCode ?? "";

                        result.Add(incident);
                    }
                    catch (Exception ex)
                    {

                    }

                }
                //result.RemoveAll(s => s.Location == null && s.Coordinates == ",");
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
