namespace Aldaman.Web.Configuration;

/// <summary>
/// Settings for application localization.
/// </summary>
public sealed class LocalizationSettings
{
    public const string SectionName = "Localization";

    public string DefaultCulture { get; set; } = "cs";

    public string[] SupportedCultures { get; set; } = ["cs", "en"];
}
