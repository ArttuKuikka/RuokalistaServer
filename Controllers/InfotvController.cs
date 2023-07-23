using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RuokalistaServer.Data;

namespace RuokalistaServer.Controllers
{
    public class InfotvController : Controller
    {
        private ApplicationDbContext db;

        public InfotvController(ApplicationDbContext db)
        {
            this.db = db;
        }

        public IActionResult Index()
        {
            ViewBag.Nykyinenviikko = false;
            ViewBag.RuokaOlemassa = false;
            var viikko = System.Globalization.ISOWeek.GetWeekOfYear(DateTime.Now);
            var vuosi = DateTime.Now.Year;
            if (DateTime.Today.DayOfWeek == DayOfWeek.Sunday || DateTime.Today.DayOfWeek == DayOfWeek.Saturday)
            {
                ViewBag.NytOnSeuraavaViikko = true;
                viikko += 1;
            }
            if (DateTime.Today.DayOfWeek == DayOfWeek.Friday && DateTime.Now.Hour > 12)
            {
                if (db.Ruokalista.Where(m => m.Year == DateTime.Now.Year)?.FirstOrDefault(k => k.WeekId == viikko + 1) != null)
                {
                    ViewBag.NytOnSeuraavaViikko = true;
                    viikko += 1;
                }
            }

            ViewBag.viikko = viikko;

            var ruokalista = db.Ruokalista.Where(m => m.Year == vuosi)?.FirstOrDefault(k => k.WeekId == viikko);


            if (ruokalista != null)
            {
                if (ruokalista.WeekId == System.Globalization.ISOWeek.GetWeekOfYear(DateTime.Now))
                {
                    ViewBag.Nykyinenviikko = true;
                    
                }
                ViewBag.RuokaOlemassa = true;
            }


            ViewBag.SeuraavaViikkoNumero = viikko + 1;

            ViewBag.ruokalista = ruokalista;
            return View();
        }
    }
}
