using System.ComponentModel.DataAnnotations;

namespace IglesiaGPS.modelo
{
    public class RegistroActividad
    {
        [Key]
        public int RegistroActividadId { get; set; }
        public int UsuarioId { get; set; }
        public string TipoAccion { get; set; }
        public string Descripcion { get; set; }
        public DateTime FechaAccion { get; set; }
        public string? EntidadAfectada { get; set; }
        public int? EntidadId { get; set; }

        public Usuario? Usuario { get; set; }
    }
}
