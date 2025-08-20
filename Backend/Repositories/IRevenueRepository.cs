using BizOpsAPI.Models;

namespace BizOpsAPI.Repositories
{
    public interface IRevenueRepository
    {
        Task<IEnumerable<Revenue>> GetAllAsync();
        Task<Revenue?> GetByIdAsync(int id);
        Task<Revenue?> GetByInvoiceIdAsync(int invoiceId);
        Task<Revenue> AddAsync(Revenue revenue);
        Task UpdateAsync(Revenue revenue);
        Task DeleteAsync(int id);
    }
}