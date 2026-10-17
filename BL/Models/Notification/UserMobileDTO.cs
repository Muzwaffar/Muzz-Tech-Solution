using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Models.Notification
{
    public class UserMobileDTO
    {
        public string Id { get; set; }

        public string fireBaseToken { get; set; }

        public DateTime lastlogin { get; set; }

        public DateTime lastlogout { get; set; }

    }
}
