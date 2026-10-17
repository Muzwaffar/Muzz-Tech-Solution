using DAL.Common;
using DAL.Models.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Models.Notification
{
    public class NotificationDTO
    {
        public long NotificationId { get; set; }
        public long NotificationStatusId { get; set; }
        public string UserId { get; set; }
        public string UserDetails { get; set; } = String.Empty;
        public string ClientId { get; set; }
        public string ClientDetails { get; set; } = String.Empty;
        public string ClientEmail { get; set; }
        public string ClientPhoneNumber { get; set; }
        public string ScheduleId { get; set; }
        public string ScheduleName { get; set; }
        public Status status { get; set; }
        public string ServiceeId { get; set; }
        public string ServiceName { get; set; }
        public DateTime DateTime { get; set; }
        public string Type { get; set; } //Email,Push ,Sms
        public string Description { get; set; }

        public int Attempts { get; set; }

        public int Sent { get; set; }
        public long? NotificationBatchId { get; set; }

        //format to sent to destination

        public string Title { get; set; } //subject for email ,title for push notifiction

        public List<string> Message { get; set; } //body for email ,notif for push notification
    }
    public class GetEvent
    {
        public bool Notify { get; set; }
        public bool ShowBadge { get; set; }
        public int CountUnread { get; set; }
        public List<NPF_NOT_Event> Events { get; set; }
        public List<NPF_NOT_Event> PopEvents { get; set; }
    }
    public class EventCenter
    {
        public long Id { get; set; }
        public string UserId { get; set; }
        public string ResourceName { get; set; }
        public string ResourceId { get; set; }
        public string Title { get; set; }
        public string Detail { get; set; }
        public string AddDetail { get; set; }
        public string Target { get; set; }
        public TargetType TargetType { get; set; }
        public long TargetId { get; set; }
        public Priority Priority { get; set; }
        public string Colour { get; set; }
        public string Image { get; set; }
        public DateTime ActionDate { get; set; }
        public bool IsRead { get; set; } = false;
        public bool IsNotified { get; set; } = false;
    }
}
