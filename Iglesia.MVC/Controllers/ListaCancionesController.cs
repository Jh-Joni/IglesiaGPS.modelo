using Iglesia.Api.consumer;
using IglesiaGPS.modelo;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Iglesia.MVC.Controllers
{
    public class ListaCancionesController : Controller
    {
        // Método auxiliar para leer el UsuarioId de sesión de forma segura
        // (AuthController guarda con SetString, así que leemos con GetString)
        private int? GetUsuarioIdFromSession()
        {
            var idStr = HttpContext.Session.GetString("UsuarioId");
            if (!string.IsNullOrEmpty(idStr) && int.TryParse(idStr, out int id))
                return id;
            return null;
        }

        // GET: ListaCancionesController
        public ActionResult Index()
        {
            var userId = GetUsuarioIdFromSession();
            var userRole = HttpContext.Session.GetString("UsuarioRol");
            if (userId == null) return RedirectToAction("Login", "Auth");

            var lista = Crud<ListaCanciones>.GetAll();
            return View(lista);
        }

        // GET: ListaCancionesController/Details/5
        public ActionResult Details(int id)
        {
            var modelo = Crud<ListaCanciones>.GetById(id);
            if (modelo == null) return NotFound();
            return View(modelo);
        }

        // GET: ListaCancionesController/Create
        public ActionResult Create()
        {
            var userId = GetUsuarioIdFromSession();
            if (userId == null) return RedirectToAction("Login", "Auth");

            ViewBag.CancionesDisponibles = Crud<Cancion>.GetAll().OrderBy(c => c.Titulo).ToList();
            return View();
        }

        // POST: ListaCancionesController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ListaCanciones listaCanciones, List<int> cancionesSeleccionadas)
        {
            var userId = GetUsuarioIdFromSession();
            if (userId == null) return RedirectToAction("Login", "Auth");

            try
            {
                if (cancionesSeleccionadas == null || cancionesSeleccionadas.Count < 5 || cancionesSeleccionadas.Count > 7)
                {
                    ViewBag.Error = "Debes seleccionar entre 5 y 7 canciones exactas.";
                    ViewBag.CancionesDisponibles = Crud<Cancion>.GetAll().OrderBy(c => c.Titulo).ToList();
                    return View(listaCanciones);
                }

                listaCanciones.FechaCreacion = DateTime.Now;
                listaCanciones.FechaPublicacion = DateTime.Now;
                listaCanciones.Publicada = true;
                listaCanciones.DirectorId = userId.Value;

                var listaGuardada = Crud<ListaCanciones>.Create(listaCanciones);

                if (listaGuardada != null && listaGuardada.ListaCancionesId > 0)
                {
                    int orden = 1;
                    foreach (var cancionId in cancionesSeleccionadas)
                    {
                        var detalle = new ListaCancionDetalle
                        {
                            ListaCancionDetalleId = 0,
                            ListaCancionesId = listaGuardada.ListaCancionesId,
                            CancionId = cancionId,
                            Orden = orden++
                        };
                        Crud<ListaCancionDetalle>.Create(detalle);
                    }
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                ViewBag.CancionesDisponibles = Crud<Cancion>.GetAll().OrderBy(c => c.Titulo).ToList();
                return View(listaCanciones);
            }
        }

        // POST: ListaCancionesController/CrearDesdeSeleccion (AJAX)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CrearDesdeSeleccion([FromBody] List<int> cancionesSeleccionadas)
        {
            var userId = GetUsuarioIdFromSession();
            if (userId == null)
                return Unauthorized(new { mensaje = "Sesión expirada. Inicie sesión nuevamente." });

            if (cancionesSeleccionadas == null || cancionesSeleccionadas.Count < 5 || cancionesSeleccionadas.Count > 7)
                return BadRequest(new { mensaje = "Debes seleccionar entre 5 y 7 canciones exactas." });

            try
            {
                var nuevaLista = new ListaCanciones
                {
                    Titulo = $"Lista de Canciones - {DateTime.Now:dd/MM/yyyy HH:mm}",
                    FechaCreacion = DateTime.Now,
                    FechaPublicacion = DateTime.Now,
                    Publicada = true,
                    DirectorId = userId.Value
                };

                var listaGuardada = Crud<ListaCanciones>.Create(nuevaLista);

                if (listaGuardada != null && listaGuardada.ListaCancionesId > 0)
                {
                    int orden = 1;
                    foreach (var cancionId in cancionesSeleccionadas)
                    {
                        var detalle = new ListaCancionDetalle
                        {
                            ListaCancionDetalleId = 0,
                            ListaCancionesId = listaGuardada.ListaCancionesId,
                            CancionId = cancionId,
                            Orden = orden++
                        };
                        Crud<ListaCancionDetalle>.Create(detalle);
                    }
                    return Ok(new { mensaje = "Lista guardada exitosamente." });
                }

                return StatusCode(500, new { mensaje = "No se pudo guardar la lista en la base de datos." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error interno: " + ex.Message });
            }
        }

        // GET: ListaCancionesController/EditarSemana/5
        public IActionResult EditarSemana(int id)
        {
            var userId = GetUsuarioIdFromSession();
            var userRol = HttpContext.Session.GetString("UsuarioRol");

            if (userId == null) return RedirectToAction("Login", "Auth");
            if (userRol != "Director" && userRol != "Desarrollador") return Forbid();

            var modelo = Crud<ListaCanciones>.GetById(id);
            if (modelo == null) return NotFound();

            var detallesExistentes = Crud<ListaCancionDetalle>.GetAll().Where(d => d.ListaCancionesId == id).OrderBy(d => d.Orden).ToList();
            ViewBag.Seleccionadas = detallesExistentes.Select(x => x.CancionId).ToList();

            var cancionesBD = Crud<Cancion>.GetAll().OrderBy(c => c.Titulo).ToList();
            ViewBag.CancionesDisponibles = cancionesBD;

            return View(modelo);
        }

        // POST: ListaCancionesController/ActualizarDesdeSeleccion (AJAX)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ActualizarDesdeSeleccion(int id, [FromBody] List<int> cancionesSeleccionadas)
        {
            var userId = GetUsuarioIdFromSession();
            if (userId == null)
                return Unauthorized(new { mensaje = "Sesión expirada. Inicie sesión nuevamente." });

            if (cancionesSeleccionadas == null || cancionesSeleccionadas.Count < 5 || cancionesSeleccionadas.Count > 7)
                return BadRequest(new { mensaje = "Debes seleccionar entre 5 y 7 canciones exactas." });

            try
            {
                var listaExistente = Crud<ListaCanciones>.GetById(id);
                if (listaExistente == null) return NotFound(new { mensaje = "Lista no encontrada" });

                var detallesPrevios = Crud<ListaCancionDetalle>.GetAll().Where(d => d.ListaCancionesId == id).ToList();
                foreach (var detalle in detallesPrevios)
                {
                    Crud<ListaCancionDetalle>.Delete(detalle.ListaCancionDetalleId);
                }

                int orden = 1;
                foreach (var cancionId in cancionesSeleccionadas)
                {
                    var nuevoDetalle = new ListaCancionDetalle
                    {
                        ListaCancionDetalleId = 0,
                        ListaCancionesId = id,
                        CancionId = cancionId,
                        Orden = orden++
                    };
                    Crud<ListaCancionDetalle>.Create(nuevoDetalle);
                }

                return Ok(new { mensaje = "Repertorio actualizado exitosamente." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error interno al actualizar: " + ex.Message });
            }
        }

        // ==============================================================
        // LÓGICA DE REPERTORIO SECTORIZADO (MIÉRCOLES) A PARTIR DE DOMINGO
        // ==============================================================

        // GET: ListaCancionesController/EditarMiercoles/5
        public IActionResult EditarMiercoles(int id)
        {
            var userId = GetUsuarioIdFromSession();
            var userRol = HttpContext.Session.GetString("UsuarioRol");

            if (userId == null) return RedirectToAction("Login", "Auth");
            if (userRol != "Director" && userRol != "Desarrollador") return Forbid();

            // 'id' es la lista PADRE (Domingo)
            var listaPadre = Crud<ListaCanciones>.GetById(id);
            if (listaPadre == null) return NotFound();

            // Rescatar las canciones del PADRE (son las únicas permitidas)
            var detallesPadre = listaPadre.Detalles?.OrderBy(d => d.Orden).ToList() ?? new List<ListaCancionDetalle>();
            var idsDisponiblesParaMiercoles = detallesPadre.Select(d => d.CancionId).ToList();

            var cancionesTodas = Crud<Cancion>.GetAll();
            var cancionesPermitidas = cancionesTodas.Where(c => idsDisponiblesParaMiercoles.Contains(c.CancionId)).ToList();
            
            ViewBag.CancionesDisponibles = cancionesPermitidas;
            ViewBag.ListaPadre = listaPadre;

            // Verificar si ya existe una lista hija de miércoles
            string tituloMiercolesEsperado = $"Miércoles_{id}";
            var todasLasListas = Crud<ListaCanciones>.GetAll();
            var listaMiercoles = todasLasListas.FirstOrDefault(l => l.Titulo == tituloMiercolesEsperado);

            if (listaMiercoles != null)
            {
                // Ya existe, precargar selecciones
                var detallesMiercoles = Crud<ListaCancionDetalle>.GetAll()
                                        .Where(d => d.ListaCancionesId == listaMiercoles.ListaCancionesId)
                                        .OrderBy(d => d.Orden).ToList();
                ViewBag.Seleccionadas = detallesMiercoles.Select(x => x.CancionId).ToList();
                ViewBag.IdMiercoles = listaMiercoles.ListaCancionesId;
            }
            else
            {
                ViewBag.Seleccionadas = new List<int>();
                ViewBag.IdMiercoles = 0; // Se creará una nueva
            }

            return View(listaPadre); // Le pasamos el modelo padre para el título
        }

        // POST: ListaCancionesController/ActualizarMiercoles (AJAX)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ActualizarMiercoles(int idPadre, int idMiercolesExistente, [FromBody] List<int> cancionesSeleccionadas)
        {
            var userId = GetUsuarioIdFromSession();
            if (userId == null)
                return Unauthorized(new { mensaje = "Sesión expirada. Inicie sesión nuevamente." });

            if (cancionesSeleccionadas == null || cancionesSeleccionadas.Count < 2 || cancionesSeleccionadas.Count > 5)
                return BadRequest(new { mensaje = "El repertorio de miércoles debe tener entre 2 y 5 canciones exactas." });

            try
            {
                string tituloMiercoles = $"Miércoles_{idPadre}";
                int listaIdReceptor;

                if (idMiercolesExistente == 0)
                {
                    // Crear nueva lista hija
                    var nuevaLista = new ListaCanciones
                    {
                        Titulo = tituloMiercoles,
                        FechaCreacion = DateTime.Now,
                        FechaPublicacion = DateTime.Now,
                        Publicada = false, // Las de miércoles son adjuntas, no son la principal general publicable
                        DirectorId = userId.Value
                    };
                    var listaGuardada = Crud<ListaCanciones>.Create(nuevaLista);
                    if (listaGuardada == null || listaGuardada.ListaCancionesId == 0)
                        return StatusCode(500, new { mensaje = "No se pudo guardar la lista de Miércoles." });
                    
                    listaIdReceptor = listaGuardada.ListaCancionesId;
                }
                else
                {
                    // Editar la existente
                    listaIdReceptor = idMiercolesExistente;
                    // Borrar detalles anteriores
                    var detallesPrevios = Crud<ListaCancionDetalle>.GetAll().Where(d => d.ListaCancionesId == listaIdReceptor).ToList();
                    foreach (var detalle in detallesPrevios)
                    {
                        Crud<ListaCancionDetalle>.Delete(detalle.ListaCancionDetalleId);
                    }
                }

                // Insertar nuevas canciones
                int orden = 1;
                foreach (var cancionId in cancionesSeleccionadas)
                {
                    var nuevoDetalle = new ListaCancionDetalle
                    {
                        ListaCancionDetalleId = 0,
                        ListaCancionesId = listaIdReceptor,
                        CancionId = cancionId,
                        Orden = orden++
                    };
                    Crud<ListaCancionDetalle>.Create(nuevoDetalle);
                }

                return Ok(new { mensaje = "Repertorio de Miércoles actualizado exitosamente." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error interno: " + ex.Message });
            }
        }

        // GET: ListaCancionesController/Edit/5
        public ActionResult Edit(int id)
        {
            var userId = GetUsuarioIdFromSession();
            if (userId == null) return RedirectToAction("Login", "Auth");

            var modelo = Crud<ListaCanciones>.GetById(id);
            if (modelo == null) return NotFound();

            var detallesExistentes = Crud<ListaCancionDetalle>.GetAll().Where(d => d.ListaCancionesId == id).OrderBy(d => d.Orden).ToList();
            ViewBag.Seleccionadas = detallesExistentes.Select(x => x.CancionId).ToList();
            ViewBag.CancionesDisponibles = Crud<Cancion>.GetAll().OrderBy(c => c.Titulo).ToList();

            return View(modelo);
        }

        // POST: ListaCancionesController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, ListaCanciones listaCanciones, List<int> cancionesSeleccionadas)
        {
            var userId = GetUsuarioIdFromSession();
            if (userId == null) return RedirectToAction("Login", "Auth");

            try
            {
                if (cancionesSeleccionadas == null || cancionesSeleccionadas.Count < 5 || cancionesSeleccionadas.Count > 7)
                {
                    ViewBag.Error = "Debes seleccionar entre 5 y 7 canciones exactas.";
                    ViewBag.CancionesDisponibles = Crud<Cancion>.GetAll().OrderBy(c => c.Titulo).ToList();
                    return View(listaCanciones);
                }

                var original = Crud<ListaCanciones>.GetById(id);
                if (original != null)
                {
                    listaCanciones.DirectorId = original.DirectorId;
                }

                Crud<ListaCanciones>.Update(id, listaCanciones);

                var detallesActuales = Crud<ListaCancionDetalle>.GetAll().Where(d => d.ListaCancionesId == id).ToList();
                foreach (var det in detallesActuales)
                {
                    Crud<ListaCancionDetalle>.Delete(det.ListaCancionDetalleId);
                }

                int orden = 1;
                foreach (var cancionId in cancionesSeleccionadas)
                {
                    var detalle = new ListaCancionDetalle
                    {
                        ListaCancionDetalleId = 0,
                        ListaCancionesId = id,
                        CancionId = cancionId,
                        Orden = orden++
                    };
                    Crud<ListaCancionDetalle>.Create(detalle);
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                ViewBag.CancionesDisponibles = Crud<Cancion>.GetAll().OrderBy(c => c.Titulo).ToList();
                return View(listaCanciones);
            }
        }

        // GET: ListaCancionesController/Delete/5
        public ActionResult Delete(int id)
        {
            var userId = GetUsuarioIdFromSession();
            if (userId == null) return RedirectToAction("Login", "Auth");

            var modelo = Crud<ListaCanciones>.GetById(id);
            if (modelo == null) return NotFound();
            return View(modelo);
        }

        // POST: ListaCancionesController/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                var detallesActuales = Crud<ListaCancionDetalle>.GetAll().Where(d => d.ListaCancionesId == id).ToList();
                foreach (var det in detallesActuales)
                {
                    Crud<ListaCancionDetalle>.Delete(det.ListaCancionDetalleId);
                }

                Crud<ListaCanciones>.Delete(id);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Error = "No se pudo eliminar: " + ex.Message;
                return View(Crud<ListaCanciones>.GetById(id));
            }
        }
    }
}
    