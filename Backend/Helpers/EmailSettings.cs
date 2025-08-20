namespace BizOpsAPI.Helpers
{
    public class EmailSettings
    {
        public required string SenderEmail { get; set; }
        public string?   SenderName   { get; set; }
        public required string SmtpServer { get; set; }
        public int      Port         { get; set; } = 587;
        public string?  Username     { get; set; }
        public string?  Password     { get; set; }
        public string?  Security     { get; set; } = "StartTls"; // StartTls | SslOnConnect | None
        public int      TimeoutMs    { get; set; } = 20000;      // Socket timeout
        public int      MaxRetries   { get; set; } = 3;          // reconnect attempts
        public int      BackoffMs    { get; set; } = 1000;       // initial backoff
        public string?  ReplyTo      { get; set; }               // optional reply-to
        public bool     SkipAuthIfNoUsername { get; set; } = true; // allow open relays/dev sandboxes
    }
}
