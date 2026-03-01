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
    public class CancionesController : ControllerBase
    {
        private readonly IglesiaGPSApiContext _context;

        public CancionesController(IglesiaGPSApiContext context)
        {
            _context = context;
        }

        // GET: api/Canciones
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Cancion>>> GetCancion()
        {
            return await _context.Canciones.ToListAsync();
        }

        // GET: api/Canciones/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Cancion>> GetCancion(int id)
        {
            var cancion = await _context
                .Canciones
                .Include(c => c.CreadoPor)
                .Include(c => c.NotasMusicales)
                .FirstOrDefaultAsync(c => c.CancionId == id);

            if (cancion == null)
            {
                return NotFound();
            }

            return cancion;
        }

        // PUT: api/Canciones/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCancion(int id, CancionDTO cancionDto)
        {
            if (id != cancionDto.CancionId)
            {
                return BadRequest();
            }

            var cancion = await _context.Canciones.FindAsync(id);
            if (cancion == null) return NotFound();

            cancion.Titulo = cancionDto.Titulo;
            cancion.Autor = cancionDto.Autor;
            cancion.Tono = cancionDto.Tono;
            cancion.UrlAudio = cancionDto.UrlAudio;
            cancion.Letra = cancionDto.Letra;
            cancion.FechaCreacion = cancionDto.FechaCreacion;
            cancion.CreadoPorUsuarioId = cancionDto.CreadoPorUsuarioId;

            if (!string.IsNullOrEmpty(cancionDto.FotoBase64))
            {
                cancion.FotoUrl = cancionDto.FotoBase64;
            }

            _context.Entry(cancion).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CancionExists(id))
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

        // POST: api/Canciones
        [HttpPost]
        public async Task<ActionResult<Cancion>> PostCancion(CancionDTO cancionDto)
        {
            var cancion = new Cancion
            {
                CancionId = 0, // Reset ID to prevent duplicate key errors
                Titulo = cancionDto.Titulo,
                Autor = cancionDto.Autor,
                Tono = cancionDto.Tono,
                UrlAudio = cancionDto.UrlAudio,
                Letra = cancionDto.Letra,
                FechaCreacion = cancionDto.FechaCreacion,
                CreadoPorUsuarioId = cancionDto.CreadoPorUsuarioId,
                FotoUrl = cancionDto.FotoBase64
            };

            _context.Canciones.Add(cancion);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCancion", new { id = cancion.CancionId }, cancion);
        }

        // DELETE: api/Canciones/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCancion(int id)
        {
            var cancion = await _context.Canciones.FindAsync(id);
            if (cancion == null)
            {
                return NotFound();
            }

            _context.Canciones.Remove(cancion);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CancionExists(int id)
        {
            return _context.Canciones.Any(e => e.CancionId == id);
        }
    }
}
