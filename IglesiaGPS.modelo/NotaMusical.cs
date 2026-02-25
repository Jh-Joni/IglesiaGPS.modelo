using System.ComponentModel.DataAnnotations;

namespace IglesiaGPS.modelo
{
    public class NotaMusical
    {
        [Key]
        public int NotaMusicalId { get; set; }
        public int CancionId { get; set; }
        public string Contenido { get; set; }
        public string? Instrumento { get; set; }
        public DateTime UltimaEdicion { get; set; }
        public int EditadoPorUsuarioId { get; set; }

        public Cancion? Cancion { get; set; }
        public Usuario? EditadoPor { get; set; }
    }
}
