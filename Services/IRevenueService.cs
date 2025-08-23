using BizOpsAPI.DTOs;

namespace BizOpsAPI.Services
{
    public interface IRevenueService
    {
        Task<IEnumerable<RevenuesDto>> GetAllAsync();
        Task<RevenuesDto?> GetByIdAsync(int id);
        Task<RevenuesDto?> GetByInvoiceIdAsync(int invoiceId);
        Task<RevenuesDto> CreateAsync(RevenueCreateDto dto);
        Task<RevenuesDto?> CreateFromInvoicePaidAsync(int invoiceId, int userId);
        Task<RevenuesDto?> UpdateAsync(int id, RevenueUpdateDto dto);
        Task<bool> DeleteAsync(int id);
    }
}