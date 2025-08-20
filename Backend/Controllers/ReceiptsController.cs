using BizOpsAPI.DTOs;
using BizOpsAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace BizOpsAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReceiptsController : ControllerBase
    {
        private readonly IReceiptService _receiptService;
        private readonly ReceiptLinkService _links;

        public ReceiptsController(IReceiptService receiptService, ReceiptLinkService links)
        {
            _receiptService = receiptService;
            _links = links;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken ct)
        {
            var items = await _receiptService.GetAllAsync();
            await RewriteUrlsAsync(items, ct);
            return Ok(items);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id, CancellationToken ct)
        {
            var receipt = await _receiptService.GetByIdAsync(id);
            if (receipt is null) return NotFound();

            receipt.ReceiptUrl = await _links.ToDownloadUrlAsync(receipt.ReceiptUrl, ct: ct);
            return Ok(receipt);
        }

        // GET /api/Receipts/by-invoice/3
        [HttpGet("by-invoice/{invoiceId:int}")]
        public async Task<IActionResult> GetByInvoice(int invoiceId, CancellationToken ct)
        {
            var receipts = await _receiptService.GetByInvoiceIdAsync(invoiceId);
            await RewriteUrlsAsync(receipts, ct);
            return Ok(receipts);
        }

        // multipart/form-data upload
        [HttpPost]
        [Consumes("multipart/form-data")]
        [RequestSizeLimit(25_000_000)]
        public async Task<IActionResult> Create([FromForm] ReceiptCreateDto dto, CancellationToken ct)
        {
            var created = await _receiptService.CreateAsync(dto, ct);
            // Convert stored path to downloadable URL before returning
            created.ReceiptUrl = await _links.ToDownloadUrlAsync(created.ReceiptUrl, ct: ct);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] ReceiptUpdateDto dto, CancellationToken ct)
        {
            var updated = await _receiptService.UpdateAsync(id, dto, ct);
            if (updated is null) return NotFound();

            updated.ReceiptUrl = await _links.ToDownloadUrlAsync(updated.ReceiptUrl, ct: ct);
            return Ok(updated);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var ok = await _receiptService.DeleteAsync(id);
            return ok ? NoContent() : NotFound();
        }

        // Helper: batch rewrite list URLs to signed/public URLs
        private async Task RewriteUrlsAsync(IEnumerable<ReceiptListDto> items, CancellationToken ct)
        {
            var list = items.ToList();
            if (list.Count == 0) return;

            var map = await _links.ToDownloadUrlsAsync(list.Select(r => r.ReceiptUrl), ct: ct);
            foreach (var r in list)
            {
                if (map.TryGetValue(r.ReceiptUrl, out var url) && !string.IsNullOrEmpty(url))
                    r.ReceiptUrl = url;
            }
        }
    }
}
