using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Models.Administration
{
    public class RankDTO
    {
        public long Id { get; set; }
        public string Code { get; set; } = "AUTO-GENERATED";
        public string Name { get; set; }
        public string Description { get; set; } = string.Empty;
        public long ResourceCategory { get; set; }
        public int Order { get; set; }
    }

    public class RankListDTO
    {
        public long Id { get; set; }
        public string Code { get; set; } = "AUTO-GENERATED";
        public string Name { get; set; }
        public string Description { get; set; } = string.Empty;
        public string ResourceCategory { get; set; }
    }
}
