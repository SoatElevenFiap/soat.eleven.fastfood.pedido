using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Soat.Eleven.FastFood.Api.Controllers;
using Soat.Eleven.FastFood.Core.DTOs.Pagamentos;
using Soat.Eleven.FastFood.Core.Enums;
using Soat.Eleven.FastFood.Core.Interfaces.DataSources;
using Xunit;

namespace Soat.Eleven.FastFood.Tests.Api.Endpoints;

public class PagamentoRestEndpointTests
{
    private readonly Mock<ILogger<PagamentoRestEndpoint>> _loggerMock;
    private readonly Mock<IPedidoDataSource> _pedidoDataSourceMock;
    private readonly PagamentoRestEndpoint _endpoint;

    public PagamentoRestEndpointTests()
    {
        _loggerMock = new Mock<ILogger<PagamentoRestEndpoint>>();
        _pedidoDataSourceMock = new Mock<IPedidoDataSource>();
        _endpoint = new PagamentoRestEndpoint(
            _loggerMock.Object,
            _pedidoDataSourceMock.Object
        );
    }

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
}
