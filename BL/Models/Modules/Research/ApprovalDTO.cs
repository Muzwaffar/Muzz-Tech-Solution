using DAL.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Models.Modules.Research
{
    public class ApprovalDTO
    {
        public long Id { get; set; }
        public ResearchSubModule RSM { get; set; }
        public ResearchType ResearchType { get; set; }
        public string Url { get; set; } = string.Empty;
        public string ApprovalRemarks { get; set; } = string.Empty;
        public long ProjectStatus { get; set; }
        public ApprovalStatus ApprovalStatus { get; set; } = ApprovalStatus.Declined;
        public List<ApprovalUserDTO> Approvals { get;set; } = new List<ApprovalUserDTO>();
        
    }
    public class ReworkDTO
    {
        public long Id { get; set; }
        public ResearchSubModule RSM { get; set; }

    }
    public class ApprovalUserDTO
    {
        public long TId { get; set; }
        public string UId { get; set; }
        public string Username { get; set; }
    }

    public class ApprovalDropdownDTO
    {
        public long DepartmentId { get; set; }
        public long ResourceTypeId { get; set; }
    }
}
