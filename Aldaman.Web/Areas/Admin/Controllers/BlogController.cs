using System.Security.Claims;
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

    [HttpGet]
    public IActionResult Create()
    {
        var model = BlogService.GetPostForCreate();
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Aldaman.Services.Dtos.Blog.BlogPostEditDto model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            await BlogService.CreatePostAsync(userId, model);
            TempData["SuccessMessage"] = "Post created successfully.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", "Error creating post: " + ex.Message);
            return View(model);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Details(Guid id)
    {
        var post = await BlogService.GetPostForEditAsync(id);
        if (post == null)
        {
            return NotFound();
        }

        return View(post);
    }

    [HttpGet]
    public async Task<IActionResult> Update(Guid id)
    {
        var post = await BlogService.GetPostForEditAsync(id);
        if (post == null)
        {
            return NotFound();
        }

        return View(post);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(Guid id, Aldaman.Services.Dtos.Blog.BlogPostEditDto model)
    {
        if (id != model.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            await BlogService.UpdatePostAsync(id, userId, model);
            TempData["SuccessMessage"] = "Post updated successfully.";
            return RedirectToAction(nameof(Index));
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", "Error updating post: " + ex.Message);
            return View(model);
        }
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
