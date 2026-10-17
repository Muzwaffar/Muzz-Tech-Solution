using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Models.Administration
{
    public class VehicleListDTO
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string RegistrationNo { get; set; }
        public string Description { get; set; } = String.Empty;
        public string Category { get; set; }  //Category
        public string Abbreviation { get; set; } // Abbreviation
        public string? ParentVehicle { get; set; } //Parent Vehicle
        public string Make { get; set; } //Asset Make
        public string Model { get; set; } //Asset Model
        public string Manufacturer { get; set; } = string.Empty;
        public DateTime? ManufacturingDate { get; set; }
        public string SerialNo { get; set; } = string.Empty;
        public string ExternalReferenceCode { get; set; } = string.Empty;
        public string StationName { get; set; } = string.Empty;
    }
    public class VehicleCRUDDTO
    {
        public long Id { get; set; }
        public string Code { get; set; } = "Auto-Generated";
        public string RegistrationNo { get; set; }
        public string Description { get; set; } = String.Empty;
        public long Station { get; set; }
        public long Category { get; set; }  //Category
        public long Abbreviation { get; set; } // Abbreviation
        public long? ParentVehicle { get; set; } //Parent Vehicle
        public long Make { get; set; } //Asset Make
        public long Model { get; set; } //Asset Model
        public string Manufacturer { get; set; } = string.Empty;
        public DateTime? ManufacturingDate { get; set; }
        public string SerialNo { get; set; } = string.Empty;
        public string ExternalReferenceCode { get; set; } = string.Empty;
    }
}
