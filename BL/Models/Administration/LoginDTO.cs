using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Models.Administration
{
    public class LoginDTO
    {
        public string? Email { get; set; }
        public string? Password { get; set; }
        public bool RememberMe { get; set; }
        public string? FirebaseToken { get; set; }
    }
    public class LogoutDTO
    {
        public string id { get; set; }
        public string fireBaseToken { get; set; }
    }
}
