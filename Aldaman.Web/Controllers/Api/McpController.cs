using Aldaman.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Aldaman.Web.Controllers.Api;

[ApiController]
[Route("api/[controller]")]
public sealed class McpController : ControllerBase
{
    private ISearchService SearchService { get; }
    private ILogger<McpController> Logger { get; }

    public McpController(ISearchService searchService, ILogger<McpController> logger)
    {
        SearchService = searchService;
        Logger = logger;
    }

    /// <summary>
    /// Searches for content in the website.
    /// </summary>
    /// <param name="query">The search term.</param>
    /// <param name="culture">The culture code (defaults to 'cs').</param>
    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string query, [FromQuery] string culture = "cs")
    {
        try
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return BadRequest("Query parameter is required.");
            }

            // Construct base URL for absolute links
            var baseUrl = $"{Request.Scheme}://{Request.Host}{Request.PathBase}";
            
            var results = await SearchService.SearchAsync(query, culture, baseUrl, HttpContext.RequestAborted);
            
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
            Logger.LogError(ex, "Error during MCP search for query: {Query}", query);
            return StatusCode(500, "An internal error occurred during search.");
        }
    }
}
