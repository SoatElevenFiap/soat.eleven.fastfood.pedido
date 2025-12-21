using FluentAssertions;
using Soat.Eleven.Pedidos.Core.Entities;
using Xunit;

namespace Soat.Eleven.Pedidos.Tests.Core.Entities;

public class ItemPedidoTests
{
    #region Constructor Tests

    [Fact]
    public void Construtor_DeveInicializarPropriedadesCorretamente()
    {
        // Arrange
        var produtoId = Guid.NewGuid();
        var quantidade = 3;
        var precoUnitario = 25.00m;
        var descontoUnitario = 2.50m;

        // Act
        var item = new ItemPedido(produtoId, quantidade, precoUnitario, descontoUnitario);

        // Assert
        item.ProdutoId.Should().Be(produtoId);
        item.Quantidade.Should().Be(quantidade);
        item.PrecoUnitario.Should().Be(precoUnitario);
        item.DescontoUnitario.Should().Be(descontoUnitario);
    }

    [Fact]
    public void ConstrutorVazio_DevePermitirInstanciacaoPeloORM()
    {
        // Arrange & Act
        var item = new ItemPedido();

        // Assert
        item.Should().NotBeNull();
        item.Id.Should().Be(Guid.Empty);
        item.PedidoId.Should().Be(Guid.Empty);
    }

    [Fact]
    public void Construtor_DeveAceitarQuantidadeZero()
    {
        // Arrange & Act
        var item = new ItemPedido(Guid.NewGuid(), 0, 25.00m, 0.00m);

        // Assert
        item.Quantidade.Should().Be(0);
    }

    [Fact]
    public void Construtor_DeveAceitarPrecoUnitarioZero()
    {
        // Arrange & Act
        var item = new ItemPedido(Guid.NewGuid(), 1, 0.00m, 0.00m);

        // Assert
        item.PrecoUnitario.Should().Be(0.00m);
    }

    [Fact]
    public void Construtor_DeveAceitarDescontoUnitarioZero()
    {
        // Arrange & Act
        var item = new ItemPedido(Guid.NewGuid(), 1, 25.00m, 0.00m);

        // Assert
        item.DescontoUnitario.Should().Be(0.00m);
    }

    #endregion

    #region Properties Tests

    [Fact]
    public void Id_DevePermitirDefinirValor()
    {
        // Arrange
        var item = new ItemPedido();
        var novoId = Guid.NewGuid();

        // Act
        item.Id = novoId;

        // Assert
        item.Id.Should().Be(novoId);
    }

    [Fact]
    public void PedidoId_DevePermitirDefinirValor()
    {
        // Arrange
        var item = new ItemPedido();
        var pedidoId = Guid.NewGuid();

        // Act
        item.PedidoId = pedidoId;

        // Assert
        item.PedidoId.Should().Be(pedidoId);
    }

    [Fact]
    public void ProdutoId_DevePermitirDefinirValor()
    {
        // Arrange
        var item = new ItemPedido();
        var produtoId = Guid.NewGuid();

        // Act
        item.ProdutoId = produtoId;

        // Assert
        item.ProdutoId.Should().Be(produtoId);
    }

    [Fact]
    public void Quantidade_DevePermitirDefinirValor()
    {
        // Arrange
        var item = new ItemPedido();

        // Act
        item.Quantidade = 5;

        // Assert
        item.Quantidade.Should().Be(5);
    }

    [Fact]
    public void PrecoUnitario_DevePermitirDefinirValor()
    {
        // Arrange
        var item = new ItemPedido();

        // Act
        item.PrecoUnitario = 99.99m;

        // Assert
        item.PrecoUnitario.Should().Be(99.99m);
    }

    [Fact]
    public void DescontoUnitario_DevePermitirDefinirValor()
    {
        // Arrange
        var item = new ItemPedido();

        // Act
        item.DescontoUnitario = 5.00m;

        // Assert
        item.DescontoUnitario.Should().Be(5.00m);
    }

    [Fact]
    public void Pedido_DevePermitirDefinirReferencia()
    {
        // Arrange
        var item = new ItemPedido();
        var pedido = new Pedido(Guid.NewGuid(), Guid.NewGuid(), 100.00m, 0.00m, 100.00m);

        // Act
        item.Pedido = pedido;

        // Assert
        item.Pedido.Should().Be(pedido);
    }

    #endregion

    #region Valor Calculado Tests

    [Fact]
    public void Item_DevePermitirCalcularValorTotal()
    {
        // Arrange
        var quantidade = 3;
        var precoUnitario = 25.00m;
        var item = new ItemPedido(Guid.NewGuid(), quantidade, precoUnitario, 0.00m);

        // Act
        var valorTotal = item.Quantidade * item.PrecoUnitario;

        // Assert
        valorTotal.Should().Be(75.00m);
    }

    [Fact]
    public void Item_DevePermitirCalcularValorComDesconto()
    {
        // Arrange
        var quantidade = 2;
        var precoUnitario = 50.00m;
        var descontoUnitario = 5.00m;
        var item = new ItemPedido(Guid.NewGuid(), quantidade, precoUnitario, descontoUnitario);

        // Act
        var valorTotal = item.Quantidade * (item.PrecoUnitario - item.DescontoUnitario);

        // Assert
        valorTotal.Should().Be(90.00m); // 2 * (50 - 5) = 90
    }

    [Theory]
    [InlineData(1, 10.00, 0.00, 10.00)]
    [InlineData(2, 25.00, 5.00, 40.00)]
    [InlineData(5, 100.00, 10.00, 450.00)]
    [InlineData(0, 50.00, 0.00, 0.00)]
    public void Item_DeveCalcularValorCorretamente_ComDiferentesCenarios(
        int quantidade, decimal preco, decimal desconto, decimal valorEsperado)
    {
        // Arrange
        var item = new ItemPedido(Guid.NewGuid(), quantidade, preco, desconto);

        // Act
        var valorTotal = item.Quantidade * (item.PrecoUnitario - item.DescontoUnitario);

        // Assert
        valorTotal.Should().Be(valorEsperado);
    }

    #endregion

    #region Navegacao Tests

    [Fact]
    public void Item_DeveInicializarPedidoComoNull()
    {
        // Arrange & Act
        var item = new ItemPedido(Guid.NewGuid(), 1, 25.00m, 0.00m);

        // Assert
        // O Pedido é inicializado como null! (non-nullable reference type com valor null)
        // Isso é permitido para navegação do EF
        item.Pedido.Should().BeNull();
    }

    [Fact]
    public void Item_DeveManterRelacionamentoComPedido()
    {
        // Arrange
        var pedido = new Pedido(Guid.NewGuid(), Guid.NewGuid(), 100.00m, 0.00m, 100.00m);
        var item = new ItemPedido(Guid.NewGuid(), 1, 25.00m, 0.00m);
        item.PedidoId = pedido.Id;
        item.Pedido = pedido;

        // Act & Assert
        item.PedidoId.Should().Be(pedido.Id);
        item.Pedido.Should().Be(pedido);
    }

    #endregion

    #region Precisao Decimal Tests

    [Fact]
    public void PrecoUnitario_DeveManterPrecisaoDecimal()
    {
        // Arrange & Act
        var item = new ItemPedido(Guid.NewGuid(), 1, 19.99m, 0.00m);

        // Assert
        item.PrecoUnitario.Should().Be(19.99m);
    }

    [Fact]
    public void DescontoUnitario_DeveManterPrecisaoDecimal()
    {
        // Arrange & Act
        var item = new ItemPedido(Guid.NewGuid(), 1, 100.00m, 10.555m);

        // Assert
        item.DescontoUnitario.Should().Be(10.555m);
    }

    #endregion
}
