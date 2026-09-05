using Aldaman.Persistence.Context;
using Aldaman.Persistence.Entities;
using Aldaman.Persistence.Enums;
using Aldaman.Services.Configuration;
using Aldaman.Services.Dtos.Search;
using Aldaman.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace Aldaman.Services.Services;

public sealed class SearchService : ISearchService
{
    private const int MaxItemsForSearch = 20;
    private const int MaxItemsForAutocomplete = 10;

    private AppDbContext Context { get; }
    private IMemoryCache Cache { get; }
    private MemoryCacheEntryOptions CacheOptions { get; }

    public SearchService(AppDbContext context, IMemoryCache cache, IOptions<CacheSettings> cacheOptions)
    {
        Context = context;
        Cache = cache;
        CacheOptions = new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(cacheOptions.Value.SearchExpirationMinutes));
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
            var results = await SearchContentInternal(query, cultureCode)
                .Take(MaxItemsForSearch)
                .Select(t => new SearchResultDto
                {
                    Title = t.Title,
                    Content = t.PlainText ?? string.Empty,
                    Url = t.Content.PlaceToShow.HasFlag(PlaceToShowEnum.HomePage)
                        ? $"{baseUrl}/{cultureCode}#{t.Slug}"
                        : $"{baseUrl}/{cultureCode}/content/{t.Slug}",
                    Type = "Content"
                })
                .ToListAsync(ct);

            cachedResults = results
                .OrderByDescending(r => r.Title.Contains(query, StringComparison.OrdinalIgnoreCase))
                .Take(MaxItemsForSearch)
                .ToList();

            Cache.Set(cacheKey, cachedResults, CacheOptions);
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
            List<AutocompleteResultDto> results = await SearchContentInternal(query, cultureCode)
                .Take(MaxItemsForAutocomplete)
                .Select(t => new AutocompleteResultDto
                {
                    Title = t.Title,
                    Url = t.Content.PlaceToShow.HasFlag(PlaceToShowEnum.HomePage)
                        ? $"{baseUrl}/{cultureCode}#{t.Slug}"
                        : $"{baseUrl}/{cultureCode}/content/{t.Slug}"
                })
                .ToListAsync(ct);

            cachedResults = [.. results
                .OrderByDescending(r => r.Title.Contains(query, StringComparison.OrdinalIgnoreCase))
                .Take(MaxItemsForAutocomplete)];

            Cache.Set(cacheKey, cachedResults, CacheOptions);
        }

        return cachedResults;
    }

    private IOrderedQueryable<ContentTranslationEntity> SearchContentInternal(string query, string cultureCode)
    {
        return Context.ContentTranslations
            .Include(t => t.Content)
            .Where(t =>
                t.CultureCode == cultureCode
                && t.Content.IsPublished
                && (t.Title.Contains(query) || (t.PlainText != null && t.PlainText.Contains(query))))
            .OrderByDescending(t => t.Content.PublishedAtUtc ?? t.Content.CreatedAtUtc);
    }
}
