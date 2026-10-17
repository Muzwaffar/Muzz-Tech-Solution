using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models.Administration
{
    public class NPF_SYS_GlobalParam : BaseAuditModel, IAuditable
    {
        [Key]
        public Guid Id { get; set; }
        [StringLength(120, ErrorMessage = "Name cannot exceed 120 characters. ")]
        public string Name { get; set; }
        public string Value { get; set; }
        public string AdditionalValue { get; set; } = String.Empty;
        [StringLength(120, ErrorMessage = "Comment cannot exceed 120 characters. ")]
        public string Comment { get; set; } = String.Empty;
        public bool AdminOnly { get; set; } = false;
        public bool Enable { get; set; } = false;
    }
}

