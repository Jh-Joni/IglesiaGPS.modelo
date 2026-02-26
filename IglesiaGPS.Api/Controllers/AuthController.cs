using System.Collections.Concurrent;
using IglesiaGPS.Api.DTOs;
using IglesiaGPS.Api.Services;
using IglesiaGPS.modelo;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IglesiaGPS.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IglesiaGPSApiContext _context;
        private readonly EmailService _emailService;

        // Almacenamiento temporal: guarda datos de registro + código (NO se guarda en BD hasta verificar)
        private static readonly ConcurrentDictionary<string, RegistroPendiente> _registrosPendientes = new();

        public AuthController(IglesiaGPSApiContext context, EmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        // Clase interna para almacenar datos de registro temporalmente
        private class RegistroPendiente
        {
            public string Nombre { get; set; } = "";
            public string Apellido { get; set; } = "";
            public string Correo { get; set; } = "";
            public string Contrasena { get; set; } = "";
            public string Codigo { get; set; } = "";
            public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
        }

        // POST: api/Auth/login
        [HttpPost("login")]
        public async Task<ActionResult<Usuario>> Login(LoginDto dto)
        {
            var usuario = await _context.Usuarios
                .Include(u => u.Rol)
                .FirstOrDefaultAsync(u => u.Correo == dto.Correo && u.Contrasena == dto.Contrasena);

            if (usuario == null)
            {
                return Unauthorized(new { mensaje = "Correo o contraseña incorrectos." });
            }

            return Ok(usuario);
        }

        // POST: api/Auth/registro
        [HttpPost("registro")]
        public async Task<ActionResult> Registro(RegistroDto dto)
        {
            // Verificar si el correo ya existe en la BD
            var existeCorreo = await _context.Usuarios.AnyAsync(u => u.Correo == dto.Correo);
            if (existeCorreo)
            {
                return BadRequest(new { mensaje = "Ya existe una cuenta con este correo electrónico." });
            }

            // Verificar que existe el rol "Usuario"
            var rolUsuario = await _context.Roles.FirstOrDefaultAsync(r => r.Nombre == "Usuario");
            if (rolUsuario == null)
            {
                return BadRequest(new { mensaje = "No se encontró el rol 'Usuario' en la base de datos. Asegúrate de crearlo primero." });
            }

            // Generar código de verificación
            var codigo = _emailService.GenerarCodigo();

            // Guardar datos de registro en MEMORIA (NO en la BD)
            _registrosPendientes[dto.Correo] = new RegistroPendiente
            {
                Nombre = dto.Nombre,
                Apellido = dto.Apellido,
                Correo = dto.Correo,
                Contrasena = dto.Contrasena,
                Codigo = codigo,
                FechaCreacion = DateTime.UtcNow
            };

            // Intentar enviar el código por correo
            try
            {
                await _emailService.EnviarCodigoVerificacion(dto.Correo, codigo);
                return Ok(new { mensaje = "Se envió un código de verificación a tu correo." });
            }
            catch (Exception ex)
            {
                // Si falla el correo, igual mantener el registro pendiente
                // pero informar al usuario el error detallado
                return Ok(new { 
                    mensaje = "No se pudo enviar el correo de verificación.", 
                    error = ex.Message,
                    // En desarrollo, mostrar el código para pruebas
                    codigoDebug = codigo
                });
            }
        }

        // POST: api/Auth/verificar
        [HttpPost("verificar")]
        public async Task<ActionResult> VerificarCodigo(VerificarCodigoDto dto)
        {
            // Buscar registro pendiente
            if (!_registrosPendientes.TryGetValue(dto.Correo, out var registro))
            {
                return BadRequest(new { mensaje = "No se encontró un registro pendiente para este correo. Regístrate de nuevo." });
            }

            // Verificar que el código coincida
            if (registro.Codigo != dto.Codigo)
            {
                return BadRequest(new { mensaje = "El código de verificación es incorrecto." });
            }

            // Buscar el rol "Usuario"
            var rolUsuario = await _context.Roles.FirstOrDefaultAsync(r => r.Nombre == "Usuario");
            if (rolUsuario == null)
            {
                return BadRequest(new { mensaje = "No se encontró el rol 'Usuario'." });
            }

            // AHORA SÍ crear el usuario en la base de datos
            var usuario = new Usuario
            {
                Nombre = registro.Nombre,
                Apellido = registro.Apellido,
                Correo = registro.Correo,
                Contrasena = registro.Contrasena,
                RolId = rolUsuario.RolId,
                FechaRegistro = DateTime.UtcNow,
                Activo = true,
                PuedeEditarNotas = false
            };

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            // Eliminar el registro pendiente
            _registrosPendientes.TryRemove(dto.Correo, out _);

            return Ok(new { mensaje = "Cuenta verificada y creada exitosamente. Ya puedes iniciar sesión." });
        }

        // POST: api/Auth/reenviar
        [HttpPost("reenviar")]
        public async Task<ActionResult> ReenviarCodigo([FromBody] string correo)
        {
            if (!_registrosPendientes.TryGetValue(correo, out var registro))
            {
                return NotFound(new { mensaje = "No se encontró un registro pendiente. Regístrate de nuevo." });
            }

            // Generar nuevo código
            var nuevoCodigo = _emailService.GenerarCodigo();
            registro.Codigo = nuevoCodigo;

            try
            {
                await _emailService.EnviarCodigoVerificacion(correo, nuevoCodigo);
            }
            catch (Exception ex)
            {
                return BadRequest(new { 
                    mensaje = "No se pudo enviar el correo.", 
                    error = ex.Message,
                    codigoDebug = nuevoCodigo
                });
            }

            return Ok(new { mensaje = "Se reenvió el código de verificación a tu correo." });
        }
    }
}

