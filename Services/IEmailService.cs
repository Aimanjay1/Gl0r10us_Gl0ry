namespace BizOpsAPI.Services
{
    public interface IEmailService
    {
        // 🔹 return Message-Id so we can track the thread
        Task<string> SendEmailAsync(string toEmail, string subject, string htmlContent,
                                    byte[]? attachmentBytes = null, string? attachmentName = null);
    }
}
