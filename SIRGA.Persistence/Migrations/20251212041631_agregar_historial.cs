using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SIRGA.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class agregar_historial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NombreUsuarioModificador",
                table: "HistorialCalificaciones");

            migrationBuilder.DropColumn(
                name: "TotalAnterior",
                table: "HistorialCalificaciones");

            migrationBuilder.DropColumn(
                name: "TotalNuevo",
                table: "HistorialCalificaciones");

            migrationBuilder.RenameColumn(
                name: "UsuarioModificadorId",
                table: "HistorialCalificaciones",
                newName: "UsuarioId");

            migrationBuilder.RenameColumn(
                name: "RolUsuarioModificador",
                table: "HistorialCalificaciones",
                newName: "UsuarioRol");

            migrationBuilder.RenameColumn(
                name: "MotivoModificacion",
                table: "HistorialCalificaciones",
                newName: "MotivoEdicion");

            migrationBuilder.RenameIndex(
                name: "IX_HistorialCalificacion_Fecha",
                table: "HistorialCalificaciones",
                newName: "IX_HistorialCalificaciones_FechaModificacion");

            migrationBuilder.RenameIndex(
                name: "IX_HistorialCalificacion_Calificacion",
                table: "HistorialCalificaciones",
                newName: "IX_HistorialCalificaciones_IdCalificacion");

            migrationBuilder.AddColumn<string>(
                name: "Estado",
                table: "Inscripciones",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "NumeroPeriodo",
                table: "HistorialCalificaciones",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "UsuarioNombre",
                table: "HistorialCalificaciones",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Estado",
                table: "Inscripciones");

            migrationBuilder.DropColumn(
                name: "NumeroPeriodo",
                table: "HistorialCalificaciones");

            migrationBuilder.DropColumn(
                name: "UsuarioNombre",
                table: "HistorialCalificaciones");

            migrationBuilder.RenameColumn(
                name: "UsuarioRol",
                table: "HistorialCalificaciones",
                newName: "RolUsuarioModificador");

            migrationBuilder.RenameColumn(
                name: "UsuarioId",
                table: "HistorialCalificaciones",
                newName: "UsuarioModificadorId");

            migrationBuilder.RenameColumn(
                name: "MotivoEdicion",
                table: "HistorialCalificaciones",
                newName: "MotivoModificacion");

            migrationBuilder.RenameIndex(
                name: "IX_HistorialCalificaciones_IdCalificacion",
                table: "HistorialCalificaciones",
                newName: "IX_HistorialCalificacion_Calificacion");

            migrationBuilder.RenameIndex(
                name: "IX_HistorialCalificaciones_FechaModificacion",
                table: "HistorialCalificaciones",
                newName: "IX_HistorialCalificacion_Fecha");

            migrationBuilder.AddColumn<string>(
                name: "NombreUsuarioModificador",
                table: "HistorialCalificaciones",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "TotalAnterior",
                table: "HistorialCalificaciones",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalNuevo",
                table: "HistorialCalificaciones",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
