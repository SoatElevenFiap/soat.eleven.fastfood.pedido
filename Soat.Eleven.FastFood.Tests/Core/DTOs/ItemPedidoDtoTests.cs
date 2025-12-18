using FluentAssertions;
using Soat.Eleven.FastFood.Core.DTOs.Pedidos;
using Xunit;

namespace Soat.Eleven.FastFood.Tests.Core.DTOs;

public class ItemPedidoDtoTests
{
    #region ItemPedidoInputDto Tests

    [Fact]
    public void ItemPedidoInputDto_DeveInicializarComValoresPadrao()
    {
        // Arrange & Act
        var dto = new ItemPedidoInputDto();

        // Assert
        dto.ProdutoId.Should().Be(Guid.Empty);
        dto.Quantidade.Should().Be(0);
        dto.PrecoUnitario.Should().Be(0);
        dto.DescontoUnitario.Should().Be(0);
    }

    [Fact]
    public void ItemPedidoInputDto_DevePermitirDefinirPropriedades()
    {
        // Arrange
        var produtoId = Guid.NewGuid();

        // Act
        var dto = new ItemPedidoInputDto
        {
            ProdutoId = produtoId,
            Quantidade = 5,
            PrecoUnitario = 19.99m,
            DescontoUnitario = 2.00m
        };

        // Assert
        dto.ProdutoId.Should().Be(produtoId);
        dto.Quantidade.Should().Be(5);
        dto.PrecoUnitario.Should().Be(19.99m);
        dto.DescontoUnitario.Should().Be(2.00m);
    }

    [Fact]
    public void ItemPedidoInputDto_DevePermitirQuantidadeZero()
    {
        // Arrange & Act
        var dto = new ItemPedidoInputDto { Quantidade = 0 };

        // Assert
        dto.Quantidade.Should().Be(0);
    }

    [Fact]
    public void ItemPedidoInputDto_DevePermitirDescontoZero()
    {
        // Arrange & Act
        var dto = new ItemPedidoInputDto { DescontoUnitario = 0m };

        // Assert
        dto.DescontoUnitario.Should().Be(0m);
    }

    #endregion

    #region ItemPedidoOutputDto Tests

    [Fact]
    public void ItemPedidoOutputDto_DeveInicializarComValoresPadrao()
    {
        // Arrange & Act
        var dto = new ItemPedidoOutputDto();

        // Assert
        dto.ProdutoId.Should().Be(Guid.Empty);
        dto.Quantidade.Should().Be(0);
        dto.PrecoUnitario.Should().Be(0);
        dto.DescontoUnitario.Should().Be(0);
    }

    [Fact]
    public void ItemPedidoOutputDto_DevePermitirDefinirPropriedades()
    {
        // Arrange
        var produtoId = Guid.NewGuid();

        // Act
        var dto = new ItemPedidoOutputDto
        {
            ProdutoId = produtoId,
            Quantidade = 10,
            PrecoUnitario = 49.99m,
            DescontoUnitario = 5.00m
        };

        // Assert
        dto.ProdutoId.Should().Be(produtoId);
        dto.Quantidade.Should().Be(10);
        dto.PrecoUnitario.Should().Be(49.99m);
        dto.DescontoUnitario.Should().Be(5.00m);
    }

    [Fact]
    public void ItemPedidoOutputDto_DeveManterPrecisaoDecimal()
    {
        // Arrange & Act
        var dto = new ItemPedidoOutputDto
        {
            PrecoUnitario = 99.999m,
            DescontoUnitario = 0.001m
        };

        // Assert
        dto.PrecoUnitario.Should().Be(99.999m);
        dto.DescontoUnitario.Should().Be(0.001m);
    }

    #endregion

    #region Implicit Conversion Tests (ItemPedidoInputDto -> ItemPedido)

    [Fact]
    public void ItemPedidoInputDto_ImplicitConversion_DeveConverterParaItemPedido()
    {
        // Arrange
        var produtoId = Guid.NewGuid();
        var dto = new ItemPedidoInputDto
        {
            ProdutoId = produtoId,
            Quantidade = 3,
            PrecoUnitario = 25.00m,
            DescontoUnitario = 2.50m
        };

        // Act
        Soat.Eleven.FastFood.Core.Entities.ItemPedido itemPedido = dto;

        // Assert
        itemPedido.Should().NotBeNull();
        itemPedido.ProdutoId.Should().Be(produtoId);
        itemPedido.Quantidade.Should().Be(3);
        itemPedido.PrecoUnitario.Should().Be(25.00m);
        itemPedido.DescontoUnitario.Should().Be(2.50m);
    }

    [Fact]
    public void ItemPedidoInputDto_ImplicitConversion_DevePreservarTodosOsValores()
    {
        // Arrange
        var dto = new ItemPedidoInputDto
        {
            ProdutoId = Guid.NewGuid(),
            Quantidade = 100,
            PrecoUnitario = 999.99m,
            DescontoUnitario = 99.99m
        };

        // Act
        Soat.Eleven.FastFood.Core.Entities.ItemPedido resultado = dto;

        // Assert
        resultado.ProdutoId.Should().Be(dto.ProdutoId);
        resultado.Quantidade.Should().Be(dto.Quantidade);
        resultado.PrecoUnitario.Should().Be(dto.PrecoUnitario);
        resultado.DescontoUnitario.Should().Be(dto.DescontoUnitario);
    }

    #endregion

    #region Explicit Conversion Tests (ItemPedido -> ItemPedidoOutputDto)

    [Fact]
    public void ItemPedidoOutputDto_ExplicitConversion_DeveConverterDeItemPedido()
    {
        // Arrange
        var produtoId = Guid.NewGuid();
        var itemPedido = new Soat.Eleven.FastFood.Core.Entities.ItemPedido(produtoId, 5, 50.00m, 5.00m);

        // Act
        var dto = (ItemPedidoOutputDto)itemPedido;

        // Assert
        dto.Should().NotBeNull();
        dto.ProdutoId.Should().Be(produtoId);
        dto.Quantidade.Should().Be(5);
        dto.PrecoUnitario.Should().Be(50.00m);
        dto.DescontoUnitario.Should().Be(5.00m);
    }

    [Fact]
    public void ItemPedidoOutputDto_ExplicitConversion_DevePreservarTodosOsValores()
    {
        // Arrange
        var itemPedido = new Soat.Eleven.FastFood.Core.Entities.ItemPedido(
            Guid.NewGuid(), 10, 100.00m, 10.00m
        );

        // Act
        var dto = (ItemPedidoOutputDto)itemPedido;

        // Assert
        dto.ProdutoId.Should().Be(itemPedido.ProdutoId);
        dto.Quantidade.Should().Be(itemPedido.Quantidade);
        dto.PrecoUnitario.Should().Be(itemPedido.PrecoUnitario);
        dto.DescontoUnitario.Should().Be(itemPedido.DescontoUnitario);
    }

    #endregion
}
