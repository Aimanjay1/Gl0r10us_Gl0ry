using BizOpsAPI.Data;
using BizOpsAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BizOpsAPI.Repositories
{
    public class ReceiptRepository : IReceiptRepository
    {
        private readonly AppDbContext _context;
        public ReceiptRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Receipt>> GetAllAsync() =>
            await _context.Receipts
                .Include(r => r.Invoice)
                    .ThenInclude(i => i.Client)
                .ToListAsync();

        public async Task<Receipt?> GetByIdAsync(int id) =>
            await _context.Receipts
                .Include(r => r.Invoice)
                    .ThenInclude(i => i.Client)
                .FirstOrDefaultAsync(r => r.ReceiptId == id);

        public async Task<IEnumerable<Receipt>> GetByInvoiceIdAsync(int invoiceId) =>
            await _context.Receipts
                .Include(r => r.Invoice)
                    .ThenInclude(i => i.Client)
                .Where(r => r.InvoiceId == invoiceId)
                .OrderBy(r => r.ReceiptId)
                .ToListAsync();

        public async Task<Receipt> AddAsync(Receipt receipt)
        {
            _context.Receipts.Add(receipt);
            await _context.SaveChangesAsync();
            return receipt;
        }

        public async Task UpdateAsync(Receipt receipt)
        {
            _context.Receipts.Update(receipt);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var receipt = await _context.Receipts.FindAsync(id);
            if (receipt != null)
            {
                _context.Receipts.Remove(receipt);
                await _context.SaveChangesAsync();
            }
        }
    }
}
