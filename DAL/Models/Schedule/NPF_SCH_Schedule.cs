using DAL.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Models.Schedule
{
    public class NPF_SCH_Schedule : BaseAuditModel, IAuditable
    {
        [Key]
        public long Id { get; set; }
        public long? FormPageId { get; set; }
        public string Form { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public long? Zone { get; set; }
        public Status Status { get; set; }
        public string Reason { get; set; } = string.Empty;
        [NotMapped]
        public int NoOfService { get; set; }
        [NotMapped]
        public string ExpectedEndDate { get; set; }
        public DateTime? CompletedTime { get; set; }
        public DateTime? AcceptedTime { get; set; }
        public DateTime? RejectedTime { get; set; }
        public DateTime? StartedTime { get; set; }
        public DateTime? StoppedTime { get; set; }
        public DateTime? Time { get; set; }
        public bool IsMobile { get; set; } = false;

    }
}
