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
    [Authorize(AuthenticationSchemes = "Bearer")]
    [ApiController]
    public class RuokalistaController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RuokalistaController(ApplicationDbContext context)
        {
            _context = context;
        }


       

        // GET: Ruokalista
        [AllowAnonymous]
        [HttpGet]
        [Route("api/v1/Ruokalista")]
        public async Task<IActionResult> Index()
        {
            if (_context.Ruokalista == null)
            {
                return NotFound();
            }

            var viikko = System.Globalization.ISOWeek.GetWeekOfYear(DateTime.Now);

            var ruokalista = await _context.Ruokalista
              .Where(m => m.Year == DateTime.Now.Year).FirstOrDefaultAsync(k => k.WeekId == viikko);

            return Json(ruokalista);
        }

        //GET: Ruokalista/Details/5
        [HttpGet]
        [AllowAnonymous]
        [Route("api/v1/Ruokalista/Details/{year}/{week}")]
        public async Task<IActionResult> Details(int? year, int? week)
        {
            if (year == null || _context.Ruokalista == null || week == null)
            {
                return NotFound();
            }

            var ruokalista = await _context.Ruokalista
                .Where(m => m.Year == year).FirstOrDefaultAsync(k => k.WeekId == week);
            if (ruokalista == null)
            {
                return NotFound();
            }

            return Json(ruokalista);
        }

        // GET: Ruokalista/Create
        //[HttpGet]
        //[Route("api/v1/Ruokalista/Create")]
        //public IActionResult Create()
        //{
        //    return View();
        //}

        // POST: Ruokalista/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Route("api/v1/Ruokalista/Create")]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("WeekId,Year,Maanantai,Tiistai,Keskiviikko,Torstai,Perjantai"),] Ruokalista ruokalista)
        {
            if (_context.Ruokalista == null)
            {
                return NotFound();
            }

            foreach (var item in _context.Ruokalista)
            {
                if (item.Year == ruokalista.Year && item.WeekId == ruokalista.WeekId)
                {
                    return Json(new { Status = "Error", Message = "Already exist" });
                }
            }
            if (ruokalista.Year! < 2020)
            {
                return Json(new { Status = "Error", Message = "Year must be higher than 2020" });

            }
            if (ruokalista.WeekId == 0)
            {
                return Json(new { Status = "Error", Message = "WeekId cannot be 0" });
            }

            if (ruokalista.Id != 0)
            {
                return Json(new { Status = "Error", Message = "ID must always be 0" });
            }
            if (ModelState.IsValid)
            {

                _context.Add(ruokalista);
                await _context.SaveChangesAsync();
                return Json(new { Status = "OK" });
            }
            else
            {
                return Json(new { Status = "Error" });
            }

        }
        //[HttpGet]
        //[Route("api/v1/Ruokalista/Edit/{id}")]

        //public async Task<IActionResult> Edit(int? id)
        //{
        //    if (id == null || _context.Ruokalista == null)
        //    {
        //        return NotFound();
        //    }

        //    var ruokalista = await _context.Ruokalista.FindAsync(id);
        //    if (ruokalista == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(ruokalista);
        //}

        [HttpGet]
        [AllowAnonymous]
        [Route("api/v1/Ruokalista/GetId/{year}/{week}")]
        public async Task<IActionResult> GetId(int? year, int? week)
        {

            if (year == null || _context.Ruokalista == null || week == null)
            {
                return NotFound();
            }

            var ruokalista = await _context.Ruokalista
                .Where(m => m.Year == year).FirstOrDefaultAsync(k => k.WeekId == week);
            if (ruokalista == null)
            {
                return NotFound();
            }

            return Json(ruokalista.Id);
        }


        // POST: Ruokalista/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Route("api/v1/Ruokalista/Edit/{id}")]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("WeekId,Year,Maanantai,Tiistai,Keskiviikko,Torstai,Perjantai")] Ruokalista ruokalista)
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



        private bool RuokalistaExists(int id)
        {
            return (_context.Ruokalista?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
