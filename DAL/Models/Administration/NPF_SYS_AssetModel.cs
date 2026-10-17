using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models.Administration
{
    public class NPF_SYS_AssetModel : BaseAuditModel, IAuditable
    {

        [Key]
        public long Id { get; set; }

        [StringLength(50, ErrorMessage = "Code cannot exceed 50 characters. ")]
        public string Code { get; set; }

        [StringLength(120, ErrorMessage = "Name cannot exceed 120 characters. ")]
        public string Name { get; set; }

        public string? Description { get; set; }

        [ForeignKey("AssetMakeId")]
        public virtual NPF_SYS_AssetMake AssetMake { get; set; }

    }
}
