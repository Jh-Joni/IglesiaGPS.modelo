using System.ComponentModel.DataAnnotations;

namespace IglesiaGPS.modelo
{
    public class Cancion
    {
        [Key]
        public int CancionId { get; set; }
        public string Titulo { get; set; }
        public string Autor { get; set; }
        public string? Tono { get; set; }
        public string? UrlAudio { get; set; }
        public string? Letra { get; set; }
        public DateTime FechaCreacion { get; set; }
        public int CreadoPorUsuarioId { get; set; }

        public Usuario? CreadoPor { get; set; }
        public List<NotaMusical>? NotasMusicales { get; set; }
        public List<ListaCancionDetalle>? ListaCancionDetalles { get; set; }
        public List<Recomendacion>? Recomendaciones { get; set; }
    }
}
