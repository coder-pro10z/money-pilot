namespace MoneyPilot.SecurityHeaders.Options;

public class SecurityHeadersOptions
{
    public string XFrameOptions { get; set; } = "DENY";
    public string XContentTypeOptions { get; set; } = "nosniff";
    public string ReferrerPolicy { get; set; } = "strict-origin-when-cross-origin";
    public string HstsMaxAge { get; set; } = "31536000"; // 1 year in seconds
    public bool HstsIncludeSubDomains { get; set; } = true;
    public bool HstsPreload { get; set; } = false;
    public string ContentSecurityPolicy { get; set; } = "default-src 'self'";
}