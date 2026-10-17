using DAL.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Models.Modules.Common
{
    public class ApprovalServiceDTO
    {
        public long Id { get; set; }
        public string Form { get; set; }
        public string Url { get; set; } = string.Empty;
        public ApprovalStatus ApprovalStatus { get; set; } = ApprovalStatus.Declined;
        public ResearchSubModule RSM { get; set; }
        public List<ApprovalServiceUserDTO> Approvals { get; set; } = new List<ApprovalServiceUserDTO>();

    }
    public class ApprovalServiceUserDTO
    {
        public long TId { get; set; }
        public string UId { get; set; }
        public string Username { get; set; }
    }

    public class ApprovalServiceDropdownDTO
    {
        public long DepartmentId { get; set; }
        public long ResourceTypeId { get; set; }
    }
}
