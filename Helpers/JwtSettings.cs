public class JwtSettings
{
    public string Secret { get; set; } = "";
    public string Issuer { get; set; } = "";
    public string Audience { get; set; } = "";
    public int ExpMinutes { get; set; } = 60;

    // Optional but recommended for deterministic derivation
    public string? KdfSalt { get; set; } = "BizOpsAPI.v1";
    public int KdfIterations { get; set; } = 150_000;
}
