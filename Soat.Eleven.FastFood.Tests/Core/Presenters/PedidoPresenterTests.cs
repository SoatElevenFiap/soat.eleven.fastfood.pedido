using FluentAssertions;
using Soat.Eleven.FastFood.Core.Entities;
using Soat.Eleven.FastFood.Core.Enums;
using Soat.Eleven.FastFood.Core.Presenters;
using Xunit;

namespace Soat.Eleven.FastFood.Tests.Core.Presenters;

public class PedidoPresenterTests
{
    #region Output Tests

    [Fact]
    public void Output_DeveRetornarDtoComDadosCorretos_QuandoPedidoValido()
    {
        // Arrange
        var pedido = CriarPedidoCompleto();

        // Act
        var resultado = PedidoPresenter.Output(pedido);

        // Assert
        resultado.Id.Should().Be(pedido.Id);
        resultado.TokenAtendimentoId.Should().Be(pedido.TokenAtendimentoId);
        resultado.ClienteId.Should().Be(pedido.ClienteId);
        resultado.SenhaPedido.Should().Be(pedido.SenhaPedido);
        resultado.Subtotal.Should().Be(pedido.Subtotal);
        resultado.Desconto.Should().Be(pedido.Desconto);
        resultado.Total.Should().Be(pedido.Total);
        resultado.Status.Should().Be(pedido.Status);
        resultado.CriadoEm.Should().Be(pedido.CriadoEm);
    }

    [Fact]
    public void Output_DeveMapearItensCorretamente()
    {
        // Arrange
        var pedido = CriarPedidoCompleto();
        var item = new ItemPedido(Guid.NewGuid(), 2, 25.00m, 2.50m);
        pedido.AdicionarItem(item);

        // Act
        var resultado = PedidoPresenter.Output(pedido);

        // Assert
        resultado.Itens.Should().HaveCount(1);
        var primeiroItem = resultado.Itens.First();
        primeiroItem.ProdutoId.Should().Be(item.ProdutoId);
        primeiroItem.Quantidade.Should().Be(item.Quantidade);
        primeiroItem.PrecoUnitario.Should().Be(item.PrecoUnitario);
        primeiroItem.DescontoUnitario.Should().Be(item.DescontoUnitario);
    }

    [Fact]
    public void Output_DeveMapearMultiplosItensCorretamente()
    {
        // Arrange
        var pedido = CriarPedidoCompleto();
        pedido.AdicionarItem(new ItemPedido(Guid.NewGuid(), 1, 10.00m, 0.00m));
        pedido.AdicionarItem(new ItemPedido(Guid.NewGuid(), 2, 20.00m, 1.00m));
        pedido.AdicionarItem(new ItemPedido(Guid.NewGuid(), 3, 30.00m, 2.00m));

        // Act
        var resultado = PedidoPresenter.Output(pedido);

        // Assert
        resultado.Itens.Should().HaveCount(3);
    }

    [Fact]
    public void Output_DeveRetornarValoresPadrao_QuandoPedidoNulo()
    {
        // Arrange
        Pedido? pedido = null;

        // Act
        var resultado = PedidoPresenter.Output(pedido!);

        // Assert
        resultado.Id.Should().Be(Guid.Empty);
        resultado.TokenAtendimentoId.Should().Be(Guid.Empty);
        resultado.ClienteId.Should().Be(Guid.Empty);
        resultado.SenhaPedido.Should().Be(string.Empty);
        resultado.Subtotal.Should().Be(0);
        resultado.Desconto.Should().Be(0);
        resultado.Total.Should().Be(0);
        resultado.Status.Should().Be(default(StatusPedido));
        resultado.CriadoEm.Should().Be(DateTime.MinValue);
        resultado.Itens.Should().BeEmpty();
    }

    [Fact]
    public void Output_DeveRetornarListaVazia_QuandoPedidoSemItens()
    {
        // Arrange
        var pedido = CriarPedidoCompleto();

        // Act
        var resultado = PedidoPresenter.Output(pedido);

        // Assert
        resultado.Itens.Should().BeEmpty();
    }

    [Fact]
    public void Output_DevePreservarClienteIdNulo()
    {
        // Arrange
        var pedido = new Pedido(Guid.NewGuid(), null, 100.00m, 0.00m, 100.00m);
        pedido.GerarSenha();

        // Act
        var resultado = PedidoPresenter.Output(pedido);

        // Assert
        resultado.ClienteId.Should().Be(Guid.Empty);
    }

    [Fact]
    public void Output_DevePreservarTodosStatusDoPedido()
    {
        // Arrange & Act & Assert
        foreach (StatusPedido status in Enum.GetValues(typeof(StatusPedido)))
        {
            var pedido = CriarPedidoCompleto();
            pedido.Status = status;

            var resultado = PedidoPresenter.Output(pedido);

            resultado.Status.Should().Be(status);
        }
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
