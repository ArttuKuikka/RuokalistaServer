using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RuokalistaServer.Data;
using RuokalistaServer.Models;

namespace RuokalistaServer.Controllers
{
    [Authorize(AuthenticationSchemes = "Identity.Application")]
    public class RuokalistaAdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RuokalistaAdminController(ApplicationDbContext context)
        {
            _context = context;
        }


        public async Task<IActionResult> NewRandomBG(int? id)
        {
			if (id == null || id == 0)
			{
				throw new ArgumentException("Invalid id number (0 or NULL)");
			}

            var ruokalista = await _context.Ruokalista.FindAsync(id);
            if( ruokalista == null)
            {
                throw new Exception("Ruokalista is null");
            }

            var week = ruokalista.WeekId;

			var bg = _context.BackgroundForWeek.FirstOrDefault(x => x.WeekId == week);
			if (bg == null)
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

				Random random = new Random();
				int randomNumber = random.Next(0, imageFileCount);
				var newImage = imageFiles.ElementAtOrDefault(randomNumber);
				newImage = Path.GetFileName(newImage);

				if (newImage == null)
				{
					throw new Exception("Error while picking new random backround picture, element is null at index");
				}

				bg = new BackgroundForWeek
				{
					FileName = System.Web.HttpUtility.HtmlEncode(newImage),
					WeekId = week
				};

				await _context.BackgroundForWeek.AddAsync(bg);
				await _context.SaveChangesAsync();


            }
            else
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

				Random random = new Random();
				int randomNumber = random.Next(0, imageFileCount);
				var newImage = imageFiles.ElementAtOrDefault(randomNumber);
				newImage = Path.GetFileName(newImage);

				if (newImage == null)
				{
					throw new Exception("Error while picking new random backround picture, element is null at index");
				}

                bg.FileName = System.Web.HttpUtility.HtmlEncode(newImage);

				_context.BackgroundForWeek.Update(bg);
				await _context.SaveChangesAsync();
			}
			bg.FileName = "/infotv/GetBgByFilename?filename=" + System.Web.HttpUtility.HtmlEncode(bg.FileName);
			return Ok(bg);
		}

		public async Task<IActionResult> SetBG(int? ruokalistaId)
        {
			if (ruokalistaId == null || ruokalistaId == 0)
			{
				throw new ArgumentException("Invalid id number (0 or NULL)");
			}

			var ruokalista = await _context.Ruokalista.FindAsync(ruokalistaId);
			if (ruokalista == null)
			{
				throw new Exception("Ruokalista is null");
			}


            var TaustaObject = _context.BackgroundForWeek.First(x => x.WeekId == ruokalista.WeekId);
			return View(TaustaObject); 
        }

        [HttpPost]
		public async Task<IActionResult> SetBG(BackgroundForWeek model)
        {
            var bg = await _context.BackgroundForWeek.FindAsync(model.Id);
            if (bg == null)
            {
                return BadRequest("invalid id in model");
            }
            bg.FileName = model.FileName;
            await _context.SaveChangesAsync();


            return Redirect("/RuokalistaAdmin");
        }
		// GET: RuokalistaAdmin
		public async Task<IActionResult> Index()
        {
            var lista = await _context.Ruokalista.ToListAsync();
            lista = lista.OrderBy(x => x.Year).ThenBy(y => y.WeekId).Reverse().ToList();
            return View(lista);
        }

        // GET: RuokalistaAdmin/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Ruokalista == null)
            {
                return NotFound();
            }

            var ruokalista = await _context.Ruokalista
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ruokalista == null)
            {
                return NotFound();
            }

            return View(ruokalista);
        }

        // GET: RuokalistaAdmin/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: RuokalistaAdmin/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,WeekId,Year,Maanantai,Tiistai,Keskiviikko,Torstai,Perjantai")] Ruokalista ruokalista)
        {
            foreach (var item in _context.Ruokalista)
            {
                if(ruokalista.Year < 2020)
                {
                    return BadRequest("Virheellinen vuosiluku");
                }
                if (ruokalista.WeekId <= 0)
                {
                    return BadRequest("Virheellinen viikko");
                }
                if (item.Year == ruokalista.Year && item.WeekId == ruokalista.WeekId)
                {
                    return BadRequest("Tämän viikon ruokalista on jo olemassa");
                }
            }
            if (ModelState.IsValid)
            {
                _context.Add(ruokalista);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(ruokalista);
        }

        // GET: RuokalistaAdmin/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Ruokalista == null)
            {
                return NotFound();
            }

            var ruokalista = await _context.Ruokalista.FindAsync(id);
            if (ruokalista == null)
            {
                return NotFound();
            }
            return View(ruokalista);
        }

        // POST: RuokalistaAdmin/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,WeekId,Year,Maanantai,Tiistai,Keskiviikko,Torstai,Perjantai")] Ruokalista ruokalista)
        {
            if (id != ruokalista.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ruokalista);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RuokalistaExists(ruokalista.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(ruokalista);
        }

        // GET: RuokalistaAdmin/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Ruokalista == null)
            {
                return NotFound();
            }

            var ruokalista = await _context.Ruokalista
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ruokalista == null)
            {
                return NotFound();
            }

            return View(ruokalista);
        }

        // POST: RuokalistaAdmin/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Ruokalista == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Ruokalista'  is null.");
            }
            var ruokalista = await _context.Ruokalista.FindAsync(id);
            if (ruokalista != null)
            {
                _context.Ruokalista.Remove(ruokalista);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RuokalistaExists(int id)
        {
          return _context.Ruokalista.Any(e => e.Id == id);
        }
    }
}
