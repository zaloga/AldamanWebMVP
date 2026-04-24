namespace Aldaman.Services.Configuration;

/// <summary>
/// Settings for application localization.
/// </summary>
public sealed class LocalizationSettings
{
    public const string SectionName = "Localization";

    public required string DefaultCulture { get; set; }

    public required string[] SupportedCultures { get; set; }
}
