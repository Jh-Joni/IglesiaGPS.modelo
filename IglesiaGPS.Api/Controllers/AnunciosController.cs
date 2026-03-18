using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using IglesiaGPS.modelo;

namespace IglesiaGPS.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnunciosController : ControllerBase
    {
        private readonly IglesiaGPSApiContext _context;

        public AnunciosController(IglesiaGPSApiContext context)
        {
            _context = context;
        }

        // GET: api/Anuncios
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Anuncio>>> GetAnuncios()
        {
            // Opcional: solo retornar los vigentes (últimos 7 días) o todos para el historial
            return await _context.Anuncios.OrderByDescending(a => a.FechaCreacion).ToListAsync();
        }

        // GET: api/Anuncios/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Anuncio>> GetAnuncio(int id)
        {
            var anuncio = await _context.Anuncios.FindAsync(id);

            if (anuncio == null)
            {
                return NotFound();
            }

            return anuncio;
        }

        // POST: api/Anuncios
        [HttpPost]
        public async Task<ActionResult<Anuncio>> PostAnuncio(Anuncio anuncio)
        {
            if (anuncio.FechaCreacion == default)
            {
                anuncio.FechaCreacion = DateTime.UtcNow;
            }

            _context.Anuncios.Add(anuncio);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAnuncio", new { id = anuncio.AnuncioID }, anuncio);
        }

        // DELETE: api/Anuncios/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAnuncio(int id)
        {
            var anuncio = await _context.Anuncios.FindAsync(id);
            if (anuncio == null)
            {
                return NotFound();
            }

            _context.Anuncios.Remove(anuncio);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
