using Microsoft.AspNetCore.Mvc;
using RuokalistaServer.Data;
using System.Globalization;

namespace RuokalistaServer.Controllers
{
    public class IGController : Controller
    {
        private ApplicationDbContext db;

        public IGController(ApplicationDbContext db)
        {
            this.db = db;
        }
        public async Task<IActionResult> Index(int? weekId, int? Year)
        {
            ViewBag.Nykyinenviikko = false;
            ViewBag.RuokaOlemassa = false;
            var viikko = weekId ?? System.Globalization.ISOWeek.GetWeekOfYear(DateTime.Now);
			ViewBag.Viikko = viikko;    
            var vuosi = Year ?? DateTime.Now.Year;

            var viikonekapaiva = GetFirstDayOfWeek(vuosi, viikko);
            var viikonperjantai = viikonekapaiva.AddDays(4);

            ViewBag.Paivat = viikonekapaiva.ToString("d.MM") + "-" + viikonperjantai.ToString("d.MM");


			if (weekId == null)
            {
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
            }

            ViewBag.viikko = viikko;

            var ruokalista = db.Ruokalista.Where(m => m.Year == vuosi)?.FirstOrDefault(k => k.WeekId == viikko);


            if (ruokalista != null)
            {
                if (ruokalista.WeekId == System.Globalization.ISOWeek.GetWeekOfYear(DateTime.Now))
                {
                    ViewBag.Nykyinenviikko = true;
                    ViewBag.SeuraavaViikko = db.Ruokalista.Where(m => m.Year == DateTime.Now.Year)?.FirstOrDefault(k => k.WeekId == viikko + 1);
                }
                ViewBag.RuokaOlemassa = true;
                ViewBag.Vuosi = ruokalista.Year;
            }
            else
            {
                return NotFound("Tämäm viikon ruokalistaa ei ole olemassa");
            }


            ViewBag.SeuraavaViikkoNumero = viikko + 1;

            ViewBag.ruokalista = ruokalista;
            return View();
        }

		public static DateTime GetFirstDayOfWeek(int year, int weekOfYear)
		{
			DateTime jan1 = new DateTime(year, 1, 1);
			int daysOffset = DayOfWeek.Monday - jan1.DayOfWeek;
			DateTime firstMonday = jan1.AddDays(daysOffset);
			int firstWeek = CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(jan1, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);

			if (firstWeek <= 1)
			{
				weekOfYear -= 1;
			}

			return firstMonday.AddDays(weekOfYear * 7);
		}

	}
}
