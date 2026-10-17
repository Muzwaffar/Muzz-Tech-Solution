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
    public class NPF_ACT_ActivitiesConfig : BaseAuditModel, IAuditable
    {
        [Key]
        public long Id { get; set; }
        public long ActivityType { get; set; }
        public string FieldId { get; set; }
        public string LabelName { get; set; }
        public ActivityElementType Type { get; set; }
        public ActivityElementSource? Source { get; set; }
        public long? ParentId { get; set; }
    }
}
