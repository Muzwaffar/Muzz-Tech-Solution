using Castle.Components.DictionaryAdapter;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.Metrics;
using KeyAttribute = System.ComponentModel.DataAnnotations.KeyAttribute;

namespace DAL.Models.Notifications
{
    public class NPF_NOT_NotificationDetails : BaseAuditModel, IAuditable
    {
        [Key]
        public long Id { get; set; }

        public long? UserMobileId { get; set; }

        [ForeignKey("UserMobileId")]
        public virtual NPF_NOT_UserMobiles userMobiles { get; set; }

        [ForeignKey("NotificationId")]
        public virtual NPF_NOT_NotificationHeader notifcationHeader { get; set; }

        [DefaultValue(0)]
        public bool IsMobileNotification { get; set; }

        [DefaultValue(0)]
        public int Attempts { get; set; }

        [DefaultValue(0)]
        public int Sent { get; set; }

    }
}
