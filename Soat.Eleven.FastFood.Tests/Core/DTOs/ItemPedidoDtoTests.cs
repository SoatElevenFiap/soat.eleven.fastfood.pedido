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
}
