using Microsoft.AspNetCore.Mvc;

namespace Aldaman.Web.Areas.Admin.Controllers;

public class BlogController : BaseAdminController
{
    public IActionResult Index()
    {
        return View();
    }
}
