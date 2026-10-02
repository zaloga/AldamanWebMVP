namespace Aldaman.Services.Configuration;

/// <summary>
/// Settings for Cloudflare R2 object storage.
/// </summary>
public sealed class CloudflareR2Settings
{
    public const string SectionName = "CloudflareR2";

    public string AccessKey { get; set; } = string.Empty;

    public string SecretKey { get; set; } = string.Empty;

    public string ServiceUrl { get; set; } = string.Empty;

    public string BucketName { get; set; } = string.Empty;

    public string PublicUrlBase { get; set; } = string.Empty;
}
