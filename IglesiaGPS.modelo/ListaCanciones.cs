using System.ComponentModel.DataAnnotations;

namespace IglesiaGPS.modelo
{
    public class ListaCanciones
    {
        [Key]
        public int ListaCancionesId { get; set; }
        public string Titulo { get; set; }
        public DateTime? FechaPublicacion { get; set; }
        public DateTime FechaCreacion { get; set; }
        public bool Publicada { get; set; }
        public int DirectorId { get; set; }

        public Usuario? Director { get; set; }
        public List<ListaCancionDetalle>? Detalles { get; set; }
    }
}
