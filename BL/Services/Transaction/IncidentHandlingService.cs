using BL.Constants;
using BL.Models.Access;
using BL.Models.Administration;
using BL.Models.Common;
using BL.Models.Schedule;
using BL.Models.Store;
using BL.Models.Template;
using BL.Models.Transaction;
using BL.Services.Common;
using BL.Services.Transaction;
using DAL.Common;
using DAL.DBContext;
using DAL.Models;
using DAL.Models.Administration;
using DAL.Models.Common;
using DAL.Models.Request;
using DAL.Models.Store;
using DAL.Models.Transaction;
using Grpc.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using NPOI.POIFS.Crypt.Dsig;
using NPOI.SS.Formula.Functions;
using Org.BouncyCastle.Ocsp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;
using static BL.Models.Request.RequestDTO;

namespace BL.Services.Store
{
    public class IncidentHandlingService
    {
        private NPFContext context;
        private QueryResult queryResult;
        public UserManager<ApplicationUser> UserManager;
        public NotificationHandlerService Nhs;
        public ChatService ChatService;
        public AvailabilityStatus Avl;

        public IncidentHandlingService(Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> userManager)
        {
            UserManager = userManager;
            context = new NPFContext();
            queryResult = new QueryResult();
            Nhs = new NotificationHandlerService(UserManager);
            ChatService = new ChatService();
            Avl = new AvailabilityStatus();
        }
        public BaseResponseDTO<List<IncidentHandlingListDTO>> GetAllCallHandling(BaseRequestDTO<DateRangeDTO> dt)
        {
            BaseResponseDTO<List<IncidentHandlingListDTO>> dto = new BaseResponseDTO<List<IncidentHandlingListDTO>>();
            List<IncidentHandlingListDTO> result = new List<IncidentHandlingListDTO>();
            string errorMsg = "No Data Found";
            bool HasAllAccess = false;
            List<long> StationIds = new List<long>();
            long Uid = 0;
            try
            {
                NPF_SYS_GlobalParam Gp = context.NPF_SYS_GlobalParam.Where(x => x.Name == "IsUAT").FirstOrDefault();
                try
                {
                    if (Gp.Value == "1")
                    {
                        HasAllAccess = true;
                    }
                    else
                    {
                        Uid = context.NPF_SYS_User.Where(x => x.AId == dt.userToken).FirstOrDefault().Id;
                        HasAllAccess = context.NPF_SYS_Resource.Any(x => x.UserId == Uid && x.ResourceType.Name == "CRO" && x.DeleteStatus == false);
                    }

                }
                catch (Exception ex)
                {
                    HasAllAccess = true;
                }
                if (dt == null)
                {
                    dt = new BaseRequestDTO<DateRangeDTO>();
                    dt.Data = new DateRangeDTO();
                    dt.Data.From = DateTime.Now.AddDays(-7);
                    dt.Data.To = DateTime.Now;
                }
                dt.Data.From = dt.Data.From.StartOfDay();
                dt.Data.To = dt.Data.To.EndOfDay();
                if (HasAllAccess)
                {
                    result = (from a in context.NPF_TRN_Incident
                              where a.DeleteStatus == false && a.CreatedDate >= dt.Data.From && a.CreatedDate <= dt.Data.To && a.Status != DAL.Common.TransactionStatus.Open
                              select new IncidentHandlingListDTO
                              {
                                  Id = a.Id,
                                  Code = a.Code,
                                  IncidentNo = a.IncidentNo,
                                  ParentIncident = a.ParentIncidentId != null ? context.NPF_TRN_Incident.Where(x => x.Id == (long)(a.ParentIncidentId)).FirstOrDefault().IncidentNo : string.Empty,
                                  IncidentType = a.IncidentType != null ? context.NPF_SYS_IncidentType.Where(x => x.Id == (long)(a.IncidentType)).FirstOrDefault().Name : string.Empty,
                                  IncidentSubType = a.IncidentSubType != null ? context.NPF_SYS_IncidentSubType.Where(x => x.Id == (long)(a.IncidentSubType)).FirstOrDefault().Name : string.Empty,
                                  IncidentCode = a.IncidentCode != null ? context.NPF_SYS_IncidentCode.Where(x => x.Id == (long)(a.IncidentCode)).FirstOrDefault().Name : string.Empty,
                                  DateSubmitted = a.DateTimeSubmitted != null ? a.DateTimeSubmitted.ToString() : string.Empty,
                                  DateCreated = a.CreatedDate.ToString("dd/MM/yyyy HH:mm"),
                                  Status = a.Status.ToString(),
                                  CrewStatus = a.IncidentStatus.ToString(),
                                  CallInStatus = a.StationStatus.ToString()
                              }).ToList();
                }
                else
                {
                    StationIds = context.NPF_SYS_Resource.Where(x => x.UserId == Uid && x.DeleteStatus == false && (x.ResourceType.Name == "Station" || x.ResourceType.Name == "Station Commander")).Select(y => y.StationUnitName.Id).ToList();
                    List<long> IncidentIds = new List<long>();
                    IncidentIds.AddRange(context.NPF_TRN_IncidentStation.Where(x => StationIds.Contains(x.StationId) && x.DeleteStatus == false).Select(y => y.IncidentId).ToList());
                    IncidentIds.AddRange(context.NPF_TRN_IncidentOtherStation.Where(x => StationIds.Contains(x.OtherStationId) && x.DeleteStatus == false).Select(y => y.IncidentId).ToList());
                    if (IncidentIds.Count > 0)
                    {
                        List<long> SortedIncidentIds = IncidentIds.GroupBy(x => x).Select(grp => grp.First()).ToList();
                        result = (from a in context.NPF_TRN_Incident
                                  where a.DeleteStatus == false
                                  && a.CreatedDate >= dt.Data.From && a.CreatedDate <= dt.Data.To
                                  && a.Status == DAL.Common.TransactionStatus.Submitted
                                  && SortedIncidentIds.Contains(a.Id)
                                  select new IncidentHandlingListDTO
                                  {
                                      Id = a.Id,
                                      Code = a.Code,
                                      IncidentNo = a.IncidentNo,
                                      ParentIncident = a.ParentIncidentId != null ? context.NPF_TRN_Incident.Where(x => x.Id == (long)(a.ParentIncidentId)).FirstOrDefault().IncidentNo : string.Empty,
                                      IncidentType = a.IncidentType != null ? context.NPF_SYS_IncidentType.Where(x => x.Id == (long)(a.IncidentType)).FirstOrDefault().Name : string.Empty,
                                      IncidentSubType = a.IncidentSubType != null ? context.NPF_SYS_IncidentSubType.Where(x => x.Id == (long)(a.IncidentSubType)).FirstOrDefault().Name : string.Empty,
                                      IncidentCode = a.IncidentCode != null ? context.NPF_SYS_IncidentCode.Where(x => x.Id == (long)(a.IncidentCode)).FirstOrDefault().Name : string.Empty,
                                      DateSubmitted = a.DateTimeSubmitted != null ? a.DateTimeSubmitted.ToString() : string.Empty,
                                      DateCreated = a.CreatedDate.ToString("dd/MM/yyyy HH:mm"),
                                      Status = a.Status.ToString(),
                                      CrewStatus = a.IncidentStatus.ToString(),
                                      CallInStatus = a.StationStatus.ToString()
                                  }).ToList();
                    }
                    else
                    {
                        dto.Data = result;
                        dto.QryResult = queryResult.SUCEEDED;
                        return dto;

                    }

                }

                foreach (IncidentHandlingListDTO res in result)
                {
                    NPF_TRN_IncidentLocation Loc = context.NPF_TRN_IncidentLocation.Where(x => x.IncidentId == res.Id).FirstOrDefault();
                    if (Loc != null)
                    {
                        res.Location = Loc.Address;
                    }

                    NPF_TRN_IncidentStation Sta = context.NPF_TRN_IncidentStation.Where(x => x.IncidentId == res.Id).FirstOrDefault();
                    if (Sta != null)
                    {
                        NPF_SYS_Station station = context.NPF_SYS_Station.Where(x => x.Id == Sta.StationId && x.DeleteStatus == false).FirstOrDefault();
                        if (station != null)
                        {
                            res.Station = station.Name;
                        }
                        NPF_SYS_NatureOfIncident nature = context.NPF_SYS_NatureOfIncident.Where(x => x.Id == Sta.NatureOfIncidentId && x.DeleteStatus == false).FirstOrDefault();
                        if (nature != null)
                        {
                            res.NatureOfIncident = nature.Name;
                        }
                    }

                }
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
                Dd.title = "Incident";
                Dd.items = new List<DropDownItem>();
                List<NPF_TRN_Incident> incident = context.NPF_TRN_Incident.Where(x => x.DeleteStatus == false && x.CreatedDate > DateTime.Now.AddDays(-7)).ToList();
                foreach (NPF_TRN_Incident st in incident)
                {
                    string code = st.Code;
                    string incidentNo = st.IncidentNo == string.Empty ? "No Incident Number" : st.IncidentNo;
                    string incidentCode = "No Incident Code";
                    if (st.IncidentCode != null)
                    {
                        NPF_SYS_IncidentCode IncCode = context.NPF_SYS_IncidentCode.Where(x => x.Id == st.IncidentCode).FirstOrDefault();
                        if (IncCode != null)
                        {
                            incidentCode = IncCode.Name;
                        }
                    }

                    string incidentSubType = "No Incident Sub Type";
                    if (st.IncidentSubType != null)
                    {
                        NPF_SYS_IncidentSubType IncSubType = context.NPF_SYS_IncidentSubType.Where(x => x.Id == st.IncidentSubType).FirstOrDefault();
                        if (IncSubType != null)
                        {
                            incidentSubType = IncSubType.Name;
                        }
                    }

                    DropDownItem Ddi = new DropDownItem();
                    Ddi.Id = st.Id;
                    Ddi.text = code + " - " + incidentNo + " - " + incidentCode + " - " + incidentSubType;
                    Dd.items.Add(Ddi);
                }
                Ddl.Add(Dd);

                Dd = new DropDown();
                Dd.title = "IncidentType";
                Dd.items = new List<DropDownItem>();
                List<NPF_SYS_IncidentType> IncidentType = context.NPF_SYS_IncidentType.Where(x => x.DeleteStatus == false).ToList();
                foreach (NPF_SYS_IncidentType st in IncidentType)
                {
                    DropDownItem Ddi = new DropDownItem();
                    Ddi.Id = st.Id;
                    Ddi.text = st.Name;
                    Dd.items.Add(Ddi);
                }
                Ddl.Add(Dd);

                Dd = new DropDown();
                Dd.title = "Locality";
                Dd.items = new List<DropDownItem>();
                List<NPF_SYS_LookUpValue> Locality = context.NPF_SYS_LookUpValue.Where(x => x.LookUpType.Name == "Locality" && x.DeleteStatus == false).ToList();
                foreach (NPF_SYS_LookUpValue st in Locality)
                {
                    DropDownItem Ddi = new DropDownItem();
                    Ddi.Id = st.Id;
                    Ddi.text = st.Name;
                    Dd.items.Add(Ddi);
                }
                Ddl.Add(Dd);

                Dd = new DropDown();
                Dd.title = "District";
                Dd.items = new List<DropDownItem>();
                List<NPF_SYS_LookUpValue> District = context.NPF_SYS_LookUpValue.Where(x => x.LookUpType.Name == "District" && x.DeleteStatus == false).ToList();
                foreach (NPF_SYS_LookUpValue st in District)
                {
                    DropDownItem Ddi = new DropDownItem();
                    Ddi.Id = st.Id;
                    Ddi.text = st.Name;
                    Dd.items.Add(Ddi);
                }
                Ddl.Add(Dd);

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

                Dd = new DropDown();
                Dd.title = "NatureOfIncident";
                Dd.items = new List<DropDownItem>();
                List<NPF_SYS_NatureOfIncident> NatureOfIncident = context.NPF_SYS_NatureOfIncident.Where(x => x.DeleteStatus == false).ToList();
                foreach (NPF_SYS_NatureOfIncident st in NatureOfIncident)
                {
                    DropDownItem Ddi = new DropDownItem();
                    Ddi.Id = st.Id;
                    Ddi.text = st.Name;
                    Dd.items.Add(Ddi);
                }
                Ddl.Add(Dd);

                Dd = new DropDown();
                Dd.title = "VehicleCategory";
                Dd.items = new List<DropDownItem>();
                List<NPF_SYS_LookUpValue> VehicleCategory = context.NPF_SYS_LookUpValue.Where(x => x.LookUpType.Name == "Vehicle Category" && x.DeleteStatus == false).ToList();
                foreach (NPF_SYS_LookUpValue st in VehicleCategory)
                {
                    DropDownItem Ddi = new DropDownItem();
                    Ddi.Id = st.Id;
                    Ddi.text = st.Name;
                    Dd.items.Add(Ddi);
                }
                Ddl.Add(Dd);

                Dd = new DropDown();
                Dd.title = "EquipmentCategory";
                Dd.items = new List<DropDownItem>();
                List<NPF_SYS_LookUpValue> EquipmentCategory = context.NPF_SYS_LookUpValue.Where(x => x.LookUpType.Name == "Equipment Category" && x.DeleteStatus == false).ToList();
                foreach (NPF_SYS_LookUpValue st in EquipmentCategory)
                {
                    DropDownItem Ddi = new DropDownItem();
                    Ddi.Id = st.Id;
                    Ddi.text = st.Name;
                    Dd.items.Add(Ddi);
                }
                Ddl.Add(Dd);

                Dd = new DropDown();
                Dd.title = "ResourceCategory";
                Dd.items = new List<DropDownItem>();
                List<NPF_SYS_ResourceType> ResourceCategory = context.NPF_SYS_ResourceType.Where(x => x.DeleteStatus == false).ToList();
                foreach (NPF_SYS_ResourceType st in ResourceCategory)
                {
                    DropDownItem Ddi = new DropDownItem();
                    Ddi.Id = st.Id;
                    Ddi.text = st.Name;
                    Dd.items.Add(Ddi);
                }
                Ddl.Add(Dd);

                Dd = new DropDown();
                Dd.title = "ResourceRank";
                Dd.items = new List<DropDownItem>();
                List<NPF_SYS_Rank> ResourceRank = context.NPF_SYS_Rank.Where(x => x.DeleteStatus == false).ToList();
                foreach (NPF_SYS_Rank st in ResourceRank)
                {
                    DropDownItem Ddi = new DropDownItem();
                    Ddi.Id = st.Id;
                    Ddi.text = st.Name;
                    Dd.items.Add(Ddi);
                }
                Ddl.Add(Dd);

                Dd = new DropDown();
                Dd.title = "ExternalAgency";
                Dd.items = new List<DropDownItem>();
                List<NPF_SYS_ExternalAgencies> ExternalAgency = context.NPF_SYS_ExternalAgencies.Where(x => x.DeleteStatus == false).ToList();
                foreach (NPF_SYS_ExternalAgencies st in ExternalAgency)
                {
                    DropDownItem Ddi = new DropDownItem();
                    Ddi.Id = st.Id;
                    Ddi.text = st.Name;
                    Dd.items.Add(Ddi);
                }
                Ddl.Add(Dd);

                Dd = new DropDown();
                Dd.title = "Casualty";
                Dd.items = new List<DropDownItem>();
                List<NPF_SYS_LookUpValue> Casualty = context.NPF_SYS_LookUpValue.Where(x => x.LookUpType.Name == "Casualty Type" && x.DeleteStatus == false).ToList();
                foreach (NPF_SYS_LookUpValue st in Casualty)
                {
                    DropDownItem Ddi = new DropDownItem();
                    Ddi.Id = st.Id;
                    Ddi.text = st.Name;
                    Dd.items.Add(Ddi);
                }
                Ddl.Add(Dd);

                Dd = new DropDown();
                Dd.title = "Fatality";
                Dd.items = new List<DropDownItem>();
                List<NPF_SYS_LookUpValue> Fatality = context.NPF_SYS_LookUpValue.Where(x => x.LookUpType.Name == "Fatality Type" && x.DeleteStatus == false).ToList();
                foreach (NPF_SYS_LookUpValue st in Fatality)
                {
                    DropDownItem Ddi = new DropDownItem();
                    Ddi.Id = st.Id;
                    Ddi.text = st.Name;
                    Dd.items.Add(Ddi);
                }
                Ddl.Add(Dd);

                Dd = new DropDown();
                Dd.title = "Gender";
                Dd.items = new List<DropDownItem>();
                List<NPF_SYS_LookUpValue> Gender = context.NPF_SYS_LookUpValue.Where(x => x.LookUpType.Name == "Gender" && x.DeleteStatus == false).ToList();
                foreach (NPF_SYS_LookUpValue st in Gender)
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
        public BaseResponseDTO<List<DropDown>> GetCallhandlingIncidentSubType(DropdownForm drpDto)
        {
            BaseResponseDTO<List<DropDown>> dto = new BaseResponseDTO<List<DropDown>>();
            List<DropDown> Ddl = new List<DropDown>();
            DropDown Dd = new DropDown();
            string errorMsg = "No Data Found";
            try
            {
                //Asset
                Dd = new DropDown();
                Dd.title = "IncidentSubType";
                Dd.items = new List<DropDownItem>();
                List<NPF_SYS_IncidentSubType> IncidentSubType = new List<NPF_SYS_IncidentSubType>();
                if (drpDto != null)
                {
                    if (drpDto.param != null)
                    {
                        if (drpDto.term == null)
                        {
                            IncidentSubType = context.NPF_SYS_IncidentSubType.Where(x => x.DeleteStatus == false && x.IncidentType.Id == drpDto.param).OrderBy(x => x.Name).Take(10).ToList();
                        }
                        else
                        {
                            IncidentSubType = context.NPF_SYS_IncidentSubType.Where(x => x.DeleteStatus == false && x.IncidentType.Id == drpDto.param && (x.Name.ToLower().Contains(drpDto.term.ToLower()))).OrderBy(x => x.Name).Take(10).ToList();
                        }

                    }
                    else
                    {
                        if (drpDto.term == null)
                        {
                            IncidentSubType = context.NPF_SYS_IncidentSubType.Where(x => x.DeleteStatus == false).OrderBy(x => x.Name).Take(10).ToList();
                        }
                        else
                        {
                            IncidentSubType = context.NPF_SYS_IncidentSubType.Where(x => x.DeleteStatus == false && (x.Name.ToLower().Contains(drpDto.term.ToLower()))).OrderBy(x => x.Name).Take(10).ToList();
                        }

                    }

                    foreach (NPF_SYS_IncidentSubType st in IncidentSubType)
                    {
                        DropDownItem Ddi = new DropDownItem();
                        Ddi.Id = st.Id;
                        Ddi.text = st.Name;
                        Dd.items.Add(Ddi);
                    }
                    Ddl.Add(Dd);
                }
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
        public BaseResponseDTO<List<DropDown>> GetCallhandlingIncidentCode(DropdownForm drpDto)
        {
            BaseResponseDTO<List<DropDown>> dto = new BaseResponseDTO<List<DropDown>>();
            List<DropDown> Ddl = new List<DropDown>();
            DropDown Dd = new DropDown();
            string errorMsg = "No Data Found";
            try
            {
                //Asset
                Dd = new DropDown();
                Dd.title = "IncidentCode";
                Dd.items = new List<DropDownItem>();
                List<NPF_SYS_IncidentCode> IncidentCode = new List<NPF_SYS_IncidentCode>();
                if (drpDto != null)
                {
                    if (drpDto.param != null)
                    {
                        if (drpDto.term != null)
                        {
                            IncidentCode = context.NPF_SYS_IncidentCode.Where(x => x.DeleteStatus == false && x.IncidentSubType.Id == drpDto.param && (x.Name.ToLower().Contains(drpDto.term.ToLower()))).OrderBy(x => x.Name).Take(10).ToList();
                        }
                        else
                        {
                            IncidentCode = context.NPF_SYS_IncidentCode.Where(x => x.DeleteStatus == false && x.IncidentSubType.Id == drpDto.param).OrderBy(x => x.Name).Take(10).ToList();
                        }

                    }
                    else
                    {
                        if (drpDto.term != null)
                        {
                            IncidentCode = context.NPF_SYS_IncidentCode.Where(x => x.DeleteStatus == false && (x.Name.ToLower().Contains(drpDto.term.ToLower()))).OrderBy(x => x.Name).Take(10).ToList();
                        }
                        else
                        {
                            IncidentCode = context.NPF_SYS_IncidentCode.Where(x => x.DeleteStatus == false).OrderBy(x => x.Name).Take(10).ToList();
                        }

                    }

                    foreach (NPF_SYS_IncidentCode st in IncidentCode)
                    {
                        DropDownItem Ddi = new DropDownItem();
                        Ddi.Id = st.Id;
                        Ddi.text = st.Name;
                        Dd.items.Add(Ddi);
                    }
                    Ddl.Add(Dd);
                }


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
        public BaseResponseDTO<IncidentHandlingDTO> GetById(long? Id, string UserToken)
        {

            BaseResponseDTO<IncidentHandlingDTO> dto = new BaseResponseDTO<IncidentHandlingDTO>();
            IncidentHandlingDTO result = new IncidentHandlingDTO();
            string errorMsg = "No Data Found";
            try
            {
                NPF_TRN_Incident Incident = context.NPF_TRN_Incident.Where(x => x.Id == Id && x.DeleteStatus == false).FirstOrDefault();
                if (Incident != null)
                {
                    result.Status = Incident.Status;
                    result.Incident = Incident;
                    if (Incident.IncidentSubType != null)
                    {
                        NPF_SYS_IncidentSubType IncidentSubType = context.NPF_SYS_IncidentSubType.Where(x => x.Id == Incident.IncidentSubType).FirstOrDefault();
                        if (IncidentSubType != null)
                        {
                            result.SelectedIncidentSubType = IncidentSubType.Name;
                        }

                    }
                    if (Incident.IncidentCode != null)
                    {
                        NPF_SYS_IncidentCode IncidentCode = context.NPF_SYS_IncidentCode.Where(x => x.Id == Incident.IncidentCode).FirstOrDefault();
                        if (IncidentCode != null)
                        {
                            result.SelectedIncidentCode = IncidentCode.Name;
                        }

                    }
                    NPF_TRN_IncidentCaller caller = context.NPF_TRN_IncidentCaller.Where(x => x.IncidentId == Id && x.DeleteStatus == false).FirstOrDefault();
                    if (caller != null)
                    {
                        result.IncidentCaller = caller;
                    }
                    NPF_TRN_IncidentLocation location = context.NPF_TRN_IncidentLocation.Where(x => x.IncidentId == Id && x.DeleteStatus == false).FirstOrDefault();
                    if (location != null)
                    {
                        result.IncidentLocation = location;
                    }
                    NPF_TRN_IncidentStation station = context.NPF_TRN_IncidentStation.Where(x => x.IncidentId == Id && x.DeleteStatus == false).FirstOrDefault();
                    if (station != null)
                    {
                        result.IncidentStation = station;
                    }
                    NPF_TRN_IncidentFatality fatality = context.NPF_TRN_IncidentFatality.Where(x => x.IncidentId == Id && x.DeleteStatus == false).FirstOrDefault();
                    if (fatality != null)
                    {
                        result.IncidentFatality = fatality;
                    }
                    NPF_TRN_IncidentCasualty casualty = context.NPF_TRN_IncidentCasualty.Where(x => x.IncidentId == Id && x.DeleteStatus == false).FirstOrDefault();
                    if (casualty != null)
                    {
                        result.IncidentCasualty = casualty;
                    }
                }
                if (Incident.DateTimeAcknowledge == null && Incident.DateTimeSubmitted != null)
                {
                    long StationId = context.NPF_TRN_IncidentStation.Where(x => x.IncidentId == Incident.Id && x.DeleteStatus == false).Select(y => y.StationId).FirstOrDefault();

                    long Uid = context.NPF_SYS_User.Where(x => x.AId == UserToken).FirstOrDefault().Id;
                    List<long> StationIds = context.NPF_SYS_Resource.Where(x => x.UserId == Uid && x.DeleteStatus == false && (x.ResourceType.Name == "Station" || x.ResourceType.Name == "Station Commander")).Select(y => y.StationUnitName.Id).ToList();
                    if (StationIds.Contains(StationId))
                    {
                        result.HasAckowledge = true;
                    }
                }
                dto.Data = result;
                dto.QryResult = queryResult.SUCEEDED;
            }
            catch (Exception e)
            {
                dto.Data = result;
                dto.AdditionalData = errorMsg;
                dto.QryResult = queryResult.FAILED;
            }


            return dto;
        }
        public async Task<BaseResponseDTO<IncidentHandlingGetDetailsDTO>> GetByIdDetails(long? Id)
        {
            BaseResponseDTO<IncidentHandlingGetDetailsDTO> dto = new BaseResponseDTO<IncidentHandlingGetDetailsDTO>();
            IncidentHandlingGetDetailsDTO result = new IncidentHandlingGetDetailsDTO();
            string errorMsg = "No Data Found";
            try
            {
                NPF_TRN_Incident Incident = context.NPF_TRN_Incident.Where(x => x.Id == Id).FirstOrDefault();
                if (Incident != null)
                {
                    result.IncidentVehicle = (from a in context.NPF_TRN_IncidentVehicle
                                              join b in context.NPF_SYS_Vehicle on a.VehicleId equals b.Id
                                              where a.IncidentId == Id && a.DeleteStatus == false
                                              select new IncidentVehicle
                                              {
                                                  Id = a.Id,
                                                  IncidentId = a.IncidentId,
                                                  VehicleId = a.VehicleId,
                                                  Name = b.RegistrationNo,
                                                  StationId = a.StationId,
                                                  StationName = a.StationName
                                                 
                                              }).ToList();

                    result.IncidentEquipment = (from a in context.NPF_TRN_IncidentEquipment
                                                join b in context.NPF_SYS_Equipment on a.EquipmentId equals b.Id
                                                where a.IncidentId == Id && a.DeleteStatus == false
                                                select new IncidentEquipment
                                                {
                                                    Id = a.Id,
                                                    IncidentId = a.IncidentId,
                                                    EquipmentId = a.EquipmentId,
                                                    Name = b.Name,
                                                    StationId = a.StationId,
                                                    StationName = a.StationName

                                                }).ToList();

                    result.IncidentDamage = (from a in context.NPF_TRN_IncidentDamage
                                             where a.IncidentId == Id && a.DeleteStatus == false
                                             select new IncidentDamage
                                             {
                                                 Id = a.Id,
                                                 IncidentId = a.IncidentId,
                                                 Details = a.Details,
                                                 EstimatedValue = a.EstimatedValue,
                                                 Cost = a.Cost,
                                                 Salvage = a.Salvage

                                             }).ToList();

                    result.IncidentExternalAgency = (from a in context.NPF_TRN_IncidentExternalAgency
                                                     join b in context.NPF_SYS_ExternalAgencies on a.ExternalAgencyId equals b.Id
                                                     where a.IncidentId == Id && a.DeleteStatus == false
                                                     select new IncidentExternalAgency
                                                     {
                                                         Id = a.Id,
                                                         IncidentId = a.IncidentId,
                                                         ExternalAgencyId = a.ExternalAgencyId,
                                                         ContactNumber = b.ContactNumber,
                                                         Name = b.Name,
                                                         Remarks = a.Remarks

                                                     }).ToList();

                    result.IncidentResource = (from a in context.NPF_TRN_IncidentResource
                                               join b in context.NPF_SYS_Resource on a.ResourceId equals b.Id
                                               where a.IncidentId == Id && a.DeleteStatus == false
                                               select new IncidentResource
                                               {
                                                   Id = a.Id,
                                                   IncidentId = a.IncidentId,
                                                   ResourceId = a.ResourceId,
                                                   Name = b.NAA.FullNameM,
                                                   IsCrewCommander = a.IsCrewCommander,
                                                   Order = b.Rank.Order,
                                                   StationId = a.StationId,
                                                   StationName = a.StationName
                                               }).OrderBy(y => y.Order).ToList();

                    result.IncidentOtherStation = (from a in context.NPF_TRN_IncidentOtherStation
                                                   join b in context.NPF_SYS_Station on a.OtherStationId equals b.Id
                                                   where a.IncidentId == Id && a.DeleteStatus == false
                                                   select new IncidentOtherStation
                                                   {
                                                       Id = a.Id,
                                                       IncidentId = a.IncidentId,
                                                       OtherStationId = a.OtherStationId,
                                                       Name = b.Name
                                                   }).ToList();

                    result.IncidentCasualtyDetails = (from a in context.NPF_TRN_IncidentCasualtyDetails
                                                      join b in context.NPF_SYS_LookUpValue on a.CasualtyType equals b.Id
                                                      join c in context.NPF_SYS_LookUpValue on a.Gender equals c.Id
                                                      where a.IncidentId == Id && a.DeleteStatus == false
                                                      select new IncidentCasualtyDetails
                                                      {
                                                          Id = a.Id,
                                                          IncidentId = a.IncidentId,
                                                          Name = a.Name,
                                                          Address = a.Address,
                                                          Age = a.Age,
                                                          Remarks = a.Remarks,
                                                          Gender = a.Gender,
                                                          CasualtyType = a.CasualtyType,
                                                          GenderName = c.Name,
                                                          CasualtyTypeName = b.Name
                                                      }).ToList();

                    result.IncidentFatalityDetails = (from a in context.NPF_TRN_IncidentFatalityDetails
                                                      join b in context.NPF_SYS_LookUpValue on a.FatalityType equals b.Id
                                                      join c in context.NPF_SYS_LookUpValue on a.Gender equals c.Id
                                                      where a.IncidentId == Id && a.DeleteStatus == false
                                                      select new IncidentFatalityDetails
                                                      {
                                                          Id = a.Id,
                                                          IncidentId = a.IncidentId,
                                                          Name = a.Name,
                                                          Address = a.Address,
                                                          Age = a.Age,
                                                          Remarks = a.Remarks,
                                                          Gender = a.Gender,
                                                          FatalityType = a.FatalityType,
                                                          GenderName = c.Name,
                                                          FatalityTypeName = b.Name
                                                      }).ToList();
                }
                dto.Data = result;
                dto.QryResult = queryResult.SUCEEDED;
            }
            catch (Exception e)
            {
                dto.Data = result;
                dto.AdditionalData = errorMsg;
                dto.QryResult = queryResult.FAILED;
            }


            return dto;
        }
        public async Task<BaseResponseDTO<IncidentGetRefreshedListDTO>> GetByIdRefreshed(BaseRequestDTO<long> Id)
        {
            BaseResponseDTO<IncidentGetRefreshedListDTO> dto = new BaseResponseDTO<IncidentGetRefreshedListDTO>();
            IncidentGetRefreshedListDTO result = new IncidentGetRefreshedListDTO();
            string errorMsg = "No Data Found";
            try
            {
                result.FilesList = (from a in context.NPF_TRN_IncidentFiles
                                    where a.IncidentId == Id.Data && a.DeleteStatus == false
                                    select new IncidentFilesListDTO
                                    {
                                        Id = a.Id,
                                        FileName = a.FileName
                                    }).ToList();
                dto.Data = result;
                dto.QryResult = queryResult.SUCEEDED;
            }
            catch (Exception e)
            {
                dto.Data = result;
                dto.QryResult = queryResult.FAILED;
            }
            return dto;
        }
        public async Task<BaseResponseDTO<IncidentHandlingDTO>> Initialize(string userToken)
        {
            BaseResponseDTO<IncidentHandlingDTO> BaseDto = new BaseResponseDTO<IncidentHandlingDTO>();
            BaseDto.Data = new IncidentHandlingDTO();
            NPF_TRN_Incident data = new NPF_TRN_Incident();
            BaseResponseDTO<string> BaseDtoS = new BaseResponseDTO<string>();

            try
            {
                BaseDtoS = await new GenerateCode().GCodeMain(userToken, "NPF_TRN_Incident");
                data.Code = BaseDtoS.Data;
                data.Status = DAL.Common.TransactionStatus.Open;
                data.StationStatus = DAL.Common.StationStatus.Open;
                context.NPF_TRN_Incident.Add(data);
                context.SaveChanges();
                BaseDto.Id = data.Id;
                BaseDto.AdditionalData = data.Code;
                BaseDto.QryResult = queryResult.SUCEEDED;
            }
            catch (Exception)
            {
                BaseDto.AdditionalData = "Error Adding";
                BaseDto.QryResult = queryResult.FAILED;
            }
            return BaseDto;
        }
        public async Task<BaseResponseDTO<bool>> Save(IncidentHandlingDTO data, string userToken)
        {
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
            BaseResponseDTO<string> BaseDtoS = new BaseResponseDTO<string>();
            long IncidentId = 0;
            string IncidentField = string.Empty;
            string IncidentFieldId = string.Empty;
            try
            {
                if (data.Id != 0)
                {
                    IncidentId = data.Id;
                }
                if (data.Incident != null)
                {
                    var incidentDet = context.NPF_TRN_Incident.AsNoTracking().FirstOrDefault(x => x.Id == data.Incident.Id);
                    if (incidentDet != null && incidentDet.Id == data.Incident.Id && incidentDet.Status == TransactionStatus.Submitted)
                    {
                        data.Incident.Status = TransactionStatus.Submitted;
                        data.Incident.Code = incidentDet.Code;
                    }
                    IncidentField = "Incident General Detail";
                    IncidentFieldId = "incident-info";
                    context.NPF_TRN_Incident.Update(data.Incident);
                    context.SaveChanges();
                    IncidentId = data.Incident.Id;
                }
                if (data.IncidentCaller != null)
                {
                    IncidentField = "Caller Detail";
                    IncidentFieldId = "caller";
                    if (data.IncidentCaller.Id == 0)
                    {
                        context.NPF_TRN_IncidentCaller.Add(data.IncidentCaller);
                        context.SaveChanges();
                        BaseDto.Id = data.IncidentCaller.Id;
                    }
                    else
                    {
                        context.NPF_TRN_IncidentCaller.Update(data.IncidentCaller);
                        context.SaveChanges();
                    }
                    IncidentId = data.IncidentCaller.IncidentId;
                }
                if (data.IncidentLocation != null)
                {
                    IncidentField = "Location Detail";
                    IncidentFieldId = "location";
                    if (data.IncidentLocation.Id == 0)
                    {
                        context.NPF_TRN_IncidentLocation.Add(data.IncidentLocation);
                        context.SaveChanges();
                        BaseDto.Id = data.IncidentLocation.Id;
                    }
                    else
                    {
                        context.NPF_TRN_IncidentLocation.Update(data.IncidentLocation);
                        context.SaveChanges();
                    }
                    IncidentId = data.IncidentLocation.IncidentId;
                }
                if (data.IncidentFatality != null)
                {
                    IncidentField = "Fatality Detail";
                    IncidentFieldId = "fatality";
                    if (data.IncidentFatality.Id == 0)
                    {
                        context.NPF_TRN_IncidentFatality.Add(data.IncidentFatality);
                        context.SaveChanges();
                        BaseDto.Id = data.IncidentFatality.Id;
                    }
                    else
                    {
                        context.NPF_TRN_IncidentFatality.Update(data.IncidentFatality);
                        context.SaveChanges();
                    }
                    IncidentId = data.IncidentFatality.IncidentId;
                }
                if (data.IncidentStation != null)
                {
                    IncidentField = "Station Detail";
                    IncidentFieldId = "station";
                    if (data.IncidentStation.Id == 0)
                    {
                        context.NPF_TRN_IncidentStation.Add(data.IncidentStation);
                        context.SaveChanges();
                        BaseDto.Id = data.IncidentStation.Id;
                    }
                    else
                    {
                        context.NPF_TRN_IncidentStation.Update(data.IncidentStation);
                        context.SaveChanges();
                    }
                    IncidentId = data.IncidentStation.IncidentId;
                }
                if (data.IncidentCasualty != null)
                {
                    IncidentField = "Casualty Detail";
                    IncidentFieldId = "casualty";
                    if (data.IncidentCasualty.Id == 0)
                    {
                        context.NPF_TRN_IncidentCasualty.Add(data.IncidentCasualty);
                        context.SaveChanges();
                        BaseDto.Id = data.IncidentCasualty.Id;
                    }
                    else
                    {
                        context.NPF_TRN_IncidentCasualty.Update(data.IncidentCasualty);
                        context.SaveChanges();
                    }
                    IncidentId = data.IncidentCasualty.IncidentId;
                }
                if (IncidentId != 0)
                {
                    NPF_TRN_Incident Incident = context.NPF_TRN_Incident.Where(x => x.Id == IncidentId).FirstOrDefault();
                    NPF_TRN_IncidentStation Station = context.NPF_TRN_IncidentStation.Where(x => x.IncidentId == IncidentId && x.DeleteStatus == false).FirstOrDefault();
                    if (data.Status == DAL.Common.TransactionStatus.Submitted)
                    {
                        if (Incident.Status != DAL.Common.TransactionStatus.Submitted)
                        {
                            int AlertTimer = Convert.ToInt32(context.NPF_SYS_GlobalParam.Where(x => x.Name == "AlertTimer").FirstOrDefault().Value);
                            string StationCode = context.NPF_SYS_Station.Where(x => x.Id == Station.StationId).FirstOrDefault().StationCallSign;
                            Incident.StationStatus = StationStatus.Pending;
                            Incident.Timer = DateTime.Now.AddMinutes(AlertTimer);
                            BaseDtoS = await new GenerateCode().GCodeIncident(userToken, "NPF_TRN_IncidentNumber");
                            Incident.IncidentNo = BaseDtoS.Data + StationCode;
                            Incident.Status = DAL.Common.TransactionStatus.Submitted;
                            Incident.DateTimeSubmitted = DateTime.Now;
                            context.NPF_TRN_Incident.Update(Incident);
                            context.SaveChanges();

                            List<long> StationIds = new List<long>();
                            Notification notif = new Notification()
                            {
                                Id = Incident.Id,
                                UserId = 0,
                                MType = MessageType.Assigned,
                                MName = PageName.Incident,
                                TaskName = "Incident No:" + Incident.IncidentNo,
                                URL = ""

                            };
                            StationIds.Add(context.NPF_TRN_IncidentStation.Where(x => x.IncidentId == Incident.Id && x.DeleteStatus == false).FirstOrDefault().StationId);
                            StationIds.AddRange(context.NPF_TRN_IncidentOtherStation.Where(x => x.IncidentId == Incident.Id && x.DeleteStatus == false).Select(y => y.OtherStationId).ToList());
                            List<long> UsersIds = new List<long>();
                            List<string> stalist = new List<string>();
                            stalist.Add("Station Commander");
                            stalist.Add("Station");
                            long UID = context.NPF_SYS_User.Where(x => x.AId == userToken).FirstOrDefault().Id;
                            long Cuid = context.NPF_SYS_User.Where(x => x.AId == Incident.CreatedBy.ToString()).FirstOrDefault().Id;
                            UsersIds = context.NPF_SYS_Resource.Where(x => x.UserId != null && StationIds.Contains(x.StationUnitName.Id) && stalist.Contains(x.ResourceType.Name)).Select(x => Convert.ToInt64(x.UserId)).ToList();
                            if (UsersIds.Any(x => x != Cuid))
                            {
                                UsersIds.Add(Cuid);
                            }
                            if (UsersIds.Any(x => x == UID))
                            {
                                UsersIds.RemoveAll(x => x == UID);
                            }
                            UsersIds = UsersIds.Distinct().ToList();
                            foreach (long user in UsersIds)
                            {
                                if (user > 0)
                                {
                                    string Auid = context.NPF_SYS_User.Where(x => x.Id == user).FirstOrDefault().AId;
                                    notif.UserId = user;
                                    notif.URL = "/IncidentHandling/AddEdit?Id=" + Incident.Id + "&UserToken=" + Auid + "#";
                                    new NotificationService().SendNotification(notif);

                                }

                            }

                        }


                    }
                    else if (Incident.Status == DAL.Common.TransactionStatus.Open)
                    {
                        Incident.Status = DAL.Common.TransactionStatus.Draft;
                        context.NPF_TRN_Incident.Update(Incident);
                        context.SaveChanges();
                    }
                    else
                    {
                        if (Incident.Status != DAL.Common.TransactionStatus.Draft && Incident.Status != DAL.Common.TransactionStatus.Open)
                        {
                            try
                            {
                                if (IncidentField != string.Empty)
                                {
                                    SendNotificationOnUpdate(Incident, IncidentField, userToken, IncidentFieldId);
                                }
                            }
                            catch (Exception e)
                            {

                            }
                        }
                    }

                }
                BaseDto.Data = true;
                BaseDto.QryResult = queryResult.SUCEEDED;
            }
            catch (Exception)
            {
                BaseDto.Data = false;
                BaseDto.AdditionalData = "Error Adding";
                BaseDto.QryResult = queryResult.FAILED;
            }
            return BaseDto;
        }

        public async Task<bool> SendNotificationOnUpdate(NPF_TRN_Incident Incident, string FieldText,string UserToken, string IdFIeld)
        {
            try
            {
                long UID = context.NPF_SYS_User.Where(x => x.AId == UserToken).FirstOrDefault().Id;
                if (FieldText != string.Empty)
                {
                    List<long> StationIds = new List<long>();
                    Notification notif = new Notification()
                    {
                        Id = Incident.Id,
                        UserId = 0,
                        MType = MessageType.Changes,
                        MName = PageName.Incident,
                        TaskName = "Incident No:" + Incident.IncidentNo + " " + FieldText + " Has Been Updated.",
                        URL = ""
                    };
                    StationIds.Add(context.NPF_TRN_IncidentStation.Where(x => x.IncidentId == Incident.Id && x.DeleteStatus == false).FirstOrDefault().StationId);
                    StationIds.AddRange(context.NPF_TRN_IncidentOtherStation.Where(x => x.IncidentId == Incident.Id && x.DeleteStatus == false).Select(y => y.OtherStationId).ToList());
                    List<long> UsersIds = new List<long>();
                    List<string> stalist = new List<string>();
                    stalist.Add("Station Commander");
                    stalist.Add("Station");
                    long Cuid = context.NPF_SYS_User.Where(x => x.AId == Incident.CreatedBy.ToString()).FirstOrDefault().Id;
                    UsersIds = context.NPF_SYS_Resource.Where(x => x.UserId != null && StationIds.Contains(x.StationUnitName.Id) && stalist.Contains(x.ResourceType.Name)).Select(x => Convert.ToInt64(x.UserId)).ToList();
                    if (UsersIds.Any(x => x != Cuid))
                    {
                        UsersIds.Add(Cuid);
                    }
                    if (UsersIds.Any(x => x == UID))
                    {
                        UsersIds.RemoveAll(x => x == UID);
                    }
                    UsersIds = UsersIds.Distinct().ToList();
                    foreach (long user in UsersIds)
                    {
                        if (user > 0)
                        {
                            string Auid = context.NPF_SYS_User.Where(x => x.Id == user).FirstOrDefault().AId;
                            notif.UserId = user;
                            notif.URL = "/IncidentHandling/AddEdit?Id=" + Incident.Id + "&UserToken=" + Auid + "&Field=" + IdFIeld + "#scrollspy-" + IdFIeld;
                            new NotificationService().SendNotification(notif);

                        }

                    }
                    List<long> MobUsersIds = new List<long>();
                    MobUsersIds = context.NPF_TRN_IncidentAssignment.Where(x => x.IncidentId == Incident.Id).Select(y => y.ResourceId).ToList();
                    UsersIds = new List<long>();
                    UsersIds.AddRange(context.NPF_SYS_Resource.Where(x => x.UserId != null && MobUsersIds.Contains(x.Id)).Select(x => Convert.ToInt64(x.UserId)).ToList());
                    if (UsersIds.Any(x => x == UID))
                    {
                        UsersIds.RemoveAll(x => x == UID);
                    }
                    UsersIds = UsersIds.Distinct().ToList();
                    foreach (long user in UsersIds)
                    {
                        if (user > 0)
                        {
                            PushNotif pn = new PushNotif()
                            {
                                Id = Incident.Id,
                                Type = NotifType.Push,
                                ResId = user,
                                MType = MessageType.Changes,
                                Text = Incident.IncidentNo,
                                Text2 = FieldText
                            };
                            try
                            {
                                bool x = await Nhs.Notification(pn);
                            }
                            catch (Exception ex)
                            {

                            }
                        }

                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<BaseResponseDTO<bool>> Acknowledge(BaseRequestDTO<long> dto)
        {
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
            try
            {
                NPF_TRN_Incident dt = context.NPF_TRN_Incident.Where(x => x.Id == dto.Data).FirstOrDefault();

                dt.DateTimeAcknowledge = DateTime.Now;
                dt.StationStatus = StationStatus.Acknowledged;
                context.NPF_TRN_Incident.Update(dt);
                context.SaveChanges();
                BaseDto.Data = true;
                BaseDto.QryResult = queryResult.SUCEEDED;

                try
                {

                    Notification notif = new Notification()
                    {
                        Id = dt.Id,
                        UserId = context.NPF_SYS_User.Where(x => x.AId == dt.CreatedBy.ToString()).FirstOrDefault().Id,
                        MType = MessageType.Ack,
                        MName = PageName.Incident,
                        TaskName = "Incident No:" + dt.IncidentNo + " Acknowledged.",
                        URL = "/IncidentHandling/AddEdit?Id=" + dt.Id + "&UserToken=" + dt.CreatedBy.ToString() + "#",
                        PopUp = true
                    };
                    new NotificationService().SendNotification(notif);

                }
                catch (Exception e)
                {

                }

            }
            catch (Exception)
            {
                BaseDto.Data = false;
                BaseDto.AdditionalData = "Error";
                BaseDto.QryResult = queryResult.FAILED;
            }
            return BaseDto;
        }
        public async Task<BaseResponseDTO<bool>> AbortCompleted(BaseRequestDTO<AbortCompletedDTO> dto)
        {
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
            try
            {
                NPF_TRN_Incident Incident = context.NPF_TRN_Incident.Where(x => x.Id == dto.Data.IncidentId).FirstOrDefault();
                if (dto.Data.Status == TransactionStatus.Abort)
                {
                    Incident.DateTimeIncidentAbort = DateTime.Now;
                    Incident.Status = TransactionStatus.Abort;

                }
                else
                {
                    Incident.DateTimeIncidentCompleted = DateTime.Now;
                    Incident.Status = TransactionStatus.Completed;
                }
                if ((int)Incident.IncidentStatus < 6)
                {
                    Incident.IncidentStatus = IncidentStatus.Leavesite;
                }
                context.NPF_TRN_Incident.Update(Incident);
                context.SaveChanges();
                try
                {
                    List<long> StationIds = new List<long>();
                    Notification notif = new Notification()
                    {
                        Id = Incident.Id,
                        UserId = 0,
                        MType = MessageType.Status,
                        MName = PageName.Incident,
                        TaskName = "Incident No:" + Incident.IncidentNo,
                        URL = ""

                    };
                    StationIds.Add(context.NPF_TRN_IncidentStation.Where(x => x.IncidentId == Incident.Id && x.DeleteStatus == false).FirstOrDefault().StationId);
                    StationIds.AddRange(context.NPF_TRN_IncidentOtherStation.Where(x => x.IncidentId == Incident.Id && x.DeleteStatus == false).Select(y => y.OtherStationId).ToList());
                    List<long> UsersIds = new List<long>();
                    List<string> stalist = new List<string>();
                    stalist.Add("Station Commander");
                    stalist.Add("Station");
                    long UID = context.NPF_SYS_User.Where(x => x.AId == dto.userToken).FirstOrDefault().Id;
                    long Cuid = context.NPF_SYS_User.Where(x => x.AId == Incident.CreatedBy.ToString()).FirstOrDefault().Id;
                    UsersIds = context.NPF_SYS_Resource.Where(x => x.UserId != null && StationIds.Contains(x.StationUnitName.Id) && stalist.Contains(x.ResourceType.Name)).Select(x => Convert.ToInt64(x.UserId)).ToList();
                    if (UsersIds.Any(x => x != Cuid))
                    {
                        UsersIds.Add(Cuid);
                    }
                    if (UsersIds.Any(x => x == UID))
                    {
                        UsersIds.RemoveAll(x => x == UID);
                    }
                    UsersIds = UsersIds.Distinct().ToList();
                    foreach (long user in UsersIds)
                    {
                        if (user > 0)
                        {
                            string Auid = context.NPF_SYS_User.Where(x => x.Id == user).FirstOrDefault().AId;
                            notif.UserId = user;
                            notif.URL = "/IncidentHandling/AddEdit?Id=" + Incident.Id + "&UserToken=" + Auid + "#";
                            PushNotif pn = new PushNotif()
                            {
                                Id = Incident.Id,
                                Type = NotifType.Push,
                                ResId = user,
                                MType = MessageType.Status
                            };
                            try
                            {
                                bool x = await Nhs.Notification(pn);
                            }
                            catch (Exception ex)
                            {

                            }
                            new NotificationService().SendNotification(notif);

                        }

                    }
                }
                catch (Exception e)
                {

                }
                BaseDto.Data = true;
                BaseDto.QryResult = queryResult.SUCEEDED;



            }
            catch (Exception)
            {
                BaseDto.Data = false;
                BaseDto.AdditionalData = "Error";
                BaseDto.QryResult = queryResult.FAILED;
            }
            return BaseDto;
        }
        public BaseResponseDTO<bool> Delete(long dataToDelId)
        {
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
            try
            {
                NPF_TRN_Incident toDel = context.NPF_TRN_Incident.Where(x => x.Id == dataToDelId).FirstOrDefault();
                if (toDel != null)
                {
                    toDel.DeleteStatus = true;
                    context.NPF_TRN_Incident.Update(toDel);
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
        public BaseResponseDTO<bool> AddDeleteVehicleToTable(NPF_TRN_IncidentVehicle data, string UserToken)
        {
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
            try
            {
                if (data.Id == 0)
                {
                    context.NPF_TRN_IncidentVehicle.Add(data);
                    context.SaveChanges();
                    BaseDto.Id = data.Id;
                }
                else
                {
                    NPF_TRN_IncidentVehicle dt = context.NPF_TRN_IncidentVehicle.Where(x => x.Id == data.Id).FirstOrDefault();
                    dt.DeleteStatus = true;
                    context.NPF_TRN_IncidentVehicle.Update(dt);
                    context.SaveChanges();
                }
                NPF_TRN_Incident Incident = context.NPF_TRN_Incident.Where(x => x.Id == data.IncidentId).FirstOrDefault();

                if (Incident != null)
                {
                    if (Incident.Status == DAL.Common.TransactionStatus.Open)
                    {
                        Incident.Status = DAL.Common.TransactionStatus.Draft;
                        context.NPF_TRN_Incident.Update(Incident);
                        context.SaveChanges();

                    }
                    if (Incident.Status != DAL.Common.TransactionStatus.Draft && Incident.Status != DAL.Common.TransactionStatus.Open)
                    {
                        SendNotificationOnUpdate(Incident, "Vehicle Details",UserToken, "vehicle");
                    }
                }

                BaseDto.Data = true;
                BaseDto.QryResult = queryResult.SUCEEDED;


            }
            catch (Exception)
            {
                BaseDto.Data = false;
                BaseDto.AdditionalData = "Error";
                BaseDto.QryResult = queryResult.FAILED;
            }
            return BaseDto;

        }
        public BaseResponseDTO<bool> AddDeleteEquipmentToTable(NPF_TRN_IncidentEquipment data, string UserToken)
        {
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
            try
            {
                if (data.Id == 0)
                {
                    context.NPF_TRN_IncidentEquipment.Add(data);
                    context.SaveChanges();
                    BaseDto.Id = data.Id;
                }
                else
                {
                    NPF_TRN_IncidentEquipment dt = context.NPF_TRN_IncidentEquipment.Where(x => x.Id == data.Id).FirstOrDefault();
                    dt.DeleteStatus = true;
                    context.NPF_TRN_IncidentEquipment.Update(dt);
                    context.SaveChanges();
                }
                NPF_TRN_Incident Incident = context.NPF_TRN_Incident.Where(x => x.Id == data.IncidentId).FirstOrDefault();

                if (Incident != null)
                {
                    if (Incident.Status == DAL.Common.TransactionStatus.Open)
                    {
                        Incident.Status = DAL.Common.TransactionStatus.Draft;
                        context.NPF_TRN_Incident.Update(Incident);
                        context.SaveChanges();

                    }
                    if (Incident.Status != DAL.Common.TransactionStatus.Draft && Incident.Status != DAL.Common.TransactionStatus.Open)
                    {
                        SendNotificationOnUpdate(Incident, "Equipment Details", UserToken, "equipment");
                    }
                }
                BaseDto.Data = true;
                BaseDto.QryResult = queryResult.SUCEEDED;


            }
            catch (Exception)
            {
                BaseDto.Data = false;
                BaseDto.AdditionalData = "Error";
                BaseDto.QryResult = queryResult.FAILED;
            }
            return BaseDto;
        }
        public BaseResponseDTO<bool> AddDeleteResourceToTable(NPF_TRN_IncidentResource data, string UserToken)
        {
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
            try
            {
                if (data.Id == 0)
                {
                    context.NPF_TRN_IncidentResource.Add(data);
                    context.SaveChanges();
                    BaseDto.Id = data.Id;
                }
                else
                {
                    NPF_TRN_IncidentResource dt = context.NPF_TRN_IncidentResource.Where(x => x.Id == data.Id).FirstOrDefault();
                    dt.DeleteStatus = true;
                    context.NPF_TRN_IncidentResource.Update(dt);
                    context.SaveChanges();
                }
                NPF_TRN_Incident Incident = context.NPF_TRN_Incident.Where(x => x.Id == data.IncidentId).FirstOrDefault();

                if (Incident != null)
                {
                    if (Incident.Status == DAL.Common.TransactionStatus.Open)
                    {
                        Incident.Status = DAL.Common.TransactionStatus.Draft;
                        context.NPF_TRN_Incident.Update(Incident);
                        context.SaveChanges();

                    }
                    if (Incident.Status != DAL.Common.TransactionStatus.Draft && Incident.Status != DAL.Common.TransactionStatus.Open)
                    {
                        SendNotificationOnUpdate(Incident, "Resource Details", UserToken, "resource");
                    }
                }
                BaseDto.Data = true;
                BaseDto.QryResult = queryResult.SUCEEDED;


            }
            catch (Exception)
            {
                BaseDto.Data = false;
                BaseDto.AdditionalData = "Error";
                BaseDto.QryResult = queryResult.FAILED;
            }
            return BaseDto;
        }
        public BaseResponseDTO<bool> AddDeleteExternalToTable(NPF_TRN_IncidentExternalAgency data, string UserToken)
        {
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
            try
            {
                if (data.Id == 0)
                {
                    context.NPF_TRN_IncidentExternalAgency.Add(data);
                    context.SaveChanges();
                    BaseDto.Id = data.Id;
                }
                else
                {
                    NPF_TRN_IncidentExternalAgency dt = context.NPF_TRN_IncidentExternalAgency.Where(x => x.Id == data.Id).FirstOrDefault();
                    dt.DeleteStatus = true;
                    context.NPF_TRN_IncidentExternalAgency.Update(dt);
                    context.SaveChanges();
                }
                NPF_TRN_Incident Incident = context.NPF_TRN_Incident.Where(x => x.Id == data.IncidentId).FirstOrDefault();

                if (Incident != null)
                {
                    if (Incident.Status == DAL.Common.TransactionStatus.Open)
                    {
                        Incident.Status = DAL.Common.TransactionStatus.Draft;
                        context.NPF_TRN_Incident.Update(Incident);
                        context.SaveChanges();

                    }
                    if (Incident.Status != DAL.Common.TransactionStatus.Draft && Incident.Status != DAL.Common.TransactionStatus.Open)
                    {
                        SendNotificationOnUpdate(Incident, "External Agencies Details", UserToken, "external");
                    }
                }
                BaseDto.Data = true;
                BaseDto.QryResult = queryResult.SUCEEDED;


            }
            catch (Exception)
            {
                BaseDto.Data = false;
                BaseDto.AdditionalData = "Error";
                BaseDto.QryResult = queryResult.FAILED;
            }
            return BaseDto;
        }
        public BaseResponseDTO<bool> AddDeleteCasualtyToTable(NPF_TRN_IncidentCasualtyDetails data, string UserToken)
        {
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
            try
            {
                if (data.Id == 0)
                {
                    context.NPF_TRN_IncidentCasualtyDetails.Add(data);
                    context.SaveChanges();
                    BaseDto.Id = data.Id;
                }
                else
                {
                    NPF_TRN_IncidentCasualtyDetails dt = context.NPF_TRN_IncidentCasualtyDetails.Where(x => x.Id == data.Id).FirstOrDefault();
                    dt.DeleteStatus = true;
                    context.NPF_TRN_IncidentCasualtyDetails.Update(dt);
                    context.SaveChanges();
                }
                NPF_TRN_Incident Incident = context.NPF_TRN_Incident.Where(x => x.Id == data.IncidentId).FirstOrDefault();

                if (Incident != null)
                {
                    if (Incident.Status == DAL.Common.TransactionStatus.Open)
                    {
                        Incident.Status = DAL.Common.TransactionStatus.Draft;
                        context.NPF_TRN_Incident.Update(Incident);
                        context.SaveChanges();

                    }
                    if (Incident.Status != DAL.Common.TransactionStatus.Draft && Incident.Status != DAL.Common.TransactionStatus.Open)
                    {
                        SendNotificationOnUpdate(Incident, "Casualty Details", UserToken, "casualty");
                    }
                }
                BaseDto.Data = true;
                BaseDto.QryResult = queryResult.SUCEEDED;


            }
            catch (Exception)
            {
                BaseDto.Data = false;
                BaseDto.AdditionalData = "Error";
                BaseDto.QryResult = queryResult.FAILED;
            }
            return BaseDto;
        }
        public BaseResponseDTO<bool> AddDeleteFatalityToTable(NPF_TRN_IncidentFatalityDetails data, string UserToken)
        {
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
            try
            {
                if (data.Id == 0)
                {
                    context.NPF_TRN_IncidentFatalityDetails.Add(data);
                    context.SaveChanges();
                    BaseDto.Id = data.Id;
                }
                else
                {
                    NPF_TRN_IncidentFatalityDetails dt = context.NPF_TRN_IncidentFatalityDetails.Where(x => x.Id == data.Id).FirstOrDefault();
                    dt.DeleteStatus = true;
                    context.NPF_TRN_IncidentFatalityDetails.Update(dt);
                    context.SaveChanges();
                }
                NPF_TRN_Incident Incident = context.NPF_TRN_Incident.Where(x => x.Id == data.IncidentId).FirstOrDefault();

                if (Incident != null)
                {
                    if (Incident.Status == DAL.Common.TransactionStatus.Open)
                    {
                        Incident.Status = DAL.Common.TransactionStatus.Draft;
                        context.NPF_TRN_Incident.Update(Incident);
                        context.SaveChanges();

                    }
                    if (Incident.Status != DAL.Common.TransactionStatus.Draft && Incident.Status != DAL.Common.TransactionStatus.Open)
                    {
                        SendNotificationOnUpdate(Incident, "Fatality Details", UserToken, "fatality");
                    }
                }
                BaseDto.Data = true;
                BaseDto.QryResult = queryResult.SUCEEDED;


            }
            catch (Exception)
            {
                BaseDto.Data = false;
                BaseDto.AdditionalData = "Error";
                BaseDto.QryResult = queryResult.FAILED;
            }
            return BaseDto;
        }
        public BaseResponseDTO<bool> AddDeleteOtherStationToTable(NPF_TRN_IncidentOtherStation data, string UserToken)
        {
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
            try
            {
                if (data.Id == 0)
                {
                    context.NPF_TRN_IncidentOtherStation.Add(data);
                    context.SaveChanges();
                    BaseDto.Id = data.Id;
                }
                else
                {
                    NPF_TRN_IncidentOtherStation dt = context.NPF_TRN_IncidentOtherStation.Where(x => x.Id == data.Id).FirstOrDefault();
                    dt.DeleteStatus = true;
                    context.NPF_TRN_IncidentOtherStation.Update(dt);
                    context.SaveChanges();
                }
                NPF_TRN_Incident Incident = context.NPF_TRN_Incident.Where(x => x.Id == data.IncidentId).FirstOrDefault();

                if (Incident != null)
                {
                    if (Incident.Status == DAL.Common.TransactionStatus.Open)
                    {
                        Incident.Status = DAL.Common.TransactionStatus.Draft;
                        context.NPF_TRN_Incident.Update(Incident);
                        context.SaveChanges();

                    }
                    if (Incident.Status != DAL.Common.TransactionStatus.Draft && Incident.Status != DAL.Common.TransactionStatus.Open)
                    {
                        SendNotificationOnUpdate(Incident, "Station Details", UserToken, "station");
                    }
                }
                BaseDto.Data = true;
                BaseDto.QryResult = queryResult.SUCEEDED;


            }
            catch (Exception)
            {
                BaseDto.Data = false;
                BaseDto.AdditionalData = "Error";
                BaseDto.QryResult = queryResult.FAILED;
            }
            return BaseDto;
        }
        public BaseResponseDTO<bool> AddDeleteDamageToTable(NPF_TRN_IncidentDamage data, string UserToken)
        {
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
            try
            {
                if (data.Id == 0)
                {
                    context.NPF_TRN_IncidentDamage.Add(data);
                    context.SaveChanges();
                    BaseDto.Id = data.Id;
                }
                else
                {
                    NPF_TRN_IncidentDamage dt = context.NPF_TRN_IncidentDamage.Where(x => x.Id == data.Id).FirstOrDefault();
                    dt.DeleteStatus = true;
                    context.NPF_TRN_IncidentDamage.Update(dt);
                    context.SaveChanges();
                }
                NPF_TRN_Incident Incident = context.NPF_TRN_Incident.Where(x => x.Id == data.IncidentId).FirstOrDefault();

                if (Incident != null)
                {
                    if (Incident.Status == DAL.Common.TransactionStatus.Open)
                    {
                        Incident.Status = DAL.Common.TransactionStatus.Draft;
                        context.NPF_TRN_Incident.Update(Incident);
                        context.SaveChanges();

                    }
                    if (Incident.Status != DAL.Common.TransactionStatus.Draft && Incident.Status != DAL.Common.TransactionStatus.Open)
                    {
                        SendNotificationOnUpdate(Incident, "Damage Details", UserToken, "damage");
                    }
                }
                BaseDto.Data = true;
                BaseDto.QryResult = queryResult.SUCEEDED;


            }
            catch (Exception)
            {
                BaseDto.Data = false;
                BaseDto.AdditionalData = "Error";
                BaseDto.QryResult = queryResult.FAILED;
            }
            return BaseDto;
        }
        public BaseResponseDTO<List<IncidentHandlingCallerListDTO>> GetCallerLastTransactions(long Id)
        {

            BaseResponseDTO<List<IncidentHandlingCallerListDTO>> dto = new BaseResponseDTO<List<IncidentHandlingCallerListDTO>>();
            List<IncidentHandlingCallerListDTO> result = new List<IncidentHandlingCallerListDTO>();
            string errorMsg = "No Data Found";
            try
            {
                result = (from a in context.NPF_TRN_IncidentCaller
                          join b in context.NPF_TRN_Incident on a.IncidentId equals b.Id
                          join c in context.NPF_SYS_Caller on a.CallerId equals c.Id
                          join d in context.NPF_SYS_IncidentType on b.IncidentType equals d.Id
                          join e in context.NPF_TRN_IncidentLocation on b.Id equals e.IncidentId
                          where c.Id == Id
                          select new IncidentHandlingCallerListDTO
                          {
                              Id = a.Id,
                              IncidentNo = b.IncidentNo,
                              IncidentType = d.Name,
                              DateCreated = b.CreatedDate.ToString("dd/MM/yyyy HH:mm"),
                              Location = e.Address,
                              Status = b.Status.ToString()

                          }).OrderByDescending(x => x.Id).Take(5).ToList();
                dto.Data = result;
                dto.QryResult = queryResult.SUCEEDED;
            }
            catch (Exception e)
            {
                dto.Data = result;
                dto.AdditionalData = errorMsg;
                dto.QryResult = queryResult.FAILED;
            }


            return dto;
        }
        public BaseResponseDTO<List<DropDown>> GetVehicleByCategory(DropdownParameterDTO p)
        {
            BaseResponseDTO<List<DropDown>> dto = new BaseResponseDTO<List<DropDown>>();
            List<DropDown> Ddl = new List<DropDown>();
            DropDown Dd = new DropDown();
            string errorMsg = "No Data Found";
            try
            {
                //Asset
                Dd = new DropDown();
                Dd.title = "Vehicle";
                Dd.items = new List<DropDownItem>();
                List<NPF_SYS_Vehicle> Vehicle = context.NPF_SYS_Vehicle.Where(x => x.Category == p.Parameter2 && x.Station == p.Parameter1 && x.DeleteStatus == false).ToList();

                foreach (NPF_SYS_Vehicle st in Vehicle)
                {
                    DropDownItem Ddi = new DropDownItem();
                    Ddi.Id = st.Id;
                    Ddi.text = st.RegistrationNo;
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
        public BaseResponseDTO<List<DropDown>> GetEquipmentByCategory(DropdownParameterDTO p)
        {
            BaseResponseDTO<List<DropDown>> dto = new BaseResponseDTO<List<DropDown>>();
            List<DropDown> Ddl = new List<DropDown>();
            DropDown Dd = new DropDown();
            string errorMsg = "No Data Found";
            try
            {
                //Asset
                Dd = new DropDown();
                Dd.title = "Equipment";
                Dd.items = new List<DropDownItem>();
                List<NPF_SYS_Equipment> equipmentList = context.NPF_SYS_Equipment.Where(x => x.AssignmentType == AssignmentType.Location && x.AssignmentId == p.Parameter1 && x.Category == p.Parameter2 && x.DeleteStatus == false).ToList();

                foreach (NPF_SYS_Equipment st in equipmentList)
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
        public async Task<BaseResponseDTO<List<DropDown>>> GetResourceByRank(DropdownParameterDTO p)
        {
            BaseResponseDTO<List<DropDown>> dto = new BaseResponseDTO<List<DropDown>>();
            List<DropDown> Ddl = new List<DropDown>();
            DropDown Dd = new DropDown();
            string errorMsg = "No Data Found";
            try
            {
                //Asset
                Dd = new DropDown();
                Dd.title = "Resource";
                Dd.items = new List<DropDownItem>();
                List<NPF_SYS_Resource> Resource = context.NPF_SYS_Resource.Where(x => x.Rank.Id == p.Parameter2 && x.StationUnitName.Id == p.Parameter1 && x.DeleteStatus == false).ToList();

                foreach (NPF_SYS_Resource st in Resource)
                {
                    GetByDateDTO resource = new GetByDateDTO(){
                        EndTime = DateTime.Now.AddMinutes(5),
                        StartTime = DateTime.Now.AddMinutes(-5),
                        Id = st.Id,
                    };
                    BaseResponseDTO<bool> sts = await Avl.CheckResAvailById(resource);
                    string Status = "N/D";
                    if (sts.QryResult == new QueryResult().SUCEEDED)
                    {
                        Status = sts.AdditionalData;
                    }
                    DropDownItem Ddi = new DropDownItem();
                    Ddi.Id = st.Id;
                    Ddi.text = st.NAA.FullNameM + " - Status : " + Status;
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
        public async Task<BaseResponseDTO<List<DropDown>>> GetResourceByCategory(DropdownParameterDTO p)
        {
            BaseResponseDTO<List<DropDown>> dto = new BaseResponseDTO<List<DropDown>>();
            List<DropDown> Ddl = new List<DropDown>();
            DropDown Dd = new DropDown();
            string errorMsg = "No Data Found";
            try
            {
                //Asset
                Dd = new DropDown();
                Dd.title = "Resource";
                Dd.items = new List<DropDownItem>();
                List<NPF_SYS_Resource> Resource = context.NPF_SYS_Resource.Where(x => x.ResourceType.Id == p.Parameter2 && x.StationUnitName.Id == p.Parameter1 && x.DeleteStatus == false).ToList();

                foreach (NPF_SYS_Resource st in Resource)
                {
                    GetByDateDTO resource = new GetByDateDTO()
                    {
                        EndTime = DateTime.Now.AddMinutes(5),
                        StartTime = DateTime.Now.AddMinutes(-5),
                        Id = st.Id,
                    };
                    BaseResponseDTO<bool> sts = await Avl.CheckResAvailById(resource);
                    string Status = "N/D";
                    if (sts.QryResult == new QueryResult().SUCEEDED)
                    {
                        Status = sts.AdditionalData;
                    }
                    DropDownItem Ddi = new DropDownItem();
                    Ddi.Id = st.Id;
                    Ddi.text = st.NAA.FullNameM + " - Status : " + Status;
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
        public async Task<BaseResponseDTO<bool>> AssignResource(List<IncidentAssignmentDTO> data)
        {
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
            try
            {
                foreach (IncidentAssignmentDTO dto in data)
                {
                    if (dto.Id == 0)
                    {
                        NPF_TRN_IncidentAssignment Assign = new NPF_TRN_IncidentAssignment()
                        {
                            Id = dto.Id,
                            IncidentId = dto.IncidentId,
                            ResourceId = dto.ResourceId,
                            IsMain = false
                        };
                        context.NPF_TRN_IncidentAssignment.Add(Assign);
                        context.SaveChanges();
                        NPF_TRN_Incident inc = context.NPF_TRN_Incident.Where(x => x.Id == dto.IncidentId).FirstOrDefault();
                        if(inc != null)
                        {
                            if(inc.IncidentStatus == IncidentStatus.Pending)
                            {
                                inc.DateTimeAssigned = DateTime.Now;
                                inc.IncidentStatus = IncidentStatus.Assigned;
                                context.NPF_TRN_Incident.Update(inc);
                                context.SaveChanges();
                            }
                        }

                        PushNotif pn = new PushNotif()
                        {
                            Id = dto.IncidentId,
                            Type = NotifType.Push,
                            MType = MessageType.Assigned
                        };
                        try
                        {
                            pn.ResId = context.NPF_SYS_Resource.Where(x => x.Id == dto.ResourceId).Select(x => Convert.ToInt64(x.UserId)).FirstOrDefault();
                            bool x = await Nhs.Notification(pn);
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                }



                BaseDto.Data = true;
                BaseDto.QryResult = queryResult.SUCEEDED;


            }
            catch (Exception)
            {
                BaseDto.Data = false;
                BaseDto.AdditionalData = "Error";
                BaseDto.QryResult = queryResult.FAILED;
            }
            return BaseDto;

        }
        public async Task<BaseResponseDTO<List<CRRequestListDTO>>> GetCRRequestList(long data)
        {
            BaseResponseDTO<List<CRRequestListDTO>> BaseDto = new BaseResponseDTO<List<CRRequestListDTO>>();
            List<CRRequestListDTO> list = new List<CRRequestListDTO>();
            try
            {
                List<NPF_REQ_Request> dtl = context.NPF_REQ_Request.Where(x => x.IncidentId == data && x.DeleteStatus == false).ToList();
                if (dtl != null)
                {
                    foreach (NPF_REQ_Request dt in dtl)
                    {

                        CRRequestListDTO d = new CRRequestListDTO();

                        d.Id = dt.Id;
                        d.Type = dt.Type.ToString();
                        switch (dt.Type)
                        {

                            case MRequestType.Asset:
                                d.Type = "Fire Truck";
                                d.Category = context.NPF_SYS_LookUpValue.Where(x => x.LookUpType.Name == "Vehicle Category" && x.Id == dt.EntityId).FirstOrDefault().Name;

                                break;
                            case MRequestType.Agency:
                                d.Type = "External Agencies";
                                d.Category = context.NPF_SYS_ExternalAgencies.Where(x => x.Id == dt.EntityId).FirstOrDefault().Name;

                                break;
                            case MRequestType.Resource:
                                d.Type = "Resource";
                                d.Category = context.NPF_SYS_ResourceType.Where(x => x.Id == dt.EntityId).FirstOrDefault().Name;

                                break;
                            case MRequestType.Equipment:
                                d.Type = "Equipment";
                                d.Category = context.NPF_SYS_LookUpValue.Where(x => x.LookUpType.Name == "Equipment Category" && x.Id == dt.EntityId).FirstOrDefault().Name;
                                break;

                        }
                        d.Datetime = dt.CreatedDate.ToString("dd/MM/yyyy HH:mm");
                        d.Completed = dt.Completed;
                        list.Add(d);
                    }
                    BaseDto.Data = list;
                    BaseDto.QryResult = queryResult.SUCEEDED;
                }
                else
                {
                    BaseDto.Data = list;
                    BaseDto.AdditionalData = "Error";
                    BaseDto.QryResult = queryResult.FAILED;
                }

            }
            catch (Exception)
            {
                BaseDto.Data = list;
                BaseDto.AdditionalData = "Error";
                BaseDto.QryResult = queryResult.FAILED;
            }
            return BaseDto;
        }
        public async Task<BaseResponseDTO<bool>> UpdateCRRequest(UpdateCRRequestDTO data, string userToken)
        {
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
            try
            {
                string Title = string.Empty;
                string Description = string.Empty;
                NPF_REQ_Request dt = context.NPF_REQ_Request.Where(x => x.Id == data.Id).FirstOrDefault();
                dt.Completed = data.Completed;
                context.NPF_REQ_Request.Update(dt);
                context.SaveChanges();
                switch (dt.Type)
                {
                    case MRequestType.Asset:
                        Title = "Fire Truck";
                        Description = context.NPF_SYS_LookUpValue.Where(x => x.LookUpType.Name == "Vehicle Category" && x.Id == dt.EntityId).FirstOrDefault().Name;
                        break;
                    case MRequestType.Agency:
                        Title = "External Agencies";
                        Description = context.NPF_SYS_ExternalAgencies.Where(x => x.Id == dt.EntityId).FirstOrDefault().Name;
                        break;
                    case MRequestType.Resource:
                        Title = "Resource";
                        Description = context.NPF_SYS_ResourceType.Where(x => x.Id == dt.EntityId).FirstOrDefault().Name;
                        break;
                    case MRequestType.Equipment:
                        Title = "Equipment";
                        Description = context.NPF_SYS_LookUpValue.Where(x => x.LookUpType.Name == "Equipment Category" && x.Id == dt.EntityId).FirstOrDefault().Name;
                        break;

                }
                long UID = context.NPF_SYS_User.Where(x => x.AId.ToLower() == userToken.ToLower()).FirstOrDefault().Id;
                NPF_TRN_Incident inc = context.NPF_TRN_Incident.Where(x => x.Id == dt.IncidentId).FirstOrDefault();
                List<long> StationIds = new List<long>();
                StationIds.Add(context.NPF_TRN_IncidentStation.Where(x => x.IncidentId == dt.IncidentId && x.DeleteStatus == false).FirstOrDefault().StationId);
                StationIds.AddRange(context.NPF_TRN_IncidentOtherStation.Where(x => x.IncidentId == dt.IncidentId && x.DeleteStatus == false).Select(y => y.OtherStationId).ToList());
                List<long> UsersIds = new List<long>();
                List<long> MobUsersIds = new List<long>();
                List<string> stalist = new List<string>();
                stalist.Add("Station Commander");
                stalist.Add("Station");
                long Cuid = context.NPF_SYS_User.Where(x => x.AId == inc.CreatedBy.ToString()).FirstOrDefault().Id;
                UsersIds = context.NPF_SYS_Resource.Where(x => x.UserId != null && StationIds.Contains(x.StationUnitName.Id) && stalist.Contains(x.ResourceType.Name)).Select(x => Convert.ToInt64(x.UserId)).ToList();
                if (UsersIds.Any(x => x != Cuid))
                {
                    UsersIds.Add(Cuid);
                }
                if (UsersIds.Any(x => x == UID))
                {
                    UsersIds.RemoveAll(x => x == UID);
                }
                Notification notif = new Notification()
                {
                    Id = inc.Id,
                    UserId = 0,
                    MType = MessageType.CRR,
                    MName = PageName.Incident,
                    TaskName = "Incident No:" + inc.IncidentNo + ". " + Title + " : " + Description + " Request Withdrawn.",
                    URL = "/IncidentHandling/AddEdit?Id=" + inc.Id + "&Code=" + inc.Code + "#"

                };
                string text1 = Title + " : " + Description;
                string text2 = " Request Withdrawn.";
                if (data.Completed)
                {
                    notif.TaskName = "Incident No:" + inc.IncidentNo + ". " + Title + " : " + Description + " Request Accepted.";
                    text2 = " Request Accepted.";
                }
                UsersIds = UsersIds.Distinct().ToList();
                foreach (long user in UsersIds)
                {
                    if (user > 0)
                    {
                        notif.UserId = user;
                        new NotificationService().SendNotification(notif);

                    }

                }
                MobUsersIds = context.NPF_TRN_IncidentAssignment.Where(x => x.IncidentId == inc.Id).Select(y => y.ResourceId).ToList();
                UsersIds = new List<long>();
                UsersIds.AddRange(context.NPF_SYS_Resource.Where(x => x.UserId != null && MobUsersIds.Contains(x.Id)).Select(x => Convert.ToInt64(x.UserId)).ToList());
                UsersIds = UsersIds.Distinct().ToList();
                foreach (long user in UsersIds)
                {
                    if (user > 0)
                    {
                        PushNotif pn = new PushNotif()
                        {
                            Id = inc.Id,
                            Type = NotifType.Push,
                            ResId = user,
                            MType = MessageType.REQ,
                            Text = text1,
                            Text2 = text2
                        };
                        try
                        {
                            bool x = await new NotificationHandlerService().UnotNotification(pn);
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                }
                BaseDto.Data = true;
                BaseDto.QryResult = queryResult.SUCEEDED;


            }
            catch (Exception)
            {
                BaseDto.Data = false;
                BaseDto.AdditionalData = "Error";
                BaseDto.QryResult = queryResult.FAILED;
            }
            return BaseDto;
        }
        #region file
        public BaseResponseDTO<bool> UploadFileIncident(IncidentFilesDTO data)
        {
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
            try
            {
                if (data.Id == 0)
                {
                    IFormFile source = data.File;
                    string filename = System.Net.Http.Headers.ContentDispositionHeaderValue.Parse(source.ContentDisposition).FileName.Trim('"');

                    if (filename.Contains("\\"))
                    {
                        filename = filename.Substring(filename.LastIndexOf("\\") + 1);
                    }

                    using (var ms = new MemoryStream())
                    {
                        source.CopyTo(ms);
                        var fileBytes = ms.ToArray();
                        string BaseFile = Convert.ToBase64String(fileBytes);
                        NPF_TRN_IncidentFiles incfiles = new NPF_TRN_IncidentFiles()
                        {
                            File = BaseFile,
                            FileName = filename,
                            IncidentId = data.IncidentId
                        };
                        context.NPF_TRN_IncidentFiles.Add(incfiles);
                        context.SaveChanges();
                        BaseDto.Id = incfiles.Id;
                    }
                }
                else
                {
                    IFormFile source = data.File;
                    string filename = System.Net.Http.Headers.ContentDispositionHeaderValue.Parse(source.ContentDisposition).FileName.Trim('"');

                    if (filename.Contains("\\"))
                    {
                        filename = filename.Substring(filename.LastIndexOf("\\") + 1);
                    }

                    using (var ms = new MemoryStream())
                    {
                        source.CopyTo(ms);
                        var fileBytes = ms.ToArray();
                        string BaseFile = Convert.ToBase64String(fileBytes);
                        NPF_TRN_IncidentFiles incfiles = new NPF_TRN_IncidentFiles()
                        {
                            Id = data.Id,
                            File = BaseFile,
                            FileName = filename,
                            IncidentId = data.IncidentId
                        };
                        context.NPF_TRN_IncidentFiles.Update(incfiles);
                        context.SaveChanges();
                        BaseDto.Id = incfiles.Id;
                    }
                }
                BaseDto.Data = true;
                BaseDto.QryResult = queryResult.SUCEEDED;


            }
            catch (Exception)
            {
                BaseDto.Data = false;
                BaseDto.AdditionalData = "Error";
                BaseDto.QryResult = queryResult.FAILED;
            }
            return BaseDto;
        }
        public BaseResponseDTO<bool> DeleteFileIncident(long data)
        {
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
            try
            {
                NPF_TRN_IncidentFiles dt = context.NPF_TRN_IncidentFiles.Where(x => x.Id == data).FirstOrDefault();
                if (dt != null)
                {
                    dt.DeleteStatus = true;
                    context.NPF_TRN_IncidentFiles.Update(dt);
                    context.SaveChanges();
                    BaseDto.Data = true;
                    BaseDto.QryResult = queryResult.SUCEEDED;
                }
                else
                {
                    BaseDto.Data = false;
                    BaseDto.AdditionalData = "Error";
                    BaseDto.QryResult = queryResult.FAILED;
                }

            }
            catch (Exception)
            {
                BaseDto.Data = false;
                BaseDto.AdditionalData = "Error";
                BaseDto.QryResult = queryResult.FAILED;
            }
            return BaseDto;
        }
        public async Task<BaseResponseDTO<AttachmentDTO>> DownloadIncidentFile(long Id)
        {
            BaseResponseDTO<AttachmentDTO> dto = new BaseResponseDTO<AttachmentDTO>();
            dto.Data = new AttachmentDTO();
            try
            {
                dto.Data = (from a in context.NPF_TRN_IncidentFiles
                            where a.Id == Id && a.DeleteStatus == false
                            select new AttachmentDTO
                            {
                                File = a.File,
                                Filename = a.FileName
                            }).FirstOrDefault();

                dto.QryResult = queryResult.SUCEEDED;
            }
            catch (Exception ex)
            {
                dto.Data = new AttachmentDTO();
                dto.QryResult = queryResult.FAILED;
            }
            return dto;
        }
        #endregion region
        #region PDA
        public async Task<BaseResponseDTO<IncidentPDADTO>> GetIncidentPDA(IncidentPDASearchDTO data)
        {
            BaseResponseDTO<IncidentPDADTO> BaseDto = new BaseResponseDTO<IncidentPDADTO>();
            IncidentPDADTO result = new IncidentPDADTO();
            string NoPDA = "No PDA Found.";
            try
            {

                long stationType = context.NPF_SYS_Station.Where(x => x.Id == data.StationId).FirstOrDefault().StationType.Id;
                NPF_SYS_PDA PDA = context.NPF_SYS_PDA.Where(x => x.StationType.Id == stationType && x.NatureOfIncident.Id == data.NatureofIncidentId && x.NatureOfIncident.Id == data.IncidentTypeId && x.DeleteStatus == false).FirstOrDefault();
                if (PDA != null)
                {
                    List<NPF_SYS_PDAEquipmentMapping> equipment = context.NPF_SYS_PDAEquipmentMapping.Where(x => x.Fk_PDA == PDA.Id && x.DeleteStatus == false).ToList();
                    if (equipment.Count > 0)
                    {
                        result.Equipment = "<u>PDA for Equipment </u><br/>";
                        foreach (NPF_SYS_PDAEquipmentMapping item in equipment)
                        {
                            result.Equipment += "• " + item.EquipmentQuantity + " " + item.EquipmentCategory.Name + "<br/>";
                        }
                    }
                    else
                    {
                        result.Equipment = NoPDA;
                    }
                    List<NPF_SYS_PDAVehicleMapping> vehicle = context.NPF_SYS_PDAVehicleMapping.Where(x => x.Fk_PDA == PDA.Id && x.DeleteStatus == false).ToList();
                    if (vehicle.Count > 0)
                    {
                        result.Vehicle = "<u>PDA for Vehicle </u> <br/>";
                        foreach (NPF_SYS_PDAVehicleMapping item in vehicle)
                        {
                            result.Vehicle += "• " + item.VehicleQuantity + " " + item.VehicleCategory.Name + "<br/>";
                        }
                    }
                    else
                    {
                        result.Vehicle = NoPDA;
                    }
                    List<NPF_SYS_PDAResourceMapping> resource = context.NPF_SYS_PDAResourceMapping.Where(x => x.Fk_PDA == PDA.Id && x.DeleteStatus == false).ToList();
                    if (resource.Count > 0)
                    {
                        result.Resource = "<u>PDA for Resource </u> <br/>";
                        foreach (NPF_SYS_PDAResourceMapping item in resource)
                        {
                            result.Resource += "• " + item.RankQuantity + " " + item.Rank.Name + "<br/>";
                        }
                    }
                    else
                    {
                        result.Resource = NoPDA;
                    }
                }
                else
                {
                    result.Equipment = NoPDA;
                    result.Vehicle = NoPDA;
                    result.Resource = NoPDA;
                }

                BaseDto.Data = result;
                BaseDto.QryResult = queryResult.SUCEEDED;
            }
            catch (Exception)
            {
                BaseDto.Data = result;
                BaseDto.AdditionalData = "Error";
                BaseDto.QryResult = queryResult.FAILED;
            }
            return BaseDto;
        }
        #endregion
    }
}
