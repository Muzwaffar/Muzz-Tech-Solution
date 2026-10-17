using DAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackerEnabledDbContext.Core.Identity;

namespace DAL.DBContext
{
    public class SessionUser
    {
        public string UserID;
    }

    public class AuditContext<TUser, TRole, TKey> : TrackerIdentityContext<ApplicationUser, ApplicationRole, string>
      where TUser : IdentityUser<TKey>
      where TRole : IdentityRole<TKey>
      where TKey : IEquatable<TKey>
    {
        public SessionUser user;
        public AuditContext()
        {
            user = new SessionUser();
        }
        public AuditContext(DbContextOptions options) : base(options) { }
        //public override int SaveChanges()
        //{
        //    return base.SaveChanges(user.UserID);
        //}
    }
}
