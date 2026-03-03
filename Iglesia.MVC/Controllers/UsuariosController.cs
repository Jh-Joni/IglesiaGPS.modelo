using Iglesia.Api.consumer;
using IglesiaGPS.modelo;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Iglesia.MVC.Controllers
{
    public class UsuariosController : Controller
    {
        // GET: UsuariosController
        public ActionResult Index()
        {
            var lista = Crud<Usuario>.GetAll();
            return View(lista);
        }

        // GET: UsuariosController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: UsuariosController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: UsuariosController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Usuario usuario)
        {
            try
            {
                Crud<Usuario>.Create(usuario);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(usuario);
            }
        }

        // GET: UsuariosController/Edit/5
        public ActionResult Edit(int id)
        {
            var modelo = Crud<Usuario>.GetById(id);
            if (modelo == null) return NotFound();
            return View(modelo);
        }

        // POST: UsuariosController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Usuario usuario)
        {
            try
            {
                Crud<Usuario>.Update(id, usuario);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(usuario);
            }
        }

        // GET: UsuariosController/Delete/5
        public ActionResult Delete(int id)
        {
            var modelo = Crud<Usuario>.GetById(id);
            if (modelo == null) return NotFound();
            return View(modelo);
        }

        // POST: UsuariosController/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                Crud<Usuario>.Delete(id);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Error = "No se pudo eliminar: " + ex.Message;
                return View(Crud<Usuario>.GetById(id));
            }
        }

        // GET: /Usuarios/Gestion
        public ActionResult Gestion()
        {
            // Solo Desarrolladores
            if (HttpContext.Session.GetString("UsuarioRol") != "Desarrollador")
            {
                return RedirectToAction("Index", "Home");
            }

            var usuarios = Crud<Usuario>.GetAll() ?? new List<Usuario>();
            return View(usuarios);
        }

        // POST: /Usuarios/CambiarRol
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CambiarRol(int usuarioId, string nuevoRol)
        {
            // Solo Desarrolladores
            if (HttpContext.Session.GetString("UsuarioRol") != "Desarrollador")
            {
                return RedirectToAction("Index", "Home");
            }

            try
            {
                var usuario = Crud<Usuario>.GetById(usuarioId);
                if (usuario == null) return NotFound();

                var roles = Crud<Rol>.GetAll();
                var rolAsignar = roles?.FirstOrDefault(r => r.Nombre.Equals(nuevoRol, StringComparison.OrdinalIgnoreCase));

                if (rolAsignar != null)
                {
                    usuario.RolId = rolAsignar.RolId;
                    Crud<Usuario>.Update(usuario.UsuarioId, usuario);
                    TempData["MensajeGestion"] = $"Rol actualizado a {nuevoRol} para {usuario.Nombre} {usuario.Apellido}.";
                }
                else
                {
                    TempData["ErrorGestion"] = $"No se encontró el rol '{nuevoRol}' en el sistema.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorGestion"] = $"Error al cambiar rol: {ex.Message}";
            }

            return RedirectToAction(nameof(Gestion));
        }

        // POST: /Usuarios/CambiarRolAjax
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CambiarRolAjax(int usuarioId, string nuevoRol)
        {
            if (HttpContext.Session.GetString("UsuarioRol") != "Desarrollador")
            {
                return Json(new { success = false, message = "No autorizado." });
            }

            try
            {
                var usuario = Crud<Usuario>.GetById(usuarioId);
                if (usuario == null) return Json(new { success = false, message = "Usuario no encontrado." });

                var roles = Crud<Rol>.GetAll();
                var rolAsignar = roles?.FirstOrDefault(r => r.Nombre.Equals(nuevoRol, StringComparison.OrdinalIgnoreCase));

                if (rolAsignar != null)
                {
                    usuario.RolId = rolAsignar.RolId;
                    Crud<Usuario>.Update(usuario.UsuarioId, usuario);
                    return Json(new { 
                        success = true, 
                        message = $"Rol actualizado a {nuevoRol} para {usuario.Nombre} {usuario.Apellido}.",
                        usuario = new { id = usuario.UsuarioId, nombre = usuario.Nombre, apellido = usuario.Apellido, correo = usuario.Correo }
                    });
                }
                
                return Json(new { success = false, message = $"Rol '{nuevoRol}' no encontrado." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
