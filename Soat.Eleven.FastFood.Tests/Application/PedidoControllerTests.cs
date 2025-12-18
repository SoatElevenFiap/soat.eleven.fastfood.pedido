using FluentAssertions;
using Moq;
using Soat.Eleven.FastFood.Application.Controllers;
using Soat.Eleven.FastFood.Core.DTOs.Pagamentos;
using Soat.Eleven.FastFood.Core.DTOs.Pedidos;
using Soat.Eleven.FastFood.Core.Entities;
using Soat.Eleven.FastFood.Core.Enums;
using Soat.Eleven.FastFood.Core.Interfaces.DataSources;
using Soat.Eleven.FastFood.Core.Interfaces.Services;
using Xunit;

namespace Soat.Eleven.FastFood.Tests.Application;

public class PedidoControllerTests
{
    private readonly Mock<IPedidoDataSource> _pedidoDataSourceMock;
    private readonly Mock<IPagamentoService> _pagamentoServiceMock;
    private readonly PedidoController _controller;
    private readonly PedidoController _controllerWithPayment;
    private const string ClientId = "test-client-id";

    public PedidoControllerTests()
    {
        _pedidoDataSourceMock = new Mock<IPedidoDataSource>();
        _pagamentoServiceMock = new Mock<IPagamentoService>();
        
        _pedidoDataSourceMock
            .Setup(x => x.GetClientId())
            .Returns(ClientId);
        
        _controller = new PedidoController(_pedidoDataSourceMock.Object);
        _controllerWithPayment = new PedidoController(
            _pedidoDataSourceMock.Object,
            _pagamentoServiceMock.Object
        );
    }

    #region CriarPedido Tests

    [Fact]
    public async Task CriarPedido_DeveRetornarCriarPedidoOutputDto_QuandoDadosValidos()
    {
        // Arrange
        var inputDto = CriarPedidoInputDtoValido();
        var outputDto = CriarPedidoOutputDtoValido(inputDto);

        _pedidoDataSourceMock
            .Setup(x => x.AddAsync(It.IsAny<PedidoInputDto>()))
            .ReturnsAsync(outputDto);

        // Act
        var resultado = await _controller.CriarPedido(inputDto);

        // Assert
        resultado.Should().NotBeNull();
        resultado.TokenAtendimentoId.Should().Be(inputDto.TokenAtendimentoId);
        resultado.RedirectUrl.Should().BeNull();
        _pedidoDataSourceMock.Verify(x => x.AddAsync(It.IsAny<PedidoInputDto>()), Times.Once);
    }

    [Fact]
    public async Task CriarPedido_DeveRetornarRedirectUrl_QuandoPagamentoServiceConfigurado()
    {
        // Arrange
        var inputDto = CriarPedidoInputDtoValido();
        var outputDto = CriarPedidoOutputDtoValido(inputDto);
        var pagamentoResponse = CriarOrdemPagamentoResponseValido();

        _pedidoDataSourceMock
            .Setup(x => x.AddAsync(It.IsAny<PedidoInputDto>()))
            .ReturnsAsync(outputDto);

        _pagamentoServiceMock
            .Setup(x => x.CriarOrdemPagamentoAsync(It.IsAny<CriarOrdemPagamentoRequest>()))
            .ReturnsAsync(pagamentoResponse);

        // Act
        var resultado = await _controllerWithPayment.CriarPedido(inputDto);

        // Assert
        resultado.Should().NotBeNull();
        resultado.RedirectUrl.Should().Be(pagamentoResponse.RedirectUrl);
        _pagamentoServiceMock.Verify(x => x.CriarOrdemPagamentoAsync(It.IsAny<CriarOrdemPagamentoRequest>()), Times.Once);
    }

    [Fact]
    public async Task CriarPedido_DeveChamarDataSourceAddAsync()
    {
        // Arrange
        var inputDto = CriarPedidoInputDtoValido();
        var outputDto = CriarPedidoOutputDtoValido(inputDto);

        _pedidoDataSourceMock
            .Setup(x => x.AddAsync(It.IsAny<PedidoInputDto>()))
            .ReturnsAsync(outputDto);

        // Act
        await _controller.CriarPedido(inputDto);

        // Assert
        _pedidoDataSourceMock.Verify(x => x.AddAsync(It.IsAny<PedidoInputDto>()), Times.Once);
    }

    [Fact]
    public async Task CriarPedido_DeveCriarOrdemPagamentoComDadosCorretos_QuandoPagamentoServiceConfigurado()
    {
        // Arrange
        var inputDto = CriarPedidoInputDtoValido();
        var outputDto = CriarPedidoOutputDtoValido(inputDto);
        var pagamentoResponse = CriarOrdemPagamentoResponseValido();

        _pedidoDataSourceMock
            .Setup(x => x.AddAsync(It.IsAny<PedidoInputDto>()))
            .ReturnsAsync(outputDto);

        _pagamentoServiceMock
            .Setup(x => x.CriarOrdemPagamentoAsync(It.IsAny<CriarOrdemPagamentoRequest>()))
            .ReturnsAsync(pagamentoResponse);

        // Act
        await _controllerWithPayment.CriarPedido(inputDto);

        // Assert
        _pagamentoServiceMock.Verify(x => x.CriarOrdemPagamentoAsync(
            It.Is<CriarOrdemPagamentoRequest>(r =>
                r.ClientId == ClientId &&
                r.EndToEndId == outputDto.Id.ToString() &&
                r.Items.Count == outputDto.Itens.Count
            )), Times.Once);
    }

    #endregion

    #region AtualizarPedido Tests

    [Fact]
    public async Task AtualizarPedido_DeveRetornarPedidoAtualizado_QuandoStatusPendente()
    {
        // Arrange
        var inputDto = CriarPedidoInputDtoValido();
        var outputDto = CriarPedidoOutputDtoValido(inputDto);
        outputDto.Status = StatusPedido.Pendente;

        _pedidoDataSourceMock
            .Setup(x => x.GetByIdAsync(inputDto.Id))
            .ReturnsAsync(outputDto);

        _pedidoDataSourceMock
            .Setup(x => x.UpdateAsync(It.IsAny<PedidoInputDto>()))
            .Returns(Task.CompletedTask);

        // Act
        var resultado = await _controller.AtualizarPedido(inputDto);

        // Assert
        resultado.Should().NotBeNull();
        _pedidoDataSourceMock.Verify(x => x.UpdateAsync(It.IsAny<PedidoInputDto>()), Times.Once);
    }

    [Fact]
    public async Task AtualizarPedido_DeveLancarExcecao_QuandoPedidoNaoEncontrado()
    {
        // Arrange
        var inputDto = CriarPedidoInputDtoValido();

        _pedidoDataSourceMock
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((PedidoOutputDto?)null);

        // Act
        var act = () => _controller.AtualizarPedido(inputDto);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    #endregion

    #region ListarPedidos Tests

    [Fact]
    public async Task ListarPedidos_DeveRetornarListaDePedidos()
    {
        // Arrange
        var pedidos = new List<PedidoOutputDto>
        {
            CriarPedidoOutputDtoValido(CriarPedidoInputDtoValido()),
            CriarPedidoOutputDtoValido(CriarPedidoInputDtoValido())
        };

        _pedidoDataSourceMock
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(pedidos);

        // Act
        var resultado = await _controller.ListarPedidos();

        // Assert
        resultado.Should().NotBeNull();
        _pedidoDataSourceMock.Verify(x => x.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task ListarPedidos_DeveRetornarListaVazia_QuandoNaoExistemPedidos()
    {
        // Arrange
        _pedidoDataSourceMock
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(new List<PedidoOutputDto>());

        // Act
        var resultado = await _controller.ListarPedidos();

        // Assert
        resultado.Should().BeEmpty();
    }

    #endregion

    #region ObterPedidoPorId Tests

    [Fact]
    public async Task ObterPedidoPorId_DeveRetornarPedido_QuandoEncontrado()
    {
        // Arrange
        var pedidoId = Guid.NewGuid();
        var outputDto = CriarPedidoOutputDtoValido(CriarPedidoInputDtoValido());
        outputDto.Id = pedidoId;

        _pedidoDataSourceMock
            .Setup(x => x.GetByIdAsync(pedidoId))
            .ReturnsAsync(outputDto);

        // Act
        var resultado = await _controller.ObterPedidoPorId(pedidoId);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Id.Should().Be(pedidoId);
    }

    [Fact]
    public async Task ObterPedidoPorId_DeveRetornarDtoVazio_QuandoNaoEncontrado()
    {
        // Arrange
        _pedidoDataSourceMock
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((PedidoOutputDto?)null);

        // Act
        var resultado = await _controller.ObterPedidoPorId(Guid.NewGuid());

        // Assert
        resultado.Should().NotBeNull();
        resultado.Id.Should().Be(Guid.Empty);
    }

    #endregion

    #region IniciarPreparacaoPedido Tests

    [Fact]
    public async Task IniciarPreparacaoPedido_DeveAtualizarStatus_QuandoStatusRecebido()
    {
        // Arrange
        var pedidoId = Guid.NewGuid();
        var outputDto = CriarPedidoOutputDtoValido(CriarPedidoInputDtoValido());
        outputDto.Id = pedidoId;
        outputDto.Status = StatusPedido.Recebido;

        _pedidoDataSourceMock
            .Setup(x => x.GetByIdAsync(pedidoId))
            .ReturnsAsync(outputDto);

        _pedidoDataSourceMock
            .Setup(x => x.UpdateAsync(It.IsAny<PedidoInputDto>()))
            .Returns(Task.CompletedTask);

        // Act
        await _controller.IniciarPreparacaoPedido(pedidoId);

        // Assert
        _pedidoDataSourceMock.Verify(x => x.UpdateAsync(It.Is<PedidoInputDto>(p => p.Status == StatusPedido.EmPreparacao)), Times.Once);
    }

    [Fact]
    public async Task IniciarPreparacaoPedido_DeveLancarExcecao_QuandoStatusInvalido()
    {
        // Arrange
        var pedidoId = Guid.NewGuid();
        var outputDto = CriarPedidoOutputDtoValido(CriarPedidoInputDtoValido());
        outputDto.Id = pedidoId;
        outputDto.Status = StatusPedido.Pendente;

        _pedidoDataSourceMock
            .Setup(x => x.GetByIdAsync(pedidoId))
            .ReturnsAsync(outputDto);

        // Act
        var act = () => _controller.IniciarPreparacaoPedido(pedidoId);

        // Assert
        await act.Should().ThrowAsync<Exception>();
    }

    #endregion

    #region FinalizarPreparacao Tests

    [Fact]
    public async Task FinalizarPreparacao_DeveAtualizarStatus_QuandoStatusEmPreparacao()
    {
        // Arrange
        var pedidoId = Guid.NewGuid();
        var outputDto = CriarPedidoOutputDtoValido(CriarPedidoInputDtoValido());
        outputDto.Id = pedidoId;
        outputDto.Status = StatusPedido.EmPreparacao;

        _pedidoDataSourceMock
            .Setup(x => x.GetByIdAsync(pedidoId))
            .ReturnsAsync(outputDto);

        _pedidoDataSourceMock
            .Setup(x => x.UpdateAsync(It.IsAny<PedidoInputDto>()))
            .Returns(Task.CompletedTask);

        // Act
        await _controller.FinalizarPreparacao(pedidoId);

        // Assert
        _pedidoDataSourceMock.Verify(x => x.UpdateAsync(It.Is<PedidoInputDto>(p => p.Status == StatusPedido.Pronto)), Times.Once);
    }

    [Fact]
    public async Task FinalizarPreparacao_DeveLancarExcecao_QuandoStatusInvalido()
    {
        // Arrange
        var pedidoId = Guid.NewGuid();
        var outputDto = CriarPedidoOutputDtoValido(CriarPedidoInputDtoValido());
        outputDto.Id = pedidoId;
        outputDto.Status = StatusPedido.Pendente;

        _pedidoDataSourceMock
            .Setup(x => x.GetByIdAsync(pedidoId))
            .ReturnsAsync(outputDto);

        // Act
        var act = () => _controller.FinalizarPreparacao(pedidoId);

        // Assert
        await act.Should().ThrowAsync<Exception>();
    }

    #endregion

    #region FinalizarPedido Tests

    [Fact]
    public async Task FinalizarPedido_DeveAtualizarStatus_QuandoStatusPronto()
    {
        // Arrange
        var pedidoId = Guid.NewGuid();
        var outputDto = CriarPedidoOutputDtoValido(CriarPedidoInputDtoValido());
        outputDto.Id = pedidoId;
        outputDto.Status = StatusPedido.Pronto;

        _pedidoDataSourceMock
            .Setup(x => x.GetByIdAsync(pedidoId))
            .ReturnsAsync(outputDto);

        _pedidoDataSourceMock
            .Setup(x => x.UpdateAsync(It.IsAny<PedidoInputDto>()))
            .Returns(Task.CompletedTask);

        // Act
        await _controller.FinalizarPedido(pedidoId);

        // Assert
        _pedidoDataSourceMock.Verify(x => x.UpdateAsync(It.Is<PedidoInputDto>(p => p.Status == StatusPedido.Finalizado)), Times.Once);
    }

    [Fact]
    public async Task FinalizarPedido_DeveLancarExcecao_QuandoStatusInvalido()
    {
        // Arrange
        var pedidoId = Guid.NewGuid();
        var outputDto = CriarPedidoOutputDtoValido(CriarPedidoInputDtoValido());
        outputDto.Id = pedidoId;
        outputDto.Status = StatusPedido.EmPreparacao;

        _pedidoDataSourceMock
            .Setup(x => x.GetByIdAsync(pedidoId))
            .ReturnsAsync(outputDto);

        // Act
        var act = () => _controller.FinalizarPedido(pedidoId);

        // Assert
        await act.Should().ThrowAsync<Exception>();
    }

    #endregion

    #region CancelarPedido Tests

    [Fact]
    public async Task CancelarPedido_DeveAtualizarStatus_QuandoStatusValido()
    {
        // Arrange
        var pedidoId = Guid.NewGuid();
        var outputDto = CriarPedidoOutputDtoValido(CriarPedidoInputDtoValido());
        outputDto.Id = pedidoId;
        outputDto.Status = StatusPedido.Pendente;

        _pedidoDataSourceMock
            .Setup(x => x.GetByIdAsync(pedidoId))
            .ReturnsAsync(outputDto);

        _pedidoDataSourceMock
            .Setup(x => x.UpdateAsync(It.IsAny<PedidoInputDto>()))
            .Returns(Task.CompletedTask);

        // Act
        await _controller.CancelarPedido(pedidoId);

        // Assert
        _pedidoDataSourceMock.Verify(x => x.UpdateAsync(It.Is<PedidoInputDto>(p => p.Status == StatusPedido.Cancelado)), Times.Once);
    }

    [Fact]
    public async Task CancelarPedido_DeveLancarExcecao_QuandoStatusFinalizado()
    {
        // Arrange
        var pedidoId = Guid.NewGuid();
        var outputDto = CriarPedidoOutputDtoValido(CriarPedidoInputDtoValido());
        outputDto.Id = pedidoId;
        outputDto.Status = StatusPedido.Finalizado;

        _pedidoDataSourceMock
            .Setup(x => x.GetByIdAsync(pedidoId))
            .ReturnsAsync(outputDto);

        // Act
        var act = () => _controller.CancelarPedido(pedidoId);

        // Assert
        await act.Should().ThrowAsync<Exception>();
    }

    #endregion

    #region Helper Methods

    private static PedidoInputDto CriarPedidoInputDtoValido()
    {
        return new PedidoInputDto
        {
            Id = Guid.NewGuid(),
            TokenAtendimentoId = Guid.NewGuid(),
            ClienteId = Guid.NewGuid(),
            Subtotal = 100.00m,
            Desconto = 10.00m,
            Total = 90.00m,
            Status = StatusPedido.Pendente,
            Itens = new List<ItemPedidoInputDto>
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

    private static PedidoOutputDto CriarPedidoOutputDtoValido(PedidoInputDto input)
    {
        return new PedidoOutputDto
        {
            Id = input.Id,
            TokenAtendimentoId = input.TokenAtendimentoId,
            ClienteId = input.ClienteId,
            Subtotal = input.Subtotal,
            Desconto = input.Desconto,
            Total = input.Total,
            Status = input.Status,
            SenhaPedido = "TEST123456",
            CriadoEm = DateTime.Now,
            Itens = input.Itens.Select(i => new ItemPedidoOutputDto
            {
                ProdutoId = i.ProdutoId,
                Quantidade = i.Quantidade,
                PrecoUnitario = i.PrecoUnitario,
                DescontoUnitario = i.DescontoUnitario
            }).ToList()
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
