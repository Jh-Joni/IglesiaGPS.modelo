using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace IglesiaGPS.Api.Migrations
{
    /// <inheritdoc />
    public partial class iglesia01 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    RolId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "text", nullable: false),
                    Descripcion = table.Column<string>(type: "text", nullable: true),
                    CodigoAcceso = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.RolId);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    UsuarioId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "text", nullable: false),
                    Apellido = table.Column<string>(type: "text", nullable: false),
                    Correo = table.Column<string>(type: "text", nullable: false),
                    Contrasena = table.Column<string>(type: "text", nullable: false),
                    RolId = table.Column<int>(type: "integer", nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Activo = table.Column<bool>(type: "boolean", nullable: false),
                    PuedeEditarNotas = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.UsuarioId);
                    table.ForeignKey(
                        name: "FK_Usuarios_Roles_RolId",
                        column: x => x.RolId,
                        principalTable: "Roles",
                        principalColumn: "RolId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Canciones",
                columns: table => new
                {
                    CancionId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Titulo = table.Column<string>(type: "text", nullable: false),
                    Autor = table.Column<string>(type: "text", nullable: false),
                    Tono = table.Column<string>(type: "text", nullable: true),
                    UrlAudio = table.Column<string>(type: "text", nullable: true),
                    FotoUrl = table.Column<string>(type: "text", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreadoPorUsuarioId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Canciones", x => x.CancionId);
                    table.ForeignKey(
                        name: "FK_Canciones_Usuarios_CreadoPorUsuarioId",
                        column: x => x.CreadoPorUsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "UsuarioId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ListaCanciones",
                columns: table => new
                {
                    ListaCancionesId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Titulo = table.Column<string>(type: "text", nullable: false),
                    FechaPublicacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Publicada = table.Column<bool>(type: "boolean", nullable: false),
                    DirectorId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ListaCanciones", x => x.ListaCancionesId);
                    table.ForeignKey(
                        name: "FK_ListaCanciones_Usuarios_DirectorId",
                        column: x => x.DirectorId,
                        principalTable: "Usuarios",
                        principalColumn: "UsuarioId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RegistroActividad",
                columns: table => new
                {
                    RegistroActividadId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UsuarioId = table.Column<int>(type: "integer", nullable: false),
                    TipoAccion = table.Column<string>(type: "text", nullable: false),
                    Descripcion = table.Column<string>(type: "text", nullable: false),
                    FechaAccion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    EntidadAfectada = table.Column<string>(type: "text", nullable: true),
                    EntidadId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegistroActividad", x => x.RegistroActividadId);
                    table.ForeignKey(
                        name: "FK_RegistroActividad_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "UsuarioId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SolicitudDirectores",
                columns: table => new
                {
                    SolicitudDirectorId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UsuarioId = table.Column<int>(type: "integer", nullable: false),
                    CodigoIngresado = table.Column<string>(type: "text", nullable: false),
                    Estado = table.Column<string>(type: "text", nullable: false),
                    FechaSolicitud = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FechaRespuesta = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    RespuestaPorUsuarioId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SolicitudDirectores", x => x.SolicitudDirectorId);
                    table.ForeignKey(
                        name: "FK_SolicitudDirectores_Usuarios_RespuestaPorUsuarioId",
                        column: x => x.RespuestaPorUsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "UsuarioId");
                    table.ForeignKey(
                        name: "FK_SolicitudDirectores_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "UsuarioId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NotaMusicales",
                columns: table => new
                {
                    NotaMusicalId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CancionId = table.Column<int>(type: "integer", nullable: false),
                    Contenido = table.Column<string>(type: "text", nullable: false),
                    Instrumento = table.Column<string>(type: "text", nullable: true),
                    UltimaEdicion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    EditadoPorUsuarioId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotaMusicales", x => x.NotaMusicalId);
                    table.ForeignKey(
                        name: "FK_NotaMusicales_Canciones_CancionId",
                        column: x => x.CancionId,
                        principalTable: "Canciones",
                        principalColumn: "CancionId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NotaMusicales_Usuarios_EditadoPorUsuarioId",
                        column: x => x.EditadoPorUsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "UsuarioId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Recomendaciones",
                columns: table => new
                {
                    RecomendacionId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UsuarioId = table.Column<int>(type: "integer", nullable: false),
                    CancionId = table.Column<int>(type: "integer", nullable: false),
                    Mensaje = table.Column<string>(type: "text", nullable: true),
                    FechaRecomendacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Leida = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Recomendaciones", x => x.RecomendacionId);
                    table.ForeignKey(
                        name: "FK_Recomendaciones_Canciones_CancionId",
                        column: x => x.CancionId,
                        principalTable: "Canciones",
                        principalColumn: "CancionId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Recomendaciones_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "UsuarioId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ListaCancionDetalles",
                columns: table => new
                {
                    ListaCancionDetalleId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ListaCancionesId = table.Column<int>(type: "integer", nullable: false),
                    CancionId = table.Column<int>(type: "integer", nullable: false),
                    Orden = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ListaCancionDetalles", x => x.ListaCancionDetalleId);
                    table.ForeignKey(
                        name: "FK_ListaCancionDetalles_Canciones_CancionId",
                        column: x => x.CancionId,
                        principalTable: "Canciones",
                        principalColumn: "CancionId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ListaCancionDetalles_ListaCanciones_ListaCancionesId",
                        column: x => x.ListaCancionesId,
                        principalTable: "ListaCanciones",
                        principalColumn: "ListaCancionesId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Canciones_CreadoPorUsuarioId",
                table: "Canciones",
                column: "CreadoPorUsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_ListaCancionDetalles_CancionId",
                table: "ListaCancionDetalles",
                column: "CancionId");

            migrationBuilder.CreateIndex(
                name: "IX_ListaCancionDetalles_ListaCancionesId",
                table: "ListaCancionDetalles",
                column: "ListaCancionesId");

            migrationBuilder.CreateIndex(
                name: "IX_ListaCanciones_DirectorId",
                table: "ListaCanciones",
                column: "DirectorId");

            migrationBuilder.CreateIndex(
                name: "IX_NotaMusicales_CancionId",
                table: "NotaMusicales",
                column: "CancionId");

            migrationBuilder.CreateIndex(
                name: "IX_NotaMusicales_EditadoPorUsuarioId",
                table: "NotaMusicales",
                column: "EditadoPorUsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Recomendaciones_CancionId",
                table: "Recomendaciones",
                column: "CancionId");

            migrationBuilder.CreateIndex(
                name: "IX_Recomendaciones_UsuarioId",
                table: "Recomendaciones",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_RegistroActividad_UsuarioId",
                table: "RegistroActividad",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudDirectores_RespuestaPorUsuarioId",
                table: "SolicitudDirectores",
                column: "RespuestaPorUsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudDirectores_UsuarioId",
                table: "SolicitudDirectores",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_RolId",
                table: "Usuarios",
                column: "RolId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ListaCancionDetalles");

            migrationBuilder.DropTable(
                name: "NotaMusicales");

            migrationBuilder.DropTable(
                name: "Recomendaciones");

            migrationBuilder.DropTable(
                name: "RegistroActividad");

            migrationBuilder.DropTable(
                name: "SolicitudDirectores");

            migrationBuilder.DropTable(
                name: "ListaCanciones");

            migrationBuilder.DropTable(
                name: "Canciones");

            migrationBuilder.DropTable(
                name: "Usuarios");

            migrationBuilder.DropTable(
                name: "Roles");
        }
    }
}
