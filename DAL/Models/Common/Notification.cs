using DAL.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models.Common
{
    public class Notification
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public string AUserId { get; set; }
        public long? ResId { get; set; }
        public MessageType MType { get; set; }
        public PageName MName { get; set; }
        public string TaskName { get; set; }
        public string URL { get; set; }
        public bool PopUp { get; set; } = false;

    }
    public class PushNotif
    {
        public long Id { get; set; }
        public long ResId { get; set; }
        public NotifType Type { get; set; }
        public MessageType MType { get; set; }
        public string Text { get; set; } = string.Empty;
        public string Text2 { get; set; } = string.Empty;
    }
}
