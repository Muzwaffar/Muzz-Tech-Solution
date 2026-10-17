using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models.Schedule
{
    public class NPF_SCH_Remarks : BaseAuditModel, IAuditable
    {
        [Key]
        public long Id { get; set; }
        public long ScheduleId { get; set; }
        public long? WorkOrderId { get; set; }
        public string Remarks { get; set; }
        public string Type { get;set; }=string.Empty;
    }
}
