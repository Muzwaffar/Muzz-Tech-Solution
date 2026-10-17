using BL.Constants;
using BL.Models.Access;
using BL.Models.Common;
using BL.Models.Notification;
using DAL.Common;
using DAL.DBContext;
using DAL.Models;
using DAL.Models.Common;
using DAL.Models.Notifications;
using DAL.Models.Transaction;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Update;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NPOI.SS.Formula.Functions;
using Remotion.Mixins.Definitions;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Data;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BL.Services.Common
{
    public class EventService
    {
        private NPFContext context;
        private QueryResult queryResult;
        private MatrixService Mservice;
        public UserMatrixDTO UM = new UserMatrixDTO();

        public EventService()
        {
            context = new NPFContext();
            queryResult = new QueryResult();
            Mservice = new MatrixService();
            UM = Mservice.sGetMatrixUserByUserID();

        }
        public async Task<BaseResponseDTO<bool>> SaveEvent(NPF_NOT_Event Event)
        {
            BaseResponseDTO<bool> baseDto = new BaseResponseDTO<bool>();

            try
            {

                context.NPF_NOT_Event.Add(Event);
                context.SaveChanges();

                baseDto.Data = true;
                baseDto.QryResult = queryResult.SUCEEDED;
                return baseDto;
            }
            catch (Exception ex)
            {
                baseDto.Data = false;
                baseDto.QryResult = queryResult.FAILED;
                return baseDto;
            }
        }
        public void Alert()
        {
            try
            {
                Notification notif = new Notification();
                List<long> StationIds = context.NPF_SYS_Resource.Where(x => x.UserId == UM.UID && x.DeleteStatus == false && (x.ResourceType.Name == "Station" || x.ResourceType.Name == "Station Commander" || x.ResourceType.Name == "CRO")).Select(y => y.StationUnitName.Id).ToList();
                List<long> IncidentIds = new List<long>();
                IncidentIds.AddRange(context.NPF_TRN_IncidentStation.Where(x => StationIds.Contains(x.StationId) && x.DeleteStatus == false).Select(y => y.IncidentId).ToList());
                IncidentIds.AddRange(context.NPF_TRN_IncidentOtherStation.Where(x => StationIds.Contains(x.OtherStationId) && x.DeleteStatus == false).Select(y => y.IncidentId).ToList());
                IncidentIds = IncidentIds.Distinct().ToList();

                int AlertTimer = Convert.ToInt32(context.NPF_SYS_GlobalParam.Where(x => x.Name == "AlertTimer").FirstOrDefault().Value);
                List<NPF_TRN_Incident> LInc = context.NPF_TRN_Incident.Where(x =>
                x.StationStatus == DAL.Common.StationStatus.NotAcknowledge &&
                x.Status != DAL.Common.TransactionStatus.Abort &&
                x.Status != DAL.Common.TransactionStatus.Completed
                && x.Timer < DateTime.Now && x.DeleteStatus == false && IncidentIds.Contains(x.Id)).ToList();

                foreach (NPF_TRN_Incident Inc in LInc)
                {

                    long CreatedByUsrId = context.NPF_SYS_User.Where(x => x.AId == Inc.CreatedBy.ToString()).FirstOrDefault().Id;
                    if(UM.UID == CreatedByUsrId)
                    {
                        notif = new Notification()
                        {
                            Id = Inc.Id,
                            UserId = UM.UID,
                            MType = MessageType.Pending,
                            MName = PageName.Incident,
                            TaskName = "Incident No:" + Inc.IncidentNo + " Not Acknowledged.",
                            URL = "/IncidentHandling/AddEdit?Id=" + Inc.Id + "&UserToken=" + UM.AUID + "#",
                            PopUp = true
                        };
                        new NotificationService().SendNotification(notif);
                    }else
                    {
                        notif = new Notification()
                        {
                            Id = Inc.Id,
                            UserId = UM.UID,
                            MType = MessageType.Pending,
                            MName = PageName.Incident,
                            TaskName = "Incident No:" + Inc.IncidentNo + " Acknowledgement Pending.",
                            URL = "/IncidentHandling/AddEdit?Id=" + Inc.Id + "&UserToken=" + UM.AUID + "#",
                            PopUp = true
                        };
                        new NotificationService().SendNotification(notif);
                    }
                    Inc.Timer = DateTime.Now.AddMinutes(AlertTimer);
                    context.NPF_TRN_Incident.Update(Inc);
                    context.SaveChanges();
                }
                LInc = context.NPF_TRN_Incident.Where(x => x.StationStatus == DAL.Common.StationStatus.Pending && 
                x.Status != DAL.Common.TransactionStatus.Abort &&
                x.Status != DAL.Common.TransactionStatus.Completed &&
                x.Timer < DateTime.Now && x.DeleteStatus == false && IncidentIds.Contains(x.Id)
                ).ToList();
                foreach (NPF_TRN_Incident Inc in LInc)
                {
                    long CreatedByUsrId = context.NPF_SYS_User.Where(x => x.AId == Inc.CreatedBy.ToString()).FirstOrDefault().Id;
                    if (UM.UID != CreatedByUsrId)
                    {
                        notif = new Notification()
                        {
                            Id = Inc.Id,
                            UserId = UM.UID,
                            MType = MessageType.Pending,
                            MName = PageName.Incident,
                            TaskName = "Incident No:" + Inc.IncidentNo + " Acknowledgement Pending.",
                            URL = "/IncidentHandling/AddEdit?Id=" + Inc.Id + "&UserToken=" + UM.AUID + "#",
                            PopUp = true
                        };
                        new NotificationService().SendNotification(notif);
                    }
                    else
                    {
                        notif = new Notification()
                        {
                            Id = Inc.Id,
                            UserId = UM.UID,
                            MType = MessageType.Pending,
                            MName = PageName.Incident,
                            TaskName = "Incident No:" + Inc.IncidentNo + " Not Acknowledged.",
                            URL = "/IncidentHandling/AddEdit?Id=" + Inc.Id + "&UserToken=" + UM.AUID + "#",
                            PopUp = true
                        };
                        new NotificationService().SendNotification(notif);
                    }
                    Inc.StationStatus = StationStatus.NotAcknowledge;
                    Inc.Timer = DateTime.Now.AddMinutes(AlertTimer);
                    context.NPF_TRN_Incident.Update(Inc);
                    context.SaveChanges();

                }
            }
            catch (Exception ex)
            {

            }
        }
        public async Task<BaseResponseDTO<GetEvent>> HeartBeat()//UserId
        {
            BaseResponseDTO<GetEvent> baseDto = new BaseResponseDTO<GetEvent>();

            baseDto.Data = new GetEvent();

            try
            {
                Alert();
                int count = 0;

                bool bdg = context.NPF_NOT_Event.Any(x => x.IsRead == false && x.UserId == UM.UID.ToString() && x.ActionDate.Date >= DateTime.Now.Date && x.ActionDate.Date <= DateTime.Now.AddDays(1).Date && x.DeleteStatus == false);

                if (bdg)
                {
                    count = context.NPF_NOT_Event.Where(x => x.IsRead == false && x.UserId == UM.UID.ToString() && x.ActionDate.Date >= DateTime.Now.Date && x.ActionDate.Date <= DateTime.Now.AddDays(1).Date && x.DeleteStatus == false).Count();
                }

                bool Notf = false;
                List<NPF_NOT_Event> El = context.NPF_NOT_Event.Where(x => x.IsNotified == false && x.ActionDate.Date >= DateTime.Now.Date && x.ActionDate.Date <= DateTime.Now.AddDays(1).Date && x.UserId == UM.UID.ToString() && x.DeleteStatus == false).ToList();
                List<NPF_NOT_Event> ElP = new List<NPF_NOT_Event>();
                if (El.Count > 0)
                {
                    Notf = true;
                    foreach (NPF_NOT_Event e in El)
                    {
                        e.IsNotified = true;
                        context.NPF_NOT_Event.Update(e);
                        context.SaveChanges();
                        if (e.PopUp == true)
                        {
                            ElP.Add(e);
                        }
                    }
                }

                baseDto.Data.CountUnread = count;
                baseDto.Data.ShowBadge = bdg;
                baseDto.Data.Notify = Notf;
                baseDto.Data.Events = El;
                baseDto.Data.PopEvents = ElP;
                baseDto.QryResult = queryResult.SUCEEDED;
                return baseDto;
            }
            catch (Exception ex)
            {
                baseDto.Data.ShowBadge = false;
                baseDto.Data.Notify = false;
                baseDto.QryResult = queryResult.SUCEEDED;
                return baseDto;
            }
        }
        public BaseResponseDTO<List<EventCenter>> GetList(DateRangeDTO dt)//UserId
        {
            BaseResponseDTO<List<EventCenter>> baseDto = new BaseResponseDTO<List<EventCenter>>();
            baseDto.Data = new List<EventCenter>();

            try
            {
                List<NPF_NOT_Event> El = context.NPF_NOT_Event.Where(x => x.ActionDate.Date.Date >= dt.From && x.ActionDate.Date <= dt.To && x.UserId == UM.UID.ToString() && x.DeleteStatus == false)
                    .OrderByDescending(y => y.CreatedDate).ToList();

                List<EventCenter> ev = new List<EventCenter>();

                ev = JsonConvert.DeserializeObject<List<EventCenter>>(JsonConvert.SerializeObject(El));
                baseDto.Data = ev;

                if (El.Count > 0)
                {
                    foreach (NPF_NOT_Event e in El)
                    {
                        //TO CHANGE BEFORE PUSH
                        if (e.IsNotified != true || e.IsRead != true)
                        {
                            e.IsNotified = true;
                            e.IsRead = true;
                            context.NPF_NOT_Event.Update(e);
                            context.SaveChanges();
                        }
                    }
                }

                baseDto.QryResult = queryResult.SUCEEDED;
                return baseDto;
            }
            catch (Exception ex)
            {
                baseDto.QryResult = queryResult.SUCEEDED;
                return baseDto;
            }
        }

        public async Task<BaseResponseDTO<bool>> ClearAllByUserId()
        {
            BaseResponseDTO<bool> BaseDTO = new BaseResponseDTO<bool>();
            try
            {
                List<NPF_NOT_Event> fnel = context.NPF_NOT_Event.Where(x => x.UserId == UM.UID.ToString() && x.DeleteStatus == false).ToList();
                foreach (NPF_NOT_Event fne in fnel)
                {
                    fne.DeleteStatus = true;
                    context.NPF_NOT_Event.Update(fne);
                    context.SaveChanges();
                }
                BaseDTO.Data = true;
                BaseDTO.QryResult = queryResult.SUCEEDED;
                return BaseDTO;
            }
            catch (Exception ex)
            {
                BaseDTO.Data = false;
                BaseDTO.QryResult = queryResult.SUCEEDED;
                return BaseDTO;
            }
        }
        public async Task<BaseResponseDTO<bool>> ClearById(long Id)
        {
            BaseResponseDTO<bool> BaseDTO = new BaseResponseDTO<bool>();
            try
            {
                NPF_NOT_Event fne = context.NPF_NOT_Event.Where(x => x.Id == Id && x.DeleteStatus == false).FirstOrDefault();
                if (fne != null)
                {
                    fne.DeleteStatus = true;
                    context.NPF_NOT_Event.Update(fne);
                    context.SaveChanges();

                }

                BaseDTO.Data = true;
                BaseDTO.QryResult = queryResult.SUCEEDED;
                return BaseDTO;
            }
            catch (Exception ex)
            {
                BaseDTO.Data = false;
                BaseDTO.QryResult = queryResult.SUCEEDED;
                return BaseDTO;
            }
        }

    }
}
