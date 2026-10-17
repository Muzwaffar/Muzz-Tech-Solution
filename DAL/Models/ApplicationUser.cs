using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using DAL.Models.Administration;

namespace DAL.Models
{
    //[TrackChanges]
    public  class ApplicationUser : IdentityUser
    {
        public Guid UserToken { get; set; }

        [DefaultValue("false")]
        public bool IsSubscriber { get; set; }

        public DateTime? CreatedDate { get; set; }

        public Guid? CreatedBy { get; set; }
        public Guid? OTT { get; set; }

        public Guid? UpdatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public Boolean DeleteStatus { get; set; }
        public DateTime? LastPasswordChangedDate { get; set; }
        public bool FirstTimeLogin { get; set; } = true;
        public virtual LoginLog? LoginDetail { get; set; }
        public string Status { get; set; }

        public string Language { get; set; } = "English";
        public int Company { get; set; } = 0;

        public string? Title { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string UserCode { get; set; }
        public string FkID { get; set; } = string.Empty;
        [NotMapped]
        public string RoleId { get; set; }

        [NotMapped]
        public ApplicationRole Roles { get; set; }

        [NotMapped]
        public string FullName { get { return FirstName + " " + LastName; } }
        [NotMapped]
        public bool Logout { get; set; }
        [NotMapped]
        public long ResourceType { get; set; }
        [NotMapped]
        public string Office { get; set; }
        [NotMapped]
        public string PostHeld { get; set; }
        [NotMapped]
        public string UserType { get; set; }
        [NotMapped]
        public bool IsStation { get; set; } = false;
    }
    public class TunnelApplicationUser
    {

        public string Title { get; set; }
        public string Email { get; set; }
        public string NormEmail { get; set; }
        public string Username { get; set; }
        public string NormUsername { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string FkID { get; set; }

    }
}
