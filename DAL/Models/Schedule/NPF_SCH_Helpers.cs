using DAL.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models.Schedule
{
    public class NPF_SCH_Helpers : BaseAuditModel, IAuditable
    {
        [Key]
        public long Id { get; set; }
        public long ResourceId { get; set; }
        public long ScheduleId { get; set; }
        public HelperStatus status { get; set; } = HelperStatus.Pending;
    }
}
