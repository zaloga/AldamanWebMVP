using Aldaman.Persistence.Context;
using Aldaman.Persistence.Enums;
using Aldaman.Services.Configuration;
using Aldaman.Services.Dtos.Navigation;
using Aldaman.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

namespace Aldaman.Services.Services;

public sealed class NavigationService : INavigationService
{
    private static CancellationTokenSource _navigationCacheTokenSource = new();

    private AppDbContext Context { get; }
    private IMemoryCache Cache { get; }
    private MemoryCacheEntryOptions CacheOptions { get; }

    public NavigationService(
        AppDbContext context,
        IOptions<CacheSettings> cacheOptions,
        IMemoryCache cache)
    {
        Context = context;
        Cache = cache;
        CacheOptions = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(TimeSpan.FromHours(cacheOptions.Value.ContentExpirationHours))
            .AddExpirationToken(new CancellationChangeToken(_navigationCacheTokenSource.Token));
    }

    /// <summary>
    /// Instantly invalidates all navigation-related cached entries.
    /// It swaps the shared <see cref="_navigationCacheTokenSource"/> with a new instance and cancels the old one,
    /// triggering eviction for all cache entries associated with the cancellation change token.
    /// </summary>
    internal static void InvalidateCache()
    {
        var oldSource = Interlocked.Exchange(ref _navigationCacheTokenSource, new CancellationTokenSource());
        oldSource.Cancel();
        oldSource.Dispose();
    }

    public async Task<IEnumerable<NavigationDto>> GetHomePageNavigationAsync(string culture, CancellationToken ct = default)
    {
        return await GetNavigationInternalCachedAsync(culture, PlaceToShowEnum.HomePage, ct);
    }

    public async Task<IEnumerable<NavigationDto>> GetTopNavigationAsync(string culture, CancellationToken ct = default)
    {
        return await GetNavigationInternalCachedAsync(culture, PlaceToShowEnum.TopNavigation, ct);
    }

    public async Task<IEnumerable<NavigationDto>> GetFooterNavigationAsync(string culture, CancellationToken ct = default)
    {
        return await GetNavigationInternalCachedAsync(culture, PlaceToShowEnum.Footer, ct);
    }

    private async Task<IEnumerable<NavigationDto>> GetNavigationInternalCachedAsync(string culture, PlaceToShowEnum placeToShow, CancellationToken ct = default)
    {
        string cacheKey = $"Navigation:{culture}:{placeToShow}";

        if (!Cache.TryGetValue(cacheKey, out IEnumerable<NavigationDto>? result) || result == null)
        {
            var contents = await Context.Contents
                .Where(p => p.PlaceToShow.HasFlag(placeToShow) && p.IsPublished)
                .SelectMany(p => p.Translations.Where(t => t.CultureCode == culture))
                .Where(t => !string.IsNullOrEmpty(t.Title) && !string.IsNullOrEmpty(t.Slug))
                .Select(t => new NavigationDto
                {
                    Title = t.Title,
                    Slug = t.Slug,
                    IsGroup = false,
                    Order = t.Content.Order
                })
                .ToListAsync(ct);

            var groups = await Context.ContentGroups
                .Where(g => g.PlaceToShow.HasFlag(placeToShow))
                .SelectMany(g => g.Translations.Where(t => t.CultureCode == culture))
                .Where(t => !string.IsNullOrEmpty(t.Title) && !string.IsNullOrEmpty(t.Slug))
                .Select(t => new NavigationDto
                {
                    Title = t.Title,
                    Slug = t.Slug,
                    IsGroup = true,
                    Order = t.ContentGroup.GroupOrder
                })
                .ToListAsync(ct);

            result = contents.Concat(groups)
                .OrderBy(item => item.Order)
                .ToList();

            Cache.Set(cacheKey, result, CacheOptions);
        }

        return result;
    }
}
