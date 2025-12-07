using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Soat.Eleven.FastFood.Infra.Migrations
{
    /// <inheritdoc />
    public partial class MudarNomeTabelaUsuarioEPerfilEnum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Clientes_UsuariosSistema_UsuarioId",
                table: "Clientes");

            migrationBuilder.DropForeignKey(
                name: "FK_LogsPedido_UsuariosSistema_UsuarioSistemaId",
                table: "LogsPedido");

            migrationBuilder.DropForeignKey(
                name: "FK_UsuariosSistema_Perfis_PerfilId",
                table: "UsuariosSistema");

            migrationBuilder.DropTable(
                name: "Perfis");

            migrationBuilder.DropIndex(
                name: "IX_Clientes_UsuarioId",
                table: "Clientes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UsuariosSistema",
                table: "UsuariosSistema");

            migrationBuilder.DropIndex(
                name: "IX_UsuariosSistema_PerfilId",
                table: "UsuariosSistema");

            migrationBuilder.RenameTable(
                name: "UsuariosSistema",
                newName: "Usuarios");

            migrationBuilder.RenameColumn(
                name: "PerfilId",
                table: "Usuarios",
                newName: "Perfil");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Clientes",
                type: "uuid",
                nullable: false,
                defaultValueSql: "gen_random_uuid()",
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Usuarios",
                type: "uuid",
                nullable: false,
                defaultValueSql: "gen_random_uuid()",
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Usuarios",
                table: "Usuarios",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_UsuarioId",
                table: "Clientes",
                column: "UsuarioId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Clientes_Usuarios_UsuarioId",
                table: "Clientes",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_LogsPedido_Usuarios_UsuarioSistemaId",
                table: "LogsPedido",
                column: "UsuarioSistemaId",
                principalTable: "Usuarios",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Clientes_Usuarios_UsuarioId",
                table: "Clientes");

            migrationBuilder.DropForeignKey(
                name: "FK_LogsPedido_Usuarios_UsuarioSistemaId",
                table: "LogsPedido");

            migrationBuilder.DropIndex(
                name: "IX_Clientes_UsuarioId",
                table: "Clientes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Usuarios",
                table: "Usuarios");

            migrationBuilder.RenameTable(
                name: "Usuarios",
                newName: "UsuariosSistema");

            migrationBuilder.RenameColumn(
                name: "Perfil",
                table: "UsuariosSistema",
                newName: "PerfilId");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Clientes",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValueSql: "gen_random_uuid()");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "UsuariosSistema",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValueSql: "gen_random_uuid()");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UsuariosSistema",
                table: "UsuariosSistema",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Perfis",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nome = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Perfis", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Perfis",
                columns: new[] { "Id", "Nome" },
                values: new object[,]
                {
                    { 1, "Cliente" },
                    { 2, "Administrador" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_UsuarioId",
                table: "Clientes",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_UsuariosSistema_PerfilId",
                table: "UsuariosSistema",
                column: "PerfilId");

            migrationBuilder.AddForeignKey(
                name: "FK_Clientes_UsuariosSistema_UsuarioId",
                table: "Clientes",
                column: "UsuarioId",
                principalTable: "UsuariosSistema",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_LogsPedido_UsuariosSistema_UsuarioSistemaId",
                table: "LogsPedido",
                column: "UsuarioSistemaId",
                principalTable: "UsuariosSistema",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UsuariosSistema_Perfis_PerfilId",
                table: "UsuariosSistema",
                column: "PerfilId",
                principalTable: "Perfis",
                principalColumn: "Id");
        }
    }
}
