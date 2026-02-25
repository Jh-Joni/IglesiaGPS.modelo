using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using IglesiaGPS.modelo;

namespace IglesiaGPS.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecomendacionesController : ControllerBase
    {
        private readonly IglesiaGPSApiContext _context;

        public RecomendacionesController(IglesiaGPSApiContext context)
        {
            _context = context;
        }

        // GET: api/Recomendaciones
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Recomendacion>>> GetRecomendacion()
        {
            return await _context.Recomendaciones.ToListAsync();
        }

        // GET: api/Recomendaciones/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Recomendacion>> GetRecomendacion(int id)
        {
            var recomendacion = await _context
                .Recomendaciones
                .Include(r => r.Usuario)
                .Include(r => r.Cancion)
                .FirstOrDefaultAsync(r => r.RecomendacionId == id);

            if (recomendacion == null)
            {
                return NotFound();
            }

            return recomendacion;
        }

        // PUT: api/Recomendaciones/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRecomendacion(int id, Recomendacion recomendacion)
        {
            if (id != recomendacion.RecomendacionId)
            {
                return BadRequest();
            }

            _context.Entry(recomendacion).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RecomendacionExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Recomendaciones
        [HttpPost]
        public async Task<ActionResult<Recomendacion>> PostRecomendacion(Recomendacion recomendacion)
        {
            _context.Recomendaciones.Add(recomendacion);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRecomendacion", new { id = recomendacion.RecomendacionId }, recomendacion);
        }

        // DELETE: api/Recomendaciones/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRecomendacion(int id)
        {
            var recomendacion = await _context.Recomendaciones.FindAsync(id);
            if (recomendacion == null)
            {
                return NotFound();
            }

            _context.Recomendaciones.Remove(recomendacion);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool RecomendacionExists(int id)
        {
            return _context.Recomendaciones.Any(e => e.RecomendacionId == id);
        }
    }
}
