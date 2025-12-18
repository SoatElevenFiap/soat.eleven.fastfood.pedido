using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Soat.Eleven.FastFood.Api.Controllers;
using Soat.Eleven.FastFood.Core.DTOs.Pagamentos;
using Soat.Eleven.FastFood.Core.DTOs.Pedidos;
using Soat.Eleven.FastFood.Core.Enums;
using Soat.Eleven.FastFood.Core.Interfaces.DataSources;
using Soat.Eleven.FastFood.Core.Interfaces.Services;
using Xunit;

namespace Soat.Eleven.FastFood.Tests.Api.Endpoints;

public class PedidoRestEndpointsTests
{
    private readonly Mock<ILogger<PedidoRestEndpoints>> _loggerMock;
    private readonly Mock<IPedidoDataSource> _pedidoDataSourceMock;
    private readonly Mock<IPagamentoService> _pagamentoServiceMock;
    private readonly PedidoRestEndpoints _endpoint;

    public PedidoRestEndpointsTests()
    {
        _loggerMock = new Mock<ILogger<PedidoRestEndpoints>>();
        _pedidoDataSourceMock = new Mock<IPedidoDataSource>();
        _pagamentoServiceMock = new Mock<IPagamentoService>();
        
        _pedidoDataSourceMock
            .Setup(x => x.GetClientId())
            .Returns("test-client-id");

        _endpoint = new PedidoRestEndpoints(
            _loggerMock.Object,
            _pedidoDataSourceMock.Object,
            _pagamentoServiceMock.Object
        );
    }

    #region CriarPedido Tests

    [Fact]
    public async Task CriarPedido_DeveRetornarOk_QuandoSucesso()
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
        var resultado = await _endpoint.CriarPedido(inputDto);

        // Assert
        resultado.Should().BeOfType<OkObjectResult>();
        var okResult = resultado as OkObjectResult;
        okResult!.Value.Should().NotBeNull();
    }

    [Fact]
    public async Task CriarPedido_DeveRetornarResponseComRedirectUrl_QuandoSucesso()
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
        var resultado = await _endpoint.CriarPedido(inputDto);

        // Assert
        var okResult = resultado as OkObjectResult;
        var pedidoCriado = okResult!.Value as CriarPedidoOutputDto;
        pedidoCriado!.RedirectUrl.Should().Be(pagamentoResponse.RedirectUrl);
    }

    [Fact]
    public async Task CriarPedido_DeveRetornarStatusCode500_QuandoExcecao()
    {
        // Arrange
        var inputDto = CriarPedidoInputDtoValido();

        _pedidoDataSourceMock
            .Setup(x => x.AddAsync(It.IsAny<PedidoInputDto>()))
            .ThrowsAsync(new Exception("Erro simulado"));

        // Act
        var resultado = await _endpoint.CriarPedido(inputDto);

        // Assert
        resultado.Should().BeOfType<ObjectResult>();
        var objectResult = resultado as ObjectResult;
        objectResult!.StatusCode.Should().Be(500);
    }

    [Fact]
    public async Task CriarPedido_DeveChamarPagamentoService()
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
        await _endpoint.CriarPedido(inputDto);

        // Assert
        _pagamentoServiceMock.Verify(x => x.CriarOrdemPagamentoAsync(It.IsAny<CriarOrdemPagamentoRequest>()), Times.Once);
    }

    #endregion

    #region ListarPedidos Tests

    [Fact]
    public async Task ListarPedidos_DeveRetornarOk_QuandoSucesso()
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
        var resultado = await _endpoint.ListarPedidos();

        // Assert
        resultado.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task ListarPedidos_DeveRetornarStatusCode500_QuandoExcecao()
    {
        // Arrange
        _pedidoDataSourceMock
            .Setup(x => x.GetAllAsync())
            .ThrowsAsync(new Exception("Erro simulado"));

        // Act
        var resultado = await _endpoint.ListarPedidos();

        // Assert
        resultado.Should().BeOfType<ObjectResult>();
        var objectResult = resultado as ObjectResult;
        objectResult!.StatusCode.Should().Be(500);
    }

    #endregion

    #region ObterPedidoPorId Tests

    [Fact]
    public async Task ObterPedidoPorId_DeveRetornarOk_QuandoSucesso()
    {
        // Arrange
        var pedidoId = Guid.NewGuid();
        var outputDto = CriarPedidoOutputDtoValido(CriarPedidoInputDtoValido());
        outputDto.Id = pedidoId;

        _pedidoDataSourceMock
            .Setup(x => x.GetByIdAsync(pedidoId))
            .ReturnsAsync(outputDto);

        // Act
        var resultado = await _endpoint.ObterPedidoPorId(pedidoId);

        // Assert
        resultado.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task ObterPedidoPorId_DeveRetornarStatusCode500_QuandoExcecao()
    {
        // Arrange
        _pedidoDataSourceMock
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
            .ThrowsAsync(new Exception("Erro simulado"));

        // Act
        var resultado = await _endpoint.ObterPedidoPorId(Guid.NewGuid());

        // Assert
        resultado.Should().BeOfType<ObjectResult>();
        var objectResult = resultado as ObjectResult;
        objectResult!.StatusCode.Should().Be(500);
    }

    #endregion

    #region AtualizarPedido Tests

    [Fact]
    public async Task AtualizarPedido_DeveRetornarOk_QuandoSucesso()
    {
        // Arrange
        var pedidoId = Guid.NewGuid();
        var inputDto = CriarPedidoInputDtoValido();
        var outputDto = CriarPedidoOutputDtoValido(inputDto);
        outputDto.Id = pedidoId;
        outputDto.Status = StatusPedido.Pendente;

        _pedidoDataSourceMock
            .Setup(x => x.GetByIdAsync(pedidoId))
            .ReturnsAsync(outputDto);

        _pedidoDataSourceMock
            .Setup(x => x.UpdateAsync(It.IsAny<PedidoInputDto>()))
            .Returns(Task.CompletedTask);

        // Act
        var resultado = await _endpoint.AtualizarPedido(pedidoId, inputDto);

        // Assert
        resultado.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task AtualizarPedido_DeveRetornarStatusCode500_QuandoExcecao()
    {
        // Arrange
        var pedidoId = Guid.NewGuid();
        var inputDto = CriarPedidoInputDtoValido();

        _pedidoDataSourceMock
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
            .ThrowsAsync(new Exception("Erro simulado"));

        // Act
        var resultado = await _endpoint.AtualizarPedido(pedidoId, inputDto);

        // Assert
        resultado.Should().BeOfType<ObjectResult>();
        var objectResult = resultado as ObjectResult;
        objectResult!.StatusCode.Should().Be(500);
    }

    [Fact]
    public async Task AtualizarPedido_DeveDefinirIdNoDto()
    {
        // Arrange
        var pedidoId = Guid.NewGuid();
        var inputDto = CriarPedidoInputDtoValido();
        var outputDto = CriarPedidoOutputDtoValido(inputDto);
        outputDto.Id = pedidoId;
        outputDto.Status = StatusPedido.Pendente;

        _pedidoDataSourceMock
            .Setup(x => x.GetByIdAsync(pedidoId))
            .ReturnsAsync(outputDto);

        _pedidoDataSourceMock
            .Setup(x => x.UpdateAsync(It.IsAny<PedidoInputDto>()))
            .Returns(Task.CompletedTask);

        // Act
        await _endpoint.AtualizarPedido(pedidoId, inputDto);

        // Assert
        inputDto.Id.Should().Be(pedidoId);
    }

    #endregion

    #region IniciarPreparacaoPedido Tests

    [Fact]
    public async Task IniciarPreparacaoPedido_DeveRetornarNoContent_QuandoSucesso()
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
        var resultado = await _endpoint.IniciarPreparacaoPedido(pedidoId);

        // Assert
        resultado.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task IniciarPreparacaoPedido_DeveRetornarStatusCode500_QuandoExcecao()
    {
        // Arrange
        _pedidoDataSourceMock
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
            .ThrowsAsync(new Exception("Erro simulado"));

        // Act
        var resultado = await _endpoint.IniciarPreparacaoPedido(Guid.NewGuid());

        // Assert
        resultado.Should().BeOfType<ObjectResult>();
        var objectResult = resultado as ObjectResult;
        objectResult!.StatusCode.Should().Be(500);
    }

    #endregion

    #region FinalizarPreparacaoPedido Tests

    [Fact]
    public async Task FinalizarPreparacaoPedido_DeveRetornarNoContent_QuandoSucesso()
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
        var resultado = await _endpoint.FinalizarPreparacaoPedido(pedidoId);

        // Assert
        resultado.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task FinalizarPreparacaoPedido_DeveRetornarStatusCode500_QuandoExcecao()
    {
        // Arrange
        _pedidoDataSourceMock
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
            .ThrowsAsync(new Exception("Erro simulado"));

        // Act
        var resultado = await _endpoint.FinalizarPreparacaoPedido(Guid.NewGuid());

        // Assert
        resultado.Should().BeOfType<ObjectResult>();
        var objectResult = resultado as ObjectResult;
        objectResult!.StatusCode.Should().Be(500);
    }

    #endregion

    #region FinalizarPedido Tests

    [Fact]
    public async Task FinalizarPedido_DeveRetornarNoContent_QuandoSucesso()
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
        var resultado = await _endpoint.FinalizarPedido(pedidoId);

        // Assert
        resultado.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task FinalizarPedido_DeveRetornarStatusCode500_QuandoExcecao()
    {
        // Arrange
        _pedidoDataSourceMock
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
            .ThrowsAsync(new Exception("Erro simulado"));

        // Act
        var resultado = await _endpoint.FinalizarPedido(Guid.NewGuid());

        // Assert
        resultado.Should().BeOfType<ObjectResult>();
        var objectResult = resultado as ObjectResult;
        objectResult!.StatusCode.Should().Be(500);
    }

    #endregion

    #region CancelarPedido Tests

    [Fact]
    public async Task CancelarPedido_DeveRetornarNoContent_QuandoSucesso()
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
        var resultado = await _endpoint.CancelarPedido(pedidoId);

        // Assert
        resultado.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task CancelarPedido_DeveRetornarStatusCode500_QuandoExcecao()
    {
        // Arrange
        _pedidoDataSourceMock
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
            .ThrowsAsync(new Exception("Erro simulado"));

        // Act
        var resultado = await _endpoint.CancelarPedido(Guid.NewGuid());

        // Assert
        resultado.Should().BeOfType<ObjectResult>();
        var objectResult = resultado as ObjectResult;
        objectResult!.StatusCode.Should().Be(500);
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
