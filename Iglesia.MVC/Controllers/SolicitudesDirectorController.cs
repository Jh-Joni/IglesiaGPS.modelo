using Iglesia.Api.consumer;
using IglesiaGPS.modelo;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Iglesia.MVC.Controllers
{
    public class SolicitudesDirectorController : Controller
    {
        // GET: SolicitudesDirectorController
        public ActionResult Index()
        {
            var usuarioLogueadoIdString = HttpContext.Session.GetString("UsuarioId");
            var usuarioRol = HttpContext.Session.GetString("UsuarioRol");

            if (usuarioLogueadoIdString == null) return RedirectToAction("Login", "Auth");

            // Si es Desarrollador, listar todas las solicitudes pendientes y cruzar con usuario
            if (usuarioRol == "Desarrollador")
            {
                var solicitudes = Crud<SolicitudDirector>.GetAll().Where(s => s.Estado == "Pendiente").ToList();
                var usuarios = Crud<Usuario>.GetAll();

                foreach (var solicitud in solicitudes)
                {
                    solicitud.Usuario = usuarios.FirstOrDefault(u => u.UsuarioId == solicitud.UsuarioId);
                }

                return View("IndexDesarrollador", solicitudes);
            }
            else
            {
                // Si es usuario normal/director, mostrar su propia solicitud si existe
                if (int.TryParse(usuarioLogueadoIdString, out int uid))
                {
                    var miSolicitud = Crud<SolicitudDirector>.GetAll()
                        .OrderByDescending(s => s.FechaSolicitud)
                        .FirstOrDefault(s => s.UsuarioId == uid);
                    
                    return View("Index", miSolicitud); // Puede ser null
                }
                return RedirectToAction("Login", "Auth");
            }
        }

        // GET: SolicitudesDirectorController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: SolicitudesDirectorController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: SolicitudesDirectorController/Solicitar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Solicitar()
        {
            var usuarioLogueadoIdString = HttpContext.Session.GetString("UsuarioId");
            if (usuarioLogueadoIdString == null || !int.TryParse(usuarioLogueadoIdString, out int uid))
            {
                return RedirectToAction("Login", "Auth");
            }

            // Verificar si ya tiene solicitud
            var existe = Crud<SolicitudDirector>.GetAll().Any(s => s.UsuarioId == uid && s.Estado == "Pendiente");

            if (!existe)
            {
                var nuevaSolicitud = new SolicitudDirector
                {
                    UsuarioId = uid,
                    CodigoIngresado = "SOLICITUD-DIRECTOR", // Opcional o genérico
                    Estado = "Pendiente",
                    FechaSolicitud = DateTime.Now
                };
                Crud<SolicitudDirector>.Create(nuevaSolicitud);
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: SolicitudesDirectorController/Validar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Validar(int id)
        {
            var usuarioRol = HttpContext.Session.GetString("UsuarioRol");
            var devIdString = HttpContext.Session.GetString("UsuarioId");

            if (usuarioRol != "Desarrollador" || devIdString == null || !int.TryParse(devIdString, out int devId))
            {
                return RedirectToAction("Index", "Home");
            }

            try
            {
                var solicitud = Crud<SolicitudDirector>.GetById(id);
                if (solicitud != null && solicitud.Estado == "Pendiente")
                {
                    // 1. Marcar como aprobado
                    solicitud.Estado = "Aprobado";
                    solicitud.FechaRespuesta = DateTime.Now;
                    solicitud.RespuestaPorUsuarioId = devId;

                    Crud<SolicitudDirector>.Update(id, solicitud);

                    // 2. Buscar Rol Director
                    var roles = Crud<Rol>.GetAll();
                    var rolDirector = roles.FirstOrDefault(r => r.Nombre == "Director");

                    if (rolDirector != null)
                    {
                        var usuarioSolicitante = Crud<Usuario>.GetById(solicitud.UsuarioId);
                        if (usuarioSolicitante != null)
                        {
                            // 3. Cambiar Rol
                            usuarioSolicitante.RolId = rolDirector.RolId;
                            Crud<Usuario>.Update(usuarioSolicitante.UsuarioId, usuarioSolicitante);
                        }
                    }
                }
            }
            catch (Exception)
            {
                // Manejar error si es necesario
            }
            
            return RedirectToAction(nameof(Index));
        }

        // GET: SolicitudesDirectorController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: SolicitudesDirectorController/Edit/5
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

        // GET: SolicitudesDirectorController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: SolicitudesDirectorController/Delete/5
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
