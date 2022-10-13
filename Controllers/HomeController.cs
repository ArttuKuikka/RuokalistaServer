using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RuokalistaServer.Data;

namespace RuokalistaServer.Controllers
{
    public class HomeController : Controller
    {

        private ApplicationDbContext db;

        public HomeController(ApplicationDbContext db)
        {
            this.db = db;
        }

        public IActionResult Index()
        {
            ViewBag.RuokaOlemassa = false;
            var viikko = System.Globalization.ISOWeek.GetWeekOfYear(DateTime.Now);

            var ruokalista = db.Ruokalista
              .Where(m => m.Year == DateTime.Now.Year).FirstOrDefaultAsync(k => k.WeekId == viikko).GetAwaiter().GetResult();

            if(ruokalista != null) { ViewBag.RuokaOlemassa = true; }
            ViewBag.ruokalista = ruokalista;
            return View();
        }
    }
}
