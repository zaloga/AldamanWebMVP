using Aldaman.Services.Dtos.Blog;
using Aldaman.Services.Dtos.General;
using Aldaman.Services.Interfaces;
using Aldaman.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Aldaman.Web.Controllers;

[Route("clanky")] // TODO for more languages
public class BlogController : Controller
{
    private const string CultureCode = "cs-CZ"; // TODO for more languages
    private IBlogService BlogService { get; }

    public BlogController(IBlogService blogService)
    {
        BlogService = blogService;
    }

    [HttpGet]
    public async Task<IActionResult> Index([FromQuery] int p = 1)
    {
        int pageSize = 10;
        PagedResultDto<BlogPostListItemDto> pagedPosts = await BlogService.GetPagedPostsAsync(p, pageSize, CultureCode);

        BlogListViewModel viewModel = new()
        {
            Posts = pagedPosts
        };

        return View(viewModel);
    }

    [HttpGet("{slug}")]
    public async Task<IActionResult> Detail(string slug)
    {
        BlogPostDetailDto? postDetail = await BlogService.GetPostBySlugAsync(slug, CultureCode);

        if (postDetail is null)
        {
            return NotFound();
        }

        BlogPostViewModel viewModel = new()
        {
            Post = postDetail
        };

        return View(viewModel);
    }
}
