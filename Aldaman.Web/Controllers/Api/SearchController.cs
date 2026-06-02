using Aldaman.Services.Dtos.Search;
using Aldaman.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Aldaman.Web.Controllers.Api;

/// <summary>
/// REST API controller for website content search.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public sealed class SearchController : ControllerBase
{
    private ISearchService SearchService { get; }
    private ILogger<SearchController> Logger { get; }

    public SearchController(ISearchService searchService, ILogger<SearchController> logger)
    {
        SearchService = searchService;
        Logger = logger;
    }

    /// <summary>
    /// Searches for content in the website via REST API.
    /// </summary>
    /// <param name="query">The search term.</param>
    /// <param name="culture">Culture code (e.g., "cs", "en"). Defaults to "cs".</param>
    /// <returns>Search results.</returns>
    [HttpGet]
    public async Task<IActionResult> Search([FromQuery] string query, [FromQuery] string culture = "cs")
    {
        try
        {
            if (string.IsNullOrWhiteSpace(query) || query.Trim().Length < 2)
            {
                return BadRequest("Query parameter is required and must have at least 2 characters.");
            }

            var baseUrl = $"{Request.Scheme}://{Request.Host}{Request.PathBase}";
            var results = await SearchService.SearchCachedAsync(query, culture, baseUrl, HttpContext.RequestAborted);

            return Ok(new
            {
                Count = results.Count,
                Query = query,
                Culture = culture,
                Results = results
            });
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error during REST search for query: {Query}", query);
            return StatusCode(500, "An internal error occurred during search.");
        }
    }

    /// <summary>
    /// Searches for content for autocomplete (returns Title and Url only).
    /// </summary>
    /// <param name="query">The search term.</param>
    /// <param name="culture">Culture code (e.g., "cs", "en"). Defaults to "cs".</param>
    /// <returns>Autocomplete results.</returns>
    [HttpGet("autocomplete")]
    public async Task<IActionResult> Autocomplete([FromQuery] string query, [FromQuery] string culture = "cs")
    {
        try
        {
            if (string.IsNullOrWhiteSpace(query) || query.Trim().Length < 2)
            {
                return BadRequest("Query parameter is required and must have at least 2 characters.");
            }

            string baseUrl = $"{Request.Scheme}://{Request.Host}{Request.PathBase}";
            List<AutocompleteResultDto> results = await SearchService.AutocompleteCachedAsync(query, culture, baseUrl, HttpContext.RequestAborted);

            return Ok(new
            {
                Count = results.Count,
                Query = query,
                Culture = culture,
                Results = results
            });
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error during REST autocomplete for query: {Query}", query);
            return StatusCode(500, "An internal error occurred during autocomplete search.");
        }
    }
}
