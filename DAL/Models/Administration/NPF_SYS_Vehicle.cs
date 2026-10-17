using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models.Administration
{
    public class NPF_SYS_Vehicle : BaseAuditModel, IAuditable
    {
        [Key]
        public long Id { get; set; }

        [StringLength(50, ErrorMessage = "Code cannot exceed 50 characters. ")]
        public string Code { get; set; }
        public string RegistrationNo { get; set; }

        public string Description { get; set; } = String.Empty;

        public long? Station { get; set; }
        public long Category { get; set; }  //Category

        public long Abbreviation { get; set; } // Abbreviation

        public long? ParentVehicle { get; set; } //Parent Vehicle
        public long? FMSAssetId { get; set; }

        public long Make { get; set; } //Asset Make
        public long Model { get; set; } //Asset Model

        [StringLength(255, ErrorMessage = "Manufacturer cannot exceed 255 characters. ")]
        public string Manufacturer { get; set; } = string.Empty;

        public DateTime? ManufacturingDate { get; set; }

        [StringLength(255, ErrorMessage = "Serial No cannot exceed 255 characters. ")]
        public string SerialNo { get; set; } = string.Empty;

        [StringLength(255, ErrorMessage = "External Reference Code cannot exceed 255 characters. ")]
        public string ExternalReferenceCode { get; set; } = string.Empty;
    }
}
