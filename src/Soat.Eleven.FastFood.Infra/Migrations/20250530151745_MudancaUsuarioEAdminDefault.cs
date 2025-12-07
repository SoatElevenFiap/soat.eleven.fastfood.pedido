using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Soat.Eleven.FastFood.Infra.Migrations
{
    /// <inheritdoc />
    public partial class MudancaUsuarioEAdminDefault : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddPrimaryKey(
                name: "PK_TokensAtendimento",
                table: "TokensAtendimento",
                column: "TokenId");

            migrationBuilder.InsertData(
                table: "Usuarios",
                columns: new[] { "Id", "Email", "Nome", "Perfil", "Senha", "Telefone" },
                values: new object[] { new Guid("3b31ada8-b56a-466d-a1a6-75fe92a36552"), "sistema@fastfood.com", "Sistema Fast Food", "Administrador", "3+wuaNtvoRoxLxP7qPmYrg==", "11985203641" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_TokensAtendimento",
                table: "TokensAtendimento");

            migrationBuilder.DeleteData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: new Guid("3b31ada8-b56a-466d-a1a6-75fe92a36552"));
        }
    }
}
