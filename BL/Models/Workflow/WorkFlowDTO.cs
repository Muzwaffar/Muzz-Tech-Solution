using BL.Models.Common;
using BL.Models.FormPage;
using DAL.Common;
using DAL.Models.FormPage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Models.Workflow
{
    public class WorkFlowDTO
    {
        public long Id { get; set; }
        public string ApprovalRemarks { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public CaseStatus CaseStatus { get; set; }
        public List<WFUserDTO> WFUserDTO { get; set; } = new List<WFUserDTO>();

    }

    public class WFUserDTO
    {
        public long TId { get; set; }
        public string UId { get; set; }
        public string Username { get; set; }
    }
}
