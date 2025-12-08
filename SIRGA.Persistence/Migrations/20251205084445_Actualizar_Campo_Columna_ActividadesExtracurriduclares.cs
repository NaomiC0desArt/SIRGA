using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SIRGA.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Actualizar_Campo_Columna_ActividadesExtracurriduclares : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Categoria",
                table: "ActividadesExtracurriculares",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Categoria",
                table: "ActividadesExtracurriculares",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(30)",
                oldMaxLength: 30);
        }
    }
}
