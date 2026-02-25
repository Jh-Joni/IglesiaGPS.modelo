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
    public class ListaCancionDetallesController : ControllerBase
    {
        private readonly IglesiaGPSApiContext _context;

        public ListaCancionDetallesController(IglesiaGPSApiContext context)
        {
            _context = context;
        }

        // GET: api/ListaCancionDetalles
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ListaCancionDetalle>>> GetListaCancionDetalle()
        {
            return await _context.ListaCancionDetalles.ToListAsync();
        }

        // GET: api/ListaCancionDetalles/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ListaCancionDetalle>> GetListaCancionDetalle(int id)
        {
            var detalle = await _context
                .ListaCancionDetalles
                .Include(d => d.ListaCanciones)
                .Include(d => d.Cancion)
                .FirstOrDefaultAsync(d => d.ListaCancionDetalleId == id);

            if (detalle == null)
            {
                return NotFound();
            }

            return detalle;
        }

        // PUT: api/ListaCancionDetalles/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutListaCancionDetalle(int id, ListaCancionDetalle detalle)
        {
            if (id != detalle.ListaCancionDetalleId)
            {
                return BadRequest();
            }

            _context.Entry(detalle).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ListaCancionDetalleExists(id))
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

        // POST: api/ListaCancionDetalles
        [HttpPost]
        public async Task<ActionResult<ListaCancionDetalle>> PostListaCancionDetalle(ListaCancionDetalle detalle)
        {
            _context.ListaCancionDetalles.Add(detalle);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetListaCancionDetalle", new { id = detalle.ListaCancionDetalleId }, detalle);
        }

        // DELETE: api/ListaCancionDetalles/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteListaCancionDetalle(int id)
        {
            var detalle = await _context.ListaCancionDetalles.FindAsync(id);
            if (detalle == null)
            {
                return NotFound();
            }

            _context.ListaCancionDetalles.Remove(detalle);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ListaCancionDetalleExists(int id)
        {
            return _context.ListaCancionDetalles.Any(e => e.ListaCancionDetalleId == id);
        }
    }
}
