using Microsoft.AspNetCore.Mvc;

namespace Aldaman.Web.Areas.Admin.Controllers;

public class MediaController : BaseAdminController
{
    public IActionResult Index()
    {
        return View();
    }
}
