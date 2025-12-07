using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Soat.Eleven.FastFood.Infra.Migrations
{
    /// <inheritdoc />
    public partial class StatusUsuario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Usuarios",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Usuarios");
        }
    }
}
