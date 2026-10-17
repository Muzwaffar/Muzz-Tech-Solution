using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.DBContext
{
    [Table("AuditEntry")]
    public class AuditEntry
    {
        [Key]
      //  [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public string EntityName { get; set; }
        public string ActionType { get; set; }
        public string UserId { get; set; }
        public DateTime TimeStamp { get; set; }
        public string EntityId { get; set; }
        public Dictionary<string, object> OriginalValues { get; set; }
        public Dictionary<string, object> Changes { get; set; }

        [NotMapped]
        // TempProperties are used for properties that are only generated on save, e.g. ID's
        public List<PropertyEntry> TempProperties { get; set; }

    }
}