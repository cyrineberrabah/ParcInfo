using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using parc_App.Models;
using System.Security.Claims;

public class LoginController : Controller
{
    private readonly Appdatacontext _context;

    public LoginController(Appdatacontext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult Login1()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login1(string email, string motDePasse)
    {
        var user = _context.Users.FirstOrDefault(u => u.Email == email && u.MotDePasse == motDePasse);

        if (user != null)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Email),
                new Claim("Role", "Admin") // Facultatif
            };

            var identity = new ClaimsIdentity(claims, "MyCookieAuth");
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync("MyCookieAuth", principal);

            return RedirectToAction("Dashboard", "Admin");
        }

        ViewBag.Message = "Email ou mot de passe incorrect.";
        return View();
    }

    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync("MyCookieAuth");
        return RedirectToAction("Login1");
    }
}
