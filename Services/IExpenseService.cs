using BizOpsAPI.DTOs;

namespace BizOpsAPI.Services
{
    public interface IExpenseService
    {
        Task<IEnumerable<ExpensesDto>> GetAllAsync();
        Task<ExpensesDto?> GetByIdAsync(int id);
        Task<ExpensesDto> CreateAsync(ExpenseCreateDto dto);
        Task<ExpensesDto?> UpdateAsync(int id, ExpenseUpdateDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
