using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using DAL.Common;
using DAL.Models.Administration;

namespace DAL.Models.Store
{
    public class NPF_STR_StoreTransaction : BaseAuditModel, IAuditable
    {
        [Key]
        public long Id { get; set; }
        public string Code { get; set; }
        [ForeignKey("StoreItemId")]
        public virtual NPF_STR_StoreItem StoreItem { get; set; }
        [ForeignKey("StoreId")]
        public virtual NPF_STR_Store Store { get; set; }
        public decimal Quantity { get; set; }
        public string Identifier { get; set; }
        public decimal CostPrice { get; set; }
        public decimal SellPrice { get; set; }
        public DateTime DateReceived { get; set; }
        public string Remarks { get; set; }
        public long? TransferFrom { get; set; }
        public long? TransferTo { get; set; }
        public string BatchNo { get; set; }
        public string FromBatchNo { get; set; }
        public decimal AbsoluteValue { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public RequestType RequestType { get; set; }
        public string PONumber { get; set; }
        public long  UomDetails { get; set; }
        public long? FromUOM { get; set; }
        public long? DisposalType { get; set; }
        public long? LExt1 { get; set; }
        public long? LExt2 { get; set; }
        public long? LExt3 { get; set; }
        public string SExt1 { get; set; } = string.Empty;
        public string SExt2 { get; set; } = string.Empty;
        public string SExt3 { get; set; } = string.Empty;
        public DateTime? DExt1 { get; set; }
        public DateTime? DExt2 { get; set; }
        public DateTime? DExt3 { get; set; }
        public decimal? DecExt1 { get; set; }
        public decimal? DecExt2 { get; set; }
        public decimal? DecExt3 { get; set; }



    }
}
