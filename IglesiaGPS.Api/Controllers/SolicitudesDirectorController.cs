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
    public class SolicitudesDirectorController : ControllerBase
    {
        private readonly IglesiaGPSApiContext _context;

        public SolicitudesDirectorController(IglesiaGPSApiContext context)
        {
            _context = context;
        }

        // GET: api/SolicitudesDirector
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SolicitudDirector>>> GetSolicitudDirector()
        {
            return await _context.SolicitudDirectores.ToListAsync();
        }

        // GET: api/SolicitudesDirector/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SolicitudDirector>> GetSolicitudDirector(int id)
        {
            var solicitud = await _context
                .SolicitudDirectores
                .Include(s => s.Usuario)
                .Include(s => s.RespuestaPor)
                .FirstOrDefaultAsync(s => s.SolicitudDirectorId == id);

            if (solicitud == null)
            {
                return NotFound();
            }

            return solicitud;
        }

        // PUT: api/SolicitudesDirector/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSolicitudDirector(int id, SolicitudDirector solicitud)
        {
            if (id != solicitud.SolicitudDirectorId)
            {
                return BadRequest();
            }

            _context.Entry(solicitud).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SolicitudDirectorExists(id))
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

        // POST: api/SolicitudesDirector
        [HttpPost]
        public async Task<ActionResult<SolicitudDirector>> PostSolicitudDirector(SolicitudDirector solicitud)
        {
            _context.SolicitudDirectores.Add(solicitud);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetSolicitudDirector", new { id = solicitud.SolicitudDirectorId }, solicitud);
        }

        // DELETE: api/SolicitudesDirector/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSolicitudDirector(int id)
        {
            var solicitud = await _context.SolicitudDirectores.FindAsync(id);
            if (solicitud == null)
            {
                return NotFound();
            }

            _context.SolicitudDirectores.Remove(solicitud);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool SolicitudDirectorExists(int id)
        {
            return _context.SolicitudDirectores.Any(e => e.SolicitudDirectorId == id);
        }
    }
}
