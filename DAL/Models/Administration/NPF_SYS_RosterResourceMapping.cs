using DAL.Models.Administration;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Models.Administration
{
    public class NPF_SYS_RosterResourceMapping : BaseAuditModel, IAuditable
    {
        [Key]
        public long Id { get; set; }

        [ForeignKey("NPF_SYS_Roster")]
        [Required]
        public long RosterId { get; set; }

        [ForeignKey("NPF_SYS_Resource")]
        [Required]
        public long ResourceId { get; set; }

        [NotMapped]
        public NPF_SYS_Resource Resource { get; set; } = new NPF_SYS_Resource();



    }
}
