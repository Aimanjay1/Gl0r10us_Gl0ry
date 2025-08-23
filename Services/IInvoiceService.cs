using BizOpsAPI.DTOs;

namespace BizOpsAPI.Services
{
    public interface IInvoiceService
    {
        Task<InvoiceDto> CreateInvoiceAsync(CreateInvoiceDto dto);
        Task<IEnumerable<InvoiceDto>> GetInvoicesByUserAsync(int userId);
        Task<InvoiceDto?> GetByIdAsync(int id);
        Task<byte[]> GenerateInvoicePdfAsync(int id);
        Task SendInvoiceEmailAsync(int id, string message);
        Task<bool> MarkAsPaidAsync(int id);
    }
}
