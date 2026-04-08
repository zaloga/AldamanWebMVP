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

        PagedResultDto<BlogPostListItemDto> pagedPosts = await BlogService.GetPagedPostsAsync(
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

        BlogPostDetailDto? postDetail = await BlogService.GetPostBySlugAsync(
            slug,
            cultureCode
            /*cancellationToken*/);

        if (postDetail is null)
        {
            return NotFound();
        }

        return View(new BlogPostViewModel
        {
            Post = postDetail
        });
    }
}