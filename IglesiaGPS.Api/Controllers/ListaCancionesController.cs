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
    public class ListaCancionesController : ControllerBase
    {
        private readonly IglesiaGPSApiContext _context;

        public ListaCancionesController(IglesiaGPSApiContext context)
        {
            _context = context;
        }

        // GET: api/ListaCanciones
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ListaCanciones>>> GetListaCanciones()
        {
            var listas = await _context.ListaCanciones
                .AsNoTracking()
                .Include(l => l.Director)
                .Include(l => l.Detalles)!
                    .ThenInclude(d => d.Cancion)
                .OrderByDescending(l => l.FechaCreacion)
                .ToListAsync();

            foreach(var l in listas)
            {
                if(l.Director != null) l.Director.ListasCanciones = null;
                if(l.Detalles != null)
                {
                    foreach(var d in l.Detalles)
                    {
                        d.ListaCanciones = null;
                        if(d.Cancion != null) d.Cancion.ListaCancionDetalles = null;
                    }
                }
            }
            return listas;
        }

        // GET: api/ListaCanciones/publicadas
        [HttpGet("publicadas")]
        public async Task<ActionResult<IEnumerable<ListaCanciones>>> GetPublicadas()
        {
            var listas = await _context.ListaCanciones
                .AsNoTracking()
                .Where(l => l.Publicada)
                .Include(l => l.Director)
                .Include(l => l.Detalles)!
                    .ThenInclude(d => d.Cancion)
                .OrderByDescending(l => l.FechaCreacion)
                .Take(5)
                .ToListAsync();

            foreach(var l in listas)
            {
                if(l.Director != null) l.Director.ListasCanciones = null;
                if(l.Detalles != null)
                {
                    foreach(var d in l.Detalles)
                    {
                        d.ListaCanciones = null;
                        if(d.Cancion != null) d.Cancion.ListaCancionDetalles = null;
                    }
                }
            }
            return listas;
        }

        // GET: api/ListaCanciones/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ListaCanciones>> GetListaCanciones(int id)
        {
            var lista = await _context
                .ListaCanciones
                .AsNoTracking()
                .Include(l => l.Director)
                .Include(l => l.Detalles)
                    .ThenInclude(d => d.Cancion)
                .FirstOrDefaultAsync(l => l.ListaCancionesId == id);

            if (lista == null)
            {
                return NotFound();
            }

            if(lista.Director != null) lista.Director.ListasCanciones = null;
            if(lista.Detalles != null)
            {
                foreach(var d in lista.Detalles)
                {
                    d.ListaCanciones = null;
                    if(d.Cancion != null) d.Cancion.ListaCancionDetalles = null;
                }
            }

            return lista;
        }

        // PUT: api/ListaCanciones/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutListaCanciones(int id, ListaCanciones lista)
        {
            if (id != lista.ListaCancionesId)
            {
                return BadRequest();
            }

            _context.Entry(lista).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ListaCancionesExists(id))
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

        // POST: api/ListaCanciones
        [HttpPost]
        public async Task<ActionResult<ListaCanciones>> PostListaCanciones(ListaCanciones lista)
        {
            await using var tx = await _context.Database.BeginTransactionAsync();

            lista.ListaCancionesId = 0;
            lista.Director = null;
            lista.Detalles = null;
            lista.Publicada = true;
            lista.FechaPublicacion = DateTime.UtcNow;

            var publicadas = await _context.ListaCanciones
                .Where(l => l.Publicada)
                .ToListAsync();

            foreach (var publicada in publicadas)
            {
                publicada.Publicada = false;
                publicada.FechaPublicacion = null;
            }

            _context.ListaCanciones.Add(lista);
            await _context.SaveChangesAsync();
            await tx.CommitAsync();

            return CreatedAtAction("GetListaCanciones", new { id = lista.ListaCancionesId }, lista);
        }

        // DELETE: api/ListaCanciones/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteListaCanciones(int id)
        {
            var lista = await _context.ListaCanciones.FindAsync(id);
            if (lista == null)
            {
                return NotFound();
            }

            _context.ListaCanciones.Remove(lista);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ListaCancionesExists(int id)
        {
            return _context.ListaCanciones.Any(e => e.ListaCancionesId == id);
        }
    }
}
