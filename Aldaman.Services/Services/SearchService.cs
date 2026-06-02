using Aldaman.Persistence.Context;
using Aldaman.Persistence.Enums;
using Aldaman.Services.Dtos.Search;
using Aldaman.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Aldaman.Services.Services;

public sealed class SearchService : ISearchService
{
    private AppDbContext Context { get; }
    private IMemoryCache Cache { get; }

    public SearchService(AppDbContext context, IMemoryCache cache)
    {
        Context = context;
        Cache = cache;
    }

    public async Task<List<SearchResultDto>> SearchCachedAsync(string query, string cultureCode, string baseUrl, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return new List<SearchResultDto>();
        }

        query = query.Trim();
        baseUrl = baseUrl.TrimEnd('/');

        string cacheKey = $"Search:{cultureCode}:{baseUrl}:{query.ToLowerInvariant()}";

        if (!Cache.TryGetValue(cacheKey, out List<SearchResultDto>? cachedResults) || cachedResults == null)
        {
            // 1. Search Blog Posts
            var blogResults = await SearchBlogResultsInternal(query, cultureCode)
                .Take(20)
                .Select(t => new SearchResultDto
                {
                    Title = t.Title,
                    Content = t.PlainText ?? string.Empty,
                    Url = $"{baseUrl}/{cultureCode}/blog/{t.Slug}",
                    Type = "BlogPost"
                })
                .ToListAsync(ct);

            // 2. Search Content Pages
            var pageResults = await SearchContentPagesInternal(query, cultureCode)
                .Take(20)
                .Select(t => new SearchResultDto
                {
                    Title = t.Title,
                    Content = t.PlainText ?? string.Empty,
                    Url = $"{baseUrl}/{cultureCode}/page/{t.Slug}",
                    Type = "ContentPage"
                })
                .ToListAsync(ct);

            // Combine and return
            cachedResults = blogResults.Concat(pageResults)
                .OrderByDescending(r => r.Title.Contains(query, StringComparison.OrdinalIgnoreCase)) // Very basic relevance: title matches first
                .ToList();

            var cacheOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(5));

            Cache.Set(cacheKey, cachedResults, cacheOptions);
        }

        return cachedResults;
    }

    public async Task<List<AutocompleteResultDto>> AutocompleteCachedAsync(string query, string cultureCode, string baseUrl, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return new List<AutocompleteResultDto>();
        }

        query = query.Trim();
        baseUrl = baseUrl.TrimEnd('/');

        string cacheKey = $"Autocomplete:{cultureCode}:{baseUrl}:{query.ToLowerInvariant()}";

        if (!Cache.TryGetValue(cacheKey, out List<AutocompleteResultDto>? cachedResults) || cachedResults == null)
        {
            // 1. Search Blog Posts
            List<AutocompleteResultDto> blogResults = await SearchBlogResultsInternal(query, cultureCode)
                .Take(10)
                .Select(t => new AutocompleteResultDto
                {
                    Title = t.Title,
                    Url = $"{baseUrl}/{cultureCode}/blog/{t.Slug}"
                })
                .ToListAsync(ct);

            // 2. Search Content Pages
            List<AutocompleteResultDto> pageResults = await SearchContentPagesInternal(query, cultureCode)
                .Take(10)
                .Select(t => new AutocompleteResultDto
                {
                    Title = t.Title,
                    Url = t.ContentPage.PlaceToShow == PlaceToShowEnum.HomePage
                        ? $"{baseUrl}/{cultureCode}#{t.Slug}"
                        : $"{baseUrl}/{cultureCode}/page/{t.Slug}"
                })
                .ToListAsync(ct);

            // Combine and return
            cachedResults = [.. blogResults.Concat(pageResults)
                .OrderByDescending(r => r.Title.Contains(query, StringComparison.OrdinalIgnoreCase))
                .Take(10)];

            var cacheOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(5));

            Cache.Set(cacheKey, cachedResults, cacheOptions);
        }

        return cachedResults;
    }

    private IOrderedQueryable<Persistence.Entities.BlogPostTranslationEntity> SearchBlogResultsInternal(string query, string cultureCode)
    {
        return Context.BlogPostTranslations
                    .Include(t => t.BlogPost)
                    .Where(t =>
                        t.CultureCode == cultureCode
                        && t.BlogPost.IsPublished
                        && (t.Title.Contains(query) || (t.PlainText != null && t.PlainText.Contains(query))))
                    .OrderByDescending(t => t.BlogPost.PublishedAtUtc);
    }

    private IOrderedQueryable<Persistence.Entities.ContentPageTranslationEntity> SearchContentPagesInternal(string query, string cultureCode)
    {
        return Context.ContentPageTranslations
                    .Include(t => t.ContentPage)
                    .Where(t =>
                        t.CultureCode == cultureCode
                        && t.ContentPage.PlaceToShow != PlaceToShowEnum.None
                        && (t.Title.Contains(query) || (t.PlainText != null && t.PlainText.Contains(query))))
                    .OrderBy(t => t.ContentPage.PageOrder);
    }
}
