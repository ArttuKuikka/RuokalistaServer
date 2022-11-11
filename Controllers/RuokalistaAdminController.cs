using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
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
