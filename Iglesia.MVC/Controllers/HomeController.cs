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

            try
            {
                // Traer los anuncios desde la API
                var todosLosAnuncios = Crud<Anuncio>.GetAll() ?? new List<Anuncio>();
                
                // Mostrar solo los más recientes, por ejemplo, los últimos 10
                ViewBag.Anuncios = todosLosAnuncios.OrderByDescending(a => a.FechaCreacion).Take(10).ToList();
            }
            catch (Exception ex)
            {
                ViewBag.Anuncios = new List<Anuncio>();
                _logger.LogError(ex, "Error al cargar los anuncios.");
            }

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
                try
                {
                    var anuncio = new Anuncio
                    {
                        Autor = nombreUsuario,
                        Contenido = contenido,
                        Tipo = string.IsNullOrEmpty(tipo) ? "Anuncio" : tipo,
                        FechaCreacion = DateTime.UtcNow
                    };
                    Crud<Anuncio>.Create(anuncio);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al crear anuncio");
                }
            }

            return RedirectToAction("Index");
        }

        // POST: /Home/EliminarAnuncio
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EliminarAnuncio(int id)
        {
            try
            {
                Crud<Anuncio>.Delete(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error eliminando anuncio.");
            }
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

                // Preparar entradas (solo título y notas)
                var entradas = detalles
                    .Where(d => d.Cancion != null)
                    .Select(d => new
                    {
                        Title = d.Cancion!.Titulo ?? "Sin título",
                        Notes = (todasLasNotas.FirstOrDefault(n => n.CancionId == d.Cancion!.CancionId)?.Contenido) ?? string.Empty
                    })
                    .ToList();

                // Nombre de archivo seguro
                var tituloSeguro = string.Join("_", (lista.Titulo ?? "Repertorio").Split(Path.GetInvalidFileNameChars()));

                // Dividir en dos columnas balanceadas
                int half = (entradas.Count + 1) / 2;
                var left = entradas.Take(half).ToList();
                var right = entradas.Skip(half).ToList();

                var document = Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Size(PageSizes.A4);
                        page.Margin(1, Unit.Centimetre);
                        page.PageColor(Colors.White);
                        page.DefaultTextStyle(x => x.FontSize(10));

                        // No mostrar cabecera con "Lista de Canciones" ni fechas, solo contenido

                        page.Content().PaddingVertical(5, Unit.Millimetre).Row(row =>
                        {
                            row.RelativeItem().Column(col =>
                            {
                                foreach (var e in left)
                                {
                                    col.Item().PaddingBottom(6).Column(c =>
                                    {
                                        c.Item().Text(e.Title).FontSize(12).Bold();
                                        if (!string.IsNullOrWhiteSpace(e.Notes))
                                            c.Item().Text(e.Notes).FontSize(9).LineHeight(1.1f);
                                        else
                                            c.Item().Text(string.Empty).FontSize(9);

                                        c.Item().PaddingTop(4).LineHorizontal(0.3f).LineColor(Colors.Grey.Lighten3);
                                    });
                                }
                            });

                            row.Spacing(12);

                            row.RelativeItem().Column(col =>
                            {
                                foreach (var e in right)
                                {
                                    col.Item().PaddingBottom(6).Column(c =>
                                    {
                                        c.Item().Text(e.Title).FontSize(12).Bold();
                                        if (!string.IsNullOrWhiteSpace(e.Notes))
                                            c.Item().Text(e.Notes).FontSize(9).LineHeight(1.1f);
                                        else
                                            c.Item().Text(string.Empty).FontSize(9);

                                        c.Item().PaddingTop(4).LineHorizontal(0.3f).LineColor(Colors.Grey.Lighten3);
                                    });
                                }
                            });
                        });

                        page.Footer().AlignCenter().Text(x =>
                        {
                            x.CurrentPageNumber();
                            x.Span(" / ");
                            x.TotalPages();
                        });
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

