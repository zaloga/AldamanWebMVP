namespace Aldaman.Services.Configuration;

/// <summary>
/// Settings for memory caching.
/// </summary>
public sealed class CacheSettings
{
    public const string SectionName = "Cache";

    public int ContentPageExpirationHours { get; set; } = 24;

    public int BlogExpirationHours { get; set; } = 24;

    public int SearchExpirationMinutes { get; set; } = 5;
}
