using Microsoft.AspNetCore.Mvc;
using RuokalistaServer.Data;
using RuokalistaServer.Models;
using RuokalistaServer.ViewModels;
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
        public IActionResult Index(int? Year, int? Week, bool kasvisruokalista = false)
        {

            var viikko = Week ?? System.Globalization.ISOWeek.GetWeekOfYear(DateTime.Now);   
            var vuosi = Year ?? DateTime.Now.Year;

            var model = new IGViewModel();
			model.ShowingKasvisruokalista = kasvisruokalista;

			if (Week == null && Year == null)
            {
                if (DateTime.Today.DayOfWeek == DayOfWeek.Sunday || DateTime.Today.DayOfWeek == DayOfWeek.Saturday)
                {
                    viikko += 1;
                }
                if (DateTime.Today.DayOfWeek == DayOfWeek.Friday && DateTime.Now.Hour > 12)
                {
                    if (db.Ruokalista.Where(m => m.Year == DateTime.Now.Year)?.FirstOrDefault(k => k.WeekId == viikko + 1) != null)
                    {
                        viikko += 1;
                    }
                }
            }

			var viikonekapaiva = GetFirstDayOfWeek(vuosi, viikko);
			var viikonperjantai = viikonekapaiva.AddDays(4);

			model.Päivät = viikonekapaiva.ToString("dd.MM") + "-" + viikonperjantai.ToString("dd.MM");



			if (!kasvisruokalista)
			{
				model.Ruokalista = db.Ruokalista.Where(m => m.Year == vuosi)?.FirstOrDefault(k => k.WeekId == viikko);
			}
			else
			{
				model.Ruokalista = db.Kasvisruokalista.Where(m => m.Year == vuosi)?.FirstOrDefault(k => k.WeekId == viikko);

			}
      
		  if(model.Ruokalista == null)
		  {
			return NotFound("Tämän viikon ruokalistaa ei ole olemassa");
		  }

            return View("Index",model);
        }

		public IActionResult Kasvisruokalista(int? Year, int? Week)
		{
			return Index(Year, Week, true);
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
