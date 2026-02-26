using Iglesia.MVC.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Globalization;

namespace Iglesia.MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        // Anuncios temporales en memoria (expiran después de 7 días)
        private static readonly ConcurrentDictionary<string, AnuncioTemporal> _anuncios = new();

        public class AnuncioTemporal
        {
            public string Id { get; set; } = "";
            public string Autor { get; set; } = "";
            public string Contenido { get; set; } = "";
            public string Tipo { get; set; } = "Anuncio"; // Anuncio, Versículo, Mensaje
            public DateTime FechaCreacion { get; set; } = DateTime.Now;
        }

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            // Mes actual en español
            var cultura = new CultureInfo("es-ES");
            var mesActual = DateTime.Now.ToString("MMMM", cultura);
            mesActual = char.ToUpper(mesActual[0]) + mesActual.Substring(1);
            ViewBag.MesActual = mesActual;
            ViewBag.AnioActual = DateTime.Now.Year;
            ViewBag.FechaHoy = DateTime.Now.ToString("dd/MM/yyyy");

            // Calcular semana del mes
            var dia = DateTime.Now.Day;
            var semana = dia <= 7 ? "Primera" : dia <= 14 ? "Segunda" : dia <= 21 ? "Tercera" : dia <= 28 ? "Cuarta" : "Última";
            ViewBag.SemanaActual = semana;

            // Limpiar anuncios expirados (más de 7 días)
            var ahora = DateTime.Now;
            var expirados = _anuncios.Where(a => (ahora - a.Value.FechaCreacion).TotalDays > 7).Select(a => a.Key).ToList();
            foreach (var key in expirados)
                _anuncios.TryRemove(key, out _);

            // Pasar anuncios vigentes
            ViewBag.Anuncios = _anuncios.Values.OrderByDescending(a => a.FechaCreacion).ToList();

            // Traer canciones de la base de datos via API
            try
            {
                var handler = new HttpClientHandler();
                handler.ServerCertificateCustomValidationCallback = (m, c, ch, e) => true;
                using var client = new HttpClient(handler);
                var json = client.GetStringAsync("https://localhost:7220/api/Canciones").Result;
                var canciones = Newtonsoft.Json.JsonConvert.DeserializeObject<List<IglesiaGPS.modelo.Cancion>>(json);
                ViewBag.Canciones = canciones ?? new List<IglesiaGPS.modelo.Cancion>();
            }
            catch
            {
                ViewBag.Canciones = new List<IglesiaGPS.modelo.Cancion>();
            }

            return View();
        }

        // POST: /Home/CrearAnuncio
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CrearAnuncio(string contenido, string tipo)
        {
            var nombreUsuario = HttpContext.Session.GetString("UsuarioNombre");
            if (string.IsNullOrEmpty(nombreUsuario))
            {
                return RedirectToAction("Login", "Auth");
            }

            if (!string.IsNullOrWhiteSpace(contenido))
            {
                var id = Guid.NewGuid().ToString("N")[..8];
                _anuncios[id] = new AnuncioTemporal
                {
                    Id = id,
                    Autor = nombreUsuario,
                    Contenido = contenido,
                    Tipo = string.IsNullOrEmpty(tipo) ? "Anuncio" : tipo,
                    FechaCreacion = DateTime.Now
                };
            }

            return RedirectToAction("Index");
        }

        // POST: /Home/EliminarAnuncio
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EliminarAnuncio(string id)
        {
            _anuncios.TryRemove(id, out _);
            return RedirectToAction("Index");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

