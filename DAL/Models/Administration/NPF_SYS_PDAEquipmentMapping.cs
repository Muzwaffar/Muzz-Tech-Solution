using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models.Administration
{
    public class NPF_SYS_PDAEquipmentMapping : BaseAuditModel, IAuditable
    {
        [Key]
        public long Id { get; set; }

        [ForeignKey("EquipmentCategoryId")]
        public virtual NPF_SYS_LookUpValue EquipmentCategory { get; set; }

        public long EquipmentQuantity { get; set; }

        public long Fk_PDA { get; set; }
    }
}
