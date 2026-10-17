using BL.Constants;
using BL.Models.Access;
using BL.Models.Common;
using BL.Models.Schedule;
using BL.Services.Common;
using DAL.DBContext;
using DAL.Common;
using DAL.Models.Schedule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BL.Models.Request.RequestDTO;
using DAL.Models.Administration;
using NetTopologySuite.Mathematics;
using DAL.Models.Request;
using DAL.Models.Transaction;
using LinqToExcel.Extensions;
using NPOI.SS.Formula.Functions;
using DAL.Models.Common;
using Org.BouncyCastle.Ocsp;
using static NPOI.HSSF.Util.HSSFColor;
using NPOI.POIFS.Crypt.Dsig;

namespace BL.Services.Request
{
    public class RequestService
    {
        private NPFContext context;
        private QueryResult queryResult;
        public RequestService()
        {
            context = new NPFContext();
            queryResult = new QueryResult();
        }
        #region Web
        #endregion

        #region Mobile
        public async Task<BaseResponseDTO<ScreenDTO>> GetScreenData(BaseRequestDTO<GetScreen> data)
        {
            BaseResponseDTO<ScreenDTO> BaseDto = new BaseResponseDTO<ScreenDTO>();
            ScreenDTO Dto = new ScreenDTO();
            try
            {
                List<NPF_REQ_Request> Lreq = context.NPF_REQ_Request.Where(x => x.IncidentId == data.Data.incidentId && x.Type == data.Data.type && x.DeleteStatus == false).ToList();
                switch (data.Data.type)
                {
                    case MRequestType.Asset:
                        Dto.title = "Fire Truck";
                        List<NPF_SYS_LookUpValue> VehicleCategory = context.NPF_SYS_LookUpValue.Where(x => x.LookUpType.Name == "Vehicle Category" && x.DeleteStatus == false).ToList();
                        foreach (NPF_SYS_LookUpValue st in VehicleCategory)
                        {
                            DropDownItem Ddi = new DropDownItem();
                            Ddi.Id = st.Id;
                            Ddi.text = st.Name;
                            Dto.items.Add(Ddi);
                        }
                        foreach (NPF_REQ_Request req in Lreq)
                        {
                            GridDTO G = new GridDTO();
                            G.id = req.Id;
                            G.description = context.NPF_SYS_LookUpValue.Where(x => x.LookUpType.Name == "Vehicle Category" && x.Id == req.EntityId).FirstOrDefault().Name;
                            Dto.grid.Add(G);
                        }
                        break;
                    case MRequestType.Agency:
                        Dto.title = "External Agencies";
                        List<NPF_SYS_ExternalAgencies> ExternalAgency = context.NPF_SYS_ExternalAgencies.Where(x => x.DeleteStatus == false).ToList();
                        foreach (NPF_SYS_ExternalAgencies st in ExternalAgency)
                        {
                            DropDownItem Ddi = new DropDownItem();
                            Ddi.Id = st.Id;
                            Ddi.text = st.Name;
                            Dto.items.Add(Ddi);
                        }
                        foreach (NPF_REQ_Request req in Lreq)
                        {
                            GridDTO G = new GridDTO();
                            G.id = req.Id;
                            G.description = context.NPF_SYS_ExternalAgencies.Where(x => x.Id == req.EntityId).FirstOrDefault().Name;
                            Dto.grid.Add(G);
                        }
                        break;
                    case MRequestType.Resource:
                        Dto.title = "Resource";
                       // List<NPF_SYS_ResourceType> ResourceCategory = context.NPF_SYS_ResourceType.Where(x => x.DeleteStatus == false).ToList();
                        List<ResourceCatDTO> resource = (from a in context.NPF_SYS_ResourceType
                                                         join b in context.NPF_SYS_Rank on a.Id equals b.ResourceCategory
                                                         where a.DeleteStatus == false && b.DeleteStatus == false
                                                         select new ResourceCatDTO
                                                         {
                                                             ResourceCategory = a.Id,
                                                             Name = a.Name + " - " + b.Name,
                                                             Rank = b.Id
                                                         }).ToList();
                        foreach (ResourceCatDTO st in resource)
                        {
                            DropDownItem Ddi = new DropDownItem();
                            Ddi.Id = st.ResourceCategory;
                            Ddi.text = st.Name;
                            Ddi.Additional = st.Rank.ToString();
                            Dto.items.Add(Ddi);
                        }
                        foreach (NPF_REQ_Request req in Lreq)
                        {
                            GridDTO G = new GridDTO();
                            G.id = req.Id;
                            G.description = (from a in context.NPF_SYS_ResourceType
                                             join b in context.NPF_SYS_Rank on a.Id equals b.ResourceCategory
                                             where a.Id == req.EntityId && b.Id == long.Parse(req.Additional)
                                             select a.Name + " - " + b.Name).FirstOrDefault();
                            G.quantity = req.Quantity;
                            Dto.grid.Add(G);
                        }
                        break;
                    case MRequestType.Equipment:
                        Dto.title = "Equipment";
                        List<NPF_SYS_LookUpValue> EquipmentCategory = context.NPF_SYS_LookUpValue.Where(x => x.LookUpType.Name == "Equipment Category" && x.DeleteStatus == false).ToList();
                        foreach (NPF_SYS_LookUpValue st in EquipmentCategory)
                        {
                            DropDownItem Ddi = new DropDownItem();
                            Ddi.Id = st.Id;
                            Ddi.text = st.Name;
                            Dto.items.Add(Ddi);
                        }
                        foreach (NPF_REQ_Request req in Lreq)
                        {
                            GridDTO G = new GridDTO();
                            G.id = req.Id;
                            G.description = context.NPF_SYS_LookUpValue.Where(x => x.LookUpType.Name == "Equipment Category" && x.Id == req.EntityId).FirstOrDefault().Name;
                            Dto.grid.Add(G);
                        }
                        break;

                }
                BaseDto.Data = Dto;
                BaseDto.QryResult = queryResult.SUCEEDED;
            }
            catch (Exception ex)
            {
                BaseDto.Data = Dto;
                BaseDto.AdditionalData = "Error Fetching Record";
                BaseDto.QryResult = queryResult.FAILED;
            }
            return BaseDto;
        }
        public async Task<BaseResponseDTO<bool>> InsertRequest(BaseRequestDTO<InsertRequest> data)
        {
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
            try
            {
                string Title = string.Empty;
                string Description = string.Empty;
                NPF_REQ_Request dt = new NPF_REQ_Request()
                {
                    IncidentId = data.Data.incidentId,
                    EntityId = data.Data.entityId,
                    Type = data.Data.type,
                    Additional = data.Data.additional,
                    CreatedBy = Guid.Parse(data.userToken.ToString()),
                    CreatedDate = DateTime.Now,
                };
                context.NPF_REQ_Request.Add(dt);
                context.SaveChanges();
                switch (data.Data.type)
                {
                    case MRequestType.Asset:
                        Title = "Fire Truck";
                        Description = context.NPF_SYS_LookUpValue.Where(x => x.LookUpType.Name == "Vehicle Category" && x.Id == data.Data.entityId).FirstOrDefault().Name;
                        break;
                    case MRequestType.Agency:
                        Title = "External Agencies";
                        Description = context.NPF_SYS_ExternalAgencies.Where(x => x.Id == data.Data.entityId).FirstOrDefault().Name;
                        break;
                    case MRequestType.Resource:
                        Title = "Resource";
                        Description = context.NPF_SYS_ResourceType.Where(x => x.Id == data.Data.entityId).FirstOrDefault().Name;
                        break;
                    case MRequestType.Equipment:
                        Title = "Equipment";
                        Description = context.NPF_SYS_LookUpValue.Where(x => x.LookUpType.Name == "Equipment Category" && x.Id == data.Data.entityId).FirstOrDefault().Name;
                        break;

                }

                long UID = context.NPF_SYS_User.Where(x => x.AId.ToLower() == data.userToken.ToLower()).FirstOrDefault().Id;
                NPF_TRN_Incident inc = context.NPF_TRN_Incident.Where(x => x.Id == dt.IncidentId).FirstOrDefault();
                List<long> StationIds = new List<long>();
                StationIds.Add(context.NPF_TRN_IncidentStation.Where(x => x.IncidentId == dt.IncidentId && x.DeleteStatus == false).FirstOrDefault().StationId);
                StationIds.AddRange(context.NPF_TRN_IncidentOtherStation.Where(x => x.IncidentId == dt.IncidentId && x.DeleteStatus == false).Select(y => y.OtherStationId).ToList());
                //Start Changed for CRO 20241112
                StationIds.AddRange(context.NPF_SYS_Station.Where(x => x.Name == "Control Room" && x.DeleteStatus == false).Select(y => y.Id).ToList());
                //End Changed for CRO 20241112
                List<long> UsersIds = new List<long>();
                List<long> MobUsersIds = new List<long>();
                List<string> stalist = new List<string>();
                stalist.Add("Station Commander");
                stalist.Add("Station");
                stalist.Add("CRO");
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
                    TaskName = "Incident No:" + inc.IncidentNo + " " + Title + " : " + Description + " Has Been Requested.",
                    URL = "/IncidentHandling/AddEdit?Id=" + inc.Id + "&Code=" + inc.Code + "&Field=mobile#scrollspy-mobile",
                    AUserId = data.userToken.ToLower()
                };
                UsersIds = UsersIds.Distinct().ToList();
                foreach (long user in UsersIds)
                {
                    if (user > 0)
                    {
                        notif.UserId = user;
                        notif.AUserId = data.userToken.ToLower();
                        new NotificationService().SendNotification(notif);

                    }

                }

                BaseDto.Data = true;
                BaseDto.QryResult = queryResult.SUCEEDED;
            }
            catch (Exception ex)
            {
                BaseDto.Data = false;
                BaseDto.AdditionalData = "Error Adding Record";
                BaseDto.QryResult = queryResult.FAILED;
            }
            return BaseDto;
        }
        public async Task<BaseResponseDTO<bool>> DeleteRequest(BaseRequestDTO<long> datum)
        {
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
            try
            {
                string Title = string.Empty;
                string Description = string.Empty;
                NPF_REQ_Request dt = context.NPF_REQ_Request.Where(x => x.Id == datum.Data).FirstOrDefault();
                dt.UpdatedBy = Guid.Parse(datum.userToken.ToString());
                dt.UpdatedDate = DateTime.Now;
                dt.DeleteStatus = true;
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
                long UID = context.NPF_SYS_User.Where(x => x.AId.ToLower() == datum.userToken.ToLower()).FirstOrDefault().Id;
                NPF_TRN_Incident inc = context.NPF_TRN_Incident.Where(x => x.Id == dt.IncidentId).FirstOrDefault();
                List<long> StationIds = new List<long>();
                StationIds.Add(context.NPF_TRN_IncidentStation.Where(x => x.IncidentId == dt.IncidentId && x.DeleteStatus == false).FirstOrDefault().StationId);
                StationIds.AddRange(context.NPF_TRN_IncidentOtherStation.Where(x => x.IncidentId == dt.IncidentId && x.DeleteStatus == false).Select(y => y.OtherStationId).ToList());
                //Start Changed for CRO 20241112
                StationIds.AddRange(context.NPF_SYS_Station.Where(x => x.Name == "Control Room" && x.DeleteStatus == false).Select(y => y.Id).ToList());
                //End Changed for CRO 20241112
                List<long> UsersIds = new List<long>();
                List<long> MobUsersIds = new List<long>();
                List<string> stalist = new List<string>();
                stalist.Add("Station Commander");
                stalist.Add("Station");
                stalist.Add("CRO");
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
                    URL = "/IncidentHandling/AddEdit?Id=" + inc.Id + "&Code=" + inc.Code + "&Field=mobile#scrollspy-mobile",
                    AUserId = datum.userToken.ToLower()

                };

                UsersIds = UsersIds.Distinct().ToList();
                foreach (long user in UsersIds)
                {
                    if (user > 0)
                    {
                        notif.UserId = user;
                        new NotificationService().SendNotification(notif);

                    }

                }
                BaseDto.Data = true;
                BaseDto.QryResult = queryResult.SUCEEDED;
            }
            catch (Exception ex)
            {
                BaseDto.Data = false;
                BaseDto.AdditionalData = "Error Adding Record";
                BaseDto.QryResult = queryResult.FAILED;
            }
            return BaseDto;
        }
        public async Task<BaseResponseDTO<bool>> SaveUpdateDamage(BaseRequestDTO<NPF_TRN_IncidentDamage> data)
        {
            string Title = "Damage Details";
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
            NPF_TRN_IncidentDamage dt = new NPF_TRN_IncidentDamage();
            try
            {
                if (data.Data.Id == 0)
                {
                    dt.IncidentId = data.Data.IncidentId;
                    dt.Details = data.Data.Details;
                    dt.EstimatedValue = data.Data.EstimatedValue;
                    dt.Cost = data.Data.Cost;
                    dt.Salvage = data.Data.Salvage;
                    dt.CreatedBy = Guid.Parse(data.userToken.ToString());
                    dt.CreatedDate = DateTime.Now;
                    context.NPF_TRN_IncidentDamage.Add(dt);
                    context.SaveChanges();
                }
                else
                {
                    dt = context.NPF_TRN_IncidentDamage.Where(x => x.Id == data.Data.Id).FirstOrDefault();
                    dt.DeleteStatus = true;
                    dt.UpdatedBy = Guid.Parse(data.userToken.ToString());
                    dt.UpdatedDate = DateTime.Now;
                    context.NPF_TRN_IncidentDamage.Update(dt);
                    context.SaveChanges();
                }

                long UID = context.NPF_SYS_User.Where(x => x.AId.ToLower() == data.userToken.ToLower()).FirstOrDefault().Id;
                NPF_TRN_Incident inc = context.NPF_TRN_Incident.Where(x => x.Id == dt.IncidentId).FirstOrDefault();
                List<long> StationIds = new List<long>();
                StationIds.Add(context.NPF_TRN_IncidentStation.Where(x => x.IncidentId == dt.IncidentId && x.DeleteStatus == false).FirstOrDefault().StationId);
                StationIds.AddRange(context.NPF_TRN_IncidentOtherStation.Where(x => x.IncidentId == dt.IncidentId && x.DeleteStatus == false).Select(y => y.OtherStationId).ToList());
                //Start Changed for CRO 20241112
                StationIds.AddRange(context.NPF_SYS_Station.Where(x => x.Name == "Control Room" && x.DeleteStatus == false).Select(y => y.Id).ToList());
                //End Changed for CRO 20241112
                List<long> UsersIds = new List<long>();
                List<long> MobUsersIds = new List<long>();
                List<string> stalist = new List<string>();
                stalist.Add("Station Commander");
                stalist.Add("Station");
                stalist.Add("CRO");
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
                    MType = MessageType.Changes,
                    MName = PageName.Incident,
                    TaskName = "Incident No:" + inc.IncidentNo + " " + Title + " Has Been Changed.",
                    URL = "/IncidentHandling/AddEdit?Id=" + inc.Id + "&Code=" + inc.Code + "&Field=damage#scrollspy-damage",
                    AUserId = data.userToken.ToLower()
                };
                UsersIds = UsersIds.Distinct().ToList();
                foreach (long user in UsersIds)
                {
                    if (user > 0)
                    {
                        notif.UserId = user;
                        notif.AUserId = data.userToken.ToLower();
                        new NotificationService().SendNotification(notif);

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
        public async Task<BaseResponseDTO<List<NPF_TRN_IncidentDamage>>> GetDamageList(BaseRequestDTO<long> data)
        {
            BaseResponseDTO<List<NPF_TRN_IncidentDamage>> BaseDto = new BaseResponseDTO<List<NPF_TRN_IncidentDamage>>();
            List<NPF_TRN_IncidentDamage> dtl = new List<NPF_TRN_IncidentDamage>();
            try
            {
                dtl = context.NPF_TRN_IncidentDamage.Where(x => x.IncidentId == data.Data && x.DeleteStatus == false).ToList();
                BaseDto.Data = dtl;
                BaseDto.QryResult = queryResult.SUCEEDED;


            }
            catch (Exception)
            {
                BaseDto.Data = dtl;
                BaseDto.AdditionalData = "Error";
                BaseDto.QryResult = queryResult.FAILED;
            }
            return BaseDto;
        }
        public async Task<BaseResponseDTO<NPF_TRN_IncidentFatality>> GetFatalities(BaseRequestDTO<long> data)
        {
            BaseResponseDTO<NPF_TRN_IncidentFatality> BaseDto = new BaseResponseDTO<NPF_TRN_IncidentFatality>();
            NPF_TRN_IncidentFatality dtl = new NPF_TRN_IncidentFatality();
            dtl.IncidentId = data.Data;
            dtl.Id = 0;
            try
            {
                NPF_TRN_IncidentFatality Ndtl = context.NPF_TRN_IncidentFatality.Where(x => x.IncidentId == data.Data && x.DeleteStatus == false).FirstOrDefault();
                if (Ndtl != null)
                {
                    BaseDto.Data = Ndtl;
                }
                else
                {
                    BaseDto.Data = dtl;
                }
                BaseDto.QryResult = queryResult.SUCEEDED;


            }
            catch (Exception)
            {
                BaseDto.Data = dtl;
                BaseDto.AdditionalData = "Error";
                BaseDto.QryResult = queryResult.FAILED;
            }
            return BaseDto;
        }
        public async Task<BaseResponseDTO<bool>> UpdateFatalities(BaseRequestDTO<NPF_TRN_IncidentFatality> data)
        {
            string Title = "Fatality Details";
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
            NPF_TRN_IncidentFatality dt = new NPF_TRN_IncidentFatality();
            try
            {
                if (data.Data.Id == 0)
                {
                    dt.TotalDeath = data.Data.TotalDeath;
                    dt.IncidentId = data.Data.IncidentId;
                    dt.CreatedBy = Guid.Parse(data.userToken.ToString());
                    dt.CreatedDate = DateTime.Now;
                    context.NPF_TRN_IncidentFatality.Add(dt);
                    context.SaveChanges();
                }
                else
                {
                    dt = context.NPF_TRN_IncidentFatality.Where(x => x.Id == data.Data.Id).FirstOrDefault();
                    dt.TotalDeath = data.Data.TotalDeath;
                    dt.UpdatedBy = Guid.Parse(data.userToken.ToString());
                    dt.UpdatedDate = DateTime.Now;
                    context.NPF_TRN_IncidentFatality.Update(dt);
                    context.SaveChanges();
                }

                long UID = context.NPF_SYS_User.Where(x => x.AId.ToLower() == data.userToken.ToLower()).FirstOrDefault().Id;
                NPF_TRN_Incident inc = context.NPF_TRN_Incident.Where(x => x.Id == dt.IncidentId).FirstOrDefault();
                List<long> StationIds = new List<long>();
                StationIds.Add(context.NPF_TRN_IncidentStation.Where(x => x.IncidentId == dt.IncidentId && x.DeleteStatus == false).FirstOrDefault().StationId);
                StationIds.AddRange(context.NPF_TRN_IncidentOtherStation.Where(x => x.IncidentId == dt.IncidentId && x.DeleteStatus == false).Select(y => y.OtherStationId).ToList());
                //Start Changed for CRO 20241112
                StationIds.AddRange(context.NPF_SYS_Station.Where(x => x.Name == "Control Room" && x.DeleteStatus == false).Select(y => y.Id).ToList());
                //End Changed for CRO 20241112
                List<long> UsersIds = new List<long>();
                List<long> MobUsersIds = new List<long>();
                List<string> stalist = new List<string>();
                stalist.Add("Station Commander");
                stalist.Add("Station");
                stalist.Add("CRO");
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
                    MType = MessageType.Changes,
                    MName = PageName.Incident,
                    TaskName = "Incident No:" + inc.IncidentNo + " " + Title + " Has Been Changed.",
                    URL = "/IncidentHandling/AddEdit?Id=" + inc.Id + "&Code=" + inc.Code + "&Field=damage#scrollspy-damage",
                    AUserId = data.userToken.ToLower()
                };
                UsersIds = UsersIds.Distinct().ToList();
                foreach (long user in UsersIds)
                {
                    if (user > 0)
                    {
                        notif.UserId = user;
                        notif.AUserId = data.userToken.ToLower();
                        new NotificationService().SendNotification(notif);

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
        public async Task<BaseResponseDTO<NPF_TRN_IncidentCasualty>> GetCasualty(BaseRequestDTO<long> data)
        {
            BaseResponseDTO<NPF_TRN_IncidentCasualty> BaseDto = new BaseResponseDTO<NPF_TRN_IncidentCasualty>();
            NPF_TRN_IncidentCasualty dtl = new NPF_TRN_IncidentCasualty();
            dtl.IncidentId = data.Data;
            dtl.Id = 0;
            try
            {
                NPF_TRN_IncidentCasualty Ndtl = context.NPF_TRN_IncidentCasualty.Where(x => x.IncidentId == data.Data && x.DeleteStatus == false).FirstOrDefault();
                if (Ndtl != null)
                {
                    BaseDto.Data = Ndtl;
                }
                else
                {
                    BaseDto.Data = dtl;
                }
                BaseDto.QryResult = queryResult.SUCEEDED;


            }
            catch (Exception)
            {
                BaseDto.Data = dtl;
                BaseDto.AdditionalData = "Error";
                BaseDto.QryResult = queryResult.FAILED;
            }
            return BaseDto;
        }
        public async Task<BaseResponseDTO<bool>> UpdateCasualty(BaseRequestDTO<NPF_TRN_IncidentCasualty> data)
        {
            string Title = "Casualty Details";
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
            NPF_TRN_IncidentCasualty dt = new NPF_TRN_IncidentCasualty();
            try
            {
                if (data.Data.Id == 0)
                {
                    dt.IncidentId = data.Data.IncidentId;
                    dt.TotalRescued = data.Data.TotalRescued;
                    dt.TotalCasualties = data.Data.TotalCasualties;
                    dt.TotalEvacuated = data.Data.TotalEvacuated;
                    dt.Remarks = data.Data.Remarks;
                    dt.CreatedBy = Guid.Parse(data.userToken.ToString());
                    dt.CreatedDate = DateTime.Now;
                    context.NPF_TRN_IncidentCasualty.Add(dt);
                    context.SaveChanges();
                }
                else
                {
                    NPF_TRN_IncidentCasualty upd = context.NPF_TRN_IncidentCasualty.Where(x => x.Id == data.Data.Id).FirstOrDefault();
                    upd.TotalRescued = data.Data.TotalRescued;
                    upd.TotalCasualties = data.Data.TotalCasualties;
                    upd.TotalEvacuated = data.Data.TotalEvacuated;
                    upd.Remarks = data.Data.Remarks;
                    upd.UpdatedBy = Guid.Parse(data.userToken.ToString());
                    upd.UpdatedDate = DateTime.Now;
                    context.NPF_TRN_IncidentCasualty.Update(upd);
                    context.SaveChanges();
                }


                long UID = context.NPF_SYS_User.Where(x => x.AId.ToLower() == data.userToken.ToLower()).FirstOrDefault().Id;
                NPF_TRN_Incident inc = context.NPF_TRN_Incident.Where(x => x.Id == dt.IncidentId).FirstOrDefault();
                List<long> StationIds = new List<long>();
                StationIds.Add(context.NPF_TRN_IncidentStation.Where(x => x.IncidentId == dt.IncidentId && x.DeleteStatus == false).FirstOrDefault().StationId);
                StationIds.AddRange(context.NPF_TRN_IncidentOtherStation.Where(x => x.IncidentId == dt.IncidentId && x.DeleteStatus == false).Select(y => y.OtherStationId).ToList());
                //Start Changed for CRO 20241112
                StationIds.AddRange(context.NPF_SYS_Station.Where(x => x.Name == "Control Room" && x.DeleteStatus == false).Select(y => y.Id).ToList());
                //End Changed for CRO 20241112
                List<long> UsersIds = new List<long>();
                List<long> MobUsersIds = new List<long>();
                List<string> stalist = new List<string>();
                stalist.Add("Station Commander");
                stalist.Add("Station");
                stalist.Add("CRO");
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
                    MType = MessageType.Changes,
                    MName = PageName.Incident,
                    TaskName = "Incident No:" + inc.IncidentNo + " " + Title + " Has Been Changed.",
                    URL = "/IncidentHandling/AddEdit?Id=" + inc.Id + "&Code=" + inc.Code + "&Field=damage#scrollspy-damage",
                    AUserId = data.userToken.ToLower()
                };
                UsersIds = UsersIds.Distinct().ToList();
                foreach (long user in UsersIds)
                {
                    if (user > 0)
                    {
                        notif.UserId = user;
                        notif.AUserId = data.userToken.ToLower();
                        new NotificationService().SendNotification(notif);

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
        #endregion
    }
}
