using Aldaman.Services.Dtos.Search;

namespace Aldaman.Services.Interfaces;

public interface ISearchService
{
    /// <summary>
    /// Searches for content in blog posts and content pages.
    /// </summary>
    /// <param name="query">Search term.</param>
    /// <param name="cultureCode">Culture code (e.g. "cs", "en").</param>
    /// <param name="baseUrl">Base URL of the website (for absolute URLs).</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>List of search results.</returns>
    Task<List<SearchResultDto>> SearchAsync(string query, string cultureCode, string baseUrl, CancellationToken ct = default);
}
