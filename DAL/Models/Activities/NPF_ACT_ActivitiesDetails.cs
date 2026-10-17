using DAL.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models.Activities
{
    public class NPF_ACT_ActivitiesDetails : BaseAuditModel, IAuditable
    {
        [Key]
        public long Id { get; set; }

        [ForeignKey("ActivityId")]
        public virtual NPF_ACT_Activities Activity { get; set; }
        [ForeignKey("ConfigId")]
        public virtual NPF_ACT_ActivitiesConfig Config { get; set; }
        public string Value { get; set; }
    }
}
