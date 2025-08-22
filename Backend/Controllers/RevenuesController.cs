using BizOpsAPI.DTOs;
using BizOpsAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace BizOpsAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RevenuesController : ControllerBase
    {
        private readonly IRevenueService _revenueService;

        public RevenuesController(IRevenueService revenueService)
        {
            _revenueService = revenueService;
        }

        // GET: api/revenues
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var revenues = await _revenueService.GetAllAsync();
            return Ok(revenues);
        }

        // GET: api/revenues/{id}
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var revenue = await _revenueService.GetByIdAsync(id);
            if (revenue == null) return NotFound();
            return Ok(revenue);
        }

        // GET: api/revenues/by-invoice/{invoiceId}
        [HttpGet("by-invoice/{invoiceId:int}")]
        public async Task<IActionResult> GetByInvoiceId(int invoiceId)
        {
            var revenue = await _revenueService.GetByInvoiceIdAsync(invoiceId);
            if (revenue == null) return NotFound();
            return Ok(revenue);
        }

        // POST: api/revenues
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] RevenueCreateDto dto)
        {
            var created = await _revenueService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        // POST: api/revenues/from-invoice/{invoiceId}
        // Creates a revenue entry from a PAID invoice. If the invoice isn't paid, returns 409.
        [HttpPost("from-invoice/{invoiceId:int}")]
        public async Task<IActionResult> CreateFromPaidInvoice(int invoiceId)
        {
            // You can pull user id from auth later. For now pass 1.
            var created = await _revenueService.CreateFromInvoicePaidAsync(invoiceId, userId: 1);
            if (created == null) return Conflict(new { message = "Invoice is not in 'Paid' status or revenue already exists." });
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        // PUT: api/revenues/{id}
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] RevenueUpdateDto dto)
        {
            var updated = await _revenueService.UpdateAsync(id, dto);
            if (updated == null) return NotFound();
            return Ok(updated);
        }

        // DELETE: api/revenues/{id}
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _revenueService.DeleteAsync(id);
            if (!result) return NotFound();
            return NoContent();
        }
    }
}