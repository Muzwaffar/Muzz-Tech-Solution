using DAL.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models.Administration
{
    public class NPF_SYS_Equipment :BaseAuditModel, IAuditable
    {
        [Key]
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; } = string.Empty;
        public AssignmentType? AssignmentType { get; set; }
        public long? AssignmentId { get; set; }
        public long Category { get; set; } //Category
        public long Abbreviation { get; set; } //Abbreviation
        public long? ParentEquipment { get; set; } //ParentEquipment
        public long Make { get; set; } //Make
        public long Model { get; set; } //Model
        public long? GMSEquipmentId { get; set; } 
        public long? FMSMappingtId { get; set; } 
        public string FMSMappingDescription { get; set; } = string.Empty;

        public string Manufacturer { get; set; }
        public DateTime? ManufacturingDate { get; set;}
        public string SerialNo { get; set;}
        public string ExternalReferenceCode { get; set;}
    }
}
