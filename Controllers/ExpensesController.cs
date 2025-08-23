using BizOpsAPI.DTOs;
using BizOpsAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace BizOpsAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExpensesController : ControllerBase
    {
        private readonly IExpenseService _expenseService;

        public ExpensesController(IExpenseService expenseService)
        {
            _expenseService = expenseService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var expenses = await _expenseService.GetAllAsync();
            return Ok(expenses);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var expense = await _expenseService.GetByIdAsync(id);
            if (expense == null) return NotFound();
            return Ok(expense);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ExpenseCreateDto dto)
        {
            var created = await _expenseService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, ExpenseUpdateDto dto)
        {
            var updated = await _expenseService.UpdateAsync(id, dto);
            if (updated == null) return NotFound();
            return Ok(updated);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _expenseService.DeleteAsync(id);
            if (!result) return NotFound();
            return NoContent();
        }
    }
}
