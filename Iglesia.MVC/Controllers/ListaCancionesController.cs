using Iglesia.Api.consumer;
using IglesiaGPS.modelo;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Iglesia.MVC.Controllers
{
    public class ListaCancionesController : Controller
    {
        // GET: ListaCancionesController
        public ActionResult Index()
        {
            var lista = Crud<ListaCanciones>.GetAll();
            return View(lista);
        }

        // GET: ListaCancionesController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: ListaCancionesController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ListaCancionesController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ListaCanciones listaCanciones)
        {
            try
            {
                Crud<ListaCanciones>.Create(listaCanciones);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View("Index", Crud<ListaCanciones>.GetAll());
            }
        }

        // GET: ListaCancionesController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: ListaCancionesController/Edit/5
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

        // GET: ListaCancionesController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ListaCancionesController/Delete/5
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
