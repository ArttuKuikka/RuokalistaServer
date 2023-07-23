using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RuokalistaServer.Data;
using RuokalistaServer.Models;

namespace RuokalistaServer.Controllers
{
    public class HomeController : Controller
    {

        private ApplicationDbContext db;

        public HomeController(ApplicationDbContext db)
        {
            this.db = db;
        }

        [HttpGet("/")]
        public async Task <IActionResult> Index(int? weekId, int? Year)
        {
            ViewBag.Nykyinenviikko = false;
            ViewBag.RuokaOlemassa = false;
            var viikko = weekId ?? System.Globalization.ISOWeek.GetWeekOfYear(DateTime.Now);
            var vuosi = Year ?? DateTime.Now.Year;
            if (weekId== null)
            {
                if (DateTime.Today.DayOfWeek == DayOfWeek.Sunday || DateTime.Today.DayOfWeek == DayOfWeek.Saturday)
                {
                    ViewBag.NytOnSeuraavaViikko = true;
                    viikko += 1;
                }
                if(DateTime.Today.DayOfWeek == DayOfWeek.Friday && DateTime.Now.Hour > 12)
                {
                    if(db.Ruokalista.Where(m => m.Year == DateTime.Now.Year)?.FirstOrDefault(k => k.WeekId == viikko + 1) != null)
                    {
                        ViewBag.NytOnSeuraavaViikko = true;
                        viikko+= 1;
                    }
                }
            }
          
            ViewBag.viikko = viikko;

            var ruokalista = db.Ruokalista.Where(m => m.Year == vuosi)?.FirstOrDefault(k => k.WeekId == viikko);
           

            if(ruokalista != null)
            {
                if (ruokalista.WeekId == System.Globalization.ISOWeek.GetWeekOfYear(DateTime.Now))
                {
                    ViewBag.Nykyinenviikko = true;
                    ViewBag.SeuraavaViikko = db.Ruokalista.Where(m => m.Year == DateTime.Now.Year)?.FirstOrDefault(k => k.WeekId == viikko + 1);
                }
                ViewBag.RuokaOlemassa = true;
                ViewBag.Vuosi = ruokalista.Year;
            }


            ViewBag.SeuraavaViikkoNumero = viikko + 1;
            
            ViewBag.ruokalista = ruokalista;
            return View();
        }

        [HttpGet("Listaa")]
        public async Task<IActionResult> Listaa()
        {
            var ruokalista = db.Ruokalista?.ToList();
            ruokalista.Reverse();

            return View(ruokalista);
        }
    }
}
