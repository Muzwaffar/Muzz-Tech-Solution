using DAL.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models.Administration
{
    public class NPF_SYS_UserDetails : BaseAuditModel, IAuditable
    {
        [Key]
        public long Id { get; set; }
        [ForeignKey("UserId")]
        public virtual NPF_SYS_User User { get; set; }
        public long? Type { get; set; }
        public long? Office { get; set; }
        public long? PostHeld { get; set; }
        public long? UserType { get; set; }
    }
}