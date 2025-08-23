using BizOpsAPI.Models;

namespace BizOpsAPI.Repositories
{
    public interface IExpenseRepository
    {
        Task<IEnumerable<Expense>> GetAllAsync();
        Task<Expense?> GetByIdAsync(int id);
        Task<Expense> AddAsync(Expense expense);
        Task UpdateAsync(Expense expense);
        Task DeleteAsync(int id);
    }
}
