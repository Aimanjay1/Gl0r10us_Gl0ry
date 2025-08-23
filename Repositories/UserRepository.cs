using Microsoft.EntityFrameworkCore;
using BizOpsAPI.Data;
using BizOpsAPI.Models;

namespace BizOpsAPI.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _ctx;
        public UserRepository(AppDbContext ctx) => _ctx = ctx;

        public async Task<IEnumerable<User>> GetAllAsync() =>
            await _ctx.Users
                .Include(u => u.Clients)
                .Include(u => u.Invoices)
                .Include(u => u.Expenses)
                .Include(u => u.Revenues)
                .ToListAsync();

        public async Task<User?> GetByIdAsync(int id) =>
            await _ctx.Users
                .Include(u => u.Clients)
                .Include(u => u.Invoices)
                .Include(u => u.Expenses)
                .Include(u => u.Revenues)
                .FirstOrDefaultAsync(u => u.UserId == id);

        public async Task<User?> GetByEmailAsync(string email) =>
            await _ctx.Users
                .Include(u => u.Clients)
                .Include(u => u.Invoices)
                .Include(u => u.Expenses)
                .Include(u => u.Revenues)
                .FirstOrDefaultAsync(u => u.Email == email);

        public async Task<User> CreateAsync(User user)
        {
            _ctx.Users.Add(user);
            await _ctx.SaveChangesAsync();
            return user;
        }

        public async Task UpdateAsync(User user)
        {
            _ctx.Users.Update(user);
            await _ctx.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _ctx.Users.FindAsync(id);
            if (entity is null) return;

            _ctx.Users.Remove(entity);
            await _ctx.SaveChangesAsync();
        }
    }
}
