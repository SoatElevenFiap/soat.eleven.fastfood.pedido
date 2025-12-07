using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Soat.Eleven.FastFood.Infra.Migrations
{
    /// <inheritdoc />
    public partial class CampoActiveCategorias : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Ativo",
                table: "CategoriasProduto",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Ativo",
                table: "CategoriasProduto");
        }
    }
}
