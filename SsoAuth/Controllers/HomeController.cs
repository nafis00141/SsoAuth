using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SsoAuth.Models;
using System.Diagnostics;
using System.Security.Claims;

namespace SsoAuth.Controllers
{
  public class HomeController : Controller
  {
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
      _logger = logger;
    }

    public IActionResult Index()
    {
      return View();
    }

    public IActionResult Privacy()
    {
      return View();
    }

    [Authorize]
    public IActionResult Secured()
    {
      return View();
    }

    [HttpGet, Route("Login")]
    public IActionResult Login(string returnUrl)
    {
      ViewData["ReturnUrl"] = returnUrl;
      return View();
    }

    [HttpPost, Route("Login")]
    public async Task<IActionResult> Validate(string username, string password, string? returnUrl)
    {
       var claims = new List<Claim> { 
         new Claim("username", username), 
         new Claim(ClaimTypes.NameIdentifier, username), 
         new Claim(ClaimTypes.Name, username)
       };
       await HttpContext.SignInAsync(new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme)));
       if(string.IsNullOrEmpty(returnUrl)) { return Ok(); }
       return Redirect(returnUrl);
    }

    [Authorize]
    public async Task<IActionResult> LogOut()
    {
       await HttpContext.SignOutAsync();
       return Redirect("/");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
      return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
  }
}