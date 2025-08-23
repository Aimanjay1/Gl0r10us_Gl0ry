using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.Extensions.Options;
using BizOpsAPI.Helpers;
using MailKit.Security;
using MimeKit.Utils;

namespace BizOpsAPI.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _cfg;
        public EmailService(IOptions<EmailSettings> cfg) => _cfg = cfg.Value;

        public async Task<string> SendEmailAsync(
            string toEmail,
            string subject,
            string htmlContent,
            byte[]? attachmentBytes = null,
            string? attachmentName = null)
        {
            var msg = new MimeMessage();
            msg.From.Add(new MailboxAddress(_cfg.SenderName ?? string.Empty, _cfg.SenderEmail));
            msg.To.Add(MailboxAddress.Parse(toEmail));
            msg.Subject = subject;

            // ensure Message-Id exists and we can return it
            msg.MessageId ??= MimeUtils.GenerateMessageId();

            var body = new BodyBuilder { HtmlBody = htmlContent };
            if (attachmentBytes != null && attachmentName != null)
                body.Attachments.Add(attachmentName, attachmentBytes);
            msg.Body = body.ToMessageBody();

            var security = _cfg.Security?.ToLower() switch
            {
                "sslonconnect" or "ssl" => SecureSocketOptions.SslOnConnect,
                "none"                  => SecureSocketOptions.None,
                _                       => SecureSocketOptions.StartTls
            };

            using var smtp = new SmtpClient { Timeout = _cfg.TimeoutMs };

            try
            {
                await smtp.ConnectAsync(_cfg.SmtpServer, _cfg.Port, security);
                if (!string.IsNullOrWhiteSpace(_cfg.Username))
                    await smtp.AuthenticateAsync(_cfg.Username, _cfg.Password);

                await smtp.SendAsync(msg);
                return msg.MessageId!;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    $"SMTP failed (Host={_cfg.SmtpServer}, Port={_cfg.Port}, Security={_cfg.Security}). " +
                    $"Check host/port/SSL and credentials. Inner: {ex.Message}", ex);
            }
            finally
            {
                if (smtp.IsConnected)
                    await smtp.DisconnectAsync(true);
            }
        }
    }
}
