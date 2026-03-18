using Iglesia.Api.consumer;
using IglesiaGPS.modelo;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Iglesia.MVC.Controllers
{
    public class CancionesController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public CancionesController(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: CancionesController
        public ActionResult Index()
        {
            var lista = Crud<Cancion>.GetAll();
            
            // Lógica de Recomendaciones
            string? userIdStr = HttpContext.Session.GetString("UsuarioId");
            int usuarioId = string.IsNullOrEmpty(userIdStr) ? 0 : int.Parse(userIdStr);
            
            var todasRecomendaciones = Crud<Recomendacion>.GetAll() ?? new List<Recomendacion>();
            ViewBag.Recomendadas = todasRecomendaciones.Where(r => r.UsuarioId == usuarioId).Select(r => r.CancionId).ToList();
            
            // Lógica de Frecuencia de Uso (Últimos 3 meses)
            var limiteFecha = DateTime.Now.AddMonths(-3);
            var todasLasListas = Crud<ListaCanciones>.GetAll() ?? new List<ListaCanciones>();
            var listasRecientes = todasLasListas.Where(l => l.FechaPublicacion >= limiteFecha).ToList();
            var todosLosDetalles = Crud<ListaCancionDetalle>.GetAll() ?? new List<ListaCancionDetalle>();

            var detallesRecientes = todosLosDetalles
                .Where(d => listasRecientes.Any(l => l.ListaCancionesId == d.ListaCancionesId))
                .ToList();

            var frecuencias = new Dictionary<int, int>();
            foreach (var det in detallesRecientes)
            {
                if (frecuencias.ContainsKey(det.CancionId))
                    frecuencias[det.CancionId]++;
                else
                    frecuencias[det.CancionId] = 1;
            }

            ViewBag.Frecuencias = frecuencias;
            ViewBag.MaxFrecuencia = frecuencias.Values.Any() ? frecuencias.Values.Max() : 1;
            
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
        public ActionResult Create(string? titulo = null, string? autor = null)
        {
            if (HttpContext.Session.GetString("UsuarioNombre") == null) return RedirectToAction("Login", "Auth");
            ViewBag.Usuarios = Crud<Usuario>.GetAll();
            var cancion = new Cancion();
            if (!string.IsNullOrEmpty(titulo)) cancion.Titulo = titulo;
            if (!string.IsNullOrEmpty(autor)) cancion.Autor = autor;
            return View(cancion);
        }

        // POST: CancionesController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequestSizeLimit(50 * 1024 * 1024)] // 50 MB máximo
        public ActionResult Create(CancionDTO cancionDto, IFormFile? FotoFile, string? NotaContenido = null, string? Instrumento = null, string? submitAction = null)
        {
            var userIdStr = HttpContext.Session.GetString("UsuarioId");
            if (string.IsNullOrEmpty(userIdStr)) return RedirectToAction("Login", "Auth");
            
            try
            {
                if (cancionDto.FechaCreacion == default)
                {
                    cancionDto.FechaCreacion = DateTime.UtcNow;
                }
                else if (cancionDto.FechaCreacion.Kind == DateTimeKind.Unspecified)
                {
                    cancionDto.FechaCreacion = DateTime.SpecifyKind(cancionDto.FechaCreacion, DateTimeKind.Local).ToUniversalTime();
                }
                else
                {
                    cancionDto.FechaCreacion = cancionDto.FechaCreacion.ToUniversalTime();
                }

                // Auto-asignar creador desde sesión
                cancionDto.CreadoPorUsuarioId = int.Parse(userIdStr);
                
                // Letra eliminado del modelo

                if (FotoFile != null && FotoFile.Length > 0)
                {
                    // Validar tamaño máximo (2 MB) para Base64 es sensato
                    if (FotoFile.Length > 2 * 1024 * 1024)
                    {
                        ViewBag.Error = "La imagen es demasiado grande. Máximo 2 MB para evitar llenar la base de datos.";
                        return View(new Cancion
                        {
                            Titulo = cancionDto.Titulo,
                            Autor = cancionDto.Autor,
                            Tono = cancionDto.Tono,
                            UrlAudio = cancionDto.UrlAudio
                        });
                    }

                    using (var ms = new MemoryStream())
                    {
                        FotoFile.CopyTo(ms);
                        var fileBytes = ms.ToArray();
                        string base64String = Convert.ToBase64String(fileBytes);
                        cancionDto.FotoBase64 = $"data:{FotoFile.ContentType};base64,{base64String}";
                    }
                }

                var createdCancion = Crud<CancionDTO>.Create(cancionDto);

                if (createdCancion != null && createdCancion.CancionId > 0 && !string.IsNullOrWhiteSpace(NotaContenido))
                {
                    var nota = new NotaMusical
                    {
                        CancionId = createdCancion.CancionId,
                        Contenido = NotaContenido,
                        Instrumento = string.IsNullOrWhiteSpace(Instrumento) ? "General" : Instrumento,
                        UltimaEdicion = DateTime.UtcNow,
                        EditadoPorUsuarioId = cancionDto.CreadoPorUsuarioId
                    };
                    Crud<NotaMusical>.Create(nota);
                }
                    
                if (submitAction == "otra_version")
                {
                    return RedirectToAction("Create", new { titulo = cancionDto.Titulo, autor = cancionDto.Autor });
                }
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(new Cancion 
                { 
                    Titulo = cancionDto.Titulo, 
                    Autor = cancionDto.Autor, 
                    Tono = cancionDto.Tono, 
                    UrlAudio = cancionDto.UrlAudio 
                });
            }
        }

        // GET: CancionesController/Edit/5
        public ActionResult Edit(int id)
        {
            if (HttpContext.Session.GetString("UsuarioNombre") == null) return RedirectToAction("Login", "Auth");
            try
            {
                var cancion = Crud<Cancion>.GetById(id);
                if (cancion == null) return NotFound();
                ViewBag.Usuarios = Crud<Usuario>.GetAll();
                return View(cancion);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "No se pudo cargar la canción para edición. Verifica que la API esté conectada: " + ex.Message;
                return View(new Cancion());
            }
        }

        // POST: CancionesController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequestSizeLimit(50 * 1024 * 1024)] // 50 MB máximo
        public ActionResult Edit(int id, Cancion cancion, IFormCollection collection, IFormFile? FotoFile)
        {
            if (HttpContext.Session.GetString("UsuarioNombre") == null) return RedirectToAction("Login", "Auth");
            try
            {
                if (cancion.FechaCreacion != default)
                {
                    cancion.FechaCreacion = DateTime.SpecifyKind(cancion.FechaCreacion, DateTimeKind.Utc);
                }

                CancionDTO dto = new CancionDTO
                {
                    CancionId = id,
                    Titulo = cancion.Titulo,
                    Autor = cancion.Autor,
                    Tono = cancion.Tono,
                    UrlAudio = cancion.UrlAudio,

                    FechaCreacion = cancion.FechaCreacion,
                    CreadoPorUsuarioId = cancion.CreadoPorUsuarioId
                };

                if (FotoFile != null && FotoFile.Length > 0)
                {
                    // Validar tamaño máximo (2 MB)
                    if (FotoFile.Length > 2 * 1024 * 1024)
                    {
                        ViewBag.Error = "La imagen es demasiado grande. Máximo 2 MB para evitar datos muy grandes en la BD.";
                        ViewBag.Usuarios = Crud<Usuario>.GetAll();
                        return View(cancion);
                    }

                    using (var ms = new MemoryStream())
                    {
                        FotoFile.CopyTo(ms);
                        var fileBytes = ms.ToArray();
                        string base64String = Convert.ToBase64String(fileBytes);
                        dto.FotoBase64 = $"data:{FotoFile.ContentType};base64,{base64String}";
                    }
                }

                // Actualizar Canción usando el DTO
                Crud<CancionDTO>.Update(id, dto);

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
                                notaToUpdate.Contenido = notaContenidos[i] ?? "";
                                modified = true;
                            }
                            
                            if (modified)
                            {
                                notaToUpdate.UltimaEdicion = DateTime.UtcNow;
                                Crud<NotaMusical>.Update(nId, notaToUpdate);
                            }
                        }
                    }
                }

                // Crear nueva nota si se ingresó en Edit
                string? newNotaContenido = collection["NewNotaContenido"];
                string? newInstrumento = collection["NewInstrumento"];
                if (!string.IsNullOrWhiteSpace(newNotaContenido))
                {
                    var nota = new NotaMusical
                    {
                        CancionId = id,
                        Contenido = newNotaContenido,
                        Instrumento = string.IsNullOrWhiteSpace(newInstrumento) ? "General" : newInstrumento,
                        UltimaEdicion = DateTime.UtcNow,
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
            if (HttpContext.Session.GetString("UsuarioRol") != "Desarrollador") return RedirectToAction("Index");
            var cancion = Crud<Cancion>.GetById(id);
            if (cancion == null) return NotFound();
            return View(cancion);
        }

        // POST: CancionesController/Delete/5
        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var userRol = HttpContext.Session.GetString("UsuarioRol");
            if (userRol != "Desarrollador" && userRol != "Director") 
                return RedirectToAction("Index");
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
