namespace BizOpsAPI.Services
{
    using BizOpsAPI.DTOs;
    using BizOpsAPI.Models;
    using BizOpsAPI.Repositories;

    public class InvoiceService : IInvoiceService
    {
        private readonly IInvoiceRepository _invoices;
        private readonly IEmailService _email;
        private readonly IRevenueService _revenues; // NEW

        public InvoiceService(IInvoiceRepository invoices, IEmailService email, IRevenueService revenues)
        {
            _invoices = invoices;
            _email = email;
            _revenues = revenues; // NEW
        }

        public async Task<InvoiceDto> CreateInvoiceAsync(CreateInvoiceDto dto)
        {
            var invoice = new Invoice
            {
                ClientId = dto.ClientId,
                UserId   = dto.UserId,
                OrderDate = dto.OrderDate,
                DueDate   = dto.DueDate,
                Status    = "Unpaid",
                Items = dto.Items.Select(it => new InvoiceItem
                {
                    ItemName  = it.ItemName,
                    Quantity  = it.Quantity,
                    UnitPrice = it.UnitPrice,
                    LineTotal = it.Quantity * it.UnitPrice,
                }).ToList(),
                TotalAmount = dto.Items.Sum(x => x.Quantity * x.UnitPrice),
                Client   = null!,
                User     = null!,
                Receipts = new List<Receipt>()
            };

            var created = await _invoices.AddAsync(invoice);
            return MapToDto(created);
        }

        public async Task<IEnumerable<InvoiceDto>> GetInvoicesByUserAsync(int userId)
        {
            var all = await _invoices.GetAllAsync();
            return all.Where(i => i.UserId == userId).Select(MapToDto);
        }

        public async Task<InvoiceDto?> GetByIdAsync(int id)
        {
            var entity = await _invoices.GetByIdAsync(id);
            return entity == null ? null : MapToDto(entity);
        }

        public async Task<byte[]> GenerateInvoicePdfAsync(int id)
        {
            var invoice = await _invoices.GetByIdAsync(id)
                         ?? throw new KeyNotFoundException("Invoice not found.");

            var lines = new List<string>
            {
                "%PDF-1.4",
                "1 0 obj << /Type /Catalog /Pages 2 0 R >> endobj",
                "2 0 obj << /Type /Pages /Count 1 /Kids [3 0 R] >> endobj",
                "3 0 obj << /Type /Page /Parent 2 0 R /MediaBox [0 0 612 792] /Contents 4 0 R /Resources << /Font << /F1 5 0 R >> >> >> endobj",
                $"4 0 obj << /Length  {75 + invoice.Items.Count * 20} >> stream",
                "BT /F1 12 Tf 72 720 Td (Invoice) Tj T*",
                $"(Invoice ID: {invoice.InvoiceId}) Tj T*",
                $"(Client ID: {invoice.ClientId}) Tj T*",
                $"(Status: {invoice.Status}) Tj T*",
                $"(Order: {invoice.OrderDate:yyyy-MM-dd}) Tj T*",
                $"(Due: {invoice.DueDate:yyyy-MM-dd}) Tj T*",
                $"(Total: {invoice.TotalAmount:0.00}) Tj T*",
            };
            foreach (var it in invoice.Items)
                lines.Add($"( - {it.ItemName} x{it.Quantity} @ {it.UnitPrice:0.00} = {it.LineTotal:0.00}) Tj T*");
            lines.AddRange(new[]
            {
                "ET",
                "endstream endobj",
                "5 0 obj << /Type /Font /Subtype /Type1 /BaseFont /Helvetica >> endobj",
                "xref 0 6",
                "0000000000 65535 f ",
                "0000000010 00000 n ",
                "0000000060 00000 n ",
                "0000000113 00000 n ",
                "0000000281 00000 n ",
                "0000000000 00000 n ",
                "trailer << /Size 6 /Root 1 0 R >>",
                "startxref",
                "0",
                "%%EOF"
            });
            return System.Text.Encoding.ASCII.GetBytes(string.Join(" ", lines));
        }

        public async Task SendInvoiceEmailAsync(int id, string message)
        {
            var invoice = await _invoices.GetByIdAsync(id)
                         ?? throw new KeyNotFoundException("Invoice not found.");

            var pdf = await GenerateInvoicePdfAsync(id);

            var to = invoice.Client?.Email ?? string.Empty;
            if (string.IsNullOrWhiteSpace(to))
                throw new InvalidOperationException("Client email missing.");

            var subject = $"Invoice #{invoice.InvoiceId} - {invoice.Status}";
            var html = $"<p>{message}</p><p>Total: <b>{invoice.TotalAmount:0.00}</b></p>";

            await _email.SendEmailAsync(to, subject, html, pdf, $"invoice_{id}.pdf");
        }

        public async Task<bool> MarkAsPaidAsync(int id)
        {
            var invoice = await _invoices.GetByIdAsync(id);
            if (invoice == null) return false;

            // 1) Mark invoice paid
            invoice.Status = "Paid";
            await _invoices.UpdateAsync(invoice);

            // 2) Create revenue (idempotent in RevenueService)
            await _revenues.CreateFromInvoicePaidAsync(invoice.InvoiceId, invoice.UserId);
            return true;
        }

        private static InvoiceDto MapToDto(Invoice i) =>
            new InvoiceDto
            {
                InvoiceId   = i.InvoiceId,
                ClientId    = i.ClientId,
                UserId      = i.UserId,
                Status      = i.Status,
                OrderDate   = i.OrderDate,
                DueDate     = i.DueDate,
                TotalAmount = i.TotalAmount,
                Items = i.Items.Select(it => new InvoiceItemDto
                {
                    InvoiceItemId = it.InvoiceItemId,
                    ItemName      = it.ItemName,
                    Quantity      = it.Quantity,
                    UnitPrice     = it.UnitPrice,
                    LineTotal     = it.LineTotal
                }).ToList()
            };
    }
}