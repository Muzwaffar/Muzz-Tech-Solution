using BL.Models.Common;
using DAL.Models.Administration;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Models.Administration
{
    public class RosterShiftResourceMappingDTO
    {
        public NPF_SYS_Roster? Roster { get; set; } = new NPF_SYS_Roster();
        public List<ShiftDateDTO>? Shift { get; set; } = new List<ShiftDateDTO>();
        public List<ShiftDateDTO>? ShiftUpdate { get; set; } = new List<ShiftDateDTO>();
        public List<NPF_SYS_RosterShiftResourceMapping>? RSR { get; set; } = new List<NPF_SYS_RosterShiftResourceMapping>();

        public List<DropDown>? dropdown { get; set; } = new List<DropDown>();
    }
    public class asDTO
    {
        public RosterDTO? Roster { get; set; } = new RosterDTO();
    }
    public class RosterDTO
    {
        public long Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = String.Empty;
        public string Description { get; set; } = String.Empty;
        public DateTime RosterFrom { get; set; } = DateTime.Now;
        public DateTime RosterTo { get; set; } = DateTime.Now;
    }
    public class ShiftDateDTO
    {
        public long ShiftId { get; set; }
        public string ShiftName { get; set; }
        public string day { get; set; }
        public bool isChecked { get; set; }
        public bool isIndividual { get; set; }
        public DateTime date { get; set; }

    }
    public class SysRosterShiftResourceMappingDTO
    {
        public long Id { get; set; }

        public long RosterId { get; set; }

        public long ResourceId { get; set; }

        public long ShiftId { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;

        public bool Exists { get; set; }

        public NPF_SYS_Resource Resource { get; set; } = new NPF_SYS_Resource();
        public NPF_SYS_Roster Roster { get; set; } = new NPF_SYS_Roster();
      
        public NPF_SYS_Shift Shift { get; set; } = new NPF_SYS_Shift();
        public string day { get; set; }
    }

    public class RosterResourceMappingDTO
    {
        public long Id { get; set; }

        public long RosterId { get; set; }

        public long ResourceId { get; set; }

        public NPF_SYS_Resource Resource { get; set; } = new NPF_SYS_Resource();
    }
}
