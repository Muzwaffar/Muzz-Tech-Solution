using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models.Notifications
{
    public class NPF_NOT_UserMobiles : BaseAuditModel, IAuditable
    {
        [Key]
        public long Id { get; set; }

        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }

        [StringLength(200)]
        public string FirebaseToken { get; set; }

        public DateTime LastLogin { get; set; }

        public DateTime LastLogout { get; set; }

    }
}
