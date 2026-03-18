using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IglesiaGPS.modelo
{
    public class Anuncio
    {
        [Key]
        public int AnuncioID { get; set; }

        [Required]
        [MaxLength(100)]
        public string Autor { get; set; } = string.Empty;

        [Required]
       
        public string Contenido { get; set; } 

        [Required]
        [MaxLength(50)]
        public string Tipo { get; set; } = "Anuncio"; // Anuncio, Versículo, Mensaje

        [Required]
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
    }
}
