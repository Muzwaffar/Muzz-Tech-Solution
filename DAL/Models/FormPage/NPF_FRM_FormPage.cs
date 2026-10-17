using DAL.Common;
using System.ComponentModel.DataAnnotations;

namespace DAL.Models.FormPage
{
    public class NPF_FRM_FormPage : BaseAuditModel, IAuditable
    {
        [Key]
        public long Id { get; set; }
        public string Code { get; set; }
        public long FormId { get; set; }
        public long? PFormId { get; set; }
        public string FormName { get; set; }
        public string Additional1 { get; set; } = String.Empty;
        public string Additional2 { get; set; } = String.Empty;
        public string Additional3 { get; set; } = String.Empty;
        public string Additional4 { get; set; } = String.Empty;
        public string Colour { get; set; } = String.Empty;
        public decimal? TotalAmount { get; set; }
        public Signed IsAuthorised { get; set; }
        public ApprovalStatus Approval { get; set; } = ApprovalStatus.Draft;
        public Status Status { get; set; } = Status.Draft;
        public long? CurrentSequence { get; set; } =0;

    }
}
