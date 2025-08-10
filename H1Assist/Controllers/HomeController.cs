using System.Diagnostics;
using H1Assist.Models;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;

namespace H1Assist.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHtmlLocalizer<SharedResource> _localizer;

        public HomeController(IHtmlLocalizer<SharedResource> localizer)
        {
            this._localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
        }
        
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult SetLanguage(string culture, string returnUrl)
        {
            if (!Program.SupportedCultures.Contains(culture))
            {
                return BadRequest("Invalid culture specified.");
            }

            Response.Cookies.Append(
                key: CookieRequestCultureProvider.DefaultCookieName,
                value: CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                options: new CookieOptions() { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );

            return RedirectToAction("Index");
        }
    }
}