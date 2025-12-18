using FluentAssertions;
using Soat.Eleven.FastFood.Core.Entities;
using Xunit;

namespace Soat.Eleven.FastFood.Tests.Core.Entities;

/// <summary>
/// Testes adicionais para ItemPedido cobrindo mais cenários
/// </summary>
public class ItemPedidoExtendedTests
{
    #region Constructor Extended Tests

    [Fact]
    public void Construtor_DeveAceitarQuantidadeAlta()
    {
        // Arrange & Act
        var item = new ItemPedido(Guid.NewGuid(), int.MaxValue, 10.00m, 0);

        // Assert
        item.Quantidade.Should().Be(int.MaxValue);
    }

    [Fact]
    public void Construtor_DeveAceitarPrecoMuitoAlto()
    {
        // Arrange & Act
        var item = new ItemPedido(Guid.NewGuid(), 1, 99999999.99m, 0);

        // Assert
        item.PrecoUnitario.Should().Be(99999999.99m);
    }

    [Fact]
    public void Construtor_DeveAceitarDescontoIgualAoPreco()
    {
        // Arrange & Act
        var item = new ItemPedido(Guid.NewGuid(), 1, 100.00m, 100.00m);

        // Assert
        item.DescontoUnitario.Should().Be(100.00m);
    }

    [Fact]
    public void Construtor_DeveAceitarValoresDecimaisPrecisos()
    {
        // Arrange & Act
        var item = new ItemPedido(Guid.NewGuid(), 1, 0.01m, 0.001m);

        // Assert
        item.PrecoUnitario.Should().Be(0.01m);
        item.DescontoUnitario.Should().Be(0.001m);
    }

    #endregion

    #region ConstrutorVazio Extended Tests

    [Fact]
    public void ConstrutorVazio_DeveInicializarComValoresPadrao()
    {
        // Arrange & Act
        var item = new ItemPedido();

        // Assert
        item.ProdutoId.Should().Be(Guid.Empty);
        item.Quantidade.Should().Be(0);
        item.PrecoUnitario.Should().Be(0);
        item.DescontoUnitario.Should().Be(0);
    }

    [Fact]
    public void ConstrutorVazio_DevePermitirDefinirPropriedadesPosteriormente()
    {
        // Arrange
        var item = new ItemPedido();
        var produtoId = Guid.NewGuid();

        // Act
        item.ProdutoId = produtoId;
        item.Quantidade = 5;
        item.PrecoUnitario = 50.00m;
        item.DescontoUnitario = 5.00m;

        // Assert
        item.ProdutoId.Should().Be(produtoId);
        item.Quantidade.Should().Be(5);
        item.PrecoUnitario.Should().Be(50.00m);
        item.DescontoUnitario.Should().Be(5.00m);
    }

    #endregion

    #region Properties Extended Tests

    [Fact]
    public void ProdutoId_DevePermitirAlteracao()
    {
        // Arrange
        var item = new ItemPedido(Guid.NewGuid(), 1, 10.00m, 0);
        var novoProdutoId = Guid.NewGuid();

        // Act
        item.ProdutoId = novoProdutoId;

        // Assert
        item.ProdutoId.Should().Be(novoProdutoId);
    }

    [Fact]
    public void Quantidade_DevePermitirAlteracao()
    {
        // Arrange
        var item = new ItemPedido(Guid.NewGuid(), 1, 10.00m, 0);

        // Act
        item.Quantidade = 999;

        // Assert
        item.Quantidade.Should().Be(999);
    }

    [Fact]
    public void PrecoUnitario_DevePermitirAlteracao()
    {
        // Arrange
        var item = new ItemPedido(Guid.NewGuid(), 1, 10.00m, 0);

        // Act
        item.PrecoUnitario = 99.99m;

        // Assert
        item.PrecoUnitario.Should().Be(99.99m);
    }

    [Fact]
    public void DescontoUnitario_DevePermitirAlteracao()
    {
        // Arrange
        var item = new ItemPedido(Guid.NewGuid(), 1, 10.00m, 0);

        // Act
        item.DescontoUnitario = 5.00m;

        // Assert
        item.DescontoUnitario.Should().Be(5.00m);
    }

    [Fact]
    public void Quantidade_DeveAceitarValorNegativo()
    {
        // Arrange
        var item = new ItemPedido();

        // Act
        item.Quantidade = -1;

        // Assert
        item.Quantidade.Should().Be(-1);
    }

    [Fact]
    public void PrecoUnitario_DeveAceitarValorNegativo()
    {
        // Arrange
        var item = new ItemPedido();

        // Act
        item.PrecoUnitario = -10.00m;

        // Assert
        item.PrecoUnitario.Should().Be(-10.00m);
    }

    #endregion

    #region Equality Extended Tests

    [Fact]
    public void DoisItens_DevemSerDiferentes_QuandoProdutoIdsDiferentes()
    {
        // Arrange
        var item1 = new ItemPedido(Guid.NewGuid(), 1, 10.00m, 0);
        var item2 = new ItemPedido(Guid.NewGuid(), 1, 10.00m, 0);

        // Assert
        item1.ProdutoId.Should().NotBe(item2.ProdutoId);
    }

    [Fact]
    public void DoisItens_DevemTerMesmoProdutoId_QuandoCriadosComMesmoGuid()
    {
        // Arrange
        var produtoId = Guid.NewGuid();
        var item1 = new ItemPedido(produtoId, 1, 10.00m, 0);
        var item2 = new ItemPedido(produtoId, 2, 20.00m, 0);

        // Assert
        item1.ProdutoId.Should().Be(item2.ProdutoId);
    }

    #endregion
}
