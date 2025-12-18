using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Soat.Eleven.FastFood.Api.Controllers;
using Soat.Eleven.FastFood.Core.DTOs.Pagamentos;
using Soat.Eleven.FastFood.Core.Enums;
using Soat.Eleven.FastFood.Core.Interfaces.DataSources;
using Soat.Eleven.FastFood.Core.Interfaces.Services;
using Xunit;

namespace Soat.Eleven.FastFood.Tests.Api.Endpoints;

public class PagamentoRestEndpointTests
{
    private readonly Mock<ILogger<PagamentoRestEndpoint>> _loggerMock;
    private readonly Mock<IPedidoDataSource> _pedidoDataSourceMock;
    private readonly Mock<IPagamentoService> _pagamentoServiceMock;
    private readonly PagamentoRestEndpoint _endpoint;

    public PagamentoRestEndpointTests()
    {
        _loggerMock = new Mock<ILogger<PagamentoRestEndpoint>>();
        _pedidoDataSourceMock = new Mock<IPedidoDataSource>();
        _pagamentoServiceMock = new Mock<IPagamentoService>();
        _endpoint = new PagamentoRestEndpoint(
            _loggerMock.Object,
            _pedidoDataSourceMock.Object,
            _pagamentoServiceMock.Object
        );
    }

    #region CriarOrdemPagamento Tests

    [Fact]
    public async Task CriarOrdemPagamento_DeveRetornarOk_QuandoSucesso()
    {
        // Arrange
        var request = CriarOrdemPagamentoRequestValido();
        var response = CriarOrdemPagamentoResponseValido();

        _pagamentoServiceMock
            .Setup(x => x.CriarOrdemPagamentoAsync(It.IsAny<CriarOrdemPagamentoRequest>()))
            .ReturnsAsync(response);

        // Act
        var resultado = await _endpoint.CriarOrdemPagamento(request);

        // Assert
        resultado.Should().BeOfType<OkObjectResult>();
        var okResult = resultado as OkObjectResult;
        okResult!.Value.Should().NotBeNull();
    }

    [Fact]
    public async Task CriarOrdemPagamento_DeveRetornarStatusCode500_QuandoExcecao()
    {
        // Arrange
        var request = CriarOrdemPagamentoRequestValido();

        _pagamentoServiceMock
            .Setup(x => x.CriarOrdemPagamentoAsync(It.IsAny<CriarOrdemPagamentoRequest>()))
            .ThrowsAsync(new Exception("Erro simulado"));

        // Act
        var resultado = await _endpoint.CriarOrdemPagamento(request);

        // Assert
        resultado.Should().BeOfType<ObjectResult>();
        var objectResult = resultado as ObjectResult;
        objectResult!.StatusCode.Should().Be(500);
    }

    [Fact]
    public async Task CriarOrdemPagamento_DeveChamarPagamentoService()
    {
        // Arrange
        var request = CriarOrdemPagamentoRequestValido();
        var response = CriarOrdemPagamentoResponseValido();

        _pagamentoServiceMock
            .Setup(x => x.CriarOrdemPagamentoAsync(It.IsAny<CriarOrdemPagamentoRequest>()))
            .ReturnsAsync(response);

        // Act
        await _endpoint.CriarOrdemPagamento(request);

        // Assert
        _pagamentoServiceMock.Verify(x => x.CriarOrdemPagamentoAsync(request), Times.Once);
    }

    [Fact]
    public async Task CriarOrdemPagamento_DeveRetornarResponseComDadosCorretos()
    {
        // Arrange
        var request = CriarOrdemPagamentoRequestValido();
        var response = CriarOrdemPagamentoResponseValido();

        _pagamentoServiceMock
            .Setup(x => x.CriarOrdemPagamentoAsync(It.IsAny<CriarOrdemPagamentoRequest>()))
            .ReturnsAsync(response);

        // Act
        var resultado = await _endpoint.CriarOrdemPagamento(request);

        // Assert
        var okResult = resultado as OkObjectResult;
        var responseValue = okResult!.Value as OrdemPagamentoResponse;
        responseValue!.Id.Should().Be(response.Id);
        responseValue.Status.Should().Be(response.Status);
    }

    #endregion

    #region NotificacaoStatusPagamento Tests

    [Fact]
    public async Task NotificacaoStatusPagamento_DeveRetornarOk_QuandoSucesso()
    {
        // Arrange
        var pedidoId = Guid.NewGuid();
        var notificacao = new ConfirmacaoPagamento { Status = "paid" };

        _pedidoDataSourceMock
            .Setup(x => x.AtualizarStatusAsync(pedidoId, StatusPedido.Recebido))
            .Returns(Task.CompletedTask);

        // Act
        var resultado = await _endpoint.NotificacaoStatusPagamento(pedidoId, notificacao);

        // Assert
        resultado.Should().BeOfType<OkResult>();
    }

    [Fact]
    public async Task NotificacaoStatusPagamento_DeveRetornarStatusCode500_QuandoExcecao()
    {
        // Arrange
        var pedidoId = Guid.NewGuid();
        var notificacao = new ConfirmacaoPagamento { Status = "paid" };

        _pedidoDataSourceMock
            .Setup(x => x.AtualizarStatusAsync(It.IsAny<Guid>(), It.IsAny<StatusPedido>()))
            .ThrowsAsync(new Exception("Erro simulado"));

        // Act
        var resultado = await _endpoint.NotificacaoStatusPagamento(pedidoId, notificacao);

        // Assert
        resultado.Should().BeOfType<ObjectResult>();
        var objectResult = resultado as ObjectResult;
        objectResult!.StatusCode.Should().Be(500);
    }

    [Fact]
    public async Task NotificacaoStatusPagamento_DeveAtualizarStatusParaRecebido_QuandoPaid()
    {
        // Arrange
        var pedidoId = Guid.NewGuid();
        var notificacao = new ConfirmacaoPagamento { Status = "paid" };

        _pedidoDataSourceMock
            .Setup(x => x.AtualizarStatusAsync(pedidoId, StatusPedido.Recebido))
            .Returns(Task.CompletedTask);

        // Act
        await _endpoint.NotificacaoStatusPagamento(pedidoId, notificacao);

        // Assert
        _pedidoDataSourceMock.Verify(x => x.AtualizarStatusAsync(pedidoId, StatusPedido.Recebido), Times.Once);
    }

    [Fact]
    public async Task NotificacaoStatusPagamento_DeveAtualizarStatusParaCancelado_QuandoFailed()
    {
        // Arrange
        var pedidoId = Guid.NewGuid();
        var notificacao = new ConfirmacaoPagamento { Status = "failed" };

        _pedidoDataSourceMock
            .Setup(x => x.AtualizarStatusAsync(pedidoId, StatusPedido.Cancelado))
            .Returns(Task.CompletedTask);

        // Act
        await _endpoint.NotificacaoStatusPagamento(pedidoId, notificacao);

        // Assert
        _pedidoDataSourceMock.Verify(x => x.AtualizarStatusAsync(pedidoId, StatusPedido.Cancelado), Times.Once);
    }

    [Fact]
    public async Task NotificacaoStatusPagamento_NaoDeveAtualizarStatus_QuandoPending()
    {
        // Arrange
        var pedidoId = Guid.NewGuid();
        var notificacao = new ConfirmacaoPagamento { Status = "pending" };

        // Act
        var resultado = await _endpoint.NotificacaoStatusPagamento(pedidoId, notificacao);

        // Assert
        resultado.Should().BeOfType<OkResult>();
        _pedidoDataSourceMock.Verify(x => x.AtualizarStatusAsync(It.IsAny<Guid>(), It.IsAny<StatusPedido>()), Times.Never);
    }

    [Fact]
    public async Task NotificacaoStatusPagamento_NaoDeveAtualizarStatus_QuandoStatusDesconhecido()
    {
        // Arrange
        var pedidoId = Guid.NewGuid();
        var notificacao = new ConfirmacaoPagamento { Status = "unknown" };

        // Act
        var resultado = await _endpoint.NotificacaoStatusPagamento(pedidoId, notificacao);

        // Assert
        resultado.Should().BeOfType<OkResult>();
        _pedidoDataSourceMock.Verify(x => x.AtualizarStatusAsync(It.IsAny<Guid>(), It.IsAny<StatusPedido>()), Times.Never);
    }

    #endregion

    #region Helper Methods

    private static CriarOrdemPagamentoRequest CriarOrdemPagamentoRequestValido()
    {
        return new CriarOrdemPagamentoRequest
        {
            EndToEndId = Guid.NewGuid().ToString(),
            ClientId = "client-123",
            Items = new List<ItemPagamento>
            {
                new()
                {
                    Id = Guid.NewGuid().ToString(),
                    Title = "Hamburguer",
                    Quantity = 1,
                    UnitPrice = 25.00m
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
            Valor = 25.00m,
            Status = "pending",
            RedirectUrl = "https://pagamento.com/redirect",
            CreatedAt = DateTime.Now
        };
    }

    #endregion
}
