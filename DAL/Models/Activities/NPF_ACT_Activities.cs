using DAL.Common;
using DAL.Models.Administration;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models.Activities
{
    public class NPF_ACT_Activities : BaseAuditModel, IAuditable
    {
        [Key]
        public long Id { get; set; }
        [ForeignKey("ActivityTypeId")]
        public virtual NPF_ACT_ActivityEventType ActivityType { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Remarks { get; set; }
        public string? URL { get; set; }
        public GeneralStatus Status { get; set; }
        public long User { get; set; }
        public long? MUserId { get; set; }
        public long? ParentId { get; set; }
        public AccessLevel Access { get; set; } = AccessLevel.Matrix;
    }
}
