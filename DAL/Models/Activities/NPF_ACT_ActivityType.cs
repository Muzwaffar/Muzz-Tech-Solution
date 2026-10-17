using DAL.Common;
using DAL.Models.Administration;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models.Activities
{
    public class NPF_ACT_ActivityEventType : BaseAuditModel, IAuditable
    {
        [Key]
        public long Id { get; set; }
        public string Name { get; set; }
        [ForeignKey("TypeId")]
        public virtual NPF_SYS_LookUpValue Type { get; set; }
        public bool IsEvent { get; set; }
        public bool IsActivity { get; set; }
        public bool TriggerActivity { get; set; }
        public bool? IsStoreTransaction { get; set; } = false;
    }
}
