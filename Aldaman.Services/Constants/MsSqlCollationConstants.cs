using Aldaman.Persistence.Constants;
using Aldaman.Services.Helpers;

namespace Aldaman.Services.Constants;

/// <summary>
/// Defines collation constants for Microsoft SQL Server.
/// </summary>
public static class MsSqlCollationConstants
{
    /// <summary>
    /// Czech case-insensitive, accent-sensitive collation (standard Windows collation).
    /// </summary>
    public const string CzechCaseInsensitiveAccentSensitive = "Czech_CI_AS";

    /// <summary>
    /// Latin1 General case-insensitive, accent-sensitive collation (standard modern Windows collation for English).
    /// </summary>
    public const string Latin1GeneralCaseInsensitiveAccentSensitive = "Latin1_General_CI_AS";

    /// <summary>
    /// Returns the appropriate SQL Server collation for the given culture code.
    /// </summary>
    /// <param name="cultureCode">The culture code (e.g. "cs", "cs-CZ", "en").</param>
    /// <returns>The collation string name.</returns>
    public static string GetCollationForCulture(string? cultureCode)
    {
        string normalizedCulture = CultureHelper.NormalizeCulture(cultureCode);

        return normalizedCulture switch
        {
            CultureConstants.Czech => CzechCaseInsensitiveAccentSensitive,
            CultureConstants.English => Latin1GeneralCaseInsensitiveAccentSensitive,
            _ => Latin1GeneralCaseInsensitiveAccentSensitive
        };
    }
}
