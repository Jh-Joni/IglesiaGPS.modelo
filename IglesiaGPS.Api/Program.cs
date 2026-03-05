using IglesiaGPS.Api.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);

// Aumentar límite de tamaño del cuerpo de la solicitud para la API
builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = 50 * 1024 * 1024; // 50 MB
});

builder.Services.AddSingleton<EmailService>();
builder.Services.AddDbContext<IglesiaGPSApiContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("IglesiaGPSApiContext") ?? throw new InvalidOperationException("Connection string 'IglesiaGPSApiContext' not found.")));

builder.Services
           .AddControllers()
           .AddNewtonsoftJson(
               options => options.SerializerSettings.ReferenceLoopHandling
               = Newtonsoft.Json.ReferenceLoopHandling.Ignore
           );

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// Auto-heal PostgreSQL sequences to avoid PK duplicate key issues (23505)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<IglesiaGPSApiContext>();
    try
    {
        db.Database.ExecuteSqlRaw("SELECT setval(pg_get_serial_sequence('\"Canciones\"', 'CancionId'), coalesce(max(\"CancionId\"), 1), max(\"CancionId\") IS NOT null) FROM \"Canciones\";");
        db.Database.ExecuteSqlRaw("SELECT setval(pg_get_serial_sequence('\"ListaCanciones\"', 'ListaCancionesId'), coalesce(max(\"ListaCancionesId\"), 1), max(\"ListaCancionesId\") IS NOT null) FROM \"ListaCanciones\";");
        db.Database.ExecuteSqlRaw("SELECT setval(pg_get_serial_sequence('\"Usuarios\"', 'UsuarioId'), coalesce(max(\"UsuarioId\"), 1), max(\"UsuarioId\") IS NOT null) FROM \"Usuarios\";");
        db.Database.ExecuteSqlRaw("SELECT setval(pg_get_serial_sequence('\"Roles\"', 'RolId'), coalesce(max(\"RolId\"), 1), max(\"RolId\") IS NOT null) FROM \"Roles\";");
        db.Database.ExecuteSqlRaw("SELECT setval(pg_get_serial_sequence('\"NotaMusicales\"', 'NotaMusicalId'), coalesce(max(\"NotaMusicalId\"), 1), max(\"NotaMusicalId\") IS NOT null) FROM \"NotaMusicales\";");
        db.Database.ExecuteSqlRaw("SELECT setval(pg_get_serial_sequence('\"ListaCancionDetalles\"', 'ListaCancionDetalleId'), coalesce(max(\"ListaCancionDetalleId\"), 1), max(\"ListaCancionDetalleId\") IS NOT null) FROM \"ListaCancionDetalles\";");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error synchronizing sequences: {ex.Message}");
    }
}

app.Run();
