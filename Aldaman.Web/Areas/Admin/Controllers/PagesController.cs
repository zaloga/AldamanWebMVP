using Microsoft.AspNetCore.Mvc;

namespace Aldaman.Web.Areas.Admin.Controllers;

public class PagesController : BaseAdminController
{
    public IActionResult Index()
    {
        return View();
    }
}
