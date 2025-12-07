using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Soat.Eleven.FastFood.Infra.Migrations
{
    /// <inheritdoc />
    public partial class AtualizacaoTabelasPedido : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pedidos_Clientes_ClienteId",
                table: "Pedidos");

            migrationBuilder.DropTable(
                name: "Comandas");

            migrationBuilder.DropTable(
                name: "LogsPedido");

            migrationBuilder.DropColumn(
                name: "DataCriacao",
                table: "Pedidos");

            migrationBuilder.DropColumn(
                name: "PrecoComDesconto",
                table: "ItensPedido");

            migrationBuilder.AlterColumn<Guid>(
                name: "ClienteId",
                table: "Pedidos",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Pedidos",
                type: "uuid",
                nullable: false,
                defaultValueSql: "gen_random_uuid()",
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<DateTime>(
                name: "CriadoEm",
                table: "Pedidos",
                type: "timestamp",
                nullable: false,
                defaultValueSql: "NOW()");

            migrationBuilder.AddColumn<DateTime>(
                name: "ModificadoEm",
                table: "Pedidos",
                type: "timestamp",
                nullable: false,
                defaultValueSql: "NOW()");

            migrationBuilder.AddColumn<Guid>(
                name: "TokenAtendimentoId",
                table: "Pedidos",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "ItensPedido",
                type: "uuid",
                nullable: false,
                defaultValueSql: "gen_random_uuid()",
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<DateTime>(
                name: "CriadoEm",
                table: "ItensPedido",
                type: "timestamp",
                nullable: false,
                defaultValueSql: "NOW()");

            migrationBuilder.AddColumn<decimal>(
                name: "DescontoUnitario",
                table: "ItensPedido",
                type: "numeric(10,2)",
                precision: 10,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModificadoEm",
                table: "ItensPedido",
                type: "timestamp",
                nullable: false,
                defaultValueSql: "NOW()");

            migrationBuilder.CreateTable(
                name: "PagamentosPedido",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    PedidoId = table.Column<Guid>(type: "uuid", nullable: false),
                    Tipo = table.Column<string>(type: "text", nullable: false),
                    Valor = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    Troco = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    Autorizacao = table.Column<string>(type: "text", nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "timestamp", nullable: false, defaultValueSql: "NOW()"),
                    ModificadoEm = table.Column<DateTime>(type: "timestamp", nullable: false, defaultValueSql: "NOW()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PagamentosPedido", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PagamentosPedido_Pedidos_PedidoId",
                        column: x => x.PedidoId,
                        principalTable: "Pedidos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PagamentosPedido_PedidoId",
                table: "PagamentosPedido",
                column: "PedidoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Pedidos_Clientes_ClienteId",
                table: "Pedidos",
                column: "ClienteId",
                principalTable: "Clientes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pedidos_Clientes_ClienteId",
                table: "Pedidos");

            migrationBuilder.DropTable(
                name: "PagamentosPedido");

            migrationBuilder.DropColumn(
                name: "CriadoEm",
                table: "Pedidos");

            migrationBuilder.DropColumn(
                name: "ModificadoEm",
                table: "Pedidos");

            migrationBuilder.DropColumn(
                name: "TokenAtendimentoId",
                table: "Pedidos");

            migrationBuilder.DropColumn(
                name: "CriadoEm",
                table: "ItensPedido");

            migrationBuilder.DropColumn(
                name: "DescontoUnitario",
                table: "ItensPedido");

            migrationBuilder.DropColumn(
                name: "ModificadoEm",
                table: "ItensPedido");

            migrationBuilder.AlterColumn<Guid>(
                name: "ClienteId",
                table: "Pedidos",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Pedidos",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValueSql: "gen_random_uuid()");

            migrationBuilder.AddColumn<DateTime>(
                name: "DataCriacao",
                table: "Pedidos",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "ItensPedido",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValueSql: "gen_random_uuid()");

            migrationBuilder.AddColumn<decimal>(
                name: "PrecoComDesconto",
                table: "ItensPedido",
                type: "numeric(10,2)",
                precision: 10,
                scale: 2,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Comandas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ClienteId = table.Column<Guid>(type: "uuid", nullable: false),
                    CriadaEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comandas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Comandas_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LogsPedido",
                columns: table => new
                {
                    PedidoId = table.Column<Guid>(type: "uuid", nullable: false),
                    UsuarioSistemaId = table.Column<Guid>(type: "uuid", nullable: true),
                    AlteradoPor = table.Column<Guid>(type: "uuid", nullable: true),
                    DataAlteracao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IP = table.Column<string>(type: "text", nullable: true),
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StatusAtual = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.ForeignKey(
                        name: "FK_LogsPedido_Pedidos_PedidoId",
                        column: x => x.PedidoId,
                        principalTable: "Pedidos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LogsPedido_Usuarios_UsuarioSistemaId",
                        column: x => x.UsuarioSistemaId,
                        principalTable: "Usuarios",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Comandas_ClienteId",
                table: "Comandas",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_LogsPedido_PedidoId",
                table: "LogsPedido",
                column: "PedidoId");

            migrationBuilder.CreateIndex(
                name: "IX_LogsPedido_UsuarioSistemaId",
                table: "LogsPedido",
                column: "UsuarioSistemaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Pedidos_Clientes_ClienteId",
                table: "Pedidos",
                column: "ClienteId",
                principalTable: "Clientes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
