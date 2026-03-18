using IglesiaGPS.modelo;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace Iglesia.MVC.Controllers
{
    public class AuthController : Controller
    {
        private readonly string _apiBaseUrl = (Environment.GetEnvironmentVariable("API_URL") 
            ?? Environment.GetEnvironmentVariable("API_BASE_URL") 
            ?? "https://localhost:7220") + "/api/Auth";

        private HttpClient CrearHttpClient()
        {
            var handler = new HttpClientHandler();
            // Ignorar validación de certificado SSL en desarrollo
            handler.ServerCertificateCustomValidationCallback =
                (message, cert, chain, sslPolicyErrors) => true;
            return new HttpClient(handler);
        }

        // GET: /Auth/Login
        public IActionResult Login()
        {
            return View();
        }

        // POST: /Auth/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(string correo, string contrasena)
        {
            try
            {
                using var client = CrearHttpClient();
                var payload = JsonConvert.SerializeObject(new { correo, contrasena });
                var content = new StringContent(payload, Encoding.UTF8, "application/json");
                var response = client.PostAsync($"{_apiBaseUrl}/login", content).Result;

                var json = response.Content.ReadAsStringAsync().Result;

                if (response.IsSuccessStatusCode)
                {
                    var usuario = JsonConvert.DeserializeObject<Usuario>(json);

                    // Guardar info del usuario en sesión
                    HttpContext.Session.SetString("UsuarioId", usuario!.UsuarioId.ToString());
                    HttpContext.Session.SetString("UsuarioNombre", usuario.Nombre);
                    HttpContext.Session.SetString("UsuarioCorreo", usuario.Correo);
                    HttpContext.Session.SetString("UsuarioRol", usuario.Rol?.Nombre ?? "Usuario");

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    try
                    {
                        var error = JsonConvert.DeserializeObject<dynamic>(json);
                        ViewBag.Error = (string)(error?.mensaje ?? "Error al iniciar sesión.");
                    }
                    catch
                    {
                        ViewBag.Error = "Error al iniciar sesión. Verifica tus credenciales.";
                    }
                    return View();
                }
            }
            catch (Exception)
            {
                ViewBag.Error = "No se pudo conectar con el servidor. Verifica que la API esté ejecutándose.";
                return View();
            }
        }

        // GET: /Auth/Registro
        public IActionResult Registro()
        {
            return View();
        }

        // POST: /Auth/Registro
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Registro(string nombre, string apellido, string correo, string contrasena)
        {
            try
            {
                using var client = CrearHttpClient();
                var payload = JsonConvert.SerializeObject(new { nombre, apellido, correo, contrasena });
                var content = new StringContent(payload, Encoding.UTF8, "application/json");
                var response = client.PostAsync($"{_apiBaseUrl}/registro", content).Result;

                var json = response.Content.ReadAsStringAsync().Result;

                if (response.IsSuccessStatusCode)
                {
                    try
                    {
                        var result = JsonConvert.DeserializeObject<dynamic>(json);
                        TempData["Correo"] = correo;
                        TempData["Mensaje"] = (string)(result?.mensaje ?? "Registro exitoso.");
                    }
                    catch
                    {
                        TempData["Correo"] = correo;
                        TempData["Mensaje"] = "Registro exitoso. Revisa tu correo.";
                    }
                    return RedirectToAction("VerificarCodigo");
                }
                else
                {
                    try
                    {
                        var result = JsonConvert.DeserializeObject<dynamic>(json);
                        ViewBag.Error = (string)(result?.mensaje ?? "Error al registrarse.");
                    }
                    catch
                    {
                        ViewBag.Error = "Error al registrarse. Intenta de nuevo.";
                    }
                    return View();
                }
            }
            catch (Exception)
            {
                ViewBag.Error = "No se pudo conectar con el servidor. Verifica que la API esté ejecutándose.";
                return View();
            }
        }

        // GET: /Auth/VerificarCodigo
        public IActionResult VerificarCodigo()
        {
            ViewBag.Correo = TempData["Correo"]?.ToString() ?? "";
            ViewBag.Mensaje = TempData["Mensaje"]?.ToString() ?? "";
            return View();
        }

        // POST: /Auth/VerificarCodigo
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult VerificarCodigo(string correo, string codigo)
        {
            try
            {
                using var client = CrearHttpClient();
                var payload = JsonConvert.SerializeObject(new { correo, codigo });
                var content = new StringContent(payload, Encoding.UTF8, "application/json");
                var response = client.PostAsync($"{_apiBaseUrl}/verificar", content).Result;

                var json = response.Content.ReadAsStringAsync().Result;

                if (response.IsSuccessStatusCode)
                {
                    try
                    {
                        var result = JsonConvert.DeserializeObject<dynamic>(json);
                        TempData["Exito"] = (string)(result?.mensaje ?? "Cuenta verificada.");
                    }
                    catch
                    {
                        TempData["Exito"] = "Cuenta verificada exitosamente.";
                    }
                    return RedirectToAction("Login");
                }
                else
                {
                    try
                    {
                        var result = JsonConvert.DeserializeObject<dynamic>(json);
                        ViewBag.Error = (string)(result?.mensaje ?? "Código incorrecto.");
                    }
                    catch
                    {
                        ViewBag.Error = "Código incorrecto. Intenta de nuevo.";
                    }
                    ViewBag.Correo = correo;
                    return View();
                }
            }
            catch (Exception)
            {
                ViewBag.Error = "No se pudo conectar con el servidor. Verifica que la API esté ejecutándose.";
                ViewBag.Correo = correo;
                return View();
            }
        }

        // POST: /Auth/CerrarSesion
        public IActionResult CerrarSesion()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        // GET: /Auth/RecuperarContrasena
        public IActionResult RecuperarContrasena()
        {
            return View();
        }

        // POST: /Auth/RecuperarContrasena
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RecuperarContrasena(string correo)
        {
            try
            {
                using var client = CrearHttpClient();
                var payload = JsonConvert.SerializeObject(new { correo });
                var content = new StringContent(payload, Encoding.UTF8, "application/json");
                var response = client.PostAsync($"{_apiBaseUrl}/recuperar-contrasena", content).Result;

                var json = response.Content.ReadAsStringAsync().Result;

                if (response.IsSuccessStatusCode)
                {
                    TempData["Exito"] = "Si el correo existe, se enviará la contraseña correspondiente.";
                    return RedirectToAction("Login");
                }
                else
                {
                    try
                    {
                        var result = JsonConvert.DeserializeObject<dynamic>(json);
                        ViewBag.Error = (string)(result?.mensaje ?? "Error al recuperar cuenta.");
                    }
                    catch
                    {
                        ViewBag.Error = "No se pudo recuperar la cuenta. Intenta nuevamente.";
                    }
                    return View();
                }
            }
            catch (Exception)
            {
                ViewBag.Error = "Error de conexión con el servidor.";
                return View();
            }
        }
    }
}

