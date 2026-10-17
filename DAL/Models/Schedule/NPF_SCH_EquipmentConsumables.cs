using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models.Schedule
{
    public class NPF_SCH_EquipmentConsumables : BaseAuditModel, IAuditable
    {
        [Key]
        public long Id { get; set; }
        public long ScheduleId { get; set; }
        public long? ServiceId { get; set; }
        public long AssetId { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public bool IsEquipment { get; set; } = true;
    }
}
