using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Constants
{
    public class QueryResult
    {
        public  string SUCEEDED = "success";
        public  string FAILED = "failed";
        public string  LOCK = "lock";
        public string EXPIRED = "expired";
        public string INVALID = "invalid credentials";
    }
    public class GenericConstant
    {
        public Guid Gcons = new Guid();
    }


}
