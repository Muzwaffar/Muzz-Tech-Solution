using DAL.Models.Administration;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Models.Administration
{
    public class NPF_SYS_RosterShiftResourceMapping:BaseAuditModel, IAuditable
    {
        [Key]
        public long Id { get; set; }

        [ForeignKey("NPF_SYS_Roster")]
        [Required]
        public long RosterId { get; set; }

        [ForeignKey("NPF_SYS_Resource")]
        [Required]
        public long ResourceId { get; set; }

        [ForeignKey("NPF_SYS_Shift")]
        [Required]
        public long ShiftId { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;

        public bool Exists { get; set; }    

        [NotMapped]
        public NPF_SYS_Resource Resource { get; set; } = new NPF_SYS_Resource();
        [NotMapped]
        public NPF_SYS_Roster Roster { get; set; } = new NPF_SYS_Roster();
        [NotMapped]
        public NPF_SYS_Shift Shift { get; set; } = new NPF_SYS_Shift();

        [NotMapped]
        public string day { get; set; }
    }
}
