using Iglesia.Api.consumer;
using IglesiaGPS.modelo;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Iglesia.MVC.Controllers
{
    public class RecomendacionesController : Controller
    {
        // GET: RecomendacionesController
        public ActionResult Index()
        {
            if (HttpContext.Session.GetString("UsuarioRol") != "Director" && HttpContext.Session.GetString("UsuarioRol") != "Desarrollador")
                return RedirectToAction("Index", "Home");

            var lista = Crud<Recomendacion>.GetAll() ?? new List<Recomendacion>();
            var canciones = Crud<Cancion>.GetAll() ?? new List<Cancion>();
            var usuarios = Crud<Usuario>.GetAll() ?? new List<Usuario>();

            // Poblar objetos de navegación
            foreach (var r in lista)
            {
                r.Cancion = canciones.FirstOrDefault(c => c.CancionId == r.CancionId);
                r.Usuario = usuarios.FirstOrDefault(u => u.UsuarioId == r.UsuarioId);
            }

            // Ordenar de más recientes a antiguas
            lista = lista.OrderByDescending(r => r.FechaRecomendacion).ToList();

            return View(lista);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AjaxRecomendar(int cancionId)
        {
            string? userIdStr = HttpContext.Session.GetString("UsuarioId");
            if (string.IsNullOrEmpty(userIdStr)) return Unauthorized();

            int usuarioId = int.Parse(userIdStr);

            try
            {
                // Verificar si ya existe
                var existing = Crud<Recomendacion>.GetAll()?.FirstOrDefault(r => r.UsuarioId == usuarioId && r.CancionId == cancionId);
                if (existing != null)
                {
                    return Ok(); // Ya está recomendada
                }

                Recomendacion recomendacion = new Recomendacion
                {
                    UsuarioId = usuarioId,
                    CancionId = cancionId,
                    FechaRecomendacion = DateTime.UtcNow,
                    Leida = false,
                    Mensaje = "¡He recomendado esta canción!"
                };

                Crud<Recomendacion>.Create(recomendacion);
                return Ok();
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }

        // GET: RecomendacionesController/AceptarRecomendacion/5
        public ActionResult AceptarRecomendacion(int id)
        {
            if (HttpContext.Session.GetString("UsuarioRol") != "Director" && HttpContext.Session.GetString("UsuarioRol") != "Desarrollador")
                return RedirectToAction("Index", "Home");

            var recomendacion = Crud<Recomendacion>.GetById(id);
            if (recomendacion != null)
            {
                int cancionId = recomendacion.CancionId;
                Crud<Recomendacion>.Delete(id);
                // Redirigir a la vista de canciones con auto-selección
                return RedirectToAction("Index", "Canciones", new { seleccionar = cancionId });
            }

            return RedirectToAction("Index");
        }

        // GET: RecomendacionesController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: RecomendacionesController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: RecomendacionesController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Recomendacion recomendacion)
        {
            try
            {
                Crud<Recomendacion>.Create(recomendacion);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View("Index", Crud<Recomendacion>.GetAll());
            }
        }

        // GET: RecomendacionesController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: RecomendacionesController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: RecomendacionesController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: RecomendacionesController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
