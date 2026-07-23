using Microsoft.AspNetCore.Http.Extensions;

namespace Aldaman.Web.Extensions;

/// <summary>
/// Provides extension methods for <see cref="HttpRequest"/> to facilitate URL manipulation,
/// query string building, and pagination handling.
/// </summary>
public static class HttpRequestExtensions
{
    /// <summary>
    /// Generates a relative URL for the current request, preserving all existing query string parameters
    /// while replacing or appending the specified pagination parameter with a target page number.
    /// </summary>
    /// <param name="request">The current HTTP request instance.</param>
    /// <param name="page">The page number to set in the query string.</param>
    /// <param name="paramName">The name of the query string parameter representing the page number. Defaults to <c>"page"</c>.</param>
    /// <returns>
    /// A relative URL string combining <see cref="HttpRequest.Path"/> and the modified query string 
    /// (e.g., <c>/Admin/Media?search=test&amp;page=2</c>).
    /// </returns>
    public static string GetPageUrl(this HttpRequest request, int page, string paramName = "page")
    {
        // Filter out any pre-existing parameter with the specified name to prevent duplicate query keys
        QueryBuilder query = new(request.Query.Where(q => !string.Equals(q.Key, paramName, StringComparison.OrdinalIgnoreCase)));

        // Append the parameter with its updated page number value
        query.Add(paramName, page.ToString());

        // Recombine request path and updated query string into a relative URL
        return request.Path + query.ToQueryString();
    }
}
