using Aldaman.Services.Configuration;
using Microsoft.Extensions.Options;

namespace Aldaman.Web.Middleware;

/// <summary>
/// Middleware that ensures the URL starts with a supported culture.
/// Redirects requests without a culture segment to the default culture.
/// </summary>
public sealed class LocalizationRedirectMiddleware
{
    private readonly RequestDelegate _next;
    private readonly LocalizationSettings _settings;

    public LocalizationRedirectMiddleware(RequestDelegate next, IOptions<LocalizationSettings> options)
    {
        _next = next;
        _settings = options.Value;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        string? path = context.Request.Path.Value;

        // If we already matched a route (endpoint is not null), it means:
        // 1. It's a localized route that already matched (so culture is already present)
        // 2. It's an explicitly non-localized route (like /Admin/Media/UploadQuill)
        // In both cases, we don't want to redirect.
        if (context.GetEndpoint() != null)
        {
            await _next(context);
            return;
        }

        // Handle root or empty path
        if (string.IsNullOrWhiteSpace(path) || path == "/")
        {
            RedirectToDefault(context, "/");
            return;
        }

        // Check if the first segment is a supported culture (fallback)
        string[] segments = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
        if (segments.Length > 0)
        {
            string firstSegment = segments[0];

            // If it's a supported culture, we are good
            if (_settings.SupportedCultures.Contains(firstSegment, StringComparer.OrdinalIgnoreCase))
            {
                await _next(context);
                return;
            }

            // Skip common paths that should not be localized
            if (IsExcludedPath(firstSegment))
            {
                await _next(context);
                return;
            }

            // If it's a file request (has extension), don't redirect
            if (firstSegment.Contains('.') || Path.HasExtension(path))
            {
                await _next(context);
                return;
            }
        }

        // Redirect to default culture
        RedirectToDefault(context, path);
    }

    private void RedirectToDefault(HttpContext context, string path)
    {
        string culture = _settings.DefaultCulture;
        string newPath = $"/{culture}{path.EnsureStartsWithSlash()}";

        if (context.Request.QueryString.HasValue)
        {
            newPath += context.Request.QueryString.Value;
        }

        context.Response.Redirect(newPath, permanent: false);
    }

    private bool IsExcludedPath(string segment)
    {
        string[] excludedPrefixes = { "api", "health", "metrics", "swagger", "_framework", "_content", "media" };
        return excludedPrefixes.Contains(segment, StringComparer.OrdinalIgnoreCase);
    }
}

internal static class StringExtensions
{
    public static string EnsureStartsWithSlash(this string path)
    {
        if (string.IsNullOrEmpty(path)) return "/";
        return path.StartsWith('/') ? path : "/" + path;
    }
}
