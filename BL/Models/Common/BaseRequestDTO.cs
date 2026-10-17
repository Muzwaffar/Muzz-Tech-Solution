using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Models.Common
{
    public class BaseRequestDTO<T>
    {
        public T? Data { get; set; }
        public string userToken { get; set; }
        public string oneTimeToken { get; set; }
        public string RoleId { get; set; }
        public Guid? ModuleId { get; set; }
    }
}
