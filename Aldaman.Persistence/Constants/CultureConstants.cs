namespace Aldaman.Persistence.Constants;

/// <summary>
/// Defines culture constants (ISO 639-1 language codes).
/// </summary>
public static class CultureConstants
{
    /// <summary>
    /// Czech language code.
    /// </summary>
    public const string Czech = "cs";

    /// <summary>
    /// English language code.
    /// </summary>
    public const string English = "en";

    /// <summary>
    /// Default application culture code.
    /// </summary>
    public const string Default = Czech;

    /// <summary>
    /// Supported application culture codes.
    /// </summary>
    public static readonly string[] Supported = [Czech, English];
}
