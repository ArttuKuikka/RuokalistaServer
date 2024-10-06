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
        public async Task <IActionResult> Index(int? Year, int? Week, bool kasvisruokalista = false)
        {
            var nykyinenViikko = System.Globalization.ISOWeek.GetWeekOfYear(DateTime.Now);
            var viikko = Week ?? nykyinenViikko;
            var vuosi = Year ?? DateTime.Now.Year;

            var model = new IndexViewModel();

            model.ShowingKasvisruokalista = kasvisruokalista;

            //load temporarily the next weeks menu to do checks
            if (!kasvisruokalista)
            {
                model.Ruokalista = db.Ruokalista.Where(m => m.Year == DateTime.Now.Year)?.FirstOrDefault(k => k.WeekId == viikko + 1);
            }
            else
            {
                model.Ruokalista = db.Kasvisruokalista.Where(m => m.Year == DateTime.Now.Year)?.FirstOrDefault(k => k.WeekId == viikko + 1);
            }

            

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
            

            //check if there is kasvisruokalista for the current week
            model.KasviruokalistaExists = db.Kasvisruokalista.Where(m => m.Year == DateTime.Now.Year)?.FirstOrDefault(k => k.WeekId == viikko) != null;

            if (!model.ShowingNextWeeksMenu)
            {
                if(!kasvisruokalista)
                {
                    model.Ruokalista = db.Ruokalista.Where(m => m.Year == vuosi)?.FirstOrDefault(k => k.WeekId == viikko);
                }
                else
                {
                    model.Ruokalista = db.Kasvisruokalista.Where(m => m.Year == vuosi)?.FirstOrDefault(k => k.WeekId == viikko);
                }
            }
           

            if(model.Ruokalista != null)
            {
                if (model.Ruokalista.WeekId == nykyinenViikko)
                {
                    model.ShowingCurrentWeeksMenu = true;
                    
                }
                else if (model.Ruokalista.WeekId == (nykyinenViikko + 1))
                {
                    model.ShowingNextWeeksMenu = true;

                }
            }

            return View("Index", model);
        }

        [HttpGet("/{Year:int}/{Week:int}")]
        public async Task<IActionResult> SpesificIndex(int? Year, int? Week)
        {
            return await Index(Year, Week);
        }

        [HttpGet("/Kasvisruokalista")]
        public async Task<IActionResult> Kasvisruokalista()
        {
            return await Index(DateTime.Now.Year, System.Globalization.ISOWeek.GetWeekOfYear(DateTime.Now), true);
        }

        [HttpGet("Kasvisruokalista/{Year:int}/{Week:int}")]
        public async Task<IActionResult> SpesificKasvisruokalista(int? Year, int? Week)
        {
            return await Index(Year, Week, true);
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
