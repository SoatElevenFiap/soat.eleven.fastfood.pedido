using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Soat.Eleven.FastFood.Infra.Migrations
{
    /// <inheritdoc />
    public partial class SyncMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_DescontosProduto",
                table: "DescontosProduto");

            migrationBuilder.AddColumn<string>(
                name: "CpfCliente",
                table: "TokensAtendimento",
                type: "text",
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CriadoEm",
                table: "DescontosProduto",
                type: "timestamp",
                nullable: false,
                defaultValueSql: "NOW()",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "DescontosProduto",
                type: "uuid",
                nullable: false,
                defaultValueSql: "gen_random_uuid()");

            migrationBuilder.AddColumn<DateTime>(
                name: "ModificadoEm",
                table: "DescontosProduto",
                type: "timestamp",
                nullable: false,
                defaultValueSql: "NOW()");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DescontosProduto",
                table: "DescontosProduto",
                column: "Id");

            // Removed duplicate InsertData for Usuarios as it was already inserted in previous migration
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_DescontosProduto",
                table: "DescontosProduto");

            // Removed DeleteData for Usuarios as it was not inserted in this migration

            migrationBuilder.DropColumn(
                name: "CpfCliente",
                table: "TokensAtendimento");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "DescontosProduto");

            migrationBuilder.DropColumn(
                name: "ModificadoEm",
                table: "DescontosProduto");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CriadoEm",
                table: "DescontosProduto",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp",
                oldDefaultValueSql: "NOW()");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DescontosProduto",
                table: "DescontosProduto",
                column: "DescontoProdutoId");
        }
    }
}
