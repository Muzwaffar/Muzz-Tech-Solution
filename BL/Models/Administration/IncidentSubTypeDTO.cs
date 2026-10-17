using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Models.Administration
{
    public class IncidentSubTypeDTO 
    {
        public long Id { get; set; }
        public string Code { get; set; } = "AUTO-GENERATED";
        public string Name { get; set; }
        public string Description { get; set; } = String.Empty;
        public long IncidentType { get; set; }
    }

    public class IncidentListDTO
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; } = String.Empty;
        public string IncidentType { get; set; }
    }
}
