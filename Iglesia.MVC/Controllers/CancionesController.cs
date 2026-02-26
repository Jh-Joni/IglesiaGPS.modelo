using Iglesia.Api.consumer;
using IglesiaGPS.modelo;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Iglesia.MVC.Controllers
{
    public class CancionesController : Controller
    {
        // GET: CancionesController
        public ActionResult Index()
        {
            var lista = Crud<Cancion>.GetAll();
            return View(lista);
        }

        // GET: CancionesController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        [HttpGet]
        public IActionResult GetNotas(int id)
        {
            try
            {
                var todasLasNotas = Crud<NotaMusical>.GetAll();
                var notasDeCancion = todasLasNotas?.Where(n => n.CancionId == id).ToList() ?? new List<NotaMusical>();
                return Json(notasDeCancion);
            }
            catch (Exception ex)
            {
                return BadRequest("Error al cargar notas: " + ex.Message);
            }
        }

        // GET: CancionesController/Create
        public ActionResult Create()
        {
            ViewBag.Usuarios = Crud<Usuario>.GetAll();
            return View();
        }

        // POST: CancionesController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Cancion cancion, string NotaContenido = null, string Instrumento = null)
        {
            try
            {
                if (cancion.FechaCreacion == default)
                    cancion.FechaCreacion = DateTime.Now;

                var createdCancion = Crud<Cancion>.Create(cancion);

                if (createdCancion != null && createdCancion.CancionId > 0 && !string.IsNullOrWhiteSpace(NotaContenido))
                {
                    var nota = new NotaMusical
                    {
                        CancionId = createdCancion.CancionId,
                        Contenido = NotaContenido,
                        Instrumento = string.IsNullOrWhiteSpace(Instrumento) ? "General" : Instrumento,
                        UltimaEdicion = DateTime.Now,
                        EditadoPorUsuarioId = cancion.CreadoPorUsuarioId
                    };
                    Crud<NotaMusical>.Create(nota);
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                ViewBag.Usuarios = Crud<Usuario>.GetAll();
                return View(cancion);
            }
        }

        // GET: CancionesController/Edit/5
        public ActionResult Edit(int id)
        {
            var cancion = Crud<Cancion>.GetById(id);
            if (cancion == null) return NotFound();
            ViewBag.Usuarios = Crud<Usuario>.GetAll();
            return View(cancion);
        }

        // POST: CancionesController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Cancion cancion, IFormCollection collection)
        {
            try
            {
                // Actualizar Canción
                Crud<Cancion>.Update(id, cancion);

                // Actualizar Notas existentes
                var notaIds = collection["NotaIds"].ToList();
                var notaContenidos = collection["NotaContenidos"].ToList();

                for (int i = 0; i < notaIds.Count; i++)
                {
                    if (int.TryParse(notaIds[i], out int nId))
                    {
                        var notaToUpdate = Crud<NotaMusical>.GetById(nId);
                        if (notaToUpdate != null)
                        {
                            bool modified = false;
                            if (i < notaContenidos.Count && notaToUpdate.Contenido != notaContenidos[i])
                            {
                                notaToUpdate.Contenido = notaContenidos[i];
                                modified = true;
                            }
                            
                            if (modified)
                            {
                                notaToUpdate.UltimaEdicion = DateTime.Now;
                                Crud<NotaMusical>.Update(nId, notaToUpdate);
                            }
                        }
                    }
                }

                // Crear nueva nota si se ingresó en Edit
                string newNotaContenido = collection["NewNotaContenido"];
                string newInstrumento = collection["NewInstrumento"];
                if (!string.IsNullOrWhiteSpace(newNotaContenido))
                {
                    var nota = new NotaMusical
                    {
                        CancionId = id,
                        Contenido = newNotaContenido,
                        Instrumento = string.IsNullOrWhiteSpace(newInstrumento) ? "General" : newInstrumento,
                        UltimaEdicion = DateTime.Now,
                        EditadoPorUsuarioId = cancion.CreadoPorUsuarioId
                    };
                    Crud<NotaMusical>.Create(nota);
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                ViewBag.Usuarios = Crud<Usuario>.GetAll();
                return View(cancion);
            }
        }

        // GET: CancionesController/Delete/5
        public ActionResult Delete(int id)
        {
            var cancion = Crud<Cancion>.GetById(id);
            if (cancion == null) return NotFound();
            return View(cancion);
        }

        // POST: CancionesController/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                Crud<Cancion>.Delete(id);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Error = "No se pudo eliminar: " + ex.Message;
                return View(Crud<Cancion>.GetById(id));
            }
        }
    }
}
