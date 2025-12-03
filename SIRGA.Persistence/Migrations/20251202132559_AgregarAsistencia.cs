using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SIRGA.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AgregarAsistencia : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Descripcion",
                table: "Asignaturas",
                type: "nvarchar(125)",
                maxLength: 125,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateTable(
                name: "Asistencias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HoraRegistro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Observaciones = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    RequiereJustificacion = table.Column<bool>(type: "bit", nullable: false),
                    Justificacion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    FechaJustificacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UsuarioJustificacionId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IdEstudiante = table.Column<int>(type: "int", nullable: false),
                    IdClaseProgramada = table.Column<int>(type: "int", nullable: false),
                    IdProfesor = table.Column<int>(type: "int", nullable: false),
                    RegistradoPorId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UltimaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModificadoPorId = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Asistencias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Asistencias_ClasesProgramadas_IdClaseProgramada",
                        column: x => x.IdClaseProgramada,
                        principalTable: "ClasesProgramadas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Asistencias_Estudiantes_IdEstudiante",
                        column: x => x.IdEstudiante,
                        principalTable: "Estudiantes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Asistencias_Profesores_IdProfesor",
                        column: x => x.IdProfesor,
                        principalTable: "Profesores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Asistencia_ClaseProgramada",
                table: "Asistencias",
                column: "IdClaseProgramada");

            migrationBuilder.CreateIndex(
                name: "IX_Asistencia_Estudiante",
                table: "Asistencias",
                column: "IdEstudiante");

            migrationBuilder.CreateIndex(
                name: "IX_Asistencia_Estudiante_Clase_Fecha",
                table: "Asistencias",
                columns: new[] { "IdEstudiante", "IdClaseProgramada", "Fecha" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Asistencia_Fecha",
                table: "Asistencias",
                column: "Fecha");

            migrationBuilder.CreateIndex(
                name: "IX_Asistencia_RequiereJustificacion",
                table: "Asistencias",
                columns: new[] { "RequiereJustificacion", "Justificacion" });

            migrationBuilder.CreateIndex(
                name: "IX_Asistencias_IdProfesor",
                table: "Asistencias",
                column: "IdProfesor");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Asistencias");

            migrationBuilder.AlterColumn<string>(
                name: "Descripcion",
                table: "Asignaturas",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(125)",
                oldMaxLength: 125);
        }
    }
}
