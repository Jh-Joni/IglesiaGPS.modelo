using System.ComponentModel.DataAnnotations;

namespace IglesiaGPS.modelo
{
    public class SolicitudDirector
    {
        [Key]
        public int SolicitudDirectorId { get; set; }
        public int UsuarioId { get; set; }
        public string CodigoIngresado { get; set; }
        public string Estado { get; set; }
        public DateTime FechaSolicitud { get; set; }
        public DateTime? FechaRespuesta { get; set; }
        public int? RespuestaPorUsuarioId { get; set; }

        public Usuario? Usuario { get; set; }
        public Usuario? RespuestaPor { get; set; }
    }
}
