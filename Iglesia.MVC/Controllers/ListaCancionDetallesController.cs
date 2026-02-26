using Iglesia.Api.consumer;
using IglesiaGPS.modelo;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Iglesia.MVC.Controllers
{
    public class ListaCancionDetallesController : Controller
    {
        // GET: ListaCancionDetallesController
        public ActionResult Index()
        {
            var lista = Crud<ListaCancionDetalle>.GetAll();
            return View(lista);
        }

        // GET: ListaCancionDetallesController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: ListaCancionDetallesController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ListaCancionDetallesController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ListaCancionDetalle detalle)
        {
            try
            {
                Crud<ListaCancionDetalle>.Create(detalle);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View("Index", Crud<ListaCancionDetalle>.GetAll());
            }
        }

        // GET: ListaCancionDetallesController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: ListaCancionDetallesController/Edit/5
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

        // GET: ListaCancionDetallesController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ListaCancionDetallesController/Delete/5
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
