﻿using Microsoft.AspNetCore.Mvc;
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

            ViewBag.Paivat = viikonekapaiva.ToString("dd.MM") + "-" + viikonperjantai.ToString("dd.MM");


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
			int daysOffset = DayOfWeek.Thursday - jan1.DayOfWeek;

			// Use first Thursday in January to get first week of the year as
			// it will never be in Week 52/53
			DateTime firstThursday = jan1.AddDays(daysOffset);
			var cal = CultureInfo.CurrentCulture.Calendar;
			int firstWeek = cal.GetWeekOfYear(firstThursday, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);

			var weekNum = weekOfYear;
			// As we're adding days to a date in Week 1,
			// we need to subtract 1 in order to get the right date for week #1
			if (firstWeek == 1)
			{
				weekNum -= 1;
			}

			// Using the first Thursday as starting week ensures that we are starting in the right year
			// then we add number of weeks multiplied with days
			var result = firstThursday.AddDays(weekNum * 7);

			// Subtract 3 days from Thursday to get Monday, which is the first weekday in ISO8601
			return result.AddDays(-3);
		}

	}
}
