using DAL.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models.WorkFlow
{
    public class NPF_SYS_WorkFlowProcessDetails : BaseAuditModel, IAuditable
    {
        [Key]
        public long Id { get; set; }
        public long FkWFHId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? Message { get; set; }
        public string? Ext1 { get; set; }
        public string? Ext2 { get; set; }
        public string? Remarks { get; set; }
        public long? MatrixId { get; set; }
        public CaseStatus CStatus { get; set; }
        public GeneralStatus GeneralStatus { get; set; }
        public AType AType { get; set; }
        public WFType WFType { get; set; }
        public long? ParentId { get; set; }
        public long Sequence { get; set; }
        public long? ActivityType { get; set; }
        public long FormSequence { get; set; }
        public Guid ModuleId { get; set; }
        public long? BIID { get; set; }
        public long? InterfaceId { get; set; }
        public bool Print { get; set; } = false;
        public bool Notification { get; set; } = false;
        public long EntityId { get; set; }//Id of record
        public AttributeType AttributeType { get; set; }//Enum sr/pow/rp
        public string? AttributeId { get; set; }//id of section if any

    }
}
