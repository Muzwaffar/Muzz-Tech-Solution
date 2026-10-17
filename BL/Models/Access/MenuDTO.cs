using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Models.Access
{
    public class SideMenuDTO
    {
        public Guid Id { get; set; }
        public string DisplayName { get; set; }
        public int Order { get; set; }
        public string Icon { get; set; }
    }
}
