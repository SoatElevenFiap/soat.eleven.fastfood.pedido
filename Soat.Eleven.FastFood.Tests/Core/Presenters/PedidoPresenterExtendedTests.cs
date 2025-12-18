using FluentAssertions;
using Soat.Eleven.FastFood.Core.DTOs.Pedidos;
using Soat.Eleven.FastFood.Core.Entities;
using Soat.Eleven.FastFood.Core.Enums;
using Soat.Eleven.FastFood.Core.Presenters;
using Xunit;

namespace Soat.Eleven.FastFood.Tests.Core.Presenters;

/// <summary>
/// Testes adicionais para PedidoPresenter cobrindo mais cenários
/// </summary>
public class PedidoPresenterExtendedTests
{
    #region Output Extended Tests

    [Fact]
    public void Output_DeveMapearPedidoComMultiplosItens()
    {
        // Arrange
        var pedido = CriarPedidoCompleto();
        pedido.AdicionarItem(new ItemPedido(Guid.NewGuid(), 1, 10.00m, 1.00m));
        pedido.AdicionarItem(new ItemPedido(Guid.NewGuid(), 2, 20.00m, 2.00m));
        pedido.AdicionarItem(new ItemPedido(Guid.NewGuid(), 3, 30.00m, 3.00m));
        pedido.AdicionarItem(new ItemPedido(Guid.NewGuid(), 4, 40.00m, 4.00m));
        pedido.AdicionarItem(new ItemPedido(Guid.NewGuid(), 5, 50.00m, 5.00m));

        // Act
        var resultado = PedidoPresenter.Output(pedido);

        // Assert
        resultado.Itens.Should().HaveCount(5);
    }

    [Fact]
    public void Output_DeveMapearItemComValoresDecimaisPrecisos()
    {
        // Arrange
        var pedido = CriarPedidoCompleto();
        var item = new ItemPedido(Guid.NewGuid(), 1, 99.999m, 0.001m);
        pedido.AdicionarItem(item);

        // Act
        var resultado = PedidoPresenter.Output(pedido);

        // Assert
        var itemOutput = resultado.Itens.First();
        itemOutput.PrecoUnitario.Should().Be(99.999m);
        itemOutput.DescontoUnitario.Should().Be(0.001m);
    }

    [Fact]
    public void Output_DeveMapearPedidoComSubtotalAlto()
    {
        // Arrange
        var pedido = new Pedido(Guid.NewGuid(), Guid.NewGuid(), 999999.99m, 99999.99m, 900000.00m);
        pedido.GerarSenha();

        // Act
        var resultado = PedidoPresenter.Output(pedido);

        // Assert
        resultado.Subtotal.Should().Be(999999.99m);
        resultado.Desconto.Should().Be(99999.99m);
        resultado.Total.Should().Be(900000.00m);
    }

    [Fact]
    public void Output_DeveMapearPedidoComDescontoZero()
    {
        // Arrange
        var pedido = new Pedido(Guid.NewGuid(), Guid.NewGuid(), 100.00m, 0.00m, 100.00m);
        pedido.GerarSenha();

        // Act
        var resultado = PedidoPresenter.Output(pedido);

        // Assert
        resultado.Desconto.Should().Be(0.00m);
        resultado.Subtotal.Should().Be(resultado.Total);
    }

    [Theory]
    [InlineData(StatusPedido.Pendente)]
    [InlineData(StatusPedido.Recebido)]
    [InlineData(StatusPedido.EmPreparacao)]
    [InlineData(StatusPedido.Pronto)]
    [InlineData(StatusPedido.Finalizado)]
    [InlineData(StatusPedido.Cancelado)]
    public void Output_DeveMapearTodosOsStatusCorretamente(StatusPedido status)
    {
        // Arrange
        var pedido = CriarPedidoCompleto();
        pedido.Status = status;

        // Act
        var resultado = PedidoPresenter.Output(pedido);

        // Assert
        resultado.Status.Should().Be(status);
    }

    [Fact]
    public void Output_DeveMapearDataCriacaoCorretamente()
    {
        // Arrange
        var pedido = CriarPedidoCompleto();
        var dataCriacao = new DateTime(2025, 12, 18, 10, 30, 0);
        pedido.CriadoEm = dataCriacao;

        // Act
        var resultado = PedidoPresenter.Output(pedido);

        // Assert
        resultado.CriadoEm.Should().Be(dataCriacao);
    }

    [Fact]
    public void Output_DeveMapearSenhaPedidoCorretamente()
    {
        // Arrange
        var pedido = CriarPedidoCompleto();
        // A senha é gerada automaticamente

        // Act
        var resultado = PedidoPresenter.Output(pedido);

        // Assert
        resultado.SenhaPedido.Should().NotBeNullOrEmpty();
        resultado.SenhaPedido.Should().HaveLength(10);
    }

    [Fact]
    public void Output_DeveMapearTokenAtendimentoIdCorretamente()
    {
        // Arrange
        var tokenId = Guid.NewGuid();
        var pedido = new Pedido(tokenId, Guid.NewGuid(), 100.00m, 10.00m, 90.00m);
        pedido.GerarSenha();

        // Act
        var resultado = PedidoPresenter.Output(pedido);

        // Assert
        resultado.TokenAtendimentoId.Should().Be(tokenId);
    }

    [Fact]
    public void Output_DeveMapearClienteIdNuloParaGuidEmpty()
    {
        // Arrange
        var pedido = new Pedido(Guid.NewGuid(), null, 100.00m, 10.00m, 90.00m);
        pedido.GerarSenha();

        // Act
        var resultado = PedidoPresenter.Output(pedido);

        // Assert
        resultado.ClienteId.Should().Be(Guid.Empty);
    }

    [Fact]
    public void Output_DeveMapearClienteIdCorretamente()
    {
        // Arrange
        var clienteId = Guid.NewGuid();
        var pedido = new Pedido(Guid.NewGuid(), clienteId, 100.00m, 10.00m, 90.00m);
        pedido.GerarSenha();

        // Act
        var resultado = PedidoPresenter.Output(pedido);

        // Assert
        resultado.ClienteId.Should().Be(clienteId);
    }

    [Fact]
    public void Output_DeveMapearPedidoIdCorretamente()
    {
        // Arrange
        var pedido = CriarPedidoCompleto();
        var pedidoId = Guid.NewGuid();
        pedido.AtualizarId(pedidoId);

        // Act
        var resultado = PedidoPresenter.Output(pedido);

        // Assert
        resultado.Id.Should().Be(pedidoId);
    }

    [Fact]
    public void Output_DeveRetornarItensVaziosQuandoPedidoSemItens()
    {
        // Arrange
        var pedido = CriarPedidoCompleto();

        // Act
        var resultado = PedidoPresenter.Output(pedido);

        // Assert
        resultado.Itens.Should().NotBeNull();
        resultado.Itens.Should().BeEmpty();
    }

    [Fact]
    public void Output_DeveMapearQuantidadeDeItemCorretamente()
    {
        // Arrange
        var pedido = CriarPedidoCompleto();
        var item = new ItemPedido(Guid.NewGuid(), 999, 10.00m, 1.00m);
        pedido.AdicionarItem(item);

        // Act
        var resultado = PedidoPresenter.Output(pedido);

        // Assert
        resultado.Itens.First().Quantidade.Should().Be(999);
    }

    [Fact]
    public void Output_DeveMapearProdutoIdDeItemCorretamente()
    {
        // Arrange
        var pedido = CriarPedidoCompleto();
        var produtoId = Guid.NewGuid();
        var item = new ItemPedido(produtoId, 1, 10.00m, 1.00m);
        pedido.AdicionarItem(item);

        // Act
        var resultado = PedidoPresenter.Output(pedido);

        // Assert
        resultado.Itens.First().ProdutoId.Should().Be(produtoId);
    }

    #endregion

    #region Helper Methods

    private static Pedido CriarPedidoCompleto()
    {
        var pedido = new Pedido(
            tokenAtendimentoId: Guid.NewGuid(),
            clienteId: Guid.NewGuid(),
            subtotal: 100.00m,
            desconto: 10.00m,
            total: 90.00m
        );
        pedido.AtualizarId(Guid.NewGuid());
        pedido.GerarSenha();
        return pedido;
    }

    #endregion
}
