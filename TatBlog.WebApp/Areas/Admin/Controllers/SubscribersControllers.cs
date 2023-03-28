using Microsoft.AspNetCore.Mvc;

namespace TatBlog.WebApp.Areas.Admin.Controllers;

public class SubscribersControllers : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}