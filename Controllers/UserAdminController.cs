using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RuokalistaServer.Data;

namespace RuokalistaServer.Controllers
{
    [Authorize]
    public class UserAdminController : Controller
    {
        private ApplicationDbContext db;

        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;

        public UserAdminController(ApplicationDbContext db, SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager)
        {
            this.db = db;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            ViewBag.users = _userManager.Users.ToList();
            return View();
        }

        public async Task<IActionResult> PoistaKayttaja(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            var result = await _userManager.DeleteAsync(user);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> PalautaSalasana(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            var result = await _userManager.RemovePasswordAsync(user);
            var r2 = await _userManager.AddPasswordAsync(user, "Y0yB0ogYs5!");
            return Ok("Käyttäjän salasana on nyt 'Y0yB0ogYs5!'");
        }

        [HttpPost]
        public async Task<IActionResult> UusiKayttaja(string email, string password)
        {
            var user = new IdentityUser { UserName = email, Email = email, EmailConfirmed = true };
            var result = await _userManager.CreateAsync(user, password);


            return RedirectToAction("Index");
        }
    }
}
