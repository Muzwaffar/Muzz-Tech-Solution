using BL.Constants;
using BL.Models.Notification;
using DAL.Common;
using DAL.DBContext;
using DAL.Models;
using DAL.Models.Common;
using DAL.Models.Transaction;
using Microsoft.AspNetCore.Identity;
using Org.BouncyCastle.Pqc.Crypto.Lms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Services.Common
{
    public class NotificationHandlerService
    {
        private NPFContext context;
        private QueryResult queryResult;
        private NotificationService service;
        public UserManager<ApplicationUser> UserManager;
        public NotificationHandlerService(Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> userManager)
        {
            context = new NPFContext();
            queryResult = new QueryResult();
            service = new NotificationService();
            UserManager = userManager;

        }
        public NotificationHandlerService()
        {
            context = new NPFContext();
            queryResult = new QueryResult();
            service = new NotificationService();

        }
        public async Task<Boolean> UnotNotification(PushNotif dt)
        {
            using StreamWriter file = new("logNotif.txt");
            NotificationDTO notif = new NotificationDTO();
            List<string> msgs = new List<string>();
            bool resp = false;
            string Uid = "";
            string Description = "";
            try
            {
                switch (dt.Type)
                {
                    case NotifType.Push:
                        NPF_TRN_Incident sch = context.NPF_TRN_Incident.Where(x => x.Id == dt.Id).FirstOrDefault();
                        Uid = context.NPF_SYS_User.Where(x => x.Id == dt.ResId).FirstOrDefault().AId.ToString();
                        string Title = "";
                        switch (dt.MType)
                        {
                            case MessageType.Txt:
                                string[] mslU = new string[] { sch.IncidentNo + " - ", "New Chat Received." };
                                foreach (string ms in mslU)
                                {
                                    msgs.Add(ms);
                                }
                                Title = "New Chat Received.";
                                Description = "New Chat Received.";
                                break;
                            case MessageType.REQ:
                                string[] msl = new string[] { sch.IncidentNo + " - ",dt.Text,dt.Text2 };
                                foreach (string ms in msl)
                                {
                                    msgs.Add(ms);
                                }
                                Title = "CR Request";
                                Description = dt.Text + " " + dt.Text2;
                                break;
                            default:
                                break;

                        }


                        notif.UserId = Uid;
                        notif.ClientId = sch.Id.ToString();
                        notif.ClientEmail = "";//cs.NAA.Email;
                        notif.ClientPhoneNumber = "";//cs.NAA.MobileNo;
                        notif.ScheduleId = sch.Id.ToString();
                        notif.ScheduleName = sch.IncidentNo;
                        notif.ServiceeId = sch.Id.ToString();
                        notif.ServiceName = "";//cs.NAA.FullName;
                        notif.DateTime = DateTime.Now;///
                        notif.Type = NotifType.Push.ToString();
                        notif.Description = Description;
                        notif.Title = Title;
                        notif.Message = msgs;
                        notif.Attempts = 0;
                        notif.Sent = 0;
                        break;

                    case NotifType.Sms:

                        break;

                    case NotifType.Email:

                        break;

                    default:
                        break;

                }
                resp = await service.UnotSentNotification(notif);

            }
            catch (Exception ex)
            {
                file.WriteLine(ex);
                resp = false;
            }

            return resp;
        }
        public async Task<Boolean> Notification(PushNotif dt)
        {
            using StreamWriter file = new("logNotif.txt");
            NotificationDTO notif = new NotificationDTO();
            List<string> msgs = new List<string>();
            bool resp = false;
            string Uid = "";
            try
            {
                switch (dt.Type)
                {
                    case NotifType.Push:
                        NPF_TRN_Incident sch = context.NPF_TRN_Incident.Where(x => x.Id == dt.Id).FirstOrDefault();
                        Uid = context.NPF_SYS_User.Where(x => x.Id == dt.ResId).FirstOrDefault().AId.ToString();
                        string Title = "";
                        switch (dt.MType)
                        {
                            case MessageType.Assigned:
                                string[] mslA = new string[] { sch.IncidentNo + " - ", "Has Been Assigned To You." };
                                foreach (string ms in mslA)
                                {
                                    msgs.Add(ms);
                                }
                                Title = "Incident Assigned";
                                break;
                            case MessageType.UnAssigned:
                                string[] mslU = new string[] { sch.IncidentNo + " - ", "Has Been Unassigned." };
                                foreach (string ms in mslU)
                                {
                                    msgs.Add(ms);
                                }
                                Title = "Incident UnAssigned";
                                break;
                            case MessageType.Status:
                                string[] mslS = new string[] { sch.IncidentNo + " - ", "Status Changed to :- ", sch.IncidentStatus.ToString() + "." };
                                foreach (string ms in mslS)
                                {
                                    msgs.Add(ms);
                                }
                                Title = "Incident Status Change";
                                break;
                            case MessageType.Changes:
                                string[] mslUP = new string[] { dt.Text + " - ",dt.Text2 + " - ", " Has Been Updated." };
                                foreach (string ms in mslUP)
                                {
                                    msgs.Add(ms);
                                }                                
                                Title = dt.Text + " - "+dt.Text2 + " - "+" Has Been Updated.";
                                break;
                            default:
                                break;

                        }


                        notif.UserId = Uid;
                        notif.ClientId = sch.Id.ToString();
                        notif.ClientEmail = "";//cs.NAA.Email;
                        notif.ClientPhoneNumber = "";//cs.NAA.MobileNo;
                        notif.ScheduleId = sch.Id.ToString();
                        notif.ScheduleName = sch.IncidentNo;
                        notif.ServiceeId = sch.Id.ToString();
                        notif.ServiceName = "";//cs.NAA.FullName;
                        notif.DateTime = DateTime.Now;///
                        notif.Type = NotifType.Push.ToString();
                        notif.Description = "Description";
                        notif.Title = Title;
                        notif.Message = msgs;
                        notif.Attempts = 0;
                        notif.Sent = 0;
                        break;

                    case NotifType.Sms:

                        break;

                    case NotifType.Email:

                        break;

                    default:
                        break;

                }
               resp = await service.SentNotification(notif, UserManager);

            }
            catch (Exception ex)
            {
                file.WriteLine(ex);
                resp = false;
            }

            return resp;
        }
    }
}
