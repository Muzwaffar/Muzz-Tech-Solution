using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Models.Access
{
    public class MatrixDTO
    {
        public long MID { get; set; }
        public long IID { get; set; }
        public long GMID { get; set; }
        public string GMDescription { get; set; }
    }
    public class MatrixListDTO
    {
        public long GMID { get; set; }
        public string GMDescription { get; set; }
    }
    public class UserMatrixDTO
    {
        public long UID { get; set; }
        public string AUID { get; set; }
        public List<long> IDS { get; set; } = new List<long>();
        public List<long> ParentIDS { get; set; } = new List<long>();

    }
    public class CRUDMatrix
    {
        public string id { get; set; }
        public string text { get; set; }
        public string parent { get; set; }
        public state state { get; set; } = new state();
    }
    public class state
    {
        public bool Checked { get; set; } = false;
        public bool Opened { get; set; } = false;
    }

}
