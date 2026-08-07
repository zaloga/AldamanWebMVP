using Microsoft.AspNetCore.Mvc.Rendering;

namespace Aldaman.Web.Extensions;

/// <summary>
/// Provides extension methods for <see cref="ViewContext"/> to evaluate navigation link active state based on route data and query string parameters.
/// </summary>
public static class NavigationExtensions
{
    /// <summary>
    /// Checks whether the current request matches the specified controller, action, and optional slug parameter.
    /// </summary>
    /// <param name="viewContext">The view context instance.</param>
    /// <param name="controller">Target controller name.</param>
    /// <param name="action">Optional target action name.</param>
    /// <param name="slug">Optional target slug parameter.</param>
    /// <returns><c>true</c> if the route matches the current context; otherwise, <c>false</c>.</returns>
    public static bool IsNavActive(this ViewContext viewContext, string controller, string? action = null, string? slug = null)
    {
        string? currentController = viewContext.RouteData.Values["controller"]?.ToString();
        if (string.IsNullOrEmpty(currentController) && viewContext.HttpContext?.Request.Query.TryGetValue("controller", out var queryController) == true)
        {
            currentController = queryController.ToString();
        }

        if (!string.Equals(currentController, controller, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        if (!string.IsNullOrEmpty(action))
        {
            string? currentAction = viewContext.RouteData.Values["action"]?.ToString();
            if (string.IsNullOrEmpty(currentAction) && viewContext.HttpContext?.Request.Query.TryGetValue("action", out var queryAction) == true)
            {
                currentAction = queryAction.ToString();
            }

            if (!string.Equals(currentAction, action, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }
        }

        if (!string.IsNullOrEmpty(slug))
        {
            string? currentSlug = viewContext.RouteData.Values["slug"]?.ToString();
            if (string.IsNullOrEmpty(currentSlug))
            {
                currentSlug = viewContext.RouteData.Values["id"]?.ToString();
            }

            if (string.IsNullOrEmpty(currentSlug) && viewContext.HttpContext != null)
            {
                if (viewContext.HttpContext.Request.Query.TryGetValue("slug", out var querySlug))
                {
                    currentSlug = querySlug.ToString();
                }
                else if (viewContext.HttpContext.Request.Query.TryGetValue("id", out var queryId))
                {
                    currentSlug = queryId.ToString();
                }
            }

            if (string.IsNullOrEmpty(currentSlug))
            {
                return false;
            }

            return string.Equals(currentSlug.Trim('/'), slug.Trim('/'), StringComparison.OrdinalIgnoreCase);
        }

        return true;
    }
}
