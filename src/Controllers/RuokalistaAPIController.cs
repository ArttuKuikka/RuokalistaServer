﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RuokalistaServer.Attributes;
using RuokalistaServer.Data;
using RuokalistaServer.Models;


namespace RuokalistaServer.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    [ApiController]
    [Feature("API")]
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
        public async Task<IActionResult> Index(bool kasvisruokalista = false)
        {
            if (_context.Ruokalista == null)
            {
                return NotFound();
            }

            var viikko = System.Globalization.ISOWeek.GetWeekOfYear(DateTime.Now);
            IRuokalista? ruokalista = null;
            if (!kasvisruokalista)
            {
				ruokalista = await _context.Ruokalista
			  .Where(m => m.Year == DateTime.Now.Year).FirstOrDefaultAsync(k => k.WeekId == viikko);
			}
            else
            {
				ruokalista = await _context.Kasvisruokalista
			  .Where(m => m.Year == DateTime.Now.Year).FirstOrDefaultAsync(k => k.WeekId == viikko);
			}

            if(ruokalista == null)
            {
                return NotFound();
            }
            else
            {
                return Json(ruokalista);
            }

            
        }

		[AllowAnonymous]
		[HttpGet]
		[Route("api/v1/KasvisRuokalista")]
		public async Task<IActionResult> KasvisIndex()
		{
			return await Index(true);
		}


		//GET: Ruokalista/Details/5
		[HttpGet]
        [AllowAnonymous]
        [Route("api/v1/Ruokalista/{year}/{week}")]
        public async Task<IActionResult> Details(int? year, int? week, bool kasvisruokalista = false)
        {
            if (year == null || _context.Ruokalista == null || week == null)
            {
                return NotFound();
            }

            IRuokalista? ruokalista = null;
            if (!kasvisruokalista)
            {
				ruokalista = await _context.Ruokalista
				.Where(m => m.Year == year).FirstOrDefaultAsync(k => k.WeekId == week);
			}
            else
            {
				ruokalista = await _context.Kasvisruokalista
				.Where(m => m.Year == year).FirstOrDefaultAsync(k => k.WeekId == week);
			}


            if (ruokalista == null)
            {
                return NotFound();
            }

            return Json(ruokalista);
        }


		[HttpGet]
		[AllowAnonymous]
		[Route("api/v1/KasvisRuokalista/{year}/{week}")]
		public async Task<IActionResult> KasvisDetails(int? year, int? week)
        {
            return await Details(year, week, true); 
		}



		 [HttpGet]
        [Route("api/v1/Ruokalista/Get/{amount}")]
        public async Task<IActionResult> GetLatest(int amount)
        {
            if (_context.Ruokalista == null)
            {
                return NotFound();
            }
            var lista = _context.Ruokalista.OrderBy(x => x.Year).ThenBy(y => y.WeekId).Reverse().ToList();

            return Json(lista.Take(amount));
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
                    return BadRequest(new { Status = "Error", Message = "Already exist" });
                }
            }
            if (ruokalista.Year! < 2020)
            {
                return BadRequest(new { Status = "Error", Message = "Year must be higher than 2020" });

            }
            if (ruokalista.WeekId == 0)
            {
                return BadRequest(new { Status = "Error", Message = "WeekId cannot be 0" });
            }

            if (ruokalista.Id != 0)
            {
                return BadRequest(new { Status = "Error", Message = "ID must always be 0" });
            }
            if (ModelState.IsValid)
            {

                _context.Add(ruokalista);
                await _context.SaveChangesAsync();
                return Json(new { Status = "OK" });
            }
            else
            {
                return BadRequest(new { Status = "Error" });
            }

        }


		[HttpPost]
		[Route("api/v1/KasvisRuokalista/Create")]
		//[ValidateAntiForgeryToken]
		public async Task<IActionResult> KasvisCreate([Bind("WeekId,Year,Maanantai,Tiistai,Keskiviikko,Torstai,Perjantai"),] KasvisRuokalista ruokalista)
		{
			if (_context.Ruokalista == null)
			{
				return NotFound();
			}

			foreach (var item in _context.Ruokalista)
			{
				if (item.Year == ruokalista.Year && item.WeekId == ruokalista.WeekId)
				{
					return BadRequest(new { Status = "Error", Message = "Already exist" });
				}
			}
			if (ruokalista.Year! < 2020)
			{
				return BadRequest(new { Status = "Error", Message = "Year must be higher than 2020" });

			}
			if (ruokalista.WeekId == 0)
			{
				return BadRequest(new { Status = "Error", Message = "WeekId cannot be 0" });
			}

			if (ruokalista.Id != 0)
			{
				return BadRequest(new { Status = "Error", Message = "ID must always be 0" });
			}
			if (ModelState.IsValid)
			{

				_context.Kasvisruokalista.Add(ruokalista);
				await _context.SaveChangesAsync();
				return Json(new { Status = "OK" });
			}
			else
			{
				return BadRequest(new { Status = "Error" });
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

            return Ok(ruokalista.Id);
        }


        // POST: Ruokalista/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Route("api/v1/Ruokalista/Edit/{id}")]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,WeekId,Year,Maanantai,Tiistai,Keskiviikko,Torstai,Perjantai")] Ruokalista ruokalista)
        {
            ruokalista.Id = id;

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
                return Ok("Ok");
            }
            return BadRequest();
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        [Route("api/v1/Ruokalista/Delete/{id}")]
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
            else
            {
                return NotFound();
            }

            await _context.SaveChangesAsync();
            return Ok("Ok");
        }



        private bool RuokalistaExists(int id)
        {
            return (_context.Ruokalista?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
