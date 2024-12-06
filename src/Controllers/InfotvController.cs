using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RuokalistaServer.Data;
using RuokalistaServer.Models;
using RuokalistaServer.ViewModels;
using RuokalistaServer.Attributes;
using System.Net.Http;
using Newtonsoft.Json.Linq;

namespace RuokalistaServer.Controllers
{
    [Feature("Infotv")]
    public class InfotvController : Controller
    {
        private ApplicationDbContext db;

        public InfotvController(ApplicationDbContext db)
        {
            this.db = db;
        }

        public IActionResult Index(int? Week, int? Year, bool kasvisruokalista = false)
        {
			var viikko = Week ?? System.Globalization.ISOWeek.GetWeekOfYear(DateTime.Now);
			var vuosi = Year ?? DateTime.Now.Year;

            var model = new InfoTVViewModel();
            model.Week = viikko;

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

			var NextWeeksMenuExists = model.Ruokalista != null;
			//if not viewing custom week and year
			if (Week == null && Year == null)
			{
				if (DateTime.Today.DayOfWeek == DayOfWeek.Sunday || DateTime.Today.DayOfWeek == DayOfWeek.Saturday)
				{
					model.ShowingNextWeeksMenu = true;
					viikko += 1;
				}
				else if (DateTime.Today.DayOfWeek == DayOfWeek.Friday && DateTime.Now.Hour > 12)
				{
					if (NextWeeksMenuExists)
					{
						model.ShowingNextWeeksMenu = true;
						viikko += 1;
					}
				}
			}


			if (!model.ShowingNextWeeksMenu)
			{
                if (!kasvisruokalista)
                {
					model.Ruokalista = db.Ruokalista.Where(m => m.Year == vuosi)?.FirstOrDefault(k => k.WeekId == viikko);
				}
                else
                {
					model.Ruokalista = db.Kasvisruokalista.Where(m => m.Year == vuosi)?.FirstOrDefault(k => k.WeekId == viikko);
				}
			}


			if (model.Ruokalista != null)
			{
				if (model.Ruokalista.WeekId == System.Globalization.ISOWeek.GetWeekOfYear(DateTime.Now))
				{
					model.ShowingCurrentWeeksMenu = true;

				}
			}


			
			return View("Index", model);
		}

        public IActionResult Kasvisruokalista(int? Week, int? Year)
        {
			return Index(Week, Year, true);
		}

		

        public async Task<IActionResult> GetBgForWeek(int week)
        {
            

            var bg = db.BackgroundForWeek.FirstOrDefault(x => x.WeekId == week && x.Year == DateTime.Now.Year);
            if(bg == null)
            {
				var newImage = "";
				bg = new BackgroundForWeek
				{
					WeekId = week,
					Year = DateTime.Today.Year
				};

				if (string.IsNullOrEmpty(GlobalConfig.StaticContentHost))
				{
					newImage = GetLocalImage();
				}
				else
				{
					try
					{
						HttpClient client = new HttpClient();
						HttpResponseMessage response = await client.GetAsync(GlobalConfig.StaticContentHost + "/api/v1/GetPictureListing");
						response.EnsureSuccessStatusCode();
						string responseBody = await response.Content.ReadAsStringAsync();
						JArray json = JArray.Parse(responseBody);


						Random random = new Random();
						int index = random.Next(json.Count);

						newImage = GlobalConfig.StaticContentHost + "/api/v1/GetPicture?filename=" + json[index];
					}
					catch(Exception)
					{
						//just return the fallback image without saving to db

						newImage = "/samplebg_infotv.jpg";

					}

				}

				bg.FileName = System.Web.HttpUtility.HtmlEncode(newImage);
				await db.BackgroundForWeek.AddAsync(bg);
                await db.SaveChangesAsync();
                

			}
			return Ok(bg);
		}

        public string GetLocalImage()
		{
			if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("BackgroundsPath")))
			{
				throw new Exception("The 'BackgroundsPath' Environment variable is Null or empty");

			}

			string[] files = Directory.GetFiles(Environment.GetEnvironmentVariable("BackgroundsPath"));



			var imageFiles = files?.Where(x => x.EndsWith(".jpg") || x.EndsWith(".png"));
			var imageFileCount = imageFiles?.Count() ?? 0;

			if (imageFileCount.Equals(0))
			{
				throw new Exception($"0 Image(.png or .jpg) files found in the 'BackgroundsPath', {files?.Count() ?? 0} files overall");
			}



			string currentMonthString = DateTime.Today.Month.ToString();
			if (currentMonthString.Length == 1)
			{
				currentMonthString = "0" + currentMonthString;
			}


			var thisMonthsImageFiles = imageFiles?.Where(x => (Path.GetFileName(x)[4].ToString() + Path.GetFileName(x)[5].ToString()) == currentMonthString);

			if (!(thisMonthsImageFiles?.Any() ?? false))
			{
				//jos tässä kuussa ei oo kuvia ota vaan jotai randomilla
				thisMonthsImageFiles = imageFiles;
			}

			var thisMonthsImageFileCount = thisMonthsImageFiles?.Count() ?? 0;


			Random random = new Random();
			int randomNumber = random.Next(0, thisMonthsImageFileCount);
			var newImage = thisMonthsImageFiles?.ElementAtOrDefault(randomNumber);
			newImage = Path.GetFileName(newImage);

			if (newImage == null)
			{
				throw new Exception("Error while picking new random backround picture, element is null at index");
			}

			return "/infotv/GetBgByFilename?filename=" + System.Web.HttpUtility.HtmlEncode(newImage);
		}

        public IActionResult GetBgByFilename(string filename)
        {
            if (!string.IsNullOrEmpty(GlobalConfig.StaticContentHost))
            {
                return BadRequest("External static content is configured for this server");
            }


            if(!filename.EndsWith(".jpg") && !filename.EndsWith(".png"))
            {
                return BadRequest("Wrong extension type");
            }

            var path = Environment.GetEnvironmentVariable("BackgroundsPath");
            if (string.IsNullOrEmpty(path))
            {
				throw new Exception("The 'BackgroundsPath' Environment variable is Null or empty");
			}


			string[] files = Directory.GetFiles(path);

			var imageFiles = files?.Where(x => x.EndsWith(".jpg") || x.EndsWith(".png"));
			var imageFileCount = imageFiles?.Count() ?? 0;

			if (imageFileCount.Equals(0))
			{
				throw new Exception($"0 Image(.png or .jpg) files found in the 'BackgroundsPath', {files?.Count() ?? 0} files overall");
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
