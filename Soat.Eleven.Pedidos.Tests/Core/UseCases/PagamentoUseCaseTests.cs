using FluentAssertions;
using Moq;
using Soat.Eleven.Pedidos.Core.DTOs.Pagamentos;
using Soat.Eleven.Pedidos.Core.DTOs.Pedidos;
using Soat.Eleven.Pedidos.Core.Enums;
using Soat.Eleven.Pedidos.Core.Interfaces.Gateways;
using Soat.Eleven.Pedidos.Core.UseCases;
using Xunit;

namespace Soat.Eleven.Pedidos.Tests.Core.UseCases;

public class PagamentoUseCaseTests
{
    private readonly Mock<IPagamentoGateway> _pagamentoGatewayMock;
    private readonly PagamentoUseCase _useCase;
    private const string ClientId = "test-client-id";

    public PagamentoUseCaseTests()
    {
        _pagamentoGatewayMock = new Mock<IPagamentoGateway>();
        _useCase = PagamentoUseCase.Create(_pagamentoGatewayMock.Object);
    }

    #region CriarOrdemPagamento Tests

    [Fact]
    public async Task CriarOrdemPagamento_DeveRetornarOrdemPagamentoResponse_QuandoDadosValidos()
    {
        // Arrange
        var pedidoOutput = CriarPedidoOutputDtoValido();
        var pagamentoResponse = CriarOrdemPagamentoResponseValido();

        _pagamentoGatewayMock
            .Setup(x => x.CriarOrdemPagamentoAsync(
                It.IsAny<Guid>(),
                It.IsAny<string>(),
                It.IsAny<List<ItemPagamento>>()))
            .ReturnsAsync(pagamentoResponse);

        // Act
        var resultado = await _useCase.CriarOrdemPagamento(pedidoOutput, ClientId);

        // Assert
        resultado.Should().NotBeNull();
        resultado!.RedirectUrl.Should().Be(pagamentoResponse.RedirectUrl);
    }

    [Fact]
    public async Task CriarOrdemPagamento_DeveRetornarNull_QuandoGatewayRetornaNull()
    {
        // Arrange
        var pedidoOutput = CriarPedidoOutputDtoValido();

        _pagamentoGatewayMock
            .Setup(x => x.CriarOrdemPagamentoAsync(
                It.IsAny<Guid>(),
                It.IsAny<string>(),
                It.IsAny<List<ItemPagamento>>()))
            .ReturnsAsync((OrdemPagamentoResponse?)null);

        // Act
        var resultado = await _useCase.CriarOrdemPagamento(pedidoOutput, string.Empty);

        // Assert
        resultado.Should().BeNull();
        _pagamentoGatewayMock.Verify(
            x => x.CriarOrdemPagamentoAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<List<ItemPagamento>>()),
            Times.Once);
    }

    [Fact]
    public async Task CriarOrdemPagamento_DeveChamarGatewayMesmoComClientIdVazio()
    {
        // Arrange
        var pedidoOutput = CriarPedidoOutputDtoValido();

        _pagamentoGatewayMock
            .Setup(x => x.CriarOrdemPagamentoAsync(
                It.IsAny<Guid>(),
                It.IsAny<string>(),
                It.IsAny<List<ItemPagamento>>()))
            .ReturnsAsync((OrdemPagamentoResponse?)null);

        // Act
        var resultado = await _useCase.CriarOrdemPagamento(pedidoOutput, null!);

        // Assert
        resultado.Should().BeNull();
        _pagamentoGatewayMock.Verify(
            x => x.CriarOrdemPagamentoAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<List<ItemPagamento>>()),
            Times.Once);
    }

    [Fact]
    public async Task CriarOrdemPagamento_DeveChamarGatewayComDadosCorretos()
    {
        // Arrange
        var pedidoOutput = CriarPedidoOutputDtoValido();
        var pagamentoResponse = CriarOrdemPagamentoResponseValido();

        _pagamentoGatewayMock
            .Setup(x => x.CriarOrdemPagamentoAsync(
                It.IsAny<Guid>(),
                It.IsAny<string>(),
                It.IsAny<List<ItemPagamento>>()))
            .ReturnsAsync(pagamentoResponse);

        // Act
        await _useCase.CriarOrdemPagamento(pedidoOutput, ClientId);

        // Assert
        _pagamentoGatewayMock.Verify(x => x.CriarOrdemPagamentoAsync(
            pedidoOutput.Id,
            ClientId,
            It.Is<List<ItemPagamento>>(items => items.Count == pedidoOutput.Itens.Count)
        ), Times.Once);
    }

    [Fact]
    public async Task CriarOrdemPagamento_DeveMapearItensCorretamente()
    {
        // Arrange
        var pedidoOutput = CriarPedidoOutputDtoValido();
        var pagamentoResponse = CriarOrdemPagamentoResponseValido();
        List<ItemPagamento>? itensCapturados = null;

        _pagamentoGatewayMock
            .Setup(x => x.CriarOrdemPagamentoAsync(
                It.IsAny<Guid>(),
                It.IsAny<string>(),
                It.IsAny<List<ItemPagamento>>()))
            .Callback<Guid, string, List<ItemPagamento>>((id, clientId, itens) => itensCapturados = itens)
            .ReturnsAsync(pagamentoResponse);

        // Act
        await _useCase.CriarOrdemPagamento(pedidoOutput, ClientId);

        // Assert
        itensCapturados.Should().NotBeNull();
        itensCapturados!.Count.Should().Be(pedidoOutput.Itens.Count);
        
        var primeiroItemPedido = pedidoOutput.Itens.First();
        var primeiroItemPagamento = itensCapturados.First();
        
        primeiroItemPagamento.Id.Should().Be(primeiroItemPedido.ProdutoId.ToString());
        primeiroItemPagamento.Quantity.Should().Be(primeiroItemPedido.Quantidade);
        primeiroItemPagamento.UnitPrice.Should().Be(primeiroItemPedido.PrecoUnitario);
    }

    [Fact]
    public async Task CriarOrdemPagamento_DeveRetornarRedirectUrl_QuandoSucesso()
    {
        // Arrange
        var pedidoOutput = CriarPedidoOutputDtoValido();
        var expectedRedirectUrl = "https://pagamento.com/redirect/123";
        var pagamentoResponse = new OrdemPagamentoResponse
        {
            Id = Guid.NewGuid().ToString(),
            PedidoId = pedidoOutput.Id,
            Valor = pedidoOutput.Total,
            Status = "pending",
            RedirectUrl = expectedRedirectUrl,
            CreatedAt = DateTime.Now
        };

        _pagamentoGatewayMock
            .Setup(x => x.CriarOrdemPagamentoAsync(
                It.IsAny<Guid>(),
                It.IsAny<string>(),
                It.IsAny<List<ItemPagamento>>()))
            .ReturnsAsync(pagamentoResponse);

        // Act
        var resultado = await _useCase.CriarOrdemPagamento(pedidoOutput, ClientId);

        // Assert
        resultado.Should().NotBeNull();
        resultado!.RedirectUrl.Should().Be(expectedRedirectUrl);
    }

    [Fact]
    public async Task CriarOrdemPagamento_DeveProcessarPedidoSemItens()
    {
        // Arrange
        var pedidoOutput = CriarPedidoOutputDtoValido();
        pedidoOutput.Itens = new List<ItemPedidoOutputDto>();
        var pagamentoResponse = CriarOrdemPagamentoResponseValido();

        _pagamentoGatewayMock
            .Setup(x => x.CriarOrdemPagamentoAsync(
                It.IsAny<Guid>(),
                It.IsAny<string>(),
                It.IsAny<List<ItemPagamento>>()))
            .ReturnsAsync(pagamentoResponse);

        // Act
        var resultado = await _useCase.CriarOrdemPagamento(pedidoOutput, ClientId);

        // Assert
        resultado.Should().NotBeNull();
        _pagamentoGatewayMock.Verify(x => x.CriarOrdemPagamentoAsync(
            It.IsAny<Guid>(),
            It.IsAny<string>(),
            It.Is<List<ItemPagamento>>(items => items.Count == 0)
        ), Times.Once);
    }

    [Fact]
    public async Task CriarOrdemPagamento_DeveProcessarMultiplosItens()
    {
        // Arrange
        var pedidoOutput = CriarPedidoOutputDtoValido();
        pedidoOutput.Itens = new List<ItemPedidoOutputDto>
        {
            new() { ProdutoId = Guid.NewGuid(), Quantidade = 1, PrecoUnitario = 10.00m, DescontoUnitario = 0 },
            new() { ProdutoId = Guid.NewGuid(), Quantidade = 2, PrecoUnitario = 20.00m, DescontoUnitario = 5.00m },
            new() { ProdutoId = Guid.NewGuid(), Quantidade = 3, PrecoUnitario = 30.00m, DescontoUnitario = 0 }
        };
        var pagamentoResponse = CriarOrdemPagamentoResponseValido();

        _pagamentoGatewayMock
            .Setup(x => x.CriarOrdemPagamentoAsync(
                It.IsAny<Guid>(),
                It.IsAny<string>(),
                It.IsAny<List<ItemPagamento>>()))
            .ReturnsAsync(pagamentoResponse);

        // Act
        var resultado = await _useCase.CriarOrdemPagamento(pedidoOutput, ClientId);

        // Assert
        resultado.Should().NotBeNull();
        _pagamentoGatewayMock.Verify(x => x.CriarOrdemPagamentoAsync(
            It.IsAny<Guid>(),
            It.IsAny<string>(),
            It.Is<List<ItemPagamento>>(items => items.Count == 3)
        ), Times.Once);
    }

    #endregion

    #region Factory Tests

    [Fact]
    public void Create_DeveRetornarInstanciaDePagamentoUseCase()
    {
        // Arrange
        var gatewayMock = new Mock<IPagamentoGateway>();

        // Act
        var useCase = PagamentoUseCase.Create(gatewayMock.Object);

        // Assert
        useCase.Should().NotBeNull();
        useCase.Should().BeOfType<PagamentoUseCase>();
    }

    #endregion

    #region Helper Methods

    private static PedidoOutputDto CriarPedidoOutputDtoValido()
    {
        return new PedidoOutputDto
        {
            Id = Guid.NewGuid(),
            TokenAtendimentoId = Guid.NewGuid(),
            ClienteId = Guid.NewGuid(),
            Subtotal = 100.00m,
            Desconto = 10.00m,
            Total = 90.00m,
            Status = StatusPedido.Pendente,
            SenhaPedido = "TEST123456",
            CriadoEm = DateTime.Now,
            Itens = new List<ItemPedidoOutputDto>
            {
                new()
                {
                    ProdutoId = Guid.NewGuid(),
                    Quantidade = 1,
                    PrecoUnitario = 100.00m,
                    DescontoUnitario = 10.00m
                }
            }
        };
    }

    private static OrdemPagamentoResponse CriarOrdemPagamentoResponseValido()
    {
        return new OrdemPagamentoResponse
        {
            Id = Guid.NewGuid().ToString(),
            PedidoId = Guid.NewGuid(),
            Valor = 90.00m,
            Status = "pending",
            RedirectUrl = "https://pagamento.com/redirect",
            CreatedAt = DateTime.Now
        };
    }

    #endregion
}
