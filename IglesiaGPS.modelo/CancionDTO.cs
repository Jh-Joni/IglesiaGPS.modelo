using System;
using System.ComponentModel.DataAnnotations;

namespace IglesiaGPS.modelo
{
    public class CancionDTO
    {
        public int CancionId { get; set; }
        
        [Required(ErrorMessage = "El título es obligatorio")]
        public string Titulo { get; set; }
        
        [Required(ErrorMessage = "El autor es obligatorio")]
        public string Autor { get; set; }
        
        public string? Tono { get; set; }
        public string? UrlAudio { get; set; }
        public string? Letra { get; set; }
        
        // El DTO lleva la imagen encapsulada en texto Base64
        public string? FotoBase64 { get; set; } 

        public DateTime FechaCreacion { get; set; }
        public int CreadoPorUsuarioId { get; set; }
    }
}
