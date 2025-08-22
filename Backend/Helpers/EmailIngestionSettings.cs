namespace BizOpsAPI.Helpers
{
    public class EmailIngestionSettings
    {
        public bool Enable { get; set; }
        public string ImapServer { get; set; } = "imap.gmail.com";
        public int Port { get; set; } = 993;
        public string Username { get; set; } = "";
        public string AppPassword { get; set; } = "";
        public string Folder { get; set; } = "INBOX";
        public string? ProcessedFolder { get; set; }
        public string? SentFolder { get; set; }
        public string? ToFilter { get; set; }
        public string SubjectRegex { get; set; } = "#(?<id>\\d+)";
        public int PollSeconds { get; set; } = 30;
        public bool ProcessSeen { get; set; } = true;
        public int LookbackDays { get; set; } = 2;
        public string? ProtocolLogDir { get; set; }
    }
}
