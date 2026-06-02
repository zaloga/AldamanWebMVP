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
    Task<List<SearchResultDto>> SearchCachedAsync(string query, string cultureCode, string baseUrl, CancellationToken ct = default);

    /// <summary>
    /// Searches for content and returns only Title and Url (for autocomplete).
    /// </summary>
    /// <param name="query">Search term.</param>
    /// <param name="cultureCode">Culture code (e.g. "cs", "en").</param>
    /// <param name="baseUrl">Base URL of the website.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>List of autocomplete results.</returns>
    Task<List<AutocompleteResultDto>> AutocompleteCachedAsync(string query, string cultureCode, string baseUrl, CancellationToken ct = default);
}
