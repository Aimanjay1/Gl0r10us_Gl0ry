using System.Security.Cryptography;
using System.Text;
using BizOpsAPI.DTOs;
using BizOpsAPI.Models;
using BizOpsAPI.Repositories;

namespace BizOpsAPI.Services
{
    public class ReceiptService : IReceiptService
    {
        private readonly IReceiptRepository _receipts;
        private readonly IInvoiceRepository _invoices;
        private readonly IFileStorage _files;

        public ReceiptService(IReceiptRepository receipts, IInvoiceRepository invoices, IFileStorage files)
        {
            _receipts = receipts;
            _invoices = invoices;
            _files = files;
        }

        public async Task<IEnumerable<ReceiptListDto>> GetAllAsync()
        {
            var list = await _receipts.GetAllAsync();
            return list.Select(Map);
        }

        public async Task<ReceiptListDto?> GetByIdAsync(int id)
        {
            var r = await _receipts.GetByIdAsync(id);
            return r is null ? null : Map(r);
        }

        public async Task<IEnumerable<ReceiptListDto>> GetByInvoiceIdAsync(int invoiceId)
        {
            var recs = await _receipts.GetByInvoiceIdAsync(invoiceId);
            return recs.Select(Map);
        }

        public async Task<ReceiptListDto> CreateAsync(ReceiptCreateDto dto, CancellationToken ct = default)
        {
            var invoice = await _invoices.GetByIdAsync(dto.InvoiceId)
                          ?? throw new InvalidOperationException("Invoice not found");

            using var ms = new MemoryStream();
            await dto.File.CopyToAsync(ms, ct);
            var bytes = ms.ToArray();

            var fileName = $"{Guid.NewGuid()}_{dto.File.FileName}";
            var url = await _files.SaveAsync(bytes, "receipts", fileName, ct);

            var rec = new Receipt
            {
                InvoiceId        = invoice.InvoiceId,
                Invoice          = invoice,

                ReceiptUrl       = url,
                UploadedAt       = DateTime.UtcNow,

                // metadata for manual upload
                OriginalFileName = dto.File.FileName,
                ContentType      = string.IsNullOrWhiteSpace(dto.File.ContentType) ? null : dto.File.ContentType,
                SizeBytes        = bytes.LongLength,
                Sha256Hex        = ComputeSha256Hex(bytes),
                ProcessedAt      = DateTime.UtcNow
            };

            var created = await _receipts.AddAsync(rec);
            return Map(created);
        }

        public async Task<ReceiptListDto> CreateFromEmailAsync(ReceiptEmailCreateDto dto, CancellationToken ct = default)
        {
            var invoice = await _invoices.GetByIdAsync(dto.InvoiceId)
                          ?? throw new InvalidOperationException("Invoice not found");

            var fileName = $"{Guid.NewGuid()}_{dto.FileName}";
            var url = await _files.SaveAsync(dto.Bytes, "receipts", fileName, ct);

            var rec = new Receipt
            {
                InvoiceId        = invoice.InvoiceId,
                Invoice          = invoice,

                ReceiptUrl       = url,
                UploadedAt       = dto.UploadedAt ?? DateTime.UtcNow,

                // persist email/attachment metadata
                EmailMessageId   = dto.EmailMessageId,
                FromAddress      = dto.FromAddress,
                OriginalFileName = dto.FileName,
                ContentType      = dto.ContentType,                          // e.g., "application/pdf"
                SizeBytes        = dto.SizeBytes ?? dto.Bytes.LongLength,
                Sha256Hex        = string.IsNullOrEmpty(dto.Sha256Hex)
                                   ? ComputeSha256Hex(dto.Bytes)
                                   : dto.Sha256Hex,
                ProcessedAt      = DateTime.UtcNow
            };

            var created = await _receipts.AddAsync(rec);
            return Map(created);
        }

        public async Task<ReceiptListDto?> UpdateAsync(int id, ReceiptUpdateDto dto, CancellationToken ct = default)
        {
            var receipt = await _receipts.GetByIdAsync(id);
            if (receipt is null) return null;

            if (dto.InvoiceId.HasValue && dto.InvoiceId.Value != receipt.InvoiceId)
            {
                var newInvoice = await _invoices.GetByIdAsync(dto.InvoiceId.Value)
                                 ?? throw new InvalidOperationException("Target invoice not found");
                receipt.InvoiceId = newInvoice.InvoiceId;
                receipt.Invoice   = newInvoice;
            }

            if (dto.Date.HasValue)
                receipt.UploadedAt = dto.Date.Value;

            await _receipts.UpdateAsync(receipt);
            return Map(receipt);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _receipts.GetByIdAsync(id);
            if (existing is null) return false;
            await _receipts.DeleteAsync(id);
            return true;
        }

        private static ReceiptListDto Map(Receipt r) => new()
        {
            Id         = r.ReceiptId,
            Payer      = r.Invoice.Client.ClientName,
            Amount     = r.Invoice.TotalAmount,
            Date       = r.UploadedAt,
            ReceiptUrl = r.ReceiptUrl
        };

        private static string ComputeSha256Hex(byte[] bytes)
        {
            using var sha = SHA256.Create();
            var hash = sha.ComputeHash(bytes);
            var sb = new StringBuilder(hash.Length * 2);
            foreach (var b in hash) sb.Append(b.ToString("x2"));
            return sb.ToString();
        }
    }
}
