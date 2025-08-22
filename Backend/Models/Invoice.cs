namespace BizOpsAPI.Models
{
    public class Invoice
    {
        public int InvoiceId { get; set; }

        // Foreign Keys
        public int ClientId { get; set; }
        public int UserId { get; set; }

        // Invoice Info
        public string Status { get; set; } = "Unpaid";
        public DateTime OrderDate { get; set; }
        public DateTime DueDate { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // 🔹 Email/Thread tracking (NEW)
        public string? InvoicePdfFileName { get; set; }   // e.g., "invoice_123.pdf"
        public string? EmailMessageId     { get; set; }   // RFC Message-Id
        public string? EmailThreadId      { get; set; }   // store Gmail thread id as string
        public DateTime? InvoiceEmailSentAt { get; set; } // when email was sent

        // Navigation
        public required Client Client { get; set; }
        public required User   User   { get; set; }
        public required ICollection<InvoiceItem> Items { get; set; }
        public required ICollection<Receipt>     Receipts { get; set; }
    }
}
