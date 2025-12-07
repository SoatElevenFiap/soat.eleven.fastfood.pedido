using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Soat.Eleven.FastFood.Infra.Migrations
{
    /// <inheritdoc />
    public partial class AlteracoesUsuarioCliente : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "Telefone",
                table: "Clientes");

            migrationBuilder.RenameColumn(
                name: "SenhaHash",
                table: "UsuariosSistema",
                newName: "Telefone");

            migrationBuilder.RenameColumn(
                name: "Nome",
                table: "Clientes",
                newName: "Cpf");

            migrationBuilder.AddColumn<DateTime>(
                name: "ModificadoEm",
                table: "UsuariosSistema",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Senha",
                table: "UsuariosSistema",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<Guid>(
                name: "UsuarioId",
                table: "Clientes",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DataDeNascimento",
                table: "Clientes",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "ModificadoEm",
                table: "Clientes",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "UsuarioId1",
                table: "Clientes",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_UsuarioId1",
                table: "Clientes",
                column: "UsuarioId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Clientes_UsuariosSistema_UsuarioId1",
                table: "Clientes",
                column: "UsuarioId1",
                principalTable: "UsuariosSistema",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Clientes_UsuariosSistema_UsuarioId1",
                table: "Clientes");

            migrationBuilder.DropIndex(
                name: "IX_Clientes_UsuarioId1",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "ModificadoEm",
                table: "UsuariosSistema");

            migrationBuilder.DropColumn(
                name: "Senha",
                table: "UsuariosSistema");

            migrationBuilder.DropColumn(
                name: "DataDeNascimento",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "ModificadoEm",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "UsuarioId1",
                table: "Clientes");

            migrationBuilder.RenameColumn(
                name: "Telefone",
                table: "UsuariosSistema",
                newName: "SenhaHash");

            migrationBuilder.RenameColumn(
                name: "Cpf",
                table: "Clientes",
                newName: "Nome");

            migrationBuilder.AlterColumn<Guid>(
                name: "UsuarioId",
                table: "Clientes",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Clientes",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Telefone",
                table: "Clientes",
                type: "text",
                nullable: true);
        }
    }
}
