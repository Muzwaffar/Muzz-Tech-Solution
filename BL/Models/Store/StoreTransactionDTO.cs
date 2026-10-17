using DAL.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Models.Store
{
    public class StoreTransactionDTO
    {
        public long StoreId { get; set; }
        public string Name { get; set; }
        public decimal CostPrice { get; set; }
        public DateTime DateRecieved { get; set; }
        public long StoreLocation { get; set; }
        public decimal Quantity { get; set; }
        public decimal AvailableQuantity { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string Supplier { get; set; } = string.Empty;
        public string BatchNo { get; set; } = string.Empty;
        public string PONo { get; set; } = string.Empty;
        public string Remarks { get; set; } = string.Empty;
    }

    public class StockRequestListDTO
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string RequestDate { get; set; }
        public string RequestBy { get; set; }
        public string ItemSource { get; set; }
        public decimal Quantity { get; set; }
        public ApprovalStatus ApprovalStatus { get; set; } = ApprovalStatus.None;
    }

    public class StockRequestADDTO
    {
        public long Id { get; set; }
        public ApprovalStatus ApprovalStatus { get; set; }
    }

    public class StoreTransactionListDTO
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string StoreLocation { get; set; }
        public decimal Quantity { get; set; }
        public string Date { get; set; }
        public string Type { get; set; }
    }

    public class StoreItemTableDTO
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Group { get; set; }
        public string Category { get; set; }
        public decimal Quantity { get; set; }
    }
}
