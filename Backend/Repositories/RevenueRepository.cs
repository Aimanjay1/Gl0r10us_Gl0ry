using BizOpsAPI.Data;
using BizOpsAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BizOpsAPI.Repositories
{
    public class RevenueRepository : IRevenueRepository
    {
        private readonly AppDbContext _context;
        public RevenueRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Revenue>> GetAllAsync() =>
            await _context.Revenues.AsNoTracking().ToListAsync();

        public async Task<Revenue?> GetByIdAsync(int id) =>
            await _context.Revenues.AsNoTracking().FirstOrDefaultAsync(r => r.RevenueId == id);

        public async Task<Revenue?> GetByInvoiceIdAsync(int invoiceId) =>
            await _context.Revenues.AsNoTracking().FirstOrDefaultAsync(r => r.InvoiceId == invoiceId);

        public async Task<Revenue> AddAsync(Revenue revenue)
        {
            _context.Revenues.Add(revenue);
            await _context.SaveChangesAsync();
            return revenue;
        }

        public async Task UpdateAsync(Revenue revenue)
        {
            _context.Revenues.Update(revenue);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var revenue = await _context.Revenues.FindAsync(id);
            if (revenue != null)
            {
                _context.Revenues.Remove(revenue);
                await _context.SaveChangesAsync();
            }
        }
    }
}