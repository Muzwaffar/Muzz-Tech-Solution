using BL.Models.Template;
using DAL.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Models.Workflow
{
    public class WorkflowMappingListDTO
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string WFType { get; set; }
        public string AttributeType { get; set; }
        public string Module { get; set; }
    }

    public class WorkflowMappingCruDTO
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public long Sequence { get; set; } = 1;
        public string Code { get; set; } = "Auto Generated";
        public Guid ModuleId { get; set; }
        public long WFType { get; set; }
        public long AttributeType { get; set; }
        public long CaseStatus { get; set; } = (long)DAL.Common.CaseStatus.Draft;
    }

    public class WorkflowMappingDTO
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public long Sequence { get; set; }
        public string ModuleId { get; set; }
        public long WFType { get; set; }
        public long AttributeType { get; set; }
        public long CaseStatus { get; set; }
        public List<WorkflowMappingDetailsDTO> Details { get; set; }
    }
    public class WorkflowMappingDetailsDTO
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Ext1 { get; set; } = string.Empty;
        public string ModuleNext { get; set; } = string.Empty;
        public string Ext2 { get; set; } = string.Empty;
        public long CaseStatusId { get; set; }
        public string CaseStatus { get; set; } = string.Empty;
        public long ATypeId { get; set; }
        public string AType { get; set; } = string.Empty;
        public long WFTypeId { get; set; }
        public string WFType { get; set; } = string.Empty;
        public long Sequence { get; set; }
        public long ActivityTypeId { get; set; }
        public string ActivityType { get; set; } = string.Empty;
        public long FormSequence { get; set; } = 0;
        public string ModuleIdCurrent { get; set; }
        public string ModuleCurrent { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }
}
