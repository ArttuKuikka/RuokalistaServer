using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RuokalistaServer.Attributes;
using RuokalistaServer.Data;

namespace RuokalistaServer.Controllers
{
    [Authorize]
    [OnlyRootAllowed]
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
            var model = new List<IdentityUser>();
            model = _userManager.Users.ToList();
            return View(model);
        }

        public async Task<IActionResult> PoistaKayttaja(string id)
        {
            if(_userManager.Users.Count() <= 1)
            {
                return BadRequest("Et voi poistaa viimeistä käyttäjää");
            }
            var user = await _userManager.FindByIdAsync(id);
            var result = await _userManager.DeleteAsync(user);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> PalautaSalasana(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            var salasana = RandomString(12);
            user.PasswordHash = _userManager.PasswordHasher.HashPassword(user, salasana);
            await _userManager.UpdateAsync(user);
            
            return Ok("Käyttäjän salasana on nyt " + salasana);
        }

        [HttpPost]
        public async Task<IActionResult> UusiKayttaja(string email, string password)
        {
            var user = new IdentityUser { UserName = email, Email = email, EmailConfirmed = true };
            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return RedirectToAction("Index");
        }

        public static string RandomString(int length)
        {
            Random rand = new Random();
            string charbase = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            return new string(Enumerable.Range(0, length)
                   .Select(_ => charbase[rand.Next(charbase.Length)])
                   .ToArray());
        }
    }
}
