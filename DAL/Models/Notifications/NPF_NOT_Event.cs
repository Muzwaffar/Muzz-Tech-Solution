using DAL.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models.Notifications
{
    public class NPF_NOT_Event : BaseAuditModel, IAuditable
    {
        [Key]
        public long Id { get; set; }
        public string UserId { get; set; }
        public string ResourceName { get; set; }
        public string ResourceId { get; set; }
        public string Title { get; set; }
        public string Detail { get; set; }
        public string AddDetail { get; set; }
        public string Target { get; set; }
        public TargetType TargetType { get; set; }
        public long TargetId { get; set; }
        public string Entity { get; set; }
        public Priority Priority { get; set; }
        public string Colour { get; set; }
        public string Image { get; set; }
        public DateTime ActionDate { get; set; }
        public bool IsRead { get; set; } = false;
        public bool IsNotified { get; set; } = false;
        public bool PopUp { get; set; } = false;


    }
}
