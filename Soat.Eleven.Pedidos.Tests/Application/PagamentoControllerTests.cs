using FluentAssertions;
using Moq;
using Soat.Eleven.Pedidos.Application.Controllers;
using Soat.Eleven.Pedidos.Core.DTOs.Pagamentos;
using Soat.Eleven.Pedidos.Core.Enums;
using Soat.Eleven.Pedidos.Core.Interfaces.DataSources;
using Xunit;

namespace Soat.Eleven.Pedidos.Tests.Application;

public class PagamentoControllerTests
{
    private readonly Mock<IPedidoDataSource> _pedidoDataSourceMock;

    public PagamentoControllerTests()
    {
        _pedidoDataSourceMock = new Mock<IPedidoDataSource>();
    }

    #region Constructor Tests

    [Fact]
    public void Constructor_DeveCriarInstancia_ComApenasDataSource()
    {
        // Act
        var controller = new PagamentoController(_pedidoDataSourceMock.Object);

        // Assert
        controller.Should().NotBeNull();
    }

    #endregion

    #region AtualizarStatusPagamento Tests

    [Fact]
    public async Task AtualizarStatusPagamento_DeveAtualizarParaRecebido_QuandoStatusPaid()
    {
        // Arrange
        var controller = new PagamentoController(_pedidoDataSourceMock.Object);
        var pedidoId = Guid.NewGuid();
        var notificacao = new ConfirmacaoPagamento { Status = "paid" };

        _pedidoDataSourceMock
            .Setup(x => x.AtualizarStatusAsync(pedidoId, StatusPedido.Recebido))
            .Returns(Task.CompletedTask);

        // Act
        await controller.AtualizarStatusPagamento(notificacao, pedidoId);

        // Assert
        _pedidoDataSourceMock.Verify(x => x.AtualizarStatusAsync(pedidoId, StatusPedido.Recebido), Times.Once);
    }

    [Fact]
    public async Task AtualizarStatusPagamento_DeveAtualizarParaCancelado_QuandoStatusFailed()
    {
        // Arrange
        var controller = new PagamentoController(_pedidoDataSourceMock.Object);
        var pedidoId = Guid.NewGuid();
        var notificacao = new ConfirmacaoPagamento { Status = "failed" };

        _pedidoDataSourceMock
            .Setup(x => x.AtualizarStatusAsync(pedidoId, StatusPedido.Cancelado))
            .Returns(Task.CompletedTask);

        // Act
        await controller.AtualizarStatusPagamento(notificacao, pedidoId);

        // Assert
        _pedidoDataSourceMock.Verify(x => x.AtualizarStatusAsync(pedidoId, StatusPedido.Cancelado), Times.Once);
    }

    [Fact]
    public async Task AtualizarStatusPagamento_DeveAtualizarParaCancelado_QuandoStatusCancelled()
    {
        // Arrange
        var controller = new PagamentoController(_pedidoDataSourceMock.Object);
        var pedidoId = Guid.NewGuid();
        var notificacao = new ConfirmacaoPagamento { Status = "cancelled" };

        _pedidoDataSourceMock
            .Setup(x => x.AtualizarStatusAsync(pedidoId, StatusPedido.Cancelado))
            .Returns(Task.CompletedTask);

        // Act
        await controller.AtualizarStatusPagamento(notificacao, pedidoId);

        // Assert
        _pedidoDataSourceMock.Verify(x => x.AtualizarStatusAsync(pedidoId, StatusPedido.Cancelado), Times.Once);
    }

    [Fact]
    public async Task AtualizarStatusPagamento_DeveAtualizarParaCancelado_QuandoStatusRefunded()
    {
        // Arrange
        var controller = new PagamentoController(_pedidoDataSourceMock.Object);
        var pedidoId = Guid.NewGuid();
        var notificacao = new ConfirmacaoPagamento { Status = "refunded" };

        _pedidoDataSourceMock
            .Setup(x => x.AtualizarStatusAsync(pedidoId, StatusPedido.Cancelado))
            .Returns(Task.CompletedTask);

        // Act
        await controller.AtualizarStatusPagamento(notificacao, pedidoId);

        // Assert
        _pedidoDataSourceMock.Verify(x => x.AtualizarStatusAsync(pedidoId, StatusPedido.Cancelado), Times.Once);
    }

    [Fact]
    public async Task AtualizarStatusPagamento_NaoDeveAtualizarStatus_QuandoStatusPending()
    {
        // Arrange
        var controller = new PagamentoController(_pedidoDataSourceMock.Object);
        var pedidoId = Guid.NewGuid();
        var notificacao = new ConfirmacaoPagamento { Status = "pending" };

        // Act
        await controller.AtualizarStatusPagamento(notificacao, pedidoId);

        // Assert
        _pedidoDataSourceMock.Verify(x => x.AtualizarStatusAsync(It.IsAny<Guid>(), It.IsAny<StatusPedido>()), Times.Never);
    }

    [Fact]
    public async Task AtualizarStatusPagamento_NaoDeveAtualizarStatus_QuandoStatusDesconhecido()
    {
        // Arrange
        var controller = new PagamentoController(_pedidoDataSourceMock.Object);
        var pedidoId = Guid.NewGuid();
        var notificacao = new ConfirmacaoPagamento { Status = "unknown_status" };

        // Act
        await controller.AtualizarStatusPagamento(notificacao, pedidoId);

        // Assert
        _pedidoDataSourceMock.Verify(x => x.AtualizarStatusAsync(It.IsAny<Guid>(), It.IsAny<StatusPedido>()), Times.Never);
    }

    [Fact]
    public async Task AtualizarStatusPagamento_NaoDeveAtualizarStatus_QuandoStatusNull()
    {
        // Arrange
        var controller = new PagamentoController(_pedidoDataSourceMock.Object);
        var pedidoId = Guid.NewGuid();
        var notificacao = new ConfirmacaoPagamento { Status = null! };

        // Act
        await controller.AtualizarStatusPagamento(notificacao, pedidoId);

        // Assert
        _pedidoDataSourceMock.Verify(x => x.AtualizarStatusAsync(It.IsAny<Guid>(), It.IsAny<StatusPedido>()), Times.Never);
    }

    #endregion
}
