using Aldaman.Services.Dtos.General;
using Aldaman.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Aldaman.Web.Areas.Admin.Controllers;

public class BlogController : BaseAdminController
{
    private IBlogService BlogService { get; }

    public BlogController(IBlogService blogService)
    {
        BlogService = blogService;
    }

    public async Task<IActionResult> Index(PaginationQuery query)
    {
        var result = await BlogService.GetPagedPostsAdminAsync(query);
        ViewData["Query"] = query;
        return View(result);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            await BlogService.DeletePostAsync(id);
            return Json(new { success = true, message = "Post deleted successfully." });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = "Error deleting post: " + ex.Message });
        }
    }
}
