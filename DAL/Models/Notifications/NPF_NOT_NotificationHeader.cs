using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models.Notifications
{
    public class NPF_NOT_NotificationHeader : BaseAuditModel, IAuditable
    {

        [Key]
        public long Id { get; set; }

        public String? Description { get; set; }

        public DateTime DateTime { get; set; }


        public long ScheduleId { get; set; }

        [Column(TypeName = "varchar(500)")]
        public String? ScheduleName { get; set; }


        public String? ServiceId { get; set; }

        [Column(TypeName = "varchar(500)")]
        public String? ServiceName { get; set; }


        public String? UserId { get; set; }

        [Column(TypeName = "varchar(500)")]
        public String? UserDetails { get; set; }


        public long? ClientId { get; set; }

        [Column(TypeName = "varchar(500)")]
        public String? ClientPhoneNumber { get; set; }

        public String? ClientEmail { get; set; }

        [Column(TypeName = "varchar(500)")]
        public String? ClientDetails { get; set; }

        public String? Type { get; set; }
        public long? NotificationBatchId { get; set; }



    }
}
