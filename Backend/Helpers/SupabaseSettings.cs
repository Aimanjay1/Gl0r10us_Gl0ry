namespace BizOpsAPI.Helpers
{
    public class SupabaseSettings
    {
        public string Url { get; set; } = string.Empty;
        public string ServiceRoleKey { get; set; } = string.Empty;
        public StorageSettings Storage { get; set; } = new();
    }

    public class StorageSettings
    {
        public string Bucket { get; set; } = "receipts";
        public bool PublicBucket { get; set; } = false;
        public int SignedUrlTtlSeconds { get; set; } = 900;
    }
}
