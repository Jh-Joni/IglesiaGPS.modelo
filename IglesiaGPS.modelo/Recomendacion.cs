using System.ComponentModel.DataAnnotations;

namespace IglesiaGPS.modelo
{
    public class Recomendacion
    {
        [Key]
        public int RecomendacionId { get; set; }
        public int UsuarioId { get; set; }
        public int CancionId { get; set; }
        public string? Mensaje { get; set; }
        public DateTime FechaRecomendacion { get; set; }
        public bool Leida { get; set; }

        public Usuario? Usuario { get; set; }
        public Cancion? Cancion { get; set; }
    }
}
