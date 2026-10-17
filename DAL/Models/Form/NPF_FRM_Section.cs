using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models.Form
{
    public class NPF_FRM_Section : BaseAuditModel, IAuditable
    {
        [Key]
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Print { get; set; } = false;
        public bool Multiple { get; set; } = false;
        public bool Notification { get; set; } = false;
        public long? AssetTypeId { get; set; }
        [NotMapped]
        public bool check { get; set; } = false;
        [NotMapped]
        public int? Order { get; set; } = 0;
        [NotMapped]
        public string? URL { get; set; } = string.Empty;
        [NotMapped]
        public string? ListingName { get; set; } = string.Empty;

    }
}

