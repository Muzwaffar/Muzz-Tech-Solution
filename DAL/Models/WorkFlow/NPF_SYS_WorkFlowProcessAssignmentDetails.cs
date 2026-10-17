using DAL.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models.WorkFlow
{
    public class NPF_SYS_WorkFlowProcessAssignmentDetails : BaseAuditModel, IAuditable
    {
        [Key]
        public long Id { get; set; }
        public long FkWFPDetails { get; set; }
        public long EntityId { get; set; }
        public string? Code { get; set; }
        public CaseStatus CaseStatus { get; set; }
        public AType AType { get; set; }
        public string? Description { get; set; }
        public long UserId { get; set; }
        public string? Ext1 { get; set; }
        public string? Ext2 { get; set; }
        public bool Notification { get; set; } = false;
    }
}
