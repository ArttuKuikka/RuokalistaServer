using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RuokalistaServer.Data;
using RuokalistaServer.Models;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

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

            if (ruokalista == null)
            {
                return NotFound();
            }
            else
            {
                return Json(ruokalista);
            }


        }

        //GET: Ruokalista/Details/5
        [HttpGet]
        [AllowAnonymous]
        [Route("api/v1/Ruokalista/{year}/{week}")]
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

        [HttpGet]
        [Route("api/v1/Ruokalista/Get/{amount}")]
        public async Task<IActionResult> GetLatest(int amount)
        {
            if (_context.Ruokalista == null)
            {
                return NotFound();
            }

            return Json(_context.Ruokalista.Take(amount));
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

        [HttpPost]
        [Route("api/v1/Ruokalista/ProcessImage")]
        public async Task<IActionResult> ProcessImage(ImageModel model)
        {

            if (ModelState.IsValid)
            {
                if(model.base64Image == null)
                {
                    return BadRequest();
                }
                var ImageBytes = Convert.FromBase64String(model.base64Image);

                MemoryStream ms = new MemoryStream();
                using (Image image = Image.Load(ImageBytes))
                {
                    int width = image.Width / 2;
                    int height = image.Height / 2;
                    image.Mutate(x => x.Resize(width, height));

                    image.SaveAsJpeg(ms);
                }
                ms.Position = 0;
                var base64String = Convert.ToBase64String(ms.ToArray());

                var content = "";
                using (var client = new HttpClient())
                {
                    var url = "http://api.ocr.space/parse/image";

                    client.DefaultRequestHeaders.Add("apikey", "K89075064488957");

                    var body = new List<KeyValuePair<string, string>>
        {
            new KeyValuePair<string, string>("language", "fin"),
            new KeyValuePair<string, string>("isTable", "true"),
            new KeyValuePair<string, string>("base64Image", "data:image/jpg;base64," + base64String)
        };

                    var response = await client.PostAsync(url, new FormUrlEncodedContent(body));
                    if (response.IsSuccessStatusCode)
                    {
                        content = await response.Content.ReadAsStringAsync();
                       
                    }
                    else
                    {
                        throw new Exception("httpclient error " + response.ReasonPhrase);
                    }
                }

                return Json(content);
            }
            else
            {
                return BadRequest();
            }
           
        }

        private bool RuokalistaExists(int id)
        {
            return (_context.Ruokalista?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
