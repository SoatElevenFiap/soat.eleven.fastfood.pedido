using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Soat.Eleven.FastFood.Adapter.Infra.DataSources;
using Soat.Eleven.FastFood.Application.Controllers;
using Soat.Eleven.FastFood.Core.DTOs.Pedidos;
using Soat.Eleven.FastFood.Core.Enums;
using Soat.Eleven.FastFood.Infra.Data;
using Xunit;

namespace Soat.Eleven.FastFood.Tests.Application;

/// <summary>
/// Testes de integração para PedidoController usando banco in-memory
/// </summary>
public class PedidoControllerIntegrationTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly PedidoDataSource _dataSource;
    private readonly PedidoController _controller;

    public PedidoControllerIntegrationTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        _dataSource = new PedidoDataSource(_context);
        _controller = new PedidoController(_dataSource);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    #region CriarPedido Integration Tests

    [Fact]
    public async Task CriarPedido_DevePersistirNoBanco()
    {
        // Arrange
        var inputDto = CriarPedidoInputDtoValido();

        // Act
        var resultado = await _controller.CriarPedido(inputDto);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Id.Should().NotBe(Guid.Empty);
        
        var pedidoNoBanco = await _context.Pedidos.FindAsync(resultado.Id);
        pedidoNoBanco.Should().NotBeNull();
    }

    [Fact]
    public async Task CriarPedido_DeveGerarSenha()
    {
        // Arrange
        var inputDto = CriarPedidoInputDtoValido();

        // Act
        var resultado = await _controller.CriarPedido(inputDto);

        // Assert
        resultado.SenhaPedido.Should().NotBeNullOrEmpty();
        resultado.SenhaPedido.Should().HaveLength(10);
    }

    [Fact]
    public async Task CriarPedido_DeveTerStatusPendente()
    {
        // Arrange
        var inputDto = CriarPedidoInputDtoValido();

        // Act
        var resultado = await _controller.CriarPedido(inputDto);

        // Assert
        resultado.Status.Should().Be(StatusPedido.Pendente);
    }

    #endregion

    #region ListarPedidos Integration Tests

    [Fact]
    public async Task ListarPedidos_DeveRetornarPedidosCriados()
    {
        // Arrange
        await _controller.CriarPedido(CriarPedidoInputDtoValido());
        await _controller.CriarPedido(CriarPedidoInputDtoValido());

        // Act
        var resultado = await _controller.ListarPedidos();

        // Assert
        resultado.Should().HaveCount(2);
    }

    [Fact]
    public async Task ListarPedidos_NaoDeveRetornarFinalizados()
    {
        // Arrange
        var pedido = await _controller.CriarPedido(CriarPedidoInputDtoValido());
        
        // Simular finalização via DataSource
        await _dataSource.AtualizarStatusAsync(pedido.Id, StatusPedido.Recebido);
        await _controller.IniciarPreparacaoPedido(pedido.Id);
        await _controller.FinalizarPreparacao(pedido.Id);
        await _controller.FinalizarPedido(pedido.Id);

        // Act
        var resultado = await _controller.ListarPedidos();

        // Assert
        resultado.Should().BeEmpty();
    }

    #endregion

    #region ObterPedidoPorId Integration Tests

    [Fact]
    public async Task ObterPedidoPorId_DeveRetornarPedidoCorreto()
    {
        // Arrange
        var pedidoCriado = await _controller.CriarPedido(CriarPedidoInputDtoValido());

        // Act
        var resultado = await _controller.ObterPedidoPorId(pedidoCriado.Id);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Id.Should().Be(pedidoCriado.Id);
        resultado.SenhaPedido.Should().Be(pedidoCriado.SenhaPedido);
    }

    #endregion

    #region Workflow Integration Tests

    [Fact]
    public async Task FluxoCompleto_DevePassarPorTodosOsStatus()
    {
        // Arrange
        var inputDto = CriarPedidoInputDtoValido();
        var pedido = await _controller.CriarPedido(inputDto);
        
        // Simular pagamento aprovado
        await _dataSource.AtualizarStatusAsync(pedido.Id, StatusPedido.Recebido);

        // Act - Iniciar preparação
        await _controller.IniciarPreparacaoPedido(pedido.Id);
        var aposIniciarPreparacao = await _controller.ObterPedidoPorId(pedido.Id);
        
        // Assert
        aposIniciarPreparacao.Status.Should().Be(StatusPedido.EmPreparacao);

        // Act - Finalizar preparação
        await _controller.FinalizarPreparacao(pedido.Id);
        var aposFinalPreparacao = await _controller.ObterPedidoPorId(pedido.Id);
        
        // Assert
        aposFinalPreparacao.Status.Should().Be(StatusPedido.Pronto);

        // Act - Finalizar pedido
        await _controller.FinalizarPedido(pedido.Id);
        var aposFinalizarPedido = await _controller.ObterPedidoPorId(pedido.Id);
        
        // Assert
        aposFinalizarPedido.Status.Should().Be(StatusPedido.Finalizado);
    }

    [Fact]
    public async Task CancelarPedido_DeveAlterarStatusParaCancelado()
    {
        // Arrange
        var inputDto = CriarPedidoInputDtoValido();
        var pedido = await _controller.CriarPedido(inputDto);

        // Act
        await _controller.CancelarPedido(pedido.Id);
        var resultado = await _controller.ObterPedidoPorId(pedido.Id);

        // Assert
        resultado.Status.Should().Be(StatusPedido.Cancelado);
    }

    #endregion

    #region AtualizarPedido Integration Tests

    [Fact]
    public async Task AtualizarPedido_DeveAtualizarDados()
    {
        // Arrange
        var inputDto = CriarPedidoInputDtoValido();
        var pedidoCriado = await _controller.CriarPedido(inputDto);

        var updateDto = new PedidoInputDto
        {
            Id = pedidoCriado.Id,
            TokenAtendimentoId = inputDto.TokenAtendimentoId,
            ClienteId = inputDto.ClienteId,
            Subtotal = 500.00m,
            Desconto = 50.00m,
            Total = 450.00m,
            Itens = inputDto.Itens
        };

        // Act
        var resultado = await _controller.AtualizarPedido(updateDto);

        // Assert
        resultado.Subtotal.Should().Be(500.00m);
        resultado.Desconto.Should().Be(50.00m);
        resultado.Total.Should().Be(450.00m);
    }

    #endregion

    #region Helper Methods

    private static PedidoInputDto CriarPedidoInputDtoValido()
    {
        return new PedidoInputDto
        {
            TokenAtendimentoId = Guid.NewGuid(),
            ClienteId = Guid.NewGuid(),
            Subtotal = 100.00m,
            Desconto = 10.00m,
            Total = 90.00m,
            Status = StatusPedido.Pendente,
            Itens = new List<ItemPedidoInputDto>
            {
                new()
                {
                    ProdutoId = Guid.NewGuid(),
                    Quantidade = 2,
                    PrecoUnitario = 50.00m,
                    DescontoUnitario = 5.00m
                }
            }
        };
    }

    #endregion
}
