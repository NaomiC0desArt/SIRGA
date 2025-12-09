using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SIRGA.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Cambios_Tabla_Asignatura : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "Asignaturas",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "Codigo",
                table: "Asignaturas",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TipoAsignatura",
                table: "Asignaturas",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Codigo",
                table: "Asignaturas");

            migrationBuilder.DropColumn(
                name: "TipoAsignatura",
                table: "Asignaturas");

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "Asignaturas",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);
        }
    }
}
