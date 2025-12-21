using FluentAssertions;
using Soat.Eleven.Pedidos.Core.DTOs.Pedidos;
using Soat.Eleven.Pedidos.Core.Enums;
using Xunit;

namespace Soat.Eleven.Pedidos.Tests.Core.DTOs;

public class PedidoInputDtoTests
{
    #region Properties Tests

    [Fact]
    public void PedidoInputDto_DeveInicializarComValoresPadrao()
    {
        // Arrange & Act
        var dto = new PedidoInputDto();

        // Assert
        dto.Id.Should().Be(Guid.Empty);
        dto.TokenAtendimentoId.Should().Be(Guid.Empty);
        dto.ClienteId.Should().BeNull();
        dto.Subtotal.Should().Be(0);
        dto.Desconto.Should().Be(0);
        dto.Total.Should().Be(0);
        dto.SenhaPedido.Should().Be(string.Empty);
        dto.Itens.Should().NotBeNull();
        dto.Itens.Should().BeEmpty();
    }

    [Fact]
    public void PedidoInputDto_DevePermitirDefinirTodasPropriedades()
    {
        // Arrange
        var id = Guid.NewGuid();
        var tokenId = Guid.NewGuid();
        var clienteId = Guid.NewGuid();

        // Act
        var dto = new PedidoInputDto
        {
            Id = id,
            TokenAtendimentoId = tokenId,
            ClienteId = clienteId,
            Status = StatusPedido.EmPreparacao,
            SenhaPedido = "TEST123456",
            Subtotal = 150.00m,
            Desconto = 15.00m,
            Total = 135.00m
        };

        // Assert
        dto.Id.Should().Be(id);
        dto.TokenAtendimentoId.Should().Be(tokenId);
        dto.ClienteId.Should().Be(clienteId);
        dto.Status.Should().Be(StatusPedido.EmPreparacao);
        dto.SenhaPedido.Should().Be("TEST123456");
        dto.Subtotal.Should().Be(150.00m);
        dto.Desconto.Should().Be(15.00m);
        dto.Total.Should().Be(135.00m);
    }

    [Fact]
    public void PedidoInputDto_DevePermitirClienteIdNulo()
    {
        // Arrange & Act
        var dto = new PedidoInputDto { ClienteId = null };

        // Assert
        dto.ClienteId.Should().BeNull();
    }

    [Fact]
    public void PedidoInputDto_DevePermitirAdicionarItens()
    {
        // Arrange
        var dto = new PedidoInputDto();
        var item = new ItemPedidoInputDto
        {
            ProdutoId = Guid.NewGuid(),
            Quantidade = 3,
            PrecoUnitario = 30.00m,
            DescontoUnitario = 5.00m
        };

        // Act
        dto.Itens.Add(item);

        // Assert
        dto.Itens.Should().HaveCount(1);
        dto.Itens.First().Should().Be(item);
    }

    [Theory]
    [InlineData(StatusPedido.Pendente)]
    [InlineData(StatusPedido.Recebido)]
    [InlineData(StatusPedido.EmPreparacao)]
    [InlineData(StatusPedido.Pronto)]
    [InlineData(StatusPedido.Finalizado)]
    [InlineData(StatusPedido.Cancelado)]
    public void PedidoInputDto_DeveAceitarTodosStatus(StatusPedido status)
    {
        // Arrange & Act
        var dto = new PedidoInputDto { Status = status };

        // Assert
        dto.Status.Should().Be(status);
    }

    #endregion
}
