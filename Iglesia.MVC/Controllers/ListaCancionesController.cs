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
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(listaCanciones);
            }
        }

        // GET: ListaCancionesController/Edit/5
        public ActionResult Edit(int id)
        {
            var modelo = Crud<ListaCanciones>.GetById(id);
            if (modelo == null) return NotFound();
            return View(modelo);
        }

        // POST: ListaCancionesController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, ListaCanciones listaCanciones)
        {
            try
            {
                Crud<ListaCanciones>.Update(id, listaCanciones);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(listaCanciones);
            }
        }

        // GET: ListaCancionesController/Delete/5
        public ActionResult Delete(int id)
        {
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
