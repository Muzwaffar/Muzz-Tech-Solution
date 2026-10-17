using BL.Constants;
using BL.Models.Access;
using BL.Models.Common;
using BL.Models.Modules.Research;
using BL.Models.Notification;
using DAL.Common;
using DAL.DBContext;
using DAL.Models;
using DAL.Models.Administration;
using DAL.Models.Common;
using DAL.Models.Notifications;
using DAL.Models.Transaction;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Identity;
using NPOI.XWPF.UserModel;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace BL.Services.Common
{
    public class NotificationService
    {
        private NPFContext context;//Declare Context
        private QueryResult queryResult;//Declare QueryResult
        private MatrixService Mservice;//Decalre MatrixService
        private EventService Event;
        public UserMatrixDTO UM = new UserMatrixDTO();//Initialise UserMatrixDTO
        public NotificationService() //Initialised Constructer
        {
            context = new NPFContext();// Initialization of context
            queryResult = new QueryResult(); ;//Constant of Query result
            Mservice = new MatrixService();//Service Initialization of matrix access
            Event = new EventService();
            UM = Mservice.sGetMatrixUserByUserID();//Service All user's matrix access into UM
        }
        public async Task<BaseResponseDTO<bool>> SendNotification(Notification dt)
        {
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
            NPF_NOT_Event ev = new NPF_NOT_Event();
            BaseResponseDTO<bool> Bsdt = new BaseResponseDTO<bool>();
            string CreatedBy = string.Empty;
            try
            {
                string Name = string.Empty;
                try
                {
                    Name = context.Users.Where(x => x.Id == UM.AUID).FirstOrDefault().FullName;
                    CreatedBy = UM.AUID;
                }
                catch (Exception ex)
                {
                    Name = context.Users.Where(x => x.Id == dt.AUserId).FirstOrDefault().FullName;
                    CreatedBy = dt.AUserId;
                }
                NPF_TRN_Incident sch = context.NPF_TRN_Incident.Where(x => x.Id == dt.Id).FirstOrDefault();
                string txt = string.Empty;
                string title = string.Empty;
                switch (dt.MType)
                {
                    case MessageType.Assigned:
                        txt = dt.TaskName + " Has Been Assigned To You By " + Name;
                        title = dt.TaskName + " Task Assigned";
                        break;
                    case MessageType.UnAssigned:
                        txt = Name + " Has unassigned You From " + dt.TaskName;
                        title = dt.TaskName + " Task UnAssigned";
                        break;
                    case MessageType.Status:
                        txt = Name + " Has Made Changes To " + dt.TaskName;
                        title = dt.TaskName + " Status Change To:-" + sch.IncidentStatus.ToString();
                        break;
                    case MessageType.Pending:
                        txt = dt.TaskName;
                        title = "Acknowledgement Pending";
                        break;
                    case MessageType.Ack:
                        txt = dt.TaskName;
                        title = "Acknowledged";
                        break;
                    case MessageType.Changes:
                        txt = dt.TaskName;
                        title = "Update";
                        break;
                    case MessageType.Txt:
                        txt = dt.TaskName;
                        title = "New Chat";
                        break;
                    case MessageType.CRR:
                        txt = dt.TaskName;
                        title = "Request To CR";
                        break;
                    default:
                        break;

                }

                ev = new NPF_NOT_Event()
                {
                    UserId = dt.UserId.ToString(),
                    ResourceName = Name,
                    ResourceId = UM.AUID,
                    Title = title,
                    Detail = txt,
                    AddDetail = dt.MName.ToString(),
                    TargetType = TargetType.Redirect,
                    Target = dt.URL,
                    TargetId = dt.Id,
                    Entity = dt.MName.ToString(),
                    Priority = Priority.Low,
                    Colour = "#2870EA",
                    Image = "",
                    IsNotified = false,
                    ActionDate = DateTime.Now,
                    IsRead = false,
                    PopUp = dt.PopUp,
                    CreatedBy = Guid.Parse(CreatedBy),
                    CreatedDate = DateTime.Now,
                };
                if (dt.ResId != null && ev.ResourceId == null)
                {
                    ev.ResourceId = dt.ResId.ToString();
                }
                if (ev.ResourceId == null)
                {
                    ev.ResourceId = dt.AUserId.ToString();
                }
                Bsdt = await Event.SaveEvent(ev);
                BaseDto.Data = true;
                BaseDto.QryResult = queryResult.SUCEEDED;

            }
            catch (Exception ex)
            {

            }
            return BaseDto;
        }

        //save or update usermobile tables 
        private static readonly string FirebaseApiUrl = "https://fcm.googleapis.com/v1/projects/fsm-mobile-ab264/messages:send";

        // Path to your service account JSON key file
        private static readonly string ServiceAccountKeyPath = "fsm-mobile-ab264-firebase-adminsdk-m60v9-16e967fa00.json";
        public BaseResponseDTO<bool> NotificationUserMobile(BaseRequestDTO<UserMobileDTO> usermbobiles)
        {
            BaseResponseDTO<bool> baseDto = new BaseResponseDTO<bool>();

            try
            {
                ApplicationUser User = context.Users.Where(x => x.Id == usermbobiles.Data.Id).FirstOrDefault();
                if (usermbobiles.Data.fireBaseToken == null || usermbobiles.Data.fireBaseToken == "" || usermbobiles.Data.fireBaseToken == " ")
                {
                    baseDto.Data = false;
                    baseDto.QryResult = queryResult.FAILED;
                    return baseDto;
                }
                NPF_NOT_UserMobiles NPF_NOT_UserMobiles = new NPF_NOT_UserMobiles();

                if (!context.NPF_NOT_UserMobiles.Any(x => x.User.Id == usermbobiles.Data.Id && x.FirebaseToken == usermbobiles.Data.fireBaseToken && x.DeleteStatus == false)) //Add
                {
                    context.Users.Update(User);
                    NPF_NOT_UserMobiles.User = User;
                    NPF_NOT_UserMobiles.FirebaseToken = usermbobiles.Data.fireBaseToken;
                    NPF_NOT_UserMobiles.LastLogin = DateTime.Now;
                    NPF_NOT_UserMobiles.LastLogout = DateTime.Now;
                    NPF_NOT_UserMobiles.CreatedDate = DateTime.Now;
                    NPF_NOT_UserMobiles.CreatedBy = Guid.Parse(usermbobiles.userToken.ToString());
                    context.NPF_NOT_UserMobiles.Add(NPF_NOT_UserMobiles);
                    context.SaveChanges();
                }
                else //Update
                {
                    NPF_NOT_UserMobiles = new NPF_NOT_UserMobiles();
                    NPF_NOT_UserMobiles = context.NPF_NOT_UserMobiles.Where(x => x.User.Id == usermbobiles.Data.Id && x.FirebaseToken == usermbobiles.Data.fireBaseToken).FirstOrDefault();
                    NPF_NOT_UserMobiles.FirebaseToken = usermbobiles.Data.fireBaseToken;
                    NPF_NOT_UserMobiles.LastLogin = DateTime.Now;
                    NPF_NOT_UserMobiles.DeleteStatus = false;
                    NPF_NOT_UserMobiles.CreatedDate = DateTime.Now;
                    NPF_NOT_UserMobiles.CreatedBy = Guid.Parse(usermbobiles.userToken.ToString());
                    context.NPF_NOT_UserMobiles.Update(NPF_NOT_UserMobiles);
                    context.SaveChanges();
                }

                baseDto.Data = true;
                baseDto.QryResult = queryResult.SUCEEDED;
            }
            catch (Exception ex)
            {
                baseDto.Data = false;
                baseDto.QryResult = queryResult.FAILED;
            }
            return baseDto;

        }


        //get notification 
        public BaseResponseDTO<List<NotificationDTO>> getNotification(NotificationReportDTO dt)
        {
            BaseResponseDTO<List<NotificationDTO>> baseDto = new BaseResponseDTO<List<NotificationDTO>>();
            try
            {
                if (!dt.dtStart.HasValue)
                {
                    dt.dtStart = DateTime.Now.AddHours(-24);
                }
                if (!dt.dtEnd.HasValue)
                {
                    dt.dtEnd = DateTime.Now;
                }

                List<NotificationDTO> lNotifs = (from header in context.NPF_NOT_NotificationHeader
                                                 join details in context.NPF_NOT_NotificationDetails on header.Id equals details.notifcationHeader.Id
                                                 where header.UserId == dt.UserId && details.IsMobileNotification == true
                                                 && details.Sent == 1 && header.DateTime >= dt.dtStart && header.DateTime <= dt.dtEnd
                                                 select new NotificationDTO
                                                 {
                                                     NotificationId = header.Id,
                                                     Description = header.Description,
                                                     DateTime = header.DateTime,
                                                     ScheduleId = header.ScheduleId.ToString(),
                                                     ScheduleName = header.ScheduleName,
                                                     ServiceeId = header.ServiceId,
                                                     ServiceName = header.ServiceName,
                                                     UserId = header.UserId,
                                                     UserDetails = header.UserDetails,
                                                     Type = header.Type,
                                                     NotificationBatchId = header.NotificationBatchId

                                                 }).ToList();

                int Rows = Convert.ToInt32(context.NPF_SYS_GlobalParam.Where(x => x.Name == "NotificationRows").FirstOrDefault().Value);
                lNotifs = lNotifs.GroupBy(p => p.NotificationId).Select(g => g.First()).ToList();
                lNotifs = lNotifs.GroupBy(p => p.NotificationBatchId).Select(g => g.First()).OrderByDescending(xx => xx.DateTime).ToList();
                baseDto.Data = lNotifs.Take(Rows).ToList();
                baseDto.TotalItems = baseDto.Data.Count();
                baseDto.QryResult = new QueryResult().SUCEEDED;
                return baseDto;
            }
            catch (Exception ex)
            {
                baseDto.Data = new List<NotificationDTO>();
                baseDto.TotalItems = 0;
                baseDto.QryResult = new QueryResult().FAILED;
                return baseDto;
            }
        }
        //sent notification email,sms,push and save to notification header and details
        public async Task<Boolean> UnotSentNotification(NotificationDTO notif)
        {
            Boolean bSent = false;
            using StreamWriter file = new("logSendnotif.txt");
            try
            {
                int NotificationBatchId = Convert.ToInt32(context.NPF_SYS_GlobalParam.Where(x => x.Name == "BatchNotification").FirstOrDefault().Value);
                NPF_SYS_GlobalParam gp = context.NPF_SYS_GlobalParam.Where(x => x.Name == "BatchNotification").FirstOrDefault();
                gp.Value = (NotificationBatchId + 1).ToString();
                context.NPF_SYS_GlobalParam.Update(gp);
                context.SaveChanges();
                List<NPF_NOT_UserMobiles> uml = context.NPF_NOT_UserMobiles.Where(x => x.User.Id == notif.UserId && x.DeleteStatus == false).GroupBy(p => p.FirebaseToken).Select(g => g.First()).ToList();
                if (uml.Count > 0)
                {
                    foreach (NPF_NOT_UserMobiles um in uml)
                    {
                        if (notif.Type.ToLower() == "push") //Push Notification
                        {
                            PushNotificationDTO pushnotification = new PushNotificationDTO();
                            pushnotification.TagMsg = notif.Title;
                            pushnotification.DeviceId = um.FirebaseToken;

                            for (int i = 0; i < notif.Message.Count; i++)
                            {
                                pushnotification.Message += notif.Message[i];
                                pushnotification.Message += Environment.NewLine;
                            }


                            string status = await PushNotification(pushnotification);


                            FireBaseReponseDTO fireBaseReponsedto = JsonSerializer.Deserialize<FireBaseReponseDTO>(status);

                            if (fireBaseReponsedto.success == 1)
                            {
                                //save to notif table with status sent 1 and attempts 1
                                notif.Sent = 1;
                                notif.Attempts = 1;
                                NotificationDTO notifNew = new NotificationDTO();
                                notifNew = notif;
                                notifNew.Title = notif.Description;
                                BaseResponseDTO<Boolean> bsave = SaveNotification(notif, NotificationBatchId);
                                bSent = true;
                            }


                            if (fireBaseReponsedto.failure == 1)
                            {
                                //save to notif table with status sent 0 and attempts 5
                                notif.Sent = 0;
                                notif.Attempts = 1;
                                BaseResponseDTO<Boolean> bsave = SaveNotification(notif, NotificationBatchId);
                                bSent = true;

                            }

                        }
                        else if (notif.Type.ToLower() == "sms") //SMS Notification
                        {
                            bSent = true;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                file.WriteLine(ex);
            }

            return bSent;
        }
        public async Task<Boolean> SentNotification(NotificationDTO notif, UserManager<ApplicationUser> userManager)
        {
            Boolean bSent = false;
            using StreamWriter file = new("logSendnotif.txt");
            try
            {
                int NotificationBatchId = Convert.ToInt32(context.NPF_SYS_GlobalParam.Where(x => x.Name == "BatchNotification").FirstOrDefault().Value);
                NPF_SYS_GlobalParam gp = context.NPF_SYS_GlobalParam.Where(x => x.Name == "BatchNotification").FirstOrDefault();
                gp.Value = (NotificationBatchId + 1).ToString();
                context.NPF_SYS_GlobalParam.Update(gp);
                context.SaveChanges();
                List<NPF_NOT_UserMobiles> uml = context.NPF_NOT_UserMobiles.Where(x => x.User.Id == notif.UserId && x.DeleteStatus == false).GroupBy(p => p.FirebaseToken).Select(g => g.First()).ToList();
                if (uml.Count > 0)
                {
                    foreach (NPF_NOT_UserMobiles um in uml)
                    {

                        if (notif.Type.ToLower() == "push") //Push Notification
                        {
                            PushNotificationDTO pushnotification = new PushNotificationDTO();
                            pushnotification.TagMsg = notif.Title;
                            pushnotification.DeviceId = um.FirebaseToken;


                            for (int i = 0; i < notif.Message.Count; i++)
                            {
                                pushnotification.Message += notif.Message[i];
                                pushnotification.Message += Environment.NewLine;
                            }


                            string status = await PushNotification(pushnotification);


                            //   FireBaseReponseDTO fireBaseReponsedto = JsonSerializer.Deserialize<FireBaseReponseDTO>(status);

                            if (status.ToLower().Contains("success"))
                            {
                                //save to notif table with status sent 1 and attempts 1
                                notif.Sent = 1;
                                notif.Attempts = 1;
                                BaseResponseDTO<Boolean> bsave = SaveNotification(notif, NotificationBatchId);
                                bSent = true;
                            }
                            else
                            {
                                //save to notif table with status sent 0 and attempts 5
                                notif.Sent = 0;
                                notif.Attempts = 1;
                                BaseResponseDTO<Boolean> bsave = SaveNotification(notif, NotificationBatchId);
                                bSent = true;

                            }

                        }
                        else if (notif.Type.ToLower() == "email") //Email Notification
                        {
                            EmailSenderModel<dynamic> esm = new EmailSenderModel<dynamic>();
                            esm.To = new List<string>() { notif.ClientEmail };
                            esm.Subject = notif.Title;
                            esm.From = "noreply@naveo.mu";
                            esm.Body = notif.Message;

                            var aaa = await new EmailSender().SendEmail(esm, userManager);

                            if (aaa.ToLower() == "success")
                                bSent = true;
                            else
                                bSent = false;
                        }
                        else if (notif.Type.ToLower() == "sms") //SMS Notification
                        {
                            bSent = true;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                file.WriteLine(ex);
            }

            return bSent;
        }
        //save to notification header and details
        public BaseResponseDTO<bool> SaveNotification(NotificationDTO notif, int NotificationBatchId)
        {
            BaseResponseDTO<bool> baseDto = new BaseResponseDTO<bool>();

            try
            {

                NPF_NOT_NotificationHeader fSM_NOT_NotificationHeader = new NPF_NOT_NotificationHeader();
                fSM_NOT_NotificationHeader.Description = notif.Title;//notif.Description;
                fSM_NOT_NotificationHeader.DateTime = notif.DateTime;
                fSM_NOT_NotificationHeader.ScheduleId = notif.ScheduleId == null ? 0 : Convert.ToInt64(notif.ScheduleId);
                fSM_NOT_NotificationHeader.ScheduleName = notif.ScheduleName;
                fSM_NOT_NotificationHeader.ServiceId = notif.ServiceeId;
                fSM_NOT_NotificationHeader.ServiceName = notif.ServiceName;
                fSM_NOT_NotificationHeader.UserId = notif.UserId;
                fSM_NOT_NotificationHeader.UserDetails = notif.UserDetails;
                fSM_NOT_NotificationHeader.ClientDetails = notif.ClientDetails == null ? "" : notif.ClientDetails;
                fSM_NOT_NotificationHeader.ClientId = notif.ClientId == null ? 0 : Convert.ToInt64(notif.ClientId);
                fSM_NOT_NotificationHeader.ClientPhoneNumber = notif.ClientPhoneNumber;
                fSM_NOT_NotificationHeader.ClientEmail = notif.ClientEmail == null ? "" : notif.ClientEmail;
                fSM_NOT_NotificationHeader.Type = notif.Type;
                fSM_NOT_NotificationHeader.NotificationBatchId = NotificationBatchId;
                context.Add(fSM_NOT_NotificationHeader);
                context.SaveChanges();

                NPF_NOT_NotificationDetails fSM_NOT_NotificationDetails = new NPF_NOT_NotificationDetails();
                NPF_NOT_NotificationHeader fnot = context.NPF_NOT_NotificationHeader.Where(x => x.Id == fSM_NOT_NotificationHeader.Id).FirstOrDefault();
                if (fSM_NOT_NotificationHeader.Type.ToLower() == "push")
                {
                    fSM_NOT_NotificationDetails.IsMobileNotification = true;
                    fSM_NOT_NotificationDetails.notifcationHeader = fnot;
                    fSM_NOT_NotificationDetails.Attempts = notif.Attempts;
                    fSM_NOT_NotificationDetails.Sent = notif.Sent;
                    fSM_NOT_NotificationDetails.CreatedDate = DateTime.Now;
                    
                    fSM_NOT_NotificationDetails.UserMobileId = context.NPF_NOT_UserMobiles.Where(x => x.User.Id == fSM_NOT_NotificationHeader.UserId).FirstOrDefault().Id;

                }


                if (fSM_NOT_NotificationHeader.Type.ToLower() == "email")
                {
                    fSM_NOT_NotificationDetails.IsMobileNotification = false;
                    fSM_NOT_NotificationDetails.notifcationHeader = fnot;
                    fSM_NOT_NotificationDetails.Attempts = notif.Attempts;
                    fSM_NOT_NotificationDetails.Sent = notif.Sent;

                }



                if (fSM_NOT_NotificationHeader.Type.ToLower() == "sms")
                {
                    fSM_NOT_NotificationDetails.IsMobileNotification = false;
                    fSM_NOT_NotificationDetails.notifcationHeader = fnot;
                    fSM_NOT_NotificationDetails.Attempts = notif.Attempts;
                    fSM_NOT_NotificationDetails.Sent = notif.Sent;

                }
                context.NPF_NOT_NotificationDetails.Add(fSM_NOT_NotificationDetails);
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

        public async Task<string> PushNotification(PushNotificationDTO obj)
        {
            string res = string.Empty;
            using StreamWriter file = new("PushNotification.txt");
            try
            {
                // Load the service account credentials
                GoogleCredential credential = GoogleCredential.FromFile(ServiceAccountKeyPath)
                    .CreateScoped("https://www.googleapis.com/auth/firebase.messaging");

                // Create the HTTP client
                using (HttpClient client = new HttpClient())
                {
                    // Get the OAuth2 token from service account credentials
                    var accessToken = await credential.UnderlyingCredential.GetAccessTokenForRequestAsync();

                    // Add authorization header with the Bearer token
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                    // Create the message payload
                    //var message = new

                    //{

                    //    to = obj.DeviceId,

                    //    notification = new

                    //    {

                    //        body = obj.Message,

                    //        title = obj.TagMsg,

                    //        icon = "myicon"

                    //    }
                    //};
                    // Create the message payload
                    var message = new
                    {
                        message = new
                        {
                            token = obj.DeviceId,  // The device registration token
                            notification = new
                            {
                                title = obj.TagMsg,
                                body = obj.Message
                            }
                        }
                    };
                    // Serialize the message to JSON
                    var jsonPayload = JsonSerializer.Serialize(message);

                    // Send the HTTP POST request to FCM
                    HttpContent content = new StringContent(jsonPayload, System.Text.Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PostAsync(FirebaseApiUrl, content);

                    if (response.IsSuccessStatusCode)
                    {
                        res = "Notification sent successfully!";
                    }
                    else
                    {
                        var responseBody = await response.Content.ReadAsStringAsync();
                        res = $"Error sending notification: {responseBody}";
                    }
                }
            }
            catch (Exception ex)
            {
                try
                {
                    string exx = " DeviceId=>" + obj.DeviceId + ", Message=>" + obj.Message.ToString() + ", TagMsg=>" + obj.TagMsg;
                    file.WriteLine(exx + "<<<====>>>" + ex);
                }
                catch (Exception e)
                {

                }

            }
            try
            {
                file.Close();
            }
            catch (Exception ex)
            {

            }
            return res;
        }

        public class FireBaseReponseDTO
        {
            public dynamic multicast_id { get; set; }
            public int success { get; set; }
            public int failure { get; set; }
            public int canonical_ids { get; set; }
        }
        public class PushNotificationDTO
        {

            public string Message { get; set; }

            public string TagMsg { get; set; }

            public string DeviceId { get; set; }

            public List<string> lEmails { get; set; } = new List<string>();

            public string emailTo { get; set; }

        }

        //push notification to mobile
        public string PushNotificationOld(PushNotificationDTO obj)
        {
            string reponse = string.Empty;
            using StreamWriter file = new("PushNotification.txt");
            try
            {
                //var applicationID = new ApplicationSettingsHandler().settings.firebase.GetValueOrDefault("applicationID");
                //var senderId = new ApplicationSettingsHandler().settings.firebase.GetValueOrDefault("senderId");

                string applicationID = context.NPF_SYS_GlobalParam.Where(x => x.Name == "ApplicationId").FirstOrDefault().Value;
                string senderId = context.NPF_SYS_GlobalParam.Where(x => x.Name == "senderId").FirstOrDefault().Value;

                //to take from driver table
                //obj.DeviceId = "ew-RFeGmREiRTiFf_yzOlO:APA91bG5CgY6Bz0MXVvgTeXfFhUavSUJiZRBTy3PCzA51IV3Nah0Yc-11DLDnlBuGQaSUOGNrDhS2hGBCzj3Q2jtiMM3NtfpxBi-S0wQ4_if8EHOQQu5p83fX0opQJ4KwBDv3g-DRKT2";

                WebRequest tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");

                tRequest.Method = "post";

                tRequest.ContentType = "application/json";

                var data = new

                {

                    to = obj.DeviceId,

                    notification = new

                    {

                        body = obj.Message,

                        title = obj.TagMsg,

                        icon = "myicon"

                    }
                };



                var json = JsonSerializer.Serialize(data);

                Byte[] byteArray = Encoding.UTF8.GetBytes(json);

                tRequest.Headers.Add(string.Format("Authorization: key={0}", applicationID));

                tRequest.Headers.Add(string.Format("Sender: id={0}", senderId));

                tRequest.ContentLength = byteArray.Length;


                using (Stream dataStream = tRequest.GetRequestStream())
                {

                    dataStream.Write(byteArray, 0, byteArray.Length);


                    using (WebResponse tResponse = tRequest.GetResponse())
                    {

                        using (Stream dataStreamResponse = tResponse.GetResponseStream())
                        {

                            using (StreamReader tReader = new StreamReader(dataStreamResponse))
                            {

                                String sResponseFromServer = tReader.ReadToEnd();

                                reponse = sResponseFromServer;


                            }
                        }
                    }
                }
            }

            catch (Exception ex)
            {

                string exx = " DeviceId=>" + obj.DeviceId + ", Message=>" + obj.Message.ToString() + ", TagMsg=>" + obj.TagMsg;
                file.WriteLine(exx + "<<<====>>>" + ex);
            }
            try
            {
                file.Close();
            }
            catch (Exception ex)
            {

            }
            return reponse;

        }
    }
}
