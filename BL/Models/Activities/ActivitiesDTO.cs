using DAL.Common;
using DAL.Models.Activities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Models.Activities
{
    public class ActivitiesDTO
    {
        public long Id { get; set; }
        public string MId { get; set; }
        public long ActivityTypeId { get; set; }
        public DateTime? StartDate { get; set; }
       public DateTime? EndDate { get; set; }
        public string Remarks { get; set; }
        public string Msg { get; set; } =string.Empty;
        public long? ParentId { get; set; }
        public GeneralStatus Status { get; set; }
        public long User { get; set; }
        public string URL { get; set; } = string.Empty;
        public AccessLevel Access { get; set; } = AccessLevel.Matrix;
        public List<ActivitiesDetailsDTO> Details { get; set; } = new List<ActivitiesDetailsDTO>();
    }

    public class ActivitiesConfigDTO
    {
        public long Id { get; set; }
        public long ActivityTypeId { get; set; }
        public string FieldId { get; set; }
        public string LabelName { get; set; }
        public ActivityElementType Type { get; set; }
        public ActivityElementSource? Source { get; set; }
        public long? ParentId { get; set; }
    }

    public class ActivitiesDetailsDTO
    {
        public long Id { get; set; }
        public ActivitiesConfigDTO Config { get; set; }
        public string Value { get; set; }
    }
    public class ActivitiesListDTO
    {
        public long? Id { get; set; }
        public string ActivityTypeName { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string Remarks { get; set; }
        public string Status { get; set; }
        public string AssignedTo { get; set; }
        public long AssignedToId { get; set; }
        public string CreatedBy { get; set; }
        public bool ML { get; set; } = false;
        public long UserId { get; set; }
        public AccessLevel Access { get; set; }
    }

    public class OverDictionanry
    {
        public string Key { get; set; }
        public int value { get; set; }
    }

    public class MyActivitiesGraphDTO
    {
        public List<OverDictionanry> ByItems { get; set; } = new List<OverDictionanry>();
        public List<OverDictionanry> PieData { get; set; } = new List<OverDictionanry>();
    }

    public class ActivitiesDropdownSearchDTO
    {
        public long? Parameter { get; set; }
        public string LabelName { get; set; }
    }
    public class ActivitiesTypeDTO
    {
        public long Id { get; set; }
        public long Type { get; set; }
        public string Name { get; set; }

    }
}
