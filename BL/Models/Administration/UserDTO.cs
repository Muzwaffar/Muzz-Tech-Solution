using DAL.Models.Administration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Models.Administration
{
    public class UserDTO
    {
        public string Id {  get; set; }
        public string Code { get; set; } = "Auto-Generated";
        public Guid UserToken { get; set; }
        public long? Title { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string? TelephoneNo { get; set; }
        public string Roles { get; set; }
        public long ResourceType { get; set; }
        public long? Office { get; set; }
        public long? PostHeld { get; set; }
        public long? UserType { get; set; }
        public List<long> UserMatrix { get; set; } = new List<long>();
    }
    public class SyncUserDTO
    {
        public string Password { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
    }
}
