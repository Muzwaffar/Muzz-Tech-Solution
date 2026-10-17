using DAL.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models.Schedule
{
    public class NPF_SCH_SheduleWOChecklist : BaseAuditModel, IAuditable
    {
        [Key]
        public long Id { get; set; }
        public long ScheduleId { get; set; }
        public long WorkorderId { get; set; }
        public long ElementId { get; set; }
        public string ElementName { get; set; }
        public string ElementType { get; set; }
        public string Value { get; set; }
        public string? AdditionalValue { get; set; } = string.Empty;
        public bool? CheckListValue { get; set; }
        public Status Status { get; set; } = Status.Draft;

        public string UOM { get; set; } = String.Empty;

    }

}
