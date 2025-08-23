using System.ComponentModel.DataAnnotations;
namespace BizOpsAPI.DTOs
{
    public class RevenuesDto
    {
        public int Id { get; set; }
        public string Source { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public int? InvoiceId { get; set; } // null for manual revenues
        public int UserId { get; set; }
    }

    public class RevenueCreateDto
    {
        public string? Source { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be > 0")]
        public decimal Amount { get; set; }

        public DateTime Date { get; set; } // server can default to UtcNow if omitted

        public int? UserId { get; set; } // optional override; else take from auth
        // Note: no InvoiceId here — manual revenues are independent
    }

    public class RevenueUpdateDto
    {
        public string? Source { get; set; }
        public decimal? Amount { get; set; }
        public DateTime? Date { get; set; }
        // No InvoiceId here to keep manual revenues independent.
    }
}