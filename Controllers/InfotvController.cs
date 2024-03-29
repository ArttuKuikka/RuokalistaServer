﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RuokalistaServer.Data;
using RuokalistaServer.Models;

namespace RuokalistaServer.Controllers
{
    public class InfotvController : Controller
    {
        private ApplicationDbContext db;

        public InfotvController(ApplicationDbContext db)
        {
            this.db = db;
        }

        public IActionResult Index(int? weekId, int? Year)
        {
			ViewBag.Nykyinenviikko = false;
			ViewBag.RuokaOlemassa = false;
			var viikko = weekId ?? System.Globalization.ISOWeek.GetWeekOfYear(DateTime.Now);
			var vuosi = Year ?? DateTime.Now.Year;
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
			ViewBag.Vuosi = vuosi;

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

		

        public async Task<IActionResult> GetBgForWeek(int week)
        {
            

            var bg = db.BackgroundForWeek.FirstOrDefault(x => x.WeekId == week && x.Year == DateTime.Now.Year);
            if(bg == null)
            {
                if (Environment.GetEnvironmentVariable("BackgroundsPath").IsNullOrEmpty())
                {
                    throw new Exception("The 'BackgroundsPath' Environment variable is Null or empty");
                    
                }

				string[] files = Directory.GetFiles(Environment.GetEnvironmentVariable("BackgroundsPath"));

                

                var imageFiles = files?.Where(x => x.EndsWith(".jpg") || x.EndsWith(".png"));
                var imageFileCount = imageFiles?.Count() ?? 0;

                if (imageFileCount.Equals(0))
                {
                    throw new Exception($"0 Image(.png or .jpg) files found in the 'BackgroundsPath', {files.Count()} files overall");
                }



                string currentMonthString = DateTime.Today.Month.ToString();
                if (currentMonthString.Length == 1)
                {
                    currentMonthString = "0" + currentMonthString;
                }


                var thisMonthsImageFiles = imageFiles?.Where(x => (Path.GetFileName(x)[4].ToString() + Path.GetFileName(x)[5].ToString()) == currentMonthString);

				if(!(thisMonthsImageFiles?.Any() ?? false))
				{
					//jos tässä kuussa ei oo kuvia ota vaan jotai randomilla
					thisMonthsImageFiles = imageFiles;
				}

				var thisMonthsImageFileCount = thisMonthsImageFiles?.Count() ?? 0;


                Random random = new Random();
				int randomNumber = random.Next(0, thisMonthsImageFileCount);
                var newImage = thisMonthsImageFiles?.ElementAtOrDefault(randomNumber);
                newImage = Path.GetFileName(newImage);

                if(newImage == null)
                {
                    throw new Exception("Error while picking new random backround picture, element is null at index");
                }

                bg = new BackgroundForWeek
                {
                    FileName = System.Web.HttpUtility.HtmlEncode(newImage),
                    WeekId = week,
                    Year = DateTime.Today.Year
                };

                await db.BackgroundForWeek.AddAsync(bg);
                await db.SaveChangesAsync();
                

			}
            bg.FileName = "/infotv/GetBgByFilename?filename=" + System.Web.HttpUtility.HtmlEncode(bg.FileName);
			return Ok(bg);
		}

        public IActionResult GetBgByFilename(string filename)
        {
            if(!filename.EndsWith(".jpg") && !filename.EndsWith(".png"))
            {
                return BadRequest("Wrong extension type");
            }

            var path = Environment.GetEnvironmentVariable("BackgroundsPath");
            if (path.IsNullOrEmpty())
            {
				throw new Exception("The 'BackgroundsPath' Environment variable is Null or empty");
			}


			string[] files = Directory.GetFiles(path);

			var imageFiles = files?.Where(x => x.EndsWith(".jpg") || x.EndsWith(".png"));
			var imageFileCount = imageFiles?.Count() ?? 0;

			if (imageFileCount.Equals(0))
			{
				throw new Exception($"0 Image(.png or .jpg) files found in the 'BackgroundsPath', {files.Count()} files overall");
			}

            var returnFile = imageFiles?.FirstOrDefault(x => Path.GetFileName(x) == System.Web.HttpUtility.HtmlDecode(filename));
            if (returnFile != null) 
            {
                if (returnFile.EndsWith(".png"))
                {
					return new FileStreamResult(new FileStream(Path.Combine(path, returnFile), FileMode.Open, FileAccess.Read), "image/png");
				}
                else
                {
					return new FileStreamResult(new FileStream(Path.Combine(path, returnFile), FileMode.Open, FileAccess.Read), "image/jpeg");
				}
                
            }


			return BadRequest("Filename not found");
        }

	}
}
