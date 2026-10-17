using System.ComponentModel.DataAnnotations;

namespace DAL.Models.FormPage
{
    public class NPF_FRM_WOElementMapping : BaseAuditModel, IAuditable
    {
        [Key]
        public long Id { get; set; }
        public long WOId { get; set; }
        public long ElementId { get; set; }
        public string ElementName { get; set; }
        public string ElementType { get; set; }
        public string UOM { get; set; } = String.Empty;
        public string Value { get; set; }
        public string AdditionalValue { get; set; } = string.Empty;
        public bool? CheckListValue { get; set; }
        public int? Row { get; set; } =null;
        public int? Column { get; set; } = null;
        public long? EntityId { get; set; } = null;
        public long? EntitySourceId { get; set; } = null;
        public string? EntityName { get; set; } = null;
        public long?  ParentId { get; set; } = null;
        public bool?  Isvalue { get; set; } = null;
    }
}
