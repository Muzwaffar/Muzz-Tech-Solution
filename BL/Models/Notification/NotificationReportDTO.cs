using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Models.Notification
{
    public class NotificationReportDTO
    {

        public DateTime? dtStart { get; set; }
        public DateTime? dtEnd { get; set; }

        public int rows { get; set; }

        public string UserId { get; set; }
    }
}
