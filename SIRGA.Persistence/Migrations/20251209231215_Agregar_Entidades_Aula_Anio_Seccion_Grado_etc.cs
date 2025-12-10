using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SIRGA.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Agregar_Entidades_Aula_Anio_Seccion_Grado_etc : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Inscripciones_IdEstudiante",
                table: "Inscripciones");

            migrationBuilder.DropIndex(
                name: "IX_CursosAcademicos_IdGrado",
                table: "CursosAcademicos");

            migrationBuilder.DropColumn(
                name: "Section",
                table: "Grados");

            migrationBuilder.DropColumn(
                name: "SchoolYear",
                table: "CursosAcademicos");

            migrationBuilder.RenameColumn(
                name: "StudentsLimit",
                table: "Grados",
                newName: "Nivel");

            migrationBuilder.AlterColumn<string>(
                name: "GradeName",
                table: "Grados",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "IdAnioEscolar",
                table: "CursosAcademicos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "IdAulaBase",
                table: "CursosAcademicos",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IdSeccion",
                table: "CursosAcademicos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "AniosEscolares",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AnioInicio = table.Column<int>(type: "int", nullable: false),
                    AnioFin = table.Column<int>(type: "int", nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AniosEscolares", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Aulas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Codigo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Tipo = table.Column<int>(type: "int", nullable: false),
                    Capacidad = table.Column<int>(type: "int", nullable: false),
                    EstaDisponible = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Aulas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Secciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    CapacidadMaxima = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Secciones", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Periodos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Numero = table.Column<int>(type: "int", nullable: false),
                    FechaInicio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaFin = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AnioEscolarId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Periodos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Periodos_AniosEscolares_AnioEscolarId",
                        column: x => x.AnioEscolarId,
                        principalTable: "AniosEscolares",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Inscripcion_Unique",
                table: "Inscripciones",
                columns: new[] { "IdEstudiante", "IdCursoAcademico" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CursoAcademico_Unique",
                table: "CursosAcademicos",
                columns: new[] { "IdGrado", "IdSeccion", "IdAnioEscolar" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CursosAcademicos_IdAnioEscolar",
                table: "CursosAcademicos",
                column: "IdAnioEscolar");

            migrationBuilder.CreateIndex(
                name: "IX_CursosAcademicos_IdAulaBase",
                table: "CursosAcademicos",
                column: "IdAulaBase");

            migrationBuilder.CreateIndex(
                name: "IX_CursosAcademicos_IdSeccion",
                table: "CursosAcademicos",
                column: "IdSeccion");

            migrationBuilder.CreateIndex(
                name: "IX_Periodos_AnioEscolarId",
                table: "Periodos",
                column: "AnioEscolarId");

            migrationBuilder.CreateIndex(
                name: "IX_Secciones_Nombre",
                table: "Secciones",
                column: "Nombre",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_CursosAcademicos_AniosEscolares_IdAnioEscolar",
                table: "CursosAcademicos",
                column: "IdAnioEscolar",
                principalTable: "AniosEscolares",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CursosAcademicos_Aulas_IdAulaBase",
                table: "CursosAcademicos",
                column: "IdAulaBase",
                principalTable: "Aulas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CursosAcademicos_Secciones_IdSeccion",
                table: "CursosAcademicos",
                column: "IdSeccion",
                principalTable: "Secciones",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CursosAcademicos_AniosEscolares_IdAnioEscolar",
                table: "CursosAcademicos");

            migrationBuilder.DropForeignKey(
                name: "FK_CursosAcademicos_Aulas_IdAulaBase",
                table: "CursosAcademicos");

            migrationBuilder.DropForeignKey(
                name: "FK_CursosAcademicos_Secciones_IdSeccion",
                table: "CursosAcademicos");

            migrationBuilder.DropTable(
                name: "Aulas");

            migrationBuilder.DropTable(
                name: "Periodos");

            migrationBuilder.DropTable(
                name: "Secciones");

            migrationBuilder.DropTable(
                name: "AniosEscolares");

            migrationBuilder.DropIndex(
                name: "IX_Inscripcion_Unique",
                table: "Inscripciones");

            migrationBuilder.DropIndex(
                name: "IX_CursoAcademico_Unique",
                table: "CursosAcademicos");

            migrationBuilder.DropIndex(
                name: "IX_CursosAcademicos_IdAnioEscolar",
                table: "CursosAcademicos");

            migrationBuilder.DropIndex(
                name: "IX_CursosAcademicos_IdAulaBase",
                table: "CursosAcademicos");

            migrationBuilder.DropIndex(
                name: "IX_CursosAcademicos_IdSeccion",
                table: "CursosAcademicos");

            migrationBuilder.DropColumn(
                name: "IdAnioEscolar",
                table: "CursosAcademicos");

            migrationBuilder.DropColumn(
                name: "IdAulaBase",
                table: "CursosAcademicos");

            migrationBuilder.DropColumn(
                name: "IdSeccion",
                table: "CursosAcademicos");

            migrationBuilder.RenameColumn(
                name: "Nivel",
                table: "Grados",
                newName: "StudentsLimit");

            migrationBuilder.AlterColumn<string>(
                name: "GradeName",
                table: "Grados",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AddColumn<string>(
                name: "Section",
                table: "Grados",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SchoolYear",
                table: "CursosAcademicos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Inscripciones_IdEstudiante",
                table: "Inscripciones",
                column: "IdEstudiante");

            migrationBuilder.CreateIndex(
                name: "IX_CursosAcademicos_IdGrado",
                table: "CursosAcademicos",
                column: "IdGrado");
        }
    }
}
