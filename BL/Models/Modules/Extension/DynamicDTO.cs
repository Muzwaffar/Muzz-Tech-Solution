using DAL.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Models.Modules.Extension
{
    public class DynamicDTO
    {
        public List<Headers> Headers { get; set; } = new List<Headers>();
        public string Rows { get; set; }
        public AccessLevel Access { get; set; }
    }
    public class Headers
    {
        public string title { get; set; }
        public string data { get; set; }
    }
    public class MatrixScheme
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public string Code { get; set; }
        public bool ML { get; set; } = false;
        public AccessLevel Access { get; set; }
        public string ApprovalStatus { get; set; }
    }
}
