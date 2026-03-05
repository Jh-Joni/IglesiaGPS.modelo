using Iglesia.Api.consumer;
using IglesiaGPS.modelo;

var builder = WebApplication.CreateBuilder(args);

// Aumentar limte de tamaño del cuerpo de la solicitud (50 MB) para subida de imagenes
builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = 50 * 1024 * 1024; // 50 MB
});

var apiBaseUrl = Environment.GetEnvironmentVariable("API_BASE_URL") ?? "https://iglesiagps-modelo-1.onrender.com";

Crud<Rol>.EndPoint = $"{apiBaseUrl}/api/Roles";
Crud<Usuario>.EndPoint = $"{apiBaseUrl}/api/Usuarios";
Crud<Cancion>.EndPoint = $"{apiBaseUrl}/api/Canciones";
Crud<CancionDTO>.EndPoint = $"{apiBaseUrl}/api/Canciones";
Crud<NotaMusical>.EndPoint = $"{apiBaseUrl}/api/NotasMusicales";
Crud<ListaCanciones>.EndPoint = $"{apiBaseUrl}/api/ListaCanciones";
Crud<ListaCancionDetalle>.EndPoint = $"{apiBaseUrl}/api/ListaCancionDetalles";
Crud<Recomendacion>.EndPoint = $"{apiBaseUrl}/api/Recomendaciones";
Crud<SolicitudDirector>.EndPoint = $"{apiBaseUrl}/api/SolicitudesDirector";

// Add services to the container.
builder.Services.AddControllersWithViews();

// Agregar sesiones
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Siempre usa captura de excepciones en produccion o desarrollo para no romper la app
app.UseExceptionHandler("/Home/Error");
app.UseHsts();


app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
