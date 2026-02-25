using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IglesiaGPS.modelo
{
    public class Usuario
    {
        [Key]
        public int UsuarioId { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Correo { get; set; }
        public string Contrasena { get; set; }
        public int RolId { get; set; }
        public DateTime FechaRegistro { get; set; }
        public bool Activo { get; set; }
        public bool PuedeEditarNotas { get; set; }

        public Rol? Rol { get; set; }
        public List<Recomendacion>? Recomendaciones { get; set; }
        public List<ListaCanciones>? ListasCanciones { get; set; }

        [InverseProperty("Usuario")]
        public List<SolicitudDirector>? Solicitudes { get; set; }

        [InverseProperty("RespuestaPor")]
        public List<SolicitudDirector>? SolicitudesRespondidas { get; set; }

        public List<RegistroActividad>? Actividades { get; set; }
    }
}
