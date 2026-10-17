using BL.Constants;
using BL.Models.Access;
using BL.Models.Common;
using BL.Models.Store;
using BL.Models.Transaction;
using BL.Services.Common;
using DAL.Common;
using DAL.DBContext;
using DAL.Models;
using DAL.Models.Common;
using DAL.Models.Store;
using DAL.Models.Transaction;
using Microsoft.AspNetCore.Identity;
using NPOI.POIFS.Crypt.Dsig;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Services.Transaction
{
    public class ChatService
    {
        private NPFContext context;
        private QueryResult queryResult;

        public ChatService()
        {
            context = new NPFContext();
            queryResult = new QueryResult();
        }
        public async Task<BaseResponseDTO<List<ChatBaseDTO>>> GetChatById(BaseRequestDTO<long> Data)
        {
            BaseResponseDTO<List<ChatBaseDTO>> dto = new BaseResponseDTO<List<ChatBaseDTO>>();
            List<ChatBaseDTO> result = new List<ChatBaseDTO>();
            string errorMsg = "No Data Found";
            try
            {
                long UID = context.NPF_SYS_User.Where(x => x.AId.ToLower() == Data.userToken.ToLower()).FirstOrDefault().Id;
                result = (from a in context.NPF_TRN_IncidentChat
                          where a.DeleteStatus == false && a.IncidentId == Data.Data
                          select new ChatBaseDTO
                          {
                              Id = a.Id,
                              Name = a.Name,
                              Text = a.Text,
                              TimeStamp = a.TimeStamp,
                              IsSender = a.UserId == UID ? true : false,
                          }).OrderBy(x => x.Id).ToList();
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
        public async Task<BaseResponseDTO<bool>> Save(BaseRequestDTO<ChatSaveDTO> DataS,bool IsMobile= false)
        {
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
            try
            {
                long UID = context.NPF_SYS_User.Where(x => x.AId.ToLower() == DataS.userToken.ToLower()).FirstOrDefault().Id;
                string Name = context.Users.Where(x => x.Id.ToLower() == DataS.userToken.ToLower()).FirstOrDefault().FullName;
                string Rank = string.Empty;
                try
                {
                    Rank = context.NPF_SYS_Resource.Where(x => x.UserId == UID && x.DeleteStatus == false).FirstOrDefault().Rank.Name;

                }
                catch (Exception ex)
                {

                }
                string FName = Name + " - " + Rank;
                string TmpS = DateTime.Now.ToString("dd/MM/yy") + " " + DateTime.Now.ToString("HH:mm");
                NPF_TRN_IncidentChat IC = new NPF_TRN_IncidentChat()
                {
                    UserId = UID,
                    Name = FName,
                    Text = DataS.Data.Text,
                    TimeStamp = TmpS,
                    IncidentId = DataS.Data.IncidentId,
                    CreatedBy = Guid.Parse(DataS.userToken.ToString()),
                    CreatedDate = DateTime.Now
                };
                context.NPF_TRN_IncidentChat.Add(IC);
                context.SaveChanges();
                NPF_TRN_Incident inc = context.NPF_TRN_Incident.Where(x => x.Id == DataS.Data.IncidentId).FirstOrDefault();
                List<long> StationIds = new List<long>();
                List<long> UsersIds = new List<long>();
                List<long> MobUsersIds = new List<long>();
                List<string> stalist = new List<string>();

                Notification notif = new Notification()
                {
                    Id = inc.Id,
                    UserId = 0,
                    MType = MessageType.Txt,
                    MName = PageName.Incident,
                    TaskName = "Incident No: " + inc.IncidentNo + " New Chat Received.",
                    URL = "/IncidentHandling/AddEdit?Id=" + inc.Id + "&Code=" + inc.Code + "&Field=chat#scrollspy-chat",
                    AUserId = DataS.userToken.ToLower()

                };
                StationIds.Add(context.NPF_TRN_IncidentStation.Where(x => x.IncidentId == inc.Id && x.DeleteStatus == false).FirstOrDefault().StationId);
                StationIds.AddRange(context.NPF_TRN_IncidentOtherStation.Where(x => x.IncidentId == inc.Id && x.DeleteStatus == false).Select(y => y.OtherStationId).ToList());
                //Start Changed for CRO 20241112
                StationIds.AddRange(context.NPF_SYS_Station.Where(x => x.Name == "Control Room" && x.DeleteStatus == false).Select(y => y.Id).ToList());
                //End Changed for CRO 20241112
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
                            Id = inc.Id,
                            Type = NotifType.Push,
                            ResId = user,
                            MType = MessageType.Txt
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
            catch (Exception ex)
            {
                BaseDto.Data = false;
                BaseDto.AdditionalData = "Error Adding Record";
                BaseDto.QryResult = queryResult.FAILED;
            }
            return BaseDto;
        }
    }
}
