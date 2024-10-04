using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RuokalistaServer.Data;
using RuokalistaServer.Models;
using RuokalistaServer.ViewModels;

namespace RuokalistaServer.Controllers
{
	[ApiExplorerSettings(IgnoreApi = true)]
	public class HomeController : Controller
    {

        private ApplicationDbContext db;

        public HomeController(ApplicationDbContext db)
        {
            this.db = db;
        }

        [HttpGet("/")]
        public async Task <IActionResult> Index(int? Year, int? Week)
        {
            var viikko = Week ?? System.Globalization.ISOWeek.GetWeekOfYear(DateTime.Now);
            var vuosi = Year ?? DateTime.Now.Year;

            var model = new IndexViewModel();

            //load temporarily the next weeks menu to do checks
            model.Ruokalista = db.Ruokalista.Where(m => m.Year == DateTime.Now.Year)?.FirstOrDefault(k => k.WeekId == viikko + 1);

            model.NextWeeksMenuExists = model.Ruokalista != null;
            //if not viewing custom week and year
            if (Week == null && Year == null)
            {
                if (DateTime.Today.DayOfWeek == DayOfWeek.Sunday || DateTime.Today.DayOfWeek == DayOfWeek.Saturday)
                {
                    model.ShowingNextWeeksMenu = true;
                    viikko += 1;
                }
                else if(DateTime.Today.DayOfWeek == DayOfWeek.Friday && DateTime.Now.Hour > 12)
                {
                    if(model.NextWeeksMenuExists)
                    {
                       model.ShowingNextWeeksMenu = true;
                        viikko += 1;
                    }
                }
            }
            

            if(!model.ShowingNextWeeksMenu)
            {
                model.Ruokalista = db.Ruokalista.Where(m => m.Year == vuosi)?.FirstOrDefault(k => k.WeekId == viikko);
            }
           

            if(model.Ruokalista != null)
            {
                if (model.Ruokalista.WeekId == System.Globalization.ISOWeek.GetWeekOfYear(DateTime.Now))
                {
                    model.ShowingCurrentWeeksMenu = true;
                    
                }
            }

            return View("Index", model);
        }

        [HttpGet("/{Year:int}/{Week:int}")]
        public async Task<IActionResult> SpesificIndex(int? Year, int? Week)
        {
            return await Index(Year, Week);
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
