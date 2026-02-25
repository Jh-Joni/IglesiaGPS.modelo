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
    public class NotasMusicalesController : ControllerBase
    {
        private readonly IglesiaGPSApiContext _context;

        public NotasMusicalesController(IglesiaGPSApiContext context)
        {
            _context = context;
        }

        // GET: api/NotasMusicales
        [HttpGet]
        public async Task<ActionResult<IEnumerable<NotaMusical>>> GetNotaMusical()
        {
            return await _context.NotaMusicales.ToListAsync();
        }

        // GET: api/NotasMusicales/5
        [HttpGet("{id}")]
        public async Task<ActionResult<NotaMusical>> GetNotaMusical(int id)
        {
            var nota = await _context
                .NotaMusicales
                .Include(n => n.Cancion)
                .Include(n => n.EditadoPor)
                .FirstOrDefaultAsync(n => n.NotaMusicalId == id);

            if (nota == null)
            {
                return NotFound();
            }

            return nota;
        }

        // PUT: api/NotasMusicales/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutNotaMusical(int id, NotaMusical nota)
        {
            if (id != nota.NotaMusicalId)
            {
                return BadRequest();
            }

            _context.Entry(nota).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!NotaMusicalExists(id))
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

        // POST: api/NotasMusicales
        [HttpPost]
        public async Task<ActionResult<NotaMusical>> PostNotaMusical(NotaMusical nota)
        {
            _context.NotaMusicales.Add(nota);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetNotaMusical", new { id = nota.NotaMusicalId }, nota);
        }

        // DELETE: api/NotasMusicales/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNotaMusical(int id)
        {
            var nota = await _context.NotaMusicales.FindAsync(id);
            if (nota == null)
            {
                return NotFound();
            }

            _context.NotaMusicales.Remove(nota);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool NotaMusicalExists(int id)
        {
            return _context.NotaMusicales.Any(e => e.NotaMusicalId == id);
        }
    }
}
