using BizOpsAPI.Models;

namespace BizOpsAPI.Repositories
{
    public interface IInvoiceItemRepository
    {
        Task<IEnumerable<InvoiceItem>> GetAllByInvoiceIdAsync(int invoiceId);
        Task<InvoiceItem?> GetByIdAsync(int id);
        Task<InvoiceItem> AddAsync(InvoiceItem item);
        Task<InvoiceItem> UpdateAsync(InvoiceItem item);
        Task<bool> DeleteAsync(int id);
    }
}
