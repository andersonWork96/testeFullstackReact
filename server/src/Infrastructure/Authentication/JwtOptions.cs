namespace HrManager.Infrastructure.Authentication;

public class JwtOptions
{
    public const string SectionName = "Jwt";

    public string Issuer { get; set; } = "hrmanager-api";
    public string Audience { get; set; } = "hrmanager-client";
    public string SecretKey { get; set; } = "change-me-please";
    public int ExpirationMinutes { get; set; } = 60;
}
