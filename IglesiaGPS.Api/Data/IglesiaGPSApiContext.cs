using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using IglesiaGPS.modelo;

    public class IglesiaGPSApiContext : DbContext
    {
        public IglesiaGPSApiContext (DbContextOptions<IglesiaGPSApiContext> options)
            : base(options)
        {
        }

        public DbSet<Rol> Roles { get; set; } = default!;
        public DbSet<Usuario> Usuarios { get; set; } = default!;
        public DbSet<Cancion> Canciones { get; set; } = default!;
        public DbSet<NotaMusical> NotaMusicales { get; set; } = default!;
        public DbSet<ListaCanciones> ListaCanciones { get; set; } = default!;
        public DbSet<ListaCancionDetalle> ListaCancionDetalles { get; set; } = default!;
        public DbSet<Recomendacion> Recomendaciones { get; set; } = default!;
        public DbSet<SolicitudDirector> SolicitudDirectores { get; set; } = default!;
        public DbSet<Anuncio> Anuncios { get; set; } = default!;
    }
