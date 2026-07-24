using Aldaman.Services.Dtos.Blog;
using Aldaman.Services.Dtos.General;
using Aldaman.Services.Interfaces;
using Aldaman.Services.Resources;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace Aldaman.Web.Areas.Admin.Controllers;

public class BlogController : BaseAdminController
{
    private IBlogService BlogService { get; }
    private IMediaService MediaService { get; }
    private IStringLocalizer<UIResources> Localizer { get; }

    public BlogController(IBlogService blogService, IMediaService mediaService, IStringLocalizer<UIResources> localizer)
    {
        BlogService = blogService;
        MediaService = mediaService;
        Localizer = localizer;
    }

    public async Task<IActionResult> Index([FromQuery] PaginationQuery query)
    {
        string culture = System.Globalization.CultureInfo.CurrentUICulture.Name;
        PagedResultDto<BlogPostListItemDto> result = await BlogService.GetPagedBlogPostsAdminAsync(query, culture);
        return View(result);
    }

    public async Task<IActionResult> Deleted([FromQuery] PaginationQuery query)
    {
        string culture = System.Globalization.CultureInfo.CurrentUICulture.Name;
        PagedResultDto<BlogPostListItemDto> result = await BlogService.GetPagedBlogPostsAdminAsync(query, culture, filterDeleted: true);
        return View(result);
    }

    [HttpGet]
    public IActionResult Create()
    {
        var model = BlogService.GetBlogPostForCreate();
        return View("Update", model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Aldaman.Services.Dtos.Blog.BlogPostEditDto model)
    {
        if (!ModelState.IsValid)
        {
            return View("Update", model);
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
            TempData["SuccessMessage"] = Localizer[UIResourceKeys.PostCreatedSuccessfully].Value;
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", Localizer[UIResourceKeys.ErrorCreatingPost, ex.Message]);
            return View("Update", model);
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
            TempData["SuccessMessage"] = Localizer[UIResourceKeys.PostUpdatedSuccessfully].Value;
            return RedirectToAction(nameof(Index));
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", Localizer[UIResourceKeys.ErrorUpdatingPost, ex.Message]);
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
            return Json(new { success = true, message = Localizer[UIResourceKeys.DeletedSuccessfully].Value });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = Localizer[UIResourceKeys.ErrorDeleting, ex.Message].Value });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Restore(Guid id)
    {
        try
        {
            await BlogService.RestoreBlogPostAsync(id);
            return Json(new { success = true, message = Localizer[UIResourceKeys.RestoredSuccessfully].Value });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = Localizer[UIResourceKeys.ErrorRestoring, ex.Message].Value });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> HardDelete(Guid id)
    {
        try
        {
            await BlogService.HardDeleteBlogPostAsync(id);
            return Json(new { success = true, message = Localizer[UIResourceKeys.PermanentlyDeleted].Value });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = Localizer[UIResourceKeys.ErrorPermanentlyDeleting, ex.Message].Value });
        }
    }
}
