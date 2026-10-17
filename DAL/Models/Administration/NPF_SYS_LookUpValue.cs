using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models.Administration
{

    public class NPF_SYS_LookUpValue : BaseAuditModel, IAuditable
    {
        [Key]
        public long Id { get; set; }
        public Guid GId { get; set; } = Guid.NewGuid();
        [StringLength(120, ErrorMessage = "Name cannot exceed 120 characters. ")]
        public string Name { get; set; }
        [StringLength(120, ErrorMessage = "Description cannot exceed 120 characters. ")]
        public string Description { get; set; }

        [ForeignKey("LookUpTypeId")]
        public virtual NPF_SYS_LookUpType LookUpType { get; set; }

    }
}
