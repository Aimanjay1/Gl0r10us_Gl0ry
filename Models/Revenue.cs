using BizOpsAPI.Models;

namespace BizOpsAPI.Models
{
    public class Revenue
    {
        public int RevenueId { get; set; }
        public int UserId { get; set; }
        public int? InvoiceId { get; set; } // optional link to Invoice
        public decimal Amount { get; set; }
        public DateTime RevenueDate { get; set; }
        public string? Source { get; set; } // NEW: persist custom source/label

        // Compiler-required navs; we initialize with null! when not loading navs.
        public required User User { get; set; }
        public Invoice? Invoice { get; set; }
    }
}