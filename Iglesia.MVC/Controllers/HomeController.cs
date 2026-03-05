using Iglesia.MVC.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Globalization;
using IglesiaGPS.modelo;
using Iglesia.Api.consumer;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

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

            // Traer el Historial de Repertorios (Máximo 5)
            try
            {
                var todasLasListas = Crud<ListaCanciones>.GetAll() ?? new List<ListaCanciones>();

                // Filtrar las 5 listas PADRE (Domingo) más recientes que estén publicadas
                var listasPadre = todasLasListas
                    .Where(l => l.Publicada && !l.Titulo.StartsWith("Miércoles_"))
                    .OrderByDescending(l => l.FechaCreacion)
                    .Take(5)
                    .ToList();

                // Extraer los IDs de esas 5 listas padre
                var idsPadre = listasPadre.Select(l => l.ListaCancionesId).ToList();

                // Buscar las listas de Miércoles asociadas a esos 5 padres (pueden ser privadas)
                var listasMiercoles = todasLasListas
                    .Where(l => l.Titulo.StartsWith("Miércoles_") && 
                                idsPadre.Contains(int.Parse(l.Titulo.Split('_')[1])))
                    .ToList();

                // Combinar padres e hijas para la vista
                var listas = new List<ListaCanciones>();
                listas.AddRange(listasPadre);
                listas.AddRange(listasMiercoles);

                ViewBag.ListasMusicales = listas;
            }
            catch (Exception ex)
            {
                ViewBag.ListasMusicales = new List<ListaCanciones>();
                _logger.LogError(ex, "Error al cargar las listas de canciones.");
            }

            // Notificación de Recomendaciones para Directores/Desarrolladores
            try
            {
                var recomendaciones = Crud<Recomendacion>.GetAll();
                ViewBag.TotalRecomendacionesPendientes = recomendaciones != null ? recomendaciones.Count : 0;
            }
            catch (Exception)
            {
                ViewBag.TotalRecomendacionesPendientes = 0;
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

        // GET: /Home/DescargarPdfListaActiva/5
        [HttpGet]
        public IActionResult DescargarPdfListaActiva(int id)
        {
            try
            {
                var lista = Crud<ListaCanciones>.GetById(id);
                if (lista == null) return NotFound();

                var detalles = lista.Detalles?.OrderBy(d => d.Orden).ToList() ?? new List<ListaCancionDetalle>();

                // Obtener notas de forma segura
                List<NotaMusical> todasLasNotas;
                try
                {
                    todasLasNotas = Crud<NotaMusical>.GetAll() ?? new List<NotaMusical>();
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "No se pudieron cargar las notas musicales para el PDF.");
                    todasLasNotas = new List<NotaMusical>();
                }

                QuestPDF.Settings.License = LicenseType.Community;

                // Sanitizar título para nombre de archivo seguro
                var tituloSeguro = string.Join("_", (lista.Titulo ?? "Repertorio").Split(Path.GetInvalidFileNameChars()));

                var document = Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Size(PageSizes.A4);
                        page.Margin(2, Unit.Centimetre);
                        page.PageColor(Colors.White);
                        page.DefaultTextStyle(x => x.FontSize(12));

                        page.Content().Column(c =>
                        {
                            c.Item().PaddingBottom(1, Unit.Centimetre).Element(ComposeHeader);
                            c.Item().Element(ComposeContent);
                        });

                        page.Footer().AlignCenter().Text(x =>
                        {
                            x.CurrentPageNumber();
                            x.Span(" / ");
                            x.TotalPages();
                        });

                        void ComposeHeader(IContainer headerContainer)
                        {
                            headerContainer.Row(row =>
                            {
                                row.RelativeItem().Column(column =>
                                {
                                    column.Item().Text($"Repertorio: {lista.Titulo ?? "Sin título"}").FontSize(20).SemiBold().FontColor(Colors.Blue.Darken2);
                                    column.Item().Text($"Fecha: {lista.FechaCreacion:dd/MM/yyyy}").FontSize(14);
                                });
                            });
                        }

                        void ComposeContent(IContainer contentContainer)
                        {
                            contentContainer.PaddingVertical(1, Unit.Centimetre).Column(column =>
                            {
                                if (detalles.Count == 0)
                                {
                                    column.Item().PaddingBottom(20).Text("Este repertorio no tiene canciones vinculadas.").FontSize(14).Italic().FontColor(Colors.Grey.Medium);
                                    return;
                                }

                                foreach (var det in detalles)
                                {
                                    var cancion = det.Cancion;
                                    if (cancion == null) continue;

                                    var notas = todasLasNotas.FirstOrDefault(n => n.CancionId == cancion.CancionId);

                                    column.Item().PaddingBottom(10).Text(text =>
                                    {
                                        text.Span(cancion.Titulo ?? "Sin título").FontSize(16).Bold();
                                        if (!string.IsNullOrEmpty(cancion.Tono))
                                        {
                                            text.Span($" (Tono: {cancion.Tono})").FontSize(14).Italic().FontColor(Colors.Grey.Darken2);
                                        }
                                    });

                                    if (notas != null && !string.IsNullOrWhiteSpace(notas.Contenido))
                                    {
                                        column.Item().PaddingBottom(20).Text(notas.Contenido).FontSize(12);
                                    }
                                    else
                                    {
                                        column.Item().PaddingBottom(20).Text("Sin notas registradas.").FontSize(12).Italic().FontColor(Colors.Grey.Medium);
                                    }

                                    column.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
                                    column.Item().PaddingBottom(15);
                                }
                            });
                        }
                    });
                });

                var pdfBytes = document.GeneratePdf();
                return File(pdfBytes, "application/pdf", $"Repertorio_{tituloSeguro}.pdf");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al generar el PDF del repertorio {Id}.", id);
                TempData["Error"] = "No se pudo generar el PDF. Intente nuevamente.";
                return RedirectToAction("Index");
            }
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

