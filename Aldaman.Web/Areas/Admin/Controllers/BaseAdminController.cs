using System.Security.Claims;
using Aldaman.Services.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace Aldaman.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "SuperAdmin,Admin")]
public abstract class BaseAdminController : Controller
{
    protected IStringLocalizer<UIResources> Localizer { get; }

    protected BaseAdminController(IStringLocalizer<UIResources> localizer)
    {
        Localizer = localizer;
    }

    protected Guid GetUserId()
    {
        return Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    }
}
