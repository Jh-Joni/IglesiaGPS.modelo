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
