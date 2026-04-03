namespace Aldaman.Services.Dtos.SiteConfiguration;

/// <summary>
/// General site-wide settings.
/// </summary>
public class SiteConfigurationDto
{
    public string SiteName { get; set; } = string.Empty;
    public string SiteTitlePrefix { get; set; } = string.Empty;
    public string? SiteDescription { get; set; }
    public string ContactEmail { get; set; } = string.Empty;
    public string? ContactPhone { get; set; }
    public string? OfficeAddress { get; set; }
    
    // Social links
    public string? FacebookUrl { get; set; }
    public string? InstagramUrl { get; set; }
    public string? LinkedInUrl { get; set; }
    
    // Default SEO
    public string? DefaultSeoKeywords { get; set; }
    public string? GoogleAnalyticsId { get; set; }
}
