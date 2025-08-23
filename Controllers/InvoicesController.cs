using Microsoft.AspNetCore.Mvc;
using BizOpsAPI.Services;
using BizOpsAPI.DTOs;
using AutoMapper;

namespace BizOpsAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InvoicesController : ControllerBase
    {
        private readonly IInvoiceService _svc;
        private readonly IMapper _mapper;

        public InvoicesController(IInvoiceService svc, IMapper mapper)
        {
            _svc = svc;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateInvoiceDto dto)
        {
            var invoice = await _svc.CreateInvoiceAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = invoice.InvoiceId }, invoice);
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetByUser(int userId)
        {
            var invoices = await _svc.GetInvoicesByUserAsync(userId);
            return Ok(invoices);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var invoice = await _svc.GetByIdAsync(id);
            if (invoice == null) return NotFound();
            return Ok(invoice);
        }

        [HttpPost("{id:int}/generate-pdf")]
        public async Task<IActionResult> GeneratePdf(int id)
        {
            var pdf = await _svc.GenerateInvoicePdfAsync(id);
            return File(pdf, "application/pdf", $"invoice_{id}.pdf");
        }

        [HttpPost("{id:int}/send-email")]
        public async Task<IActionResult> SendEmail(int id, [FromBody] SendInvoiceEmailDto dto)
        {
            await _svc.SendInvoiceEmailAsync(id, dto.Message);
            return Ok();
        }

        [HttpPost("{id:int}/mark-paid")]
        public async Task<IActionResult> MarkPaid(int id)
        {
            var ok = await _svc.MarkAsPaidAsync(id);
            if (!ok) return NotFound();
            return NoContent();
        }
    }
}
