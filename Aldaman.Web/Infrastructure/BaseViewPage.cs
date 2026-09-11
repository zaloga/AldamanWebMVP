using System.Globalization;
using Microsoft.AspNetCore.Mvc.Razor;

namespace Aldaman.Web.Infrastructure;

/// <summary>
/// Base class for Razor views providing shared properties and helpers.
/// </summary>
/// <typeparam name="TModel">The view model type.</typeparam>
public abstract class BaseViewPage<TModel> : RazorPage<TModel>
{
    /// <summary>
    /// Gets the two-letter ISO language name of the current UI culture (e.g., "cs", "en").
    /// </summary>
    public string CurrentCulture => CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
}
