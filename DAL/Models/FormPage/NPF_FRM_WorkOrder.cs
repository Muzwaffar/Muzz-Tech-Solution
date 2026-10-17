using DAL.Common;
using System.ComponentModel.DataAnnotations;
namespace DAL.Models.FormPage
{
    public class NPF_FRM_WorkOrder : BaseAuditModel, IAuditable
    {
        [Key]
        public long Id { get; set; }
        public long SectionId { get; set; }
        public long FormPageId { get; set; }
        public string SectionName { get; set; }
        public decimal? Amount { get; set; }
        public Status Status { get; set; } = Status.Draft;
        public long? BIID { get; set; }
        public long? InterfaceId { get; set; }
        public bool Print { get; set; } = false;
        public bool Notification { get; set; } = false;
        public string Reason { get; set; } = string.Empty;


    }
}
