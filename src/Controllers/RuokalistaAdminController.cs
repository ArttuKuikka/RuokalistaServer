using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
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


            var TaustaObject = _context.BackgroundForWeek.FirstOrDefault(x => x.WeekId == ruokalista.WeekId && x.Year == DateTime.Today.Year);
            if(TaustaObject == null)
            {
                TaustaObject = new BackgroundForWeek()
                {
                    WeekId = ruokalista.WeekId,
                    Year = DateTime.Today.Year,
                };
            }
			return View(TaustaObject); 
        }

        [HttpPost]
		public async Task<IActionResult> SetBG(BackgroundForWeek model)
        {
            if (ModelState.IsValid)
            {
				var bg = await _context.BackgroundForWeek.FindAsync(model.Id);
				if (bg == null)
				{
					//create new db object
					bg = new BackgroundForWeek()
					{
						WeekId = model.WeekId,
						Year = model.Year,
						FileName = model.FileName
					};

					_context.BackgroundForWeek.Add(bg);
				}
				else
				{
					bg.FileName = model.FileName;
				}

				await _context.SaveChangesAsync();

				var ruokalista = _context.Ruokalista.First(x => x.WeekId == bg.WeekId && x.Year == bg.Year);

				return Redirect("/RuokalistaAdmin");
			}
            else
            {
				return View(model);
			}
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
            var model = new Ruokalistat()
            {
               Ruokalista = new Ruokalista()
               {
                   Year = DateTime.Now.Year,
                   WeekId = System.Globalization.ISOWeek.GetWeekOfYear(DateTime.Now)
               },
            };


            return View(model);
        }

        // POST: RuokalistaAdmin/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Create(Ruokalistat ruokalistat)
        {

            if(_context.Ruokalista.Any(x => x.WeekId == ruokalistat.Ruokalista.WeekId && x.Year == ruokalistat.Ruokalista.Year))
            {
                //tee sivulle ilmotus miellummi ku badrequest
                return BadRequest("Ruokalista on jo olemassa");
            }

            if (ModelState.IsValid)
            {
                

                _context.Ruokalista.Add(ruokalistat.Ruokalista);
                
                if(ruokalistat.KasvisRuokalista != null && !KasvisRuokalista.IsNull(ruokalistat.KasvisRuokalista))
                {
                    ruokalistat.KasvisRuokalista.WeekId = ruokalistat.Ruokalista.WeekId;
                    ruokalistat.KasvisRuokalista.Year = ruokalistat.Ruokalista.Year;
                    _context.Kasvisruokalista.Add(ruokalistat.KasvisRuokalista);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(ruokalistat);
        }

        // GET: RuokalistaAdmin/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Ruokalista == null)
            {
                return BadRequest("Et määritellyt muokattavaa ruokalistaaa");
            }

            var ruokalista = await _context.Ruokalista.FindAsync(id);
            if (ruokalista == null)
            {
                return NotFound();
            }

            var kasvisruokalista = await _context.Kasvisruokalista.FirstOrDefaultAsync(x => x.WeekId == ruokalista.WeekId && x.Year == ruokalista.Year);

            var model = new Ruokalistat()
            {
                Ruokalista = ruokalista,
                KasvisRuokalista = kasvisruokalista
            };
            return View(model);
        }

        // POST: RuokalistaAdmin/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Ruokalistat ruokalistat)
        {
            
            if (ModelState.IsValid)
            {
                _context.Ruokalista.Update(ruokalistat.Ruokalista);

                if(ruokalistat.KasvisRuokalista != null && !KasvisRuokalista.IsNull(ruokalistat.KasvisRuokalista))
                {
                    ruokalistat.KasvisRuokalista.WeekId = ruokalistat.Ruokalista.WeekId;
                    ruokalistat.KasvisRuokalista.Year = ruokalistat.Ruokalista.Year;

                    var kasvisruokalista = _context.Kasvisruokalista.Find(ruokalistat.KasvisRuokalista.Id);
                    if (kasvisruokalista != null)
                    {
                        _context.Entry(kasvisruokalista).CurrentValues.SetValues(ruokalistat.KasvisRuokalista);
                    }
                    else
                    {
                        _context.Kasvisruokalista.Add(ruokalistat.KasvisRuokalista);
                    }
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(ruokalistat);
        }

        // GET: RuokalistaAdmin/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Ruokalista == null)
            {
                return BadRequest("Et määritellyt poistettavaa ruokalistaaa");
            }

            var ruokalista = await _context.Ruokalista.FindAsync(id);
            if (ruokalista == null)
            {
                return NotFound();
            }

            var kasvisruokalista = await _context.Kasvisruokalista.FirstOrDefaultAsync(x => x.WeekId == ruokalista.WeekId && x.Year == ruokalista.Year);

            var model = new Ruokalistat()
            {
                Ruokalista = ruokalista,
                KasvisRuokalista = kasvisruokalista
            };
            return View(model);
        }

        // POST: RuokalistaAdmin/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Ruokalistat ruokalistat)
        {

            if (_context.Ruokalista == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Ruokalista'  is null.");
            }
            var ruokalista = await _context.Ruokalista.FindAsync(ruokalistat.Ruokalista.Id);
            
            if (ruokalista != null)
            {
                var kasvisruokalista = await _context.Kasvisruokalista.FirstOrDefaultAsync(x => x.WeekId == ruokalista.WeekId && x.Year == ruokalista.Year);

                _context.Ruokalista.Remove(ruokalista);

                if (kasvisruokalista != null)
                {
                    _context.Kasvisruokalista.Remove(kasvisruokalista);
                }
            }
            else
            {
                return Problem("Entity 'Ruokalista' is null.");
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
