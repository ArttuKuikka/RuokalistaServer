using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;

namespace RuokalistaServer.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class LanguageController : Controller
    {
        [HttpPost]
        public IActionResult SetLanguage(string culture, string returnUrl)
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1), IsEssential = true }
            );

            return LocalRedirect(returnUrl ?? "/");
        }
    }
}