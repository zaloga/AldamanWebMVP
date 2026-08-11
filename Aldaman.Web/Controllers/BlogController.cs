using System.Globalization;
using Aldaman.Services.Configuration;
using Aldaman.Services.Dtos.Blog;
using Aldaman.Services.Interfaces;
using Aldaman.Web.Extensions;
using Aldaman.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Aldaman.Web.Controllers;

public sealed class BlogController : Controller
{
    private IBlogService BlogService { get; }
    private LocalizationSettings LocalizationSettings { get; }
    private ILogger<BlogController> Logger { get; }

    public BlogController(IBlogService blogService, IOptions<LocalizationSettings> localizationOptions, ILogger<BlogController> logger)
    {
        BlogService = blogService;
        LocalizationSettings = localizationOptions.Value;
        Logger = logger;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public async Task<IActionResult> Detail(string slug, CancellationToken cancellationToken)
    {
        string cultureCode = CultureInfo.CurrentUICulture.Name;

        BlogPostDetailDto? postDetail = await BlogService.GetBlogPostBySlugCachedAsync(
            slug,
            cultureCode
            /*cancellationToken*/);

        if (postDetail is null)
        {
            Logger.LogWarning("Requested blog post content does not exist. Slug: {Slug}, CultureCode: {CultureCode}", slug, cultureCode);
            string defaultCulture = LocalizationSettings.DefaultCulture;
            if (cultureCode != defaultCulture)
            {
                var fallbackSlug = await BlogService.GetRedirectSlugCachedAsync(slug, defaultCulture);
                if (fallbackSlug != null)
                {
                    TempData.SetShowTranslationMissingToast(true);
                    return RedirectToAction("Detail", "Blog", new { culture = defaultCulture, slug = fallbackSlug });
                }
            }
            return NotFound();
        }

        // Provide alternative URLs for the language switcher
        var alternativeSlugs = await BlogService.GetAlternativeSlugsCachedAsync(postDetail.Id);
        var alternatives = new Dictionary<string, string>();
        foreach (var slugEntry in alternativeSlugs)
        {
            alternatives[slugEntry.Key] = Url.Action("Detail", "Blog", new { culture = slugEntry.Key, slug = slugEntry.Value }) ?? $"/{slugEntry.Key}";
        }
        ViewData.SetLanguageAlternatives(alternatives);

        var navigation = await BlogService.GetBlogPostNavigationCachedAsync(postDetail.Id, cultureCode);

        return View(new BlogPostViewModel
        {
            Post = postDetail,
            PreviousPost = navigation.Previous,
            NextPost = navigation.Next
        });
    }

    [HttpGet]
    public async Task<IActionResult> GetContentBySlug(string slug, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(slug))
        {
            return BadRequest();
        }

        string cultureCode = CultureInfo.CurrentUICulture.Name;
        BlogPostDetailDto? postDetail = await BlogService.GetBlogPostBySlugCachedAsync(slug, cultureCode);

        if (postDetail is null)
        {
            return NotFound();
        }

        return Json(new { bodyHtml = postDetail.BodyHtml ?? string.Empty });
    }
}