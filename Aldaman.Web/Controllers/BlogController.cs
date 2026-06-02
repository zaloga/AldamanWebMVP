using System.Globalization;
using Aldaman.Services.Dtos.Blog;
using Aldaman.Services.Dtos.General;
using Aldaman.Services.Interfaces;
using Aldaman.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Aldaman.Web.Controllers;

public sealed class BlogController : Controller
{
    private const int DefaultPageSize = 10;

    private IBlogService BlogService { get; }

    public BlogController(IBlogService blogService)
    {
        BlogService = blogService;
    }

    [HttpGet]
    public async Task<IActionResult> Index([FromQuery] int p = 1, CancellationToken cancellationToken = default)
    {
        string cultureCode = CultureInfo.CurrentUICulture.Name;

        PagedResultDto<BlogPostListItemDto> pagedPosts = await BlogService.GetPagedBlogPostsAsync(
            p,
            DefaultPageSize,
            cultureCode
            /*cancellationToken*/);

        var viewModel = new BlogListViewModel
        {
            Posts = pagedPosts
        };

        return View(viewModel);
    }

    [HttpGet]
    public async Task<IActionResult> Detail(string slug, CancellationToken cancellationToken)
    {
        string cultureCode = CultureInfo.CurrentUICulture.Name;

        BlogPostDetailDto? postDetail = await BlogService.GetBlogPostBySlugAsync(
            slug,
            cultureCode
            /*cancellationToken*/);

        if (postDetail is null)
        {
            if (cultureCode != "cs")
            {
                var csSlug = await BlogService.GetRedirectSlugAsync(slug, "cs");
                if (csSlug != null)
                {
                    return RedirectToAction("Detail", "Blog", new { culture = "cs", slug = csSlug });
                }
            }
            return NotFound();
        }

        // Provide alternative URLs for the language switcher
        var alternativeSlugs = await BlogService.GetAlternativeSlugsAsync(postDetail.Id);
        var alternatives = new Dictionary<string, string>();
        foreach (var slugEntry in alternativeSlugs)
        {
            alternatives[slugEntry.Key] = Url.Action("Detail", "Blog", new { culture = slugEntry.Key, slug = slugEntry.Value }) ?? $"/{slugEntry.Key}";
        }
        ViewData["LanguageAlternatives"] = alternatives;

        var navigation = await BlogService.GetBlogPostNavigationAsync(postDetail.Id, cultureCode);

        return View(new BlogPostViewModel
        {
            Post = postDetail,
            PreviousPost = navigation.Previous,
            NextPost = navigation.Next
        });
    }
}