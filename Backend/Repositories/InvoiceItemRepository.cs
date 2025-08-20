using BizOpsAPI.Data;
using BizOpsAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BizOpsAPI.Repositories
{
    public class InvoiceItemRepository : IInvoiceItemRepository
    {
        private readonly AppDbContext _context;

        public InvoiceItemRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<InvoiceItem>> GetAllByInvoiceIdAsync(int invoiceId)
        {
            return await _context.InvoiceItems
                                 .Where(item => item.InvoiceId == invoiceId)
                                 .ToListAsync();
        }

        public async Task<InvoiceItem?> GetByIdAsync(int id)
        {
            return await _context.InvoiceItems.FindAsync(id);
        }

        public async Task<InvoiceItem> AddAsync(InvoiceItem item)
        {
            _context.InvoiceItems.Add(item);
            await _context.SaveChangesAsync();
            return item;
        }

        public async Task<InvoiceItem> UpdateAsync(InvoiceItem item)
        {
            _context.InvoiceItems.Update(item);
            await _context.SaveChangesAsync();
            return item;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var item = await _context.InvoiceItems.FindAsync(id);
            if (item == null) return false;

            _context.InvoiceItems.Remove(item);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
