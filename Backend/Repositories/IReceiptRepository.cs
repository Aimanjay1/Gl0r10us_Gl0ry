using BizOpsAPI.Models;

namespace BizOpsAPI.Repositories
{
    public interface IReceiptRepository
    {
        Task<IEnumerable<Receipt>> GetAllAsync();
        Task<Receipt?> GetByIdAsync(int id);
        Task<IEnumerable<Receipt>> GetByInvoiceIdAsync(int invoiceId);

        Task<Receipt> AddAsync(Receipt receipt);
        Task UpdateAsync(Receipt receipt);
        Task DeleteAsync(int id);
    }
}
