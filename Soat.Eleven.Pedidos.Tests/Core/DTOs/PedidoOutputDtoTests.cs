using FluentAssertions;
using Soat.Eleven.Pedidos.Core.DTOs.Pedidos;
using Soat.Eleven.Pedidos.Core.Enums;
using Xunit;

namespace Soat.Eleven.Pedidos.Tests.Core.DTOs;

public class PedidoOutputDtoTests
{
    #region Properties Tests

    [Fact]
    public void PedidoOutputDto_DeveInicializarComValoresPadrao()
    {
        // Arrange & Act
        var dto = new PedidoOutputDto();

        // Assert
        dto.Id.Should().Be(Guid.Empty);
        dto.TokenAtendimentoId.Should().Be(Guid.Empty);
        dto.ClienteId.Should().BeNull();
        dto.Subtotal.Should().Be(0);
        dto.Desconto.Should().Be(0);
        dto.Total.Should().Be(0);
        dto.Itens.Should().NotBeNull();
        dto.Itens.Should().BeEmpty();
    }

    [Fact]
    public void PedidoOutputDto_DevePermitirDefinirTodasPropriedades()
    {
        // Arrange
        var id = Guid.NewGuid();
        var tokenId = Guid.NewGuid();
        var clienteId = Guid.NewGuid();
        var criadoEm = DateTime.Now;

        // Act
        var dto = new PedidoOutputDto
        {
            Id = id,
            TokenAtendimentoId = tokenId,
            ClienteId = clienteId,
            Status = StatusPedido.Recebido,
            SenhaPedido = "ABCD123456",
            Subtotal = 100.00m,
            Desconto = 10.00m,
            Total = 90.00m,
            CriadoEm = criadoEm
        };

        // Assert
        dto.Id.Should().Be(id);
        dto.TokenAtendimentoId.Should().Be(tokenId);
        dto.ClienteId.Should().Be(clienteId);
        dto.Status.Should().Be(StatusPedido.Recebido);
        dto.SenhaPedido.Should().Be("ABCD123456");
        dto.Subtotal.Should().Be(100.00m);
        dto.Desconto.Should().Be(10.00m);
        dto.Total.Should().Be(90.00m);
        dto.CriadoEm.Should().Be(criadoEm);
    }

    [Theory]
    [InlineData(StatusPedido.Pendente, "Pendente")]
    [InlineData(StatusPedido.Recebido, "Recebido")]
    [InlineData(StatusPedido.EmPreparacao, "EmPreparacao")]
    [InlineData(StatusPedido.Pronto, "Pronto")]
    [InlineData(StatusPedido.Finalizado, "Finalizado")]
    [InlineData(StatusPedido.Cancelado, "Cancelado")]
    public void StatusNome_DeveRetornarNomeCorretoDoStatus(StatusPedido status, string nomeEsperado)
    {
        // Arrange
        var dto = new PedidoOutputDto { Status = status };

        // Act & Assert
        dto.StatusNome.Should().Be(nomeEsperado);
    }

    [Fact]
    public void PedidoOutputDto_DevePermitirClienteIdNulo()
    {
        // Arrange & Act
        var dto = new PedidoOutputDto { ClienteId = null };

        // Assert
        dto.ClienteId.Should().BeNull();
    }

    [Fact]
    public void PedidoOutputDto_DevePermitirAdicionarItens()
    {
        // Arrange
        var dto = new PedidoOutputDto();
        var item = new ItemPedidoOutputDto
        {
            ProdutoId = Guid.NewGuid(),
            Quantidade = 2,
            PrecoUnitario = 25.00m,
            DescontoUnitario = 2.50m
        };

        // Act
        dto.Itens.Add(item);

        // Assert
        dto.Itens.Should().HaveCount(1);
        dto.Itens.First().Should().Be(item);
    }

    #endregion
}
