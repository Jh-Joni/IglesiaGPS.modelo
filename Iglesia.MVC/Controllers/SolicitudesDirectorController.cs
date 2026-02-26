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
            var lista = Crud<SolicitudDirector>.GetAll();
            return View(lista);
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

        // POST: SolicitudesDirectorController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(SolicitudDirector solicitud)
        {
            try
            {
                Crud<SolicitudDirector>.Create(solicitud);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View("Index", Crud<SolicitudDirector>.GetAll());
            }
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
