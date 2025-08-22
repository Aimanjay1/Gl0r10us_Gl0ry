using BizOpsAPI.DTOs;
using BizOpsAPI.Models;
using BizOpsAPI.Repositories;

namespace BizOpsAPI.Services
{
    public class RevenueService : IRevenueService
    {
        private readonly IRevenueRepository _repo;
        private readonly IInvoiceRepository _invoices;

        public RevenueService(IRevenueRepository repo, IInvoiceRepository invoices)
        {
            _repo = repo;
            _invoices = invoices;
        }

        public async Task<IEnumerable<RevenuesDto>> GetAllAsync()
        {
            var revenues = await _repo.GetAllAsync();
            return revenues.Select(r => MapToDto(r));
        }

        public async Task<RevenuesDto?> GetByIdAsync(int id)
        {
            var revenue = await _repo.GetByIdAsync(id);
            return revenue == null ? null : MapToDto(revenue);
        }

        public async Task<RevenuesDto?> GetByInvoiceIdAsync(int invoiceId)
        {
            var revenue = await _repo.GetByInvoiceIdAsync(invoiceId);
            return revenue == null ? null : MapToDto(revenue);
        }

        // Manual revenue: no invoice link
        public async Task<RevenuesDto> CreateAsync(RevenueCreateDto dto)
        {
            var entity = new Revenue
            {
                UserId = dto.UserId ?? 1, // TODO: from auth context
                InvoiceId = null,
                Amount = dto.Amount,
                RevenueDate = dto.Date == default ? DateTime.UtcNow : dto.Date,
                Source = string.IsNullOrWhiteSpace(dto.Source) ? "Other" : dto.Source!.Trim(),
                User = null!,
                Invoice = null!
            };

            var created = await _repo.AddAsync(entity);
            return MapToDto(created, dto.Source);
        }

        // Auto revenue from a PAID invoice (keeps the invoice link)
        public async Task<RevenuesDto?> CreateFromInvoicePaidAsync(int invoiceId, int userId)
        {
            var existing = await _repo.GetByInvoiceIdAsync(invoiceId);
            if (existing != null) return null;

            var invoice = await _invoices.GetByIdAsync(invoiceId);
            if (invoice == null) return null;

            var isPaid = string.Equals(invoice.Status, "Paid", StringComparison.OrdinalIgnoreCase);
            if (!isPaid) return null;

            var entity = new Revenue
            {
                UserId = userId,
                InvoiceId = invoice.InvoiceId,
                Amount = invoice.TotalAmount,
                RevenueDate = DateTime.UtcNow,
                Source = $"Invoice #{invoice.InvoiceId}",
                User = null!,
                Invoice = null!
            };

            var created = await _repo.AddAsync(entity);
            return MapToDto(created, $"Invoice #{invoice.InvoiceId} (Paid)");
        }

        public async Task<RevenuesDto?> UpdateAsync(int id, RevenueUpdateDto dto)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return null;

            if (dto.Amount.HasValue) entity.Amount = dto.Amount.Value;
            if (dto.Date.HasValue)   entity.RevenueDate = dto.Date.Value;
            if (dto.Source != null)  entity.Source = string.IsNullOrWhiteSpace(dto.Source) ? null : dto.Source.Trim();

            await _repo.UpdateAsync(entity);
            return MapToDto(entity, dto.Source);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) return false;

            await _repo.DeleteAsync(id);
            return true;
        }

        // -------- Mapping helpers --------
        private static RevenuesDto MapToDto(Revenue r, string? overrideSource = null)
        {
            var source = !string.IsNullOrWhiteSpace(overrideSource)
                ? overrideSource
                : (!string.IsNullOrWhiteSpace(r.Source)
                    ? r.Source
                    : (r.InvoiceId.HasValue && r.InvoiceId.Value > 0 ? $"Invoice #{r.InvoiceId}" : "Other"));

            return new RevenuesDto
            {
                Id = r.RevenueId,
                Source = source!,
                Amount = r.Amount,
                Date = r.RevenueDate,
                InvoiceId = r.InvoiceId,
                UserId = r.UserId
            };
        }
    }
}
