using System.ComponentModel.DataAnnotations;

namespace IglesiaGPS.modelo
{
    public class ListaCancionDetalle
    {
        [Key]
        public int ListaCancionDetalleId { get; set; }
        public int ListaCancionesId { get; set; }
        public int CancionId { get; set; }
        public int Orden { get; set; }

        public ListaCanciones? ListaCanciones { get; set; }
        public Cancion? Cancion { get; set; }
    }
}
