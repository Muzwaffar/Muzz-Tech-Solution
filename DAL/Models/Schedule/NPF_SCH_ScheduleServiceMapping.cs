using DAL.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models.Schedule
{
    public class NPF_SCH_ScheduleServiceMapping : BaseAuditModel, IAuditable
    {
        [Key]
        public long Id { get; set; }
        public long ScheduleId { get; set; }
        public long WorkorderId { get; set; }
        public string? Reason { get; set; } = string.Empty;
        public Status Status { get; set; } = Status.Draft;
        public DateTime? CompletedTime { get; set; }
        public DateTime? AcceptedTime { get; set; }
        public DateTime? RejectedTime { get; set; }
        public DateTime? StartedTime { get; set; }
        public DateTime? StoppedTime { get; set; }
    }
}
