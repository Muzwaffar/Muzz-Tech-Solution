using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models.Administration
{
    public class NPF_SYS_PDAVehicleMapping : BaseAuditModel, IAuditable
    {

        [Key]
        public long Id { get; set; }

        [ForeignKey("VehicleCategoryId")]
        public virtual NPF_SYS_LookUpValue VehicleCategory { get; set; }

        public long VehicleQuantity { get; set; }

        public long Fk_PDA {  get; set; }
    }
}
