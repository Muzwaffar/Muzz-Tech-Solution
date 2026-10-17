using BL.Models.Common;
using DAL.Common;
using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Models.Request
{
    public class RequestDTO
    {
        public class GridDTO
        {
            public long id { get; set; }
            public string description { get; set; }
            public int quantity { get; set; }
        }
        public class ScreenDTO
        {
            public string title { get; set; }
            public List<DropDownItem> items { get; set; } = new List<DropDownItem>();
            public List<GridDTO> grid { get; set; } = new List<GridDTO>();
        }
        public class GetScreen
        {
            public long incidentId { get; set; }
            public MRequestType type { get; set; }
        }
        public class InsertRequest
        {
            public long incidentId { get; set; }
            public MRequestType type { get; set; }
            public long entityId { get; set; }
            public int? quantity { get; set; }
            public string additional { get; set; } = string.Empty;

        }
        public class ResourceCatDTO
        {
            public long ResourceCategory { get; set; }
            public string Name { get; set; }
            public long Rank { get; set; }
        }

    }
}
