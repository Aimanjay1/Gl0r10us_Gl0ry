// Services/IEmailReceiptIngestionJob.cs
using System.Threading;

namespace BizOpsAPI.Services
{
    public record EmailIngestionResult(
        int MessagesExamined,
        int MessagesMatched,
        int FilesSaved,
        int ReceiptsCreated
    );

    public interface IEmailReceiptIngestionJob
    {
        Task<EmailIngestionResult> RunOnceAsync(CancellationToken ct = default);
    }
}
