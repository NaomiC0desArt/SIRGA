using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SIRGA.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Agregar_ActividadesCurriculares : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ActividadesExtracurriculares",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Categoria = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Requisitos = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    HoraInicio = table.Column<TimeSpan>(type: "time", nullable: false),
                    HoraFin = table.Column<TimeSpan>(type: "time", nullable: false),
                    DiaSemana = table.Column<int>(type: "int", nullable: false),
                    Ubicacion = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ColorTarjeta = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: false),
                    RutaImagen = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    EstaActiva = table.Column<bool>(type: "bit", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdProfesorEncargado = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActividadesExtracurriculares", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ActividadesExtracurriculares_Profesores_IdProfesorEncargado",
                        column: x => x.IdProfesorEncargado,
                        principalTable: "Profesores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "InscripcionesActividades",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdEstudiante = table.Column<int>(type: "int", nullable: false),
                    IdActividad = table.Column<int>(type: "int", nullable: false),
                    FechaInscripcion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EstaActiva = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InscripcionesActividades", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InscripcionesActividades_ActividadesExtracurriculares_IdActividad",
                        column: x => x.IdActividad,
                        principalTable: "ActividadesExtracurriculares",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InscripcionesActividades_Estudiantes_IdEstudiante",
                        column: x => x.IdEstudiante,
                        principalTable: "Estudiantes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ActividadesExtracurriculares_IdProfesorEncargado",
                table: "ActividadesExtracurriculares",
                column: "IdProfesorEncargado");

            migrationBuilder.CreateIndex(
                name: "IX_InscripcionesActividades_IdActividad",
                table: "InscripcionesActividades",
                column: "IdActividad");

            migrationBuilder.CreateIndex(
                name: "IX_InscripcionesActividades_IdEstudiante",
                table: "InscripcionesActividades",
                column: "IdEstudiante");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InscripcionesActividades");

            migrationBuilder.DropTable(
                name: "ActividadesExtracurriculares");
        }
    }
}
