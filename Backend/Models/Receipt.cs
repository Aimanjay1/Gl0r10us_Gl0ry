namespace BizOpsAPI.Models
{
    public class Receipt
    {
        public int      ReceiptId   { get; set; }

        // FK + required navigation
        public int      InvoiceId   { get; set; }
        public required Invoice Invoice { get; set; }

        // Where the file can be reached (public URL or relative path you serve)
        public required string  ReceiptUrl   { get; set; }

        // Helpful metadata (optional but nice to have)
        public string?  OriginalFileName { get; set; }
        public string?  ContentType      { get; set; }   // e.g., "application/pdf"
        public long?    SizeBytes        { get; set; }   // store size for sanity checks
        public string?  Sha256Hex        { get; set; }   // enable dedupe by content

        // Email provenance (optional)
        public string?  EmailMessageId   { get; set; }   // Message-Id header
        public string?  FromAddress      { get; set; }

        public DateTime UploadedAt       { get; set; } = DateTime.UtcNow;
        public DateTime? ProcessedAt     { get; set; }   // when your pipeline finished
    }
}
