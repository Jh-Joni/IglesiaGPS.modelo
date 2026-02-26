using Iglesia.Api.consumer;
using IglesiaGPS.modelo;

var builder = WebApplication.CreateBuilder(args);

Crud<Rol>.EndPoint = "https://localhost:7220/api/Roles";
Crud<Usuario>.EndPoint = "https://localhost:7220/api/Usuarios";
Crud<Cancion>.EndPoint = "https://localhost:7220/api/Canciones";
Crud<NotaMusical>.EndPoint = "https://localhost:7220/api/NotasMusicales";
Crud<ListaCanciones>.EndPoint = "https://localhost:7220/api/ListaCanciones";
Crud<ListaCancionDetalle>.EndPoint = "https://localhost:7220/api/ListaCancionDetalles";
Crud<Recomendacion>.EndPoint = "https://localhost:7220/api/Recomendaciones";
Crud<SolicitudDirector>.EndPoint = "https://localhost:7220/api/SolicitudesDirector";

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

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
