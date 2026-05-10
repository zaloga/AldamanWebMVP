namespace Aldaman.Web.ViewModels;

public sealed class LanguageSwitcherViewModel
{
    public required string CurrentCulture { get; init; }
    public required List<LanguageInfo> SupportedLanguages { get; init; } = new();
}

public sealed class LanguageInfo
{
    public required string Culture { get; init; }
    public required string DisplayName { get; init; }
    public required string Url { get; init; }
    public bool IsActive { get; init; }
}
