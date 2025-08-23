using BizOpsAPI.DTOs;

namespace BizOpsAPI.Services
{
    public interface IReceiptService
    {
        Task<IEnumerable<ReceiptListDto>> GetAllAsync();
        Task<ReceiptListDto?> GetByIdAsync(int id);
        Task<IEnumerable<ReceiptListDto>> GetByInvoiceIdAsync(int invoiceId);

        Task<ReceiptListDto> CreateAsync(ReceiptCreateDto dto, CancellationToken ct = default);
        Task<ReceiptListDto> CreateFromEmailAsync(ReceiptEmailCreateDto dto, CancellationToken ct = default);

        Task<ReceiptListDto?> UpdateAsync(int id, ReceiptUpdateDto dto, CancellationToken ct = default);

        Task<bool> DeleteAsync(int id);
    }
}
