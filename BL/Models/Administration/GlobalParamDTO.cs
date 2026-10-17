using DAL.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Models.Administration
{
    public class GlobalParamListDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Value { get; set; }
        public string AdditionalValue { get; set; }
        public string Comment { get; set; }

    }
    public class GlobalParamCRUDDTO 
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public string AdditionalValue { get; set; } =string.Empty;
        public string Comment { get; set; } 
        public bool AdminOnly { get; set; } = false;
    }
}
