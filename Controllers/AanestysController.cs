using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RuokalistaServer.Data;
using RuokalistaServer.Models;
using System.Text.Json;

namespace RuokalistaServer.Controllers
{
	
	
	public class AanestysController : Controller
	{

		private ApplicationDbContext db;

		public AanestysController(ApplicationDbContext db)
		{
			this.db = db;
		}
		public IActionResult Tulokset(string? isApp = "false")
		{
			string layout = "_Layout";
            ViewBag.isApp = false;
            if (isApp == "true")
			{
				layout = "_LayoutApp";
				ViewBag.isApp = true;
			}
			ViewBag.Layout = layout;
			return View();
		}

        [HttpGet]
        [Route("api/v1/Aanestys/ProsenttiTuloksetTietyllaAikavalilla")]
        public IActionResult ProsenttiTuloksetTietyllaAikavalilla(string start, string? end)
		{
			var startDate = DateTime.Parse(start);
            var endDate = DateTime.Now;
            if (end != null && end != "")
			{
				endDate = DateTime.Parse(end);
			}
			var startWeek = System.Globalization.ISOWeek.GetWeekOfYear(startDate);
			var endWeek = System.Globalization.ISOWeek.GetWeekOfYear(endDate);
       
			//FIX!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! Toimii mutta antaa tiestysti koko viikon ruokalista vaikka vaan yks päivä valittu viikosta
            var ruokalistaObjects = db.Ruokalista.Where(item =>
            (item.Year > startDate.Year || (item.Year == startDate.Year && item.WeekId >= startWeek)) &&
            (item.Year < endDate.Year || (item.Year == endDate.Year && item.WeekId <= endWeek))
        );

            var returnList = new List<ProsenttiTulos>();
            foreach (var r in ruokalistaObjects)
            {

                var prosentit = new ViikonProsentit
                {
                    maanantai = 0,
					tiistai = 0,
					keskiviikko = 0,
					torstai = 0,
					perjantai = 0
                };

                var VV = db.Votes.FirstOrDefault(x => x.ruokalistaId == r.Id);
				if (VV != null)
				{
					prosentit.maanantai = CalculateFoodScore(VV.level1_votes_maanantai, VV.level2_votes_maanantai, VV.level3_votes_maanantai, VV.level4_votes_maanantai);
					prosentit.tiistai = CalculateFoodScore(VV.level1_votes_tiistai, VV.level2_votes_tiistai, VV.level3_votes_tiistai, VV.level4_votes_tiistai);
					prosentit.keskiviikko = CalculateFoodScore(VV.level1_votes_keskiviikko, VV.level2_votes_keskiviikko, VV.level3_votes_keskiviikko, VV.level4_votes_keskiviikko);
					prosentit.torstai = CalculateFoodScore(VV.level1_votes_torstai, VV.level2_votes_torstai, VV.level3_votes_torstai, VV.level4_votes_torstai);
					prosentit.perjantai = CalculateFoodScore(VV.level1_votes_perjantai, VV.level2_votes_perjantai, VV.level3_votes_perjantai, VV.level4_votes_perjantai);
				}
                


                var rObj = new ProsenttiTulos
                {
                    ruokalista = r,
                    prosentit = prosentit,

                };
                returnList.Add(rObj);
            }

            return Ok(JsonConvert.SerializeObject(returnList));
        }

		private double CalculateFoodScore(int level1, int level2, int level3, int level4)
		{
			var yhteensa = level1 + level2 + level3 + level4;
			if(yhteensa == 0)
			{
				return 0;
			}

			return (1 * ((double)level4 / yhteensa)) + (0.75 * ((double)level3 / yhteensa)) + (0.50 * ((double)level2 / yhteensa)) + (0.25 * ((double)level1 / yhteensa));

        }



		[HttpGet]
		[Route("api/v1/Aanestys/Tulokset")]
		public IActionResult ApiTulokset(int? start, int take)
		{
			if (start == null)
			{
				start = 0;
			}

			var currentWeekNumber = System.Globalization.ISOWeek.GetWeekOfYear(DateTime.Now);

            var ruokalistaObjects = db.Ruokalista.OrderByDescending(x => x.Year).ThenByDescending(x => x.WeekId).Skip((int)start).Take(take).ToList();

			var returnList = new List<ÄänestysTulos>();
			foreach (var r in ruokalistaObjects)
			{
				var isCurrectWeek = false;
				if(r.WeekId == currentWeekNumber)
				{
					isCurrectWeek = true;
				}

				var rObj = new ÄänestysTulos
				{
					ruokalista = r,
					votes = db.Votes.FirstOrDefault(x => x.ruokalistaId == r.Id),
					isCurrentWeek = isCurrectWeek,
					currentDay = (int)DateTime.Today.DayOfWeek
				};
				returnList.Add(rObj);
			}

			return Ok(JsonConvert.SerializeObject(returnList));
		}

		[HttpGet]
		[Route("api/v1/Aanestys/Tulos")]
		public IActionResult ApiTulos(int weekId, int Year)
		{
            var currentWeekNumber = System.Globalization.ISOWeek.GetWeekOfYear(DateTime.Now);

            var ruokalistaObject = db.Ruokalista.Where(x => x.Year == Year).FirstOrDefault(x => x.WeekId == weekId);
			if(ruokalistaObject == null)
			{
				return NotFound("No ruokalista for week " + weekId.ToString());
			}

            var isCurrectWeek = false;
            if (weekId == currentWeekNumber)
            {
                isCurrectWeek = true;
            }

            var rObj = new ÄänestysTulos
			{
				ruokalista = ruokalistaObject,
				votes = db.Votes.FirstOrDefault(x => x.ruokalistaId == ruokalistaObject.Id),
				isCurrentWeek = isCurrectWeek,
                currentDay = (int)DateTime.Today.DayOfWeek
            };

			return Ok(JsonConvert.SerializeObject(rObj));
		}





		/// <summary>
		/// Äänestää tämän päivän ruokaa määrätyllä tasolla (1-4)
		/// </summary>
		/// <param name="taso">Äänestykset taso 1 = huonoin, 4 = paras</param>
		/// <returns>
		/// 200 ok: äänestys onnistui
		/// 400 bad request: äänestystso ei ole välillä 1 ja 4
		/// Problem: yleinen ongelma, lisätiedot detail kohdasta
		/// </returns>
		[HttpPost]
		[Route("api/v1/Aanestys/Aanesta")]
		[Authorize(AuthenticationSchemes = "Bearer")]
		public async Task<IActionResult> Aanesta(int taso)
		{
			if(taso < 1 && taso > 5) 
			{
				return BadRequest("ei sallittu aanestystaso");
			}
			
			int viikko = System.Globalization.ISOWeek.GetWeekOfYear(DateTime.Now);
			int vuosi = DateTime.Now.Year;
			int viikonPäiva = (int)DateTime.Today.DayOfWeek;

			var ruokalista = db.Ruokalista.Where(m => m.Year == vuosi)?.FirstOrDefault(k => k.WeekId == viikko);

			if (ruokalista == null)
			{
				return Problem("Taman viikon ruokalistaa ei vielä ole olemassa");
			}
			VoteModel? VoteObject;
			bool voteModelExist = true;
			VoteObject = db.Votes.FirstOrDefault(x => x.ruokalistaId == ruokalista.Id);
			if (VoteObject == null)
			{
				voteModelExist = false;
				VoteObject =new VoteModel() 
				{
					ruokalistaId = ruokalista.Id
				};
			}

			switch (viikonPäiva)
			{
				case 1:
					switch (taso)
					{
						case 1:
							VoteObject.level1_votes_maanantai++;
							break;
						case 2:
							VoteObject.level2_votes_maanantai++;
							break;
						case 3:
							VoteObject.level3_votes_maanantai++;
							break;
						case 4:
							VoteObject.level4_votes_maanantai++;
							break;
					}
					break;

				case 2:
					switch (taso)
					{
						case 1:
							VoteObject.level1_votes_tiistai++;
							break;
						case 2:
							VoteObject.level2_votes_tiistai++;
							break;
						case 3:
							VoteObject.level3_votes_tiistai++;
							break;
						case 4:
							VoteObject.level4_votes_tiistai++;
							break;
					}
					break;

				case 3:
					switch (taso)
					{
						case 1:
							VoteObject.level1_votes_keskiviikko++;
							break;
						case 2:
							VoteObject.level2_votes_keskiviikko++;
							break;
						case 3:
							VoteObject.level3_votes_keskiviikko++;
							break;
						case 4:
							VoteObject.level4_votes_keskiviikko++;
							break;
					}
					break;

				case 4:
					switch (taso)
					{
						case 1:
							VoteObject.level1_votes_torstai++;
							break;
						case 2:
							VoteObject.level2_votes_torstai++;
							break;
						case 3:
							VoteObject.level3_votes_torstai++;
							break;
						case 4:
							VoteObject.level4_votes_torstai++;
							break;
					}
					break;

				case 5:
					switch (taso)
					{
						case 1:
							VoteObject.level1_votes_perjantai++;
							break;
						case 2:
							VoteObject.level2_votes_perjantai++;
							break;
						case 3:
							VoteObject.level3_votes_perjantai++;
							break;
						case 4:
							VoteObject.level4_votes_perjantai++;
							break;
					}
					break;

				

				default:
					return Problem("Et voi aanestaa viikonloppuna");
					
			}


			if(!voteModelExist)
			{
				await db.Votes.AddAsync(VoteObject);
			}
			await db.SaveChangesAsync();


			return Ok("aanestys onnistui");
		}


		public IActionResult Tulostaulu()
		{
			

			return View();
		}
	}
}
