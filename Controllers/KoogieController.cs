using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace RuokalistaServer.Controllers
{
    [Authorize(AuthenticationSchemes = "Identity.Application")]
    public class KoogieController : Controller
    {
        public IActionResult Index()
        {
            return Ok("Keksejä löytyi");
        }
    }
}
