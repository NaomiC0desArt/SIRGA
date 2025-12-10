using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SIRGA.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Agregar_Entidades_Calificacion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Calificaciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdEstudiante = table.Column<int>(type: "int", nullable: false),
                    IdAsignatura = table.Column<int>(type: "int", nullable: false),
                    IdCursoAcademico = table.Column<int>(type: "int", nullable: false),
                    IdPeriodo = table.Column<int>(type: "int", nullable: false),
                    IdProfesor = table.Column<int>(type: "int", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Publicada = table.Column<bool>(type: "bit", nullable: false),
                    FechaPublicacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaUltimaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Observaciones = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Calificaciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Calificaciones_Asignaturas_IdAsignatura",
                        column: x => x.IdAsignatura,
                        principalTable: "Asignaturas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Calificaciones_CursosAcademicos_IdCursoAcademico",
                        column: x => x.IdCursoAcademico,
                        principalTable: "CursosAcademicos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Calificaciones_Estudiantes_IdEstudiante",
                        column: x => x.IdEstudiante,
                        principalTable: "Estudiantes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Calificaciones_Periodos_IdPeriodo",
                        column: x => x.IdPeriodo,
                        principalTable: "Periodos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Calificaciones_Profesores_IdProfesor",
                        column: x => x.IdProfesor,
                        principalTable: "Profesores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ComponentesCalificacion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TipoAsignatura = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ValorMaximo = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Orden = table.Column<int>(type: "int", nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComponentesCalificacion", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HistorialCalificaciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdCalificacion = table.Column<int>(type: "int", nullable: false),
                    UsuarioModificadorId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    NombreUsuarioModificador = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    RolUsuarioModificador = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ValoresAnteriores = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ValoresNuevos = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TotalAnterior = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalNuevo = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MotivoModificacion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistorialCalificaciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HistorialCalificaciones_Calificaciones_IdCalificacion",
                        column: x => x.IdCalificacion,
                        principalTable: "Calificaciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CalificacionDetalles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdCalificacion = table.Column<int>(type: "int", nullable: false),
                    IdComponenteCalificacion = table.Column<int>(type: "int", nullable: false),
                    Valor = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CalificacionDetalles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CalificacionDetalles_Calificaciones_IdCalificacion",
                        column: x => x.IdCalificacion,
                        principalTable: "Calificaciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CalificacionDetalles_ComponentesCalificacion_IdComponenteCalificacion",
                        column: x => x.IdComponenteCalificacion,
                        principalTable: "ComponentesCalificacion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CalificacionDetalle_Calificacion",
                table: "CalificacionDetalles",
                column: "IdCalificacion");

            migrationBuilder.CreateIndex(
                name: "IX_CalificacionDetalle_Unique",
                table: "CalificacionDetalles",
                columns: new[] { "IdCalificacion", "IdComponenteCalificacion" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CalificacionDetalles_IdComponenteCalificacion",
                table: "CalificacionDetalles",
                column: "IdComponenteCalificacion");

            migrationBuilder.CreateIndex(
                name: "IX_Calificacion_Curso_Asignatura_Periodo",
                table: "Calificaciones",
                columns: new[] { "IdCursoAcademico", "IdAsignatura", "IdPeriodo" });

            migrationBuilder.CreateIndex(
                name: "IX_Calificacion_Profesor",
                table: "Calificaciones",
                column: "IdProfesor");

            migrationBuilder.CreateIndex(
                name: "IX_Calificacion_Publicada",
                table: "Calificaciones",
                column: "Publicada");

            migrationBuilder.CreateIndex(
                name: "IX_Calificacion_Unique",
                table: "Calificaciones",
                columns: new[] { "IdEstudiante", "IdAsignatura", "IdPeriodo" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Calificaciones_IdAsignatura",
                table: "Calificaciones",
                column: "IdAsignatura");

            migrationBuilder.CreateIndex(
                name: "IX_Calificaciones_IdPeriodo",
                table: "Calificaciones",
                column: "IdPeriodo");

            migrationBuilder.CreateIndex(
                name: "IX_ComponenteCalificacion_Activo",
                table: "ComponentesCalificacion",
                column: "Activo");

            migrationBuilder.CreateIndex(
                name: "IX_ComponenteCalificacion_Tipo_Nombre",
                table: "ComponentesCalificacion",
                columns: new[] { "TipoAsignatura", "Nombre" });

            migrationBuilder.CreateIndex(
                name: "IX_ComponenteCalificacion_Tipo_Orden",
                table: "ComponentesCalificacion",
                columns: new[] { "TipoAsignatura", "Orden" });

            migrationBuilder.CreateIndex(
                name: "IX_HistorialCalificacion_Calificacion",
                table: "HistorialCalificaciones",
                column: "IdCalificacion");

            migrationBuilder.CreateIndex(
                name: "IX_HistorialCalificacion_Fecha",
                table: "HistorialCalificaciones",
                column: "FechaModificacion");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CalificacionDetalles");

            migrationBuilder.DropTable(
                name: "HistorialCalificaciones");

            migrationBuilder.DropTable(
                name: "ComponentesCalificacion");

            migrationBuilder.DropTable(
                name: "Calificaciones");
        }
    }
}
