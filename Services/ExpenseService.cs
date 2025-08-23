using BizOpsAPI.DTOs;
using BizOpsAPI.Models;
using BizOpsAPI.Repositories;

namespace BizOpsAPI.Services
{
    public class ExpenseService : IExpenseService
    {
        private readonly IExpenseRepository _expenseRepository;

        public ExpenseService(IExpenseRepository expenseRepository)
        {
            _expenseRepository = expenseRepository;
        }

        public async Task<IEnumerable<ExpensesDto>> GetAllAsync()
        {
            var expenses = await _expenseRepository.GetAllAsync();
            return expenses.Select(e => new ExpensesDto
            {
                Id = e.ExpenseId,
                Description = e.ItemName,        // map ItemName → Description
                Amount = e.TotalPrice,           // map TotalPrice → Amount
                Date = e.CreatedAt
            });
        }

        public async Task<ExpensesDto?> GetByIdAsync(int id)
        {
            var expense = await _expenseRepository.GetByIdAsync(id);
            if (expense == null) return null;

            return new ExpensesDto
            {
                Id = expense.ExpenseId,
                Description = expense.ItemName,
                Amount = expense.TotalPrice,
                Date = expense.CreatedAt
            };
        }

        public async Task<ExpensesDto> CreateAsync(ExpenseCreateDto dto)
        {
            var expense = new Expense
            {
                UserId = 1,                       // TODO: set from auth context later
                ItemName = dto.Description,
                Category = "General",             // or add to DTO
                Quantity = 1,
                UnitPrice = dto.Amount,
                TotalPrice = dto.Amount,
                ReceiptUrl = string.Empty,        // handle upload later
                CreatedAt = dto.Date,             // honor incoming date

                // satisfy 'required' navigation if your model marks it required
                User = null!
            };

            var created = await _expenseRepository.AddAsync(expense);

            return new ExpensesDto
            {
                Id = created.ExpenseId,
                Description = created.ItemName,
                Amount = created.TotalPrice,
                Date = created.CreatedAt
            };
        }

        public async Task<ExpensesDto?> UpdateAsync(int id, ExpenseUpdateDto dto)
        {
            var existing = await _expenseRepository.GetByIdAsync(id);
            if (existing == null) return null;

            if (!string.IsNullOrWhiteSpace(dto.Description))
                existing.ItemName = dto.Description;

            if (dto.Amount.HasValue)
            {
                existing.UnitPrice = dto.Amount.Value;
                existing.TotalPrice = dto.Amount.Value; // assuming qty = 1
            }

            if (dto.Date.HasValue)
                existing.CreatedAt = dto.Date.Value;

            await _expenseRepository.UpdateAsync(existing);

            return new ExpensesDto
            {
                Id = existing.ExpenseId,
                Description = existing.ItemName,
                Amount = existing.TotalPrice,
                Date = existing.CreatedAt
            };
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _expenseRepository.GetByIdAsync(id);
            if (existing == null) return false;

            await _expenseRepository.DeleteAsync(id);
            return true;
        }
    }
}
