using Aldaman.Services.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Aldaman.Web.Areas.Admin.Controllers;

// TODO zkontrolovat jestli je potřeba a kdyžtak vyhodit
public class ToolsController : BaseAdminController
{
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
