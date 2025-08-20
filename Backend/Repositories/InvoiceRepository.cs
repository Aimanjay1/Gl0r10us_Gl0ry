using BizOpsAPI.Data;
using BizOpsAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BizOpsAPI.Repositories
{
    public class InvoiceRepository : IInvoiceRepository
    {
        private readonly AppDbContext _context;
        public InvoiceRepository(AppDbContext context) => _context = context;

        public async Task<IEnumerable<Invoice>> GetAllAsync() =>
            await _context.Invoices
                .Include(i => i.Client)
                .Include(i => i.Items)
                .Include(i => i.Receipts)
                .ToListAsync();

        public async Task<Invoice?> GetByIdAsync(int id) =>
            await _context.Invoices
                .Include(i => i.Client)
                .Include(i => i.Items)
                .Include(i => i.Receipts)
                .FirstOrDefaultAsync(i => i.InvoiceId == id);

        public async Task<Invoice> AddAsync(Invoice invoice)
        {
            await _context.Invoices.AddAsync(invoice);
            await _context.SaveChangesAsync();
            return invoice;
        }

        public async Task<Invoice> UpdateAsync(Invoice invoice)
        {
            _context.Invoices.Update(invoice);
            await _context.SaveChangesAsync();
            return invoice;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _context.Invoices.FindAsync(id);
            if (existing is null) return false;
            _context.Invoices.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Invoice?> GetByEmailThreadIdAsync(string threadId) =>
            await _context.Invoices
                .Include(i => i.Client)
                .Include(i => i.Items)
                .Include(i => i.Receipts)
                .FirstOrDefaultAsync(i => i.EmailThreadId == threadId);
    }
}