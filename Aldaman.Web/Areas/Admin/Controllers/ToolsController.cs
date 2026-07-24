using Aldaman.Services.Helpers;
using Aldaman.Services.Resources;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace Aldaman.Web.Areas.Admin.Controllers;

/// <summary>
/// Used from Generate Slug button.
/// </summary>
public class ToolsController : BaseAdminController
{
    public ToolsController(IStringLocalizer<UIResources> localizer)
        : base(localizer)
    {
    }

    [HttpGet]
    public IActionResult GenerateSlug(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return Json(new { slug = string.Empty });
        }

        var slug = StringHelpers.ToSlug(text);
        return Json(new { slug });
    }
}
