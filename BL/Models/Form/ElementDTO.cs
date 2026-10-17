using DAL.Models.Administration;
using DAL.Models.Form;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Models.Form
{
    public class ElementDTO
    {
        public long Id { get; set; }
        public int Order { get; set; }
        public long? ServiceId { get; set; }
        public string Name { get; set; }
        public string AdditionalLabel { get; set; } = string.Empty;
        public NPF_FRM_InputTypeDetails InputTypeDetails { get; set; } = new NPF_FRM_InputTypeDetails();
        public NPF_FRM_Source? Source { get; set; }
        public NPF_SYS_UomHeader? LookUpValue { get; set; } // UOM
        public bool Notification { get; set; } = false;
        public List<NPF_FRM_Parameters> Parameters { get; set; } = new List<NPF_FRM_Parameters>();

        public bool? IsMobileChecklist { get; set; } = false;
        public long? ParentId { get; set; } = null;
        public List<ElementDTO> InnerElemList { get; set; } = new List<ElementDTO>();

        public bool? IsListing { get; set; } = false;
        public string? URL { get; set; } = string.Empty;
        public string? ListingName { get; set; } = string.Empty;
    }
}
