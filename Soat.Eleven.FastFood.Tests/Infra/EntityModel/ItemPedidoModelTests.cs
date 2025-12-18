using FluentAssertions;
using Soat.Eleven.FastFood.Adapter.Infra.EntityModel;
using Xunit;

namespace Soat.Eleven.FastFood.Tests.Infra.EntityModel;

public class ItemPedidoModelTests
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
        var item = new ItemPedidoModel(produtoId, quantidade, precoUnitario, descontoUnitario);

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
        var item = new ItemPedidoModel();

        // Assert
        item.Should().NotBeNull();
        item.Id.Should().Be(Guid.Empty);
        item.PedidoId.Should().Be(Guid.Empty);
    }

    [Fact]
    public void Construtor_DeveAceitarQuantidadeZero()
    {
        // Arrange & Act
        var item = new ItemPedidoModel(Guid.NewGuid(), 0, 25.00m, 0.00m);

        // Assert
        item.Quantidade.Should().Be(0);
    }

    [Fact]
    public void Construtor_DeveAceitarPrecoUnitarioZero()
    {
        // Arrange & Act
        var item = new ItemPedidoModel(Guid.NewGuid(), 1, 0.00m, 0.00m);

        // Assert
        item.PrecoUnitario.Should().Be(0.00m);
    }

    [Fact]
    public void Construtor_DeveAceitarDescontoUnitarioZero()
    {
        // Arrange & Act
        var item = new ItemPedidoModel(Guid.NewGuid(), 1, 25.00m, 0.00m);

        // Assert
        item.DescontoUnitario.Should().Be(0.00m);
    }

    #endregion

    #region Properties Tests

    [Fact]
    public void Id_DevePermitirDefinirValor()
    {
        // Arrange
        var item = new ItemPedidoModel();
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
        var item = new ItemPedidoModel();
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
        var item = new ItemPedidoModel();
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
        var item = new ItemPedidoModel();

        // Act
        item.Quantidade = 5;

        // Assert
        item.Quantidade.Should().Be(5);
    }

    [Fact]
    public void PrecoUnitario_DevePermitirDefinirValor()
    {
        // Arrange
        var item = new ItemPedidoModel();

        // Act
        item.PrecoUnitario = 99.99m;

        // Assert
        item.PrecoUnitario.Should().Be(99.99m);
    }

    [Fact]
    public void DescontoUnitario_DevePermitirDefinirValor()
    {
        // Arrange
        var item = new ItemPedidoModel();

        // Act
        item.DescontoUnitario = 5.00m;

        // Assert
        item.DescontoUnitario.Should().Be(5.00m);
    }

    [Fact]
    public void Pedido_DevePermitirDefinirReferencia()
    {
        // Arrange
        var item = new ItemPedidoModel();
        var pedido = new PedidoModel(Guid.NewGuid(), Guid.NewGuid(), 100.00m, 0.00m, 100.00m, "SENHA123");

        // Act
        item.Pedido = pedido;

        // Assert
        item.Pedido.Should().Be(pedido);
    }

    #endregion

    #region EntityBase Tests

    [Fact]
    public void CriadoEm_DevePermitirDefinirValor()
    {
        // Arrange
        var item = new ItemPedidoModel();
        var dataCriacao = DateTime.Now;

        // Act
        item.CriadoEm = dataCriacao;

        // Assert
        item.CriadoEm.Should().Be(dataCriacao);
    }

    [Fact]
    public void ModificadoEm_DevePermitirDefinirValor()
    {
        // Arrange
        var item = new ItemPedidoModel();
        var dataModificacao = DateTime.Now;

        // Act
        item.ModificadoEm = dataModificacao;

        // Assert
        item.ModificadoEm.Should().Be(dataModificacao);
    }

    #endregion

    #region Valor Calculado Tests

    [Fact]
    public void Item_DevePermitirCalcularValorTotal()
    {
        // Arrange
        var quantidade = 3;
        var precoUnitario = 25.00m;
        var item = new ItemPedidoModel(Guid.NewGuid(), quantidade, precoUnitario, 0.00m);

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
        var item = new ItemPedidoModel(Guid.NewGuid(), quantidade, precoUnitario, descontoUnitario);

        // Act
        var valorTotal = item.Quantidade * (item.PrecoUnitario - item.DescontoUnitario);

        // Assert
        valorTotal.Should().Be(90.00m);
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
        var item = new ItemPedidoModel(Guid.NewGuid(), quantidade, preco, desconto);

        // Act
        var valorTotal = item.Quantidade * (item.PrecoUnitario - item.DescontoUnitario);

        // Assert
        valorTotal.Should().Be(valorEsperado);
    }

    #endregion

    #region Precisao Decimal Tests

    [Fact]
    public void PrecoUnitario_DeveManterPrecisaoDecimal()
    {
        // Arrange & Act
        var item = new ItemPedidoModel(Guid.NewGuid(), 1, 19.99m, 0.00m);

        // Assert
        item.PrecoUnitario.Should().Be(19.99m);
    }

    [Fact]
    public void DescontoUnitario_DeveManterPrecisaoDecimal()
    {
        // Arrange & Act
        var item = new ItemPedidoModel(Guid.NewGuid(), 1, 100.00m, 10.555m);

        // Assert
        item.DescontoUnitario.Should().Be(10.555m);
    }

    #endregion
}
