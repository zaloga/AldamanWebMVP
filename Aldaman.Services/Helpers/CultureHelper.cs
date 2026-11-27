using System.Globalization;
using Aldaman.Persistence.Constants;

namespace Aldaman.Services.Helpers;

/// <summary>
/// Helper methods for working with cultures and ISO language codes.
/// </summary>
public static class CultureHelper
{
    /// <summary>
    /// Normalizes the provided culture string to a supported two-letter ISO language code ("cs", "en").
    /// Falls back to CurrentUICulture if null or whitespace, or Default culture if unsupported or invalid.
    /// </summary>
    /// <param name="culture">The input culture code (e.g. "cs-CZ", "en-US", "cs", "en").</param>
    /// <returns>A normalized, supported culture code.</returns>
    public static string NormalizeCulture(string? culture)
    {
        string candidate = string.IsNullOrWhiteSpace(culture)
            ? CultureInfo.CurrentUICulture.TwoLetterISOLanguageName
            : ExtractTwoLetterCode(culture);

        return CultureConstants.Supported.Contains(candidate, StringComparer.OrdinalIgnoreCase)
            ? candidate.ToLowerInvariant()
            : CultureConstants.Default;
    }

    private static string ExtractTwoLetterCode(string culture)
    {
        try
        {
            return CultureInfo.GetCultureInfo(culture).TwoLetterISOLanguageName;
        }
        catch (CultureNotFoundException)
        {
            return culture.Length >= 2 ? culture[..2] : culture;
        }
    }
}
