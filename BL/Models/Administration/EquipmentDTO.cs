using DAL.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Models.Administration
{
    public class EquipmentDTO
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; } = string.Empty;
        public long Category { get; set; } //Category
        public long Abbreviation { get; set; } //Abbreviation
        public long? ParentEquipment { get; set; } //ParentEquipment
        public AssignmentType? AssignmentType { get; set; }
        public long? AssignmentId { get; set; }
        public long Make { get; set; } //Make
        public long Model { get; set; } //Model
        public string Manufacturer { get; set; }
        public DateTime? ManufacturingDate { get; set; }
        public string SerialNo { get; set; }
        public string ExternalReferenceCode { get; set; }
    }

    public class EquipmentListDTO
    {
        public long Id { get; set; }
        public string Code { get; set; } = "Auto Generated";
        public string Name { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } //Category
        public string Abbreviation { get; set; } //Abbreviation
        public string? ParentEquipment { get; set; } //ParentEquipment
        public string Make { get; set; } //Make
        public string Model { get; set; } //Model
        public string Manufacturer { get; set; }
        public DateTime? ManufacturingDate { get; set; }
        public string SerialNo { get; set; }
        public string ExternalReferenceCode { get; set; }
        public string AssignmentType { get; set; } = string.Empty;
        public string AssignmentName { get; set; } = string.Empty;
        public AssignmentType? AssignmentTypeId { get; set; }
        public long? AssignmentNameId { get; set; } = 0;

    }

    public class EquipmentListAssignmentDTO
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public long? StationId { get; set; } = 0;
        public string StationName { get; set; } = string.Empty;
    }

    public class EquipemntAssignmentDTO
    {
        public AssignmentType AssignmentType { get; set; }
        public long Id { get; set; }
    }

}
