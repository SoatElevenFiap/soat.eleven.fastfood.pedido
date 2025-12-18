using FluentAssertions;
using Moq;
using Soat.Eleven.FastFood.Application.Controllers;
using Soat.Eleven.FastFood.Core.DTOs.Pagamentos;
using Soat.Eleven.FastFood.Core.Enums;
using Soat.Eleven.FastFood.Core.Interfaces.DataSources;
using Soat.Eleven.FastFood.Core.Interfaces.Services;
using Xunit;

namespace Soat.Eleven.FastFood.Tests.Application;

public class PagamentoControllerTests
{
    private readonly Mock<IPedidoDataSource> _pedidoDataSourceMock;
    private readonly Mock<IPagamentoService> _pagamentoServiceMock;

    public PagamentoControllerTests()
    {
        _pedidoDataSourceMock = new Mock<IPedidoDataSource>();
        _pagamentoServiceMock = new Mock<IPagamentoService>();
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

    [Fact]
    public void Constructor_DeveCriarInstancia_ComDataSourceEPagamentoService()
    {
        // Act
        var controller = new PagamentoController(_pedidoDataSourceMock.Object, _pagamentoServiceMock.Object);

        // Assert
        controller.Should().NotBeNull();
    }

    #endregion

    #region CriarOrdemPagamento Tests

    [Fact]
    public async Task CriarOrdemPagamento_DeveRetornarOrdemPagamentoResponse_QuandoServicoConfigurado()
    {
        // Arrange
        var controller = new PagamentoController(_pedidoDataSourceMock.Object, _pagamentoServiceMock.Object);
        var request = CriarOrdemPagamentoRequestValido();
        var response = CriarOrdemPagamentoResponseValido();

        _pagamentoServiceMock
            .Setup(x => x.CriarOrdemPagamentoAsync(It.IsAny<CriarOrdemPagamentoRequest>()))
            .ReturnsAsync(response);

        // Act
        var resultado = await controller.CriarOrdemPagamento(request);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Id.Should().Be(response.Id);
        _pagamentoServiceMock.Verify(x => x.CriarOrdemPagamentoAsync(It.IsAny<CriarOrdemPagamentoRequest>()), Times.Once);
    }

    [Fact]
    public async Task CriarOrdemPagamento_DeveLancarInvalidOperationException_QuandoServicoNaoConfigurado()
    {
        // Arrange
        var controller = new PagamentoController(_pedidoDataSourceMock.Object);
        var request = CriarOrdemPagamentoRequestValido();

        // Act
        var act = () => controller.CriarOrdemPagamento(request);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Serviço de pagamento não configurado");
    }

    [Fact]
    public async Task CriarOrdemPagamento_DeveChamarPagamentoService()
    {
        // Arrange
        var controller = new PagamentoController(_pedidoDataSourceMock.Object, _pagamentoServiceMock.Object);
        var request = CriarOrdemPagamentoRequestValido();
        var response = CriarOrdemPagamentoResponseValido();

        _pagamentoServiceMock
            .Setup(x => x.CriarOrdemPagamentoAsync(It.IsAny<CriarOrdemPagamentoRequest>()))
            .ReturnsAsync(response);

        // Act
        await controller.CriarOrdemPagamento(request);

        // Assert
        _pagamentoServiceMock.Verify(x => x.CriarOrdemPagamentoAsync(request), Times.Once);
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
