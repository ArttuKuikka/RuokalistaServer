using Microsoft.AspNetCore.Mvc;
using RuokalistaServer.Models;

namespace RuokalistaServer.Controllers
{
	public class AppController : Controller
	{
		[HttpGet]
		[Route("api/v1/App/Config")]
		public IActionResult Config()
		{
			var config = new AppConfig
			{
				Branding = GlobalConfig.BrandingName,
				PrimaryColor = GlobalConfig.PrimaryColor,
				KasvisruokalistaEnabled = GlobalConfig.KasvisruokalistaEnabled,
				AanestysEnabled = GlobalConfig.AanestysEnabled,
				DefaultLanguage = GlobalConfig.DefaultLanguage
			};
			return Json(config);
		}
	}
}
