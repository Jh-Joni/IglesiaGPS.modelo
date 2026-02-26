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
            var lista = Crud<Recomendacion>.GetAll();
            return View(lista);
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
