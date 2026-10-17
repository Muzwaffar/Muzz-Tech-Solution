using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models.Administration
{
    public class NPF_SYS_PDAResourceMapping : BaseAuditModel, IAuditable
    {
        [Key]
        public long Id { get; set; }

        [ForeignKey("RankId")]
        public virtual NPF_SYS_Rank Rank { get; set; }

        public long RankQuantity { get; set; }

        public long Fk_PDA { get; set; }
    }
}
