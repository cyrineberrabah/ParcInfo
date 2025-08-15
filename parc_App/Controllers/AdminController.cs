using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize(AuthenticationSchemes = "MyCookieAuth")]
public class AdminController : Controller
{
    public IActionResult Dashboard()
    {
        return View();
    }
}
