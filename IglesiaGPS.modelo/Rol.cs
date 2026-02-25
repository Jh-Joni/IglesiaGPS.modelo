using System.ComponentModel.DataAnnotations;

namespace IglesiaGPS.modelo
{
    public class Rol
    {
        [Key]
        public int RolId { get; set; }
        public string Nombre { get; set; }
        public string? Descripcion { get; set; }
        public string? CodigoAcceso { get; set; }

        public List<Usuario>? Usuarios { get; set; }
    }
}
