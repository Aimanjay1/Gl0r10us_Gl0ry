using Microsoft.AspNetCore.Http;

namespace BizOpsAPI.DTOs
{
    public class ReceiptListDto
    {
        public int Id { get; set; }
        public string Payer { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string ReceiptUrl { get; set; } = string.Empty;
    }

    public class ReceiptCreateDto
    {
        public int InvoiceId { get; set; }
        public IFormFile File { get; set; } = default!;
    }

    public class ReceiptEmailCreateDto
    {
        public int InvoiceId { get; set; }
        public required string FileName { get; set; }
        public required byte[] Bytes { get; set; }
        public DateTime? UploadedAt { get; set; }

        // NEW metadata fields
        public string? EmailMessageId { get; set; }   // Message-Id from the email
        public string? FromAddress    { get; set; }   // who sent the email
        public string? ContentType    { get; set; }   // e.g. application/pdf
        public long?   SizeBytes      { get; set; }   // size of attachment
        public string? Sha256Hex      { get; set; }   // attachment checksum
    }

    // For updating metadata via JSON
    public class ReceiptUpdateDto
    {
        public int?      InvoiceId { get; set; }  // move to another invoice
        public DateTime? Date      { get; set; }  // adjust UploadedAt
    }
}
