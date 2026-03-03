using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Utils;

namespace IglesiaGPS.Api.Services
{
    public class EmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public string GenerarCodigo()
        {
            var random = new Random();
            return random.Next(100000, 999999).ToString();
        }

        public async Task EnviarCodigoVerificacion(string correoDestino, string codigo)
        {
            var smtpSettings = _config.GetSection("SmtpSettings");
            var email = smtpSettings["Email"]!;
            var password = smtpSettings["Password"]!.Replace(" ", "");
            var host = smtpSettings["Host"] ?? "smtp.gmail.com";

            Console.WriteLine($"[EmailService] Enviando código a: {correoDestino}");
            Console.WriteLine($"[EmailService] Desde: {email}");

            // Crear el mensaje con MimeKit
            var mensaje = new MimeMessage();
            mensaje.From.Add(new MailboxAddress("Iglesia GPS", email));
            mensaje.To.Add(new MailboxAddress("", correoDestino));
            mensaje.Subject = "Código de Verificación - Iglesia GPS";

            var builder = new BodyBuilder();
            string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "logosinfondo.png");
            var logo = builder.LinkedResources.Add(imagePath);
            logo.ContentId = MimeUtils.GenerateMessageId();

            builder.HtmlBody = $@"
                <div style='font-family: Arial, sans-serif; max-width: 500px; margin: 0 auto; padding: 30px; background: #f4f6f9; border-radius: 12px;'>
                    <div style='text-align: center; padding: 20px; background: #0a1f44; border-radius: 12px 12px 0 0;'>
                        <img src='cid:{logo.ContentId}' alt='Iglesia GPS Logo' style='height: 80px; margin-bottom: 10px;' />
                        <h1 style='color: #d4af37; margin: 0;'>Iglesia GPS</h1>
                        <p style='color: white; margin: 5px 0 0;'>Ministerio de Alabanza</p>
                    </div>
                    <div style='background: white; padding: 30px; border-radius: 0 0 12px 12px;'>
                        <h2 style='color: #0a1f44; text-align: center;'>Código de Verificación</h2>
                        <p style='text-align: center; color: #666;'>Usa el siguiente código para verificar tu cuenta:</p>
                        <div style='text-align: center; margin: 25px 0;'>
                            <span style='font-size: 36px; font-weight: bold; letter-spacing: 8px; color: #0a1f44; background: #f4f6f9; padding: 15px 30px; border-radius: 8px;'>{codigo}</span>
                        </div>
                        <p style='text-align: center; color: #999; font-size: 12px;'>Este código es válido por tiempo limitado. Si no solicitaste este código, ignora este mensaje.</p>
                    </div>
                </div>";
            
            mensaje.Body = builder.ToMessageBody();

            try
            {
                using var smtp = new SmtpClient();

                // Conectar con SSL directo en puerto 465
                await smtp.ConnectAsync(host, 465, SecureSocketOptions.SslOnConnect);
                await smtp.AuthenticateAsync(email, password);
                await smtp.SendAsync(mensaje);
                await smtp.DisconnectAsync(true);

                Console.WriteLine($"[EmailService] ✅ Correo enviado exitosamente a {correoDestino}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[EmailService] ❌ Error al enviar correo: {ex.Message}");
                if (ex.InnerException != null)
                    Console.WriteLine($"[EmailService] ❌ Inner: {ex.InnerException.Message}");
                throw;
            }
        }
        public async Task EnviarContrasenaOlvidada(string correoDestino, string contrasena)
        {
            var smtpSettings = _config.GetSection("SmtpSettings");
            var email = smtpSettings["Email"]!;
            var password = smtpSettings["Password"]!.Replace(" ", "");
            var host = smtpSettings["Host"] ?? "smtp.gmail.com";

            Console.WriteLine($"[EmailService] Enviando recuperación a: {correoDestino}");
            Console.WriteLine($"[EmailService] Desde: {email}");

            // Crear el mensaje con MimeKit
            var mensaje = new MimeMessage();
            mensaje.From.Add(new MailboxAddress("Iglesia GPS", email));
            mensaje.To.Add(new MailboxAddress("", correoDestino));
            mensaje.Subject = "Recuperación de Contraseña - Iglesia GPS";

            var builder = new BodyBuilder();
            string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "logosinfondo.png");
            var logo = builder.LinkedResources.Add(imagePath);
            logo.ContentId = MimeUtils.GenerateMessageId();

            builder.HtmlBody = $@"
                <div style='font-family: Arial, sans-serif; max-width: 500px; margin: 0 auto; padding: 30px; background: #f4f6f9; border-radius: 12px;'>
                    <div style='text-align: center; padding: 20px; background: #0a1f44; border-radius: 12px 12px 0 0;'>
                        <img src='cid:{logo.ContentId}' alt='Iglesia GPS Logo' style='height: 80px; margin-bottom: 10px;' />
                        <h1 style='color: #d4af37; margin: 0;'>Iglesia GPS</h1>
                        <p style='color: white; margin: 5px 0 0;'>Ministerio de Alabanza</p>
                    </div>
                    <div style='background: white; padding: 30px; border-radius: 0 0 12px 12px;'>
                        <h2 style='color: #0a1f44; text-align: center;'>Recuperación de Contraseña</h2>
                        <p style='text-align: center; color: #666;'>Hemos recibido una solicitud para recuperar tu contraseña.</p>
                        <div style='text-align: center; margin: 25px 0;'>
                            <p style='font-size: 16px; color: #0a1f44; margin-bottom: 10px;'>Tu contraseña es:</p>
                            <span style='font-size: 24px; font-weight: bold; color: #d4af37; background: #0a1f44; padding: 15px 30px; border-radius: 8px;'>{contrasena}</span>
                        </div>
                        <p style='text-align: center; color: #666; font-size: 16px; font-weight: bold;'>¡No te olvides más la contraseña descuidado ;c </p>
                        <p style='text-align: center; color: #999; font-size: 12px; margin-top: 20px;'>Si no solicitaste recuperar tu contraseña, te recomendamos cambiarla inmediatamente o contactar al administrador.</p>
                    </div>
                </div>";
            
            mensaje.Body = builder.ToMessageBody();

            try
            {
                using var smtp = new SmtpClient();
                await smtp.ConnectAsync(host, 465, SecureSocketOptions.SslOnConnect);
                await smtp.AuthenticateAsync(email, password);
                await smtp.SendAsync(mensaje);
                await smtp.DisconnectAsync(true);

                Console.WriteLine($"[EmailService] ✅ Correo de recuperación enviado exitosamente a {correoDestino}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[EmailService] ❌ Error al enviar correo: {ex.Message}");
                if (ex.InnerException != null)
                    Console.WriteLine($"[EmailService] ❌ Inner: {ex.InnerException.Message}");
                throw;
            }
        }
    }
}


