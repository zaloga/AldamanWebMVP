using Aldaman.Services.Dtos.General;
using Aldaman.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Aldaman.Web.Areas.Admin.Controllers;

public class BlogController : BaseAdminController
{
    private IBlogService BlogService { get; }
    private IMediaService MediaService { get; }

    public BlogController(IBlogService blogService, IMediaService mediaService)
    {
        BlogService = blogService;
        MediaService = mediaService;
    }

    public async Task<IActionResult> Index(
        [FromQuery] PaginationQuery query, 
        [FromQuery(Name = "deleted")] PaginationQuery deletedItemsQuery)
    {
        var culture = System.Globalization.CultureInfo.CurrentUICulture.Name;
        var result = await BlogService.GetPagedBlogPostsAdminAsync(query, culture);
        var deletedResult = await BlogService.GetPagedDeletedBlogPostsAsync(deletedItemsQuery, culture);
        
        ViewData["Query"] = query;
        ViewData["DeletedQuery"] = deletedItemsQuery;
        ViewBag.DeletedItems = deletedResult;
        
        return View(result);
    }

    [HttpGet]
    public IActionResult Create()
    {
        var model = BlogService.GetBlogPostForCreate();
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
            if (model.CoverImageFile != null && model.CoverImageFile.Length > 0)
            {
                using (var stream = model.CoverImageFile.OpenReadStream())
                {
                    var asset = await MediaService.UploadAsync(stream, model.CoverImageFile.FileName, model.CoverImageFile.ContentType);
                    model.CoverMediaAssetId = asset.Id;
                }
            }

            await BlogService.CreateBlogPostAsync(model);
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
        var post = await BlogService.GetBlogPostForEditAsync(id);
        if (post == null)
        {
            return NotFound();
        }

        return View(post);
    }

    [HttpGet]
    public async Task<IActionResult> Update(Guid id)
    {
        var post = await BlogService.GetBlogPostForEditAsync(id);
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
            if (model.CoverImageFile != null && model.CoverImageFile.Length > 0)
            {
                using (var stream = model.CoverImageFile.OpenReadStream())
                {
                    var asset = await MediaService.UploadAsync(stream, model.CoverImageFile.FileName, model.CoverImageFile.ContentType);
                    model.CoverMediaAssetId = asset.Id;
                }
            }

            await BlogService.UpdateBlogPostAsync(id, model);
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
            await BlogService.SoftDeleteBlogPostAsync(id);
            return Json(new { success = true, message = "Post deleted successfully." });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = "Error deleting post: " + ex.Message });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Restore(Guid id)
    {
        try
        {
            await BlogService.RestoreBlogPostAsync(id);
            return Json(new { success = true, message = "Post restored successfully." });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = "Error restoring post: " + ex.Message });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> HardDelete(Guid id)
    {
        try
        {
            await BlogService.HardDeleteBlogPostAsync(id);
            return Json(new { success = true, message = "Post permanently deleted." });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = "Error deleting post: " + ex.Message });
        }
    }
}
