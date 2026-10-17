using DAL.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models.Store
{
    public class NPF_STR_StockRequest : BaseAuditModel, IAuditable
    {
        [Key]
        public long Id { get; set; }
        public long StoreItemId { get; set; }
        public DateTime RequestDate { get; set; }
        public long RequestBy { get; set; }
        public decimal Quantity { get; set; }
        public long ItemSource { get; set; }
        public ApprovalStatus ApprovalStatus { get; set; } = ApprovalStatus.AwaitingApproval;
    }
}
