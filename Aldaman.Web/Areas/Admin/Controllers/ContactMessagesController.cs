using Microsoft.AspNetCore.Mvc;

namespace Aldaman.Web.Areas.Admin.Controllers;

public class ContactMessagesController : BaseAdminController
{
    public IActionResult Index()
    {
        return View();
    }
}
