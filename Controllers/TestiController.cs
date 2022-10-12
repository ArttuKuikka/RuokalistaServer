using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace RuokalistaServer.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class TestiController : Controller
    {
        
        public IActionResult Index()
        {
            return Ok("K");
        }
    }
}
