using Iglesia.Api.consumer;
using IglesiaGPS.modelo;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Iglesia.MVC.Controllers
{
    public class NotasMusicalesController : Controller
    {
        // GET: NotasMusicalesController
        public ActionResult Index()
        {
            var lista = Crud<NotaMusical>.GetAll();
            return View(lista);
        }

        // GET: NotasMusicalesController/VerNotas/5 (id is CancionId)
        public ActionResult VerNotas(int id)
        {
            var cancion = Crud<Cancion>.GetById(id);
            if (cancion == null) return NotFound();

            var notas = Crud<NotaMusical>.GetAll().FirstOrDefault(n => n.CancionId == id);
            
            if (notas == null)
            {
                notas = new NotaMusical { CancionId = id, Contenido = "" };
            }

            ViewBag.CancionTitulo = cancion.Titulo;
            return View(notas);
        }

        // GET: NotasMusicalesController/EditarNota/5 (id is CancionId)
        public ActionResult EditarNota(int id)
        {
            if (HttpContext.Session.GetString("UsuarioNombre") == null) return RedirectToAction("Login", "Auth");

            var cancion = Crud<Cancion>.GetById(id);
            if (cancion == null) return NotFound();

            var notas = Crud<NotaMusical>.GetAll().FirstOrDefault(n => n.CancionId == id);
            
            if (notas == null)
            {
                notas = new NotaMusical { CancionId = id, Contenido = "" };
            }

            ViewBag.CancionTitulo = cancion.Titulo;
            return View(notas);
        }

        // POST: NotasMusicalesController/EditarNota/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarNota(int id, NotaMusical nota)
        {
            if (HttpContext.Session.GetString("UsuarioNombre") == null) return RedirectToAction("Login", "Auth");
            
            var usuarioLogueadoIdString = HttpContext.Session.GetString("UsuarioId");
            int usuarioId = 1; 
            if (!string.IsNullOrEmpty(usuarioLogueadoIdString) && int.TryParse(usuarioLogueadoIdString, out int uid))
            {
                usuarioId = uid;
            }

            var existeNota = Crud<NotaMusical>.GetAll().FirstOrDefault(n => n.CancionId == id);

            if (existeNota == null)
            {
                nota.CancionId = id;
                nota.UltimaEdicion = DateTime.Now;
                nota.EditadoPorUsuarioId = usuarioId;
                Crud<NotaMusical>.Create(nota);
            }
            else
            {
                existeNota.Contenido = nota.Contenido;
                existeNota.UltimaEdicion = DateTime.Now;
                existeNota.EditadoPorUsuarioId = usuarioId;
                Crud<NotaMusical>.Update(existeNota.NotaMusicalId, existeNota);
            }

            return RedirectToAction("VerNotas", new { id = id });
        }

        // GET: NotasMusicalesController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: NotasMusicalesController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: NotasMusicalesController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(NotaMusical nota)
        {
            try
            {
                Crud<NotaMusical>.Create(nota);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View("Index", Crud<NotaMusical>.GetAll());
            }
        }

        // GET: NotasMusicalesController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: NotasMusicalesController/Edit/5
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

        // GET: NotasMusicalesController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: NotasMusicalesController/Delete/5
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
