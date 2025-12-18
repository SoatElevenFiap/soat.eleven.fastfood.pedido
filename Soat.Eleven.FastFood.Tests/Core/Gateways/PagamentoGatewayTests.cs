using FluentAssertions;
using Moq;
using Soat.Eleven.FastFood.Core.DTOs.Pagamentos;
using Soat.Eleven.FastFood.Core.Gateways;
using Soat.Eleven.FastFood.Core.Interfaces.Services;
using Xunit;

namespace Soat.Eleven.FastFood.Tests.Core.Gateways;

public class PagamentoGatewayTests
{
    private readonly Mock<IPagamentoService> _pagamentoServiceMock;
    private readonly PagamentoGateway _gateway;
    private const string ClientId = "test-client-id";

    public PagamentoGatewayTests()
    {
        _pagamentoServiceMock = new Mock<IPagamentoService>();
        _gateway = new PagamentoGateway(_pagamentoServiceMock.Object);
    }

    #region CriarOrdemPagamentoAsync Tests

    [Fact]
    public async Task CriarOrdemPagamentoAsync_DeveRetornarOrdemPagamentoResponse_QuandoSucesso()
    {
        // Arrange
        var pedidoId = Guid.NewGuid();
        var itens = CriarItensValidos();
        var pagamentoResponse = CriarOrdemPagamentoResponseValido(pedidoId);

        _pagamentoServiceMock
            .Setup(x => x.CriarOrdemPagamentoAsync(It.IsAny<CriarOrdemPagamentoRequest>()))
            .ReturnsAsync(pagamentoResponse);

        // Act
        var resultado = await _gateway.CriarOrdemPagamentoAsync(pedidoId, ClientId, itens);

        // Assert
        resultado.Should().NotBeNull();
        resultado!.PedidoId.Should().Be(pedidoId);
        resultado.RedirectUrl.Should().Be(pagamentoResponse.RedirectUrl);
    }

    [Fact]
    public async Task CriarOrdemPagamentoAsync_DeveRetornarNull_QuandoClientIdVazio()
    {
        // Arrange
        var pedidoId = Guid.NewGuid();
        var itens = CriarItensValidos();

        // Act
        var resultado = await _gateway.CriarOrdemPagamentoAsync(pedidoId, string.Empty, itens);

        // Assert
        resultado.Should().BeNull();
        _pagamentoServiceMock.Verify(
            x => x.CriarOrdemPagamentoAsync(It.IsAny<CriarOrdemPagamentoRequest>()),
            Times.Never);
    }

    [Fact]
    public async Task CriarOrdemPagamentoAsync_DeveRetornarNull_QuandoClientIdNull()
    {
        // Arrange
        var pedidoId = Guid.NewGuid();
        var itens = CriarItensValidos();

        // Act
        var resultado = await _gateway.CriarOrdemPagamentoAsync(pedidoId, null!, itens);

        // Assert
        resultado.Should().BeNull();
    }

    [Fact]
    public async Task CriarOrdemPagamentoAsync_DeveChamarServiceComRequestCorreto()
    {
        // Arrange
        var pedidoId = Guid.NewGuid();
        var itens = CriarItensValidos();
        var pagamentoResponse = CriarOrdemPagamentoResponseValido(pedidoId);

        _pagamentoServiceMock
            .Setup(x => x.CriarOrdemPagamentoAsync(It.IsAny<CriarOrdemPagamentoRequest>()))
            .ReturnsAsync(pagamentoResponse);

        // Act
        await _gateway.CriarOrdemPagamentoAsync(pedidoId, ClientId, itens);

        // Assert
        _pagamentoServiceMock.Verify(x => x.CriarOrdemPagamentoAsync(
            It.Is<CriarOrdemPagamentoRequest>(r =>
                r.EndToEndId == pedidoId.ToString() &&
                r.ClientId == ClientId &&
                r.Items.Count == itens.Count
            )), Times.Once);
    }

    [Fact]
    public async Task CriarOrdemPagamentoAsync_DevePassarItensCorretos()
    {
        // Arrange
        var pedidoId = Guid.NewGuid();
        var itens = CriarItensValidos();
        var pagamentoResponse = CriarOrdemPagamentoResponseValido(pedidoId);
        CriarOrdemPagamentoRequest? requestCapturado = null;

        _pagamentoServiceMock
            .Setup(x => x.CriarOrdemPagamentoAsync(It.IsAny<CriarOrdemPagamentoRequest>()))
            .Callback<CriarOrdemPagamentoRequest>(r => requestCapturado = r)
            .ReturnsAsync(pagamentoResponse);

        // Act
        await _gateway.CriarOrdemPagamentoAsync(pedidoId, ClientId, itens);

        // Assert
        requestCapturado.Should().NotBeNull();
        requestCapturado!.Items.Should().BeEquivalentTo(itens);
    }

    [Fact]
    public async Task CriarOrdemPagamentoAsync_DeveProcessarListaVaziaDeItens()
    {
        // Arrange
        var pedidoId = Guid.NewGuid();
        var itens = new List<ItemPagamento>();
        var pagamentoResponse = CriarOrdemPagamentoResponseValido(pedidoId);

        _pagamentoServiceMock
            .Setup(x => x.CriarOrdemPagamentoAsync(It.IsAny<CriarOrdemPagamentoRequest>()))
            .ReturnsAsync(pagamentoResponse);

        // Act
        var resultado = await _gateway.CriarOrdemPagamentoAsync(pedidoId, ClientId, itens);

        // Assert
        resultado.Should().NotBeNull();
        _pagamentoServiceMock.Verify(x => x.CriarOrdemPagamentoAsync(
            It.Is<CriarOrdemPagamentoRequest>(r => r.Items.Count == 0)
        ), Times.Once);
    }

    [Fact]
    public async Task CriarOrdemPagamentoAsync_DeveRetornarRedirectUrl_QuandoDisponivel()
    {
        // Arrange
        var pedidoId = Guid.NewGuid();
        var itens = CriarItensValidos();
        var expectedRedirectUrl = "https://pagamento.com/pay/abc123";
        var pagamentoResponse = new OrdemPagamentoResponse
        {
            Id = Guid.NewGuid().ToString(),
            PedidoId = pedidoId,
            Valor = 100.00m,
            Status = "pending",
            RedirectUrl = expectedRedirectUrl,
            CreatedAt = DateTime.Now
        };

        _pagamentoServiceMock
            .Setup(x => x.CriarOrdemPagamentoAsync(It.IsAny<CriarOrdemPagamentoRequest>()))
            .ReturnsAsync(pagamentoResponse);

        // Act
        var resultado = await _gateway.CriarOrdemPagamentoAsync(pedidoId, ClientId, itens);

        // Assert
        resultado.Should().NotBeNull();
        resultado!.RedirectUrl.Should().Be(expectedRedirectUrl);
    }

    #endregion

    #region Helper Methods

    private static List<ItemPagamento> CriarItensValidos()
    {
        return new List<ItemPagamento>
        {
            new()
            {
                Id = Guid.NewGuid().ToString(),
                Title = "Produto Teste",
                Quantity = 2,
                UnitPrice = 50.00m
            }
        };
    }

    private static OrdemPagamentoResponse CriarOrdemPagamentoResponseValido(Guid pedidoId)
    {
        return new OrdemPagamentoResponse
        {
            Id = Guid.NewGuid().ToString(),
            PedidoId = pedidoId,
            Valor = 100.00m,
            Status = "pending",
            RedirectUrl = "https://pagamento.com/redirect",
            CreatedAt = DateTime.Now
        };
    }

    #endregion
}
