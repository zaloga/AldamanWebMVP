using Aldaman.Persistence.Context;
using Aldaman.Services.Dtos.Search;
using Aldaman.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Aldaman.Services.Services;

public sealed class SearchService : ISearchService
{
    private AppDbContext Context { get; }

    public SearchService(AppDbContext context)
    {
        Context = context;
    }

    public async Task<List<SearchResultDto>> SearchAsync(string query, string cultureCode, string baseUrl, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return new List<SearchResultDto>();
        }

        query = query.Trim();
        baseUrl = baseUrl.TrimEnd('/');

        // 1. Search Blog Posts
        var blogResults = await Context.BlogPostTranslations
            .Include(t => t.BlogPost)
            .Where(t => t.CultureCode == cultureCode && t.BlogPost.IsPublished)
            .Where(t => t.Title.Contains(query) || (t.PlainText != null && t.PlainText.Contains(query)))
            .OrderByDescending(t => t.BlogPost.PublishedAtUtc)
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
        var pageResults = await Context.ContentPageTranslations
            .Include(t => t.ContentPage)
            .Where(t => t.CultureCode == cultureCode)
            .Where(t => t.Title.Contains(query) || (t.PlainText != null && t.PlainText.Contains(query)))
            .OrderBy(t => t.ContentPage.PageOrder)
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
        var allResults = blogResults.Concat(pageResults)
            .OrderByDescending(r => r.Title.Contains(query, StringComparison.OrdinalIgnoreCase)) // Very basic relevance: title matches first
            .ToList();

        return allResults;
    }

    public async Task<List<AutocompleteResultDto>> AutocompleteAsync(string query, string cultureCode, string baseUrl, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return new List<AutocompleteResultDto>();
        }

        query = query.Trim();
        baseUrl = baseUrl.TrimEnd('/');

        // 1. Search Blog Posts
        var blogResults = await Context.BlogPostTranslations
            .Include(t => t.BlogPost)
            .Where(t => t.CultureCode == cultureCode && t.BlogPost.IsPublished)
            .Where(t => t.Title.Contains(query))
            .OrderByDescending(t => t.BlogPost.PublishedAtUtc)
            .Take(10)
            .Select(t => new AutocompleteResultDto
            {
                Title = t.Title,
                Url = $"{baseUrl}/{cultureCode}/blog/{t.Slug}"
            })
            .ToListAsync(ct);

        // 2. Search Content Pages
        var pageResults = await Context.ContentPageTranslations
            .Include(t => t.ContentPage)
            .Where(t => t.CultureCode == cultureCode)
            .Where(t => t.Title.Contains(query))
            .OrderBy(t => t.ContentPage.PageOrder)
            .Take(10)
            .Select(t => new AutocompleteResultDto
            {
                Title = t.Title,
                Url = $"{baseUrl}/{cultureCode}/page/{t.Slug}"
            })
            .ToListAsync(ct);

        // Combine and return
        var allResults = blogResults.Concat(pageResults)
            .OrderByDescending(r => r.Title.Contains(query, StringComparison.OrdinalIgnoreCase))
            .ToList();

        return allResults;
    }
}
