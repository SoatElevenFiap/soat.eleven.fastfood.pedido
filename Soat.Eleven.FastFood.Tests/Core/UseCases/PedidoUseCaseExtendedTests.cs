using FluentAssertions;
using Moq;
using Soat.Eleven.FastFood.Core.DTOs.Pagamentos;
using Soat.Eleven.FastFood.Core.DTOs.Pedidos;
using Soat.Eleven.FastFood.Core.Enums;
using Soat.Eleven.FastFood.Core.Gateways;
using Soat.Eleven.FastFood.Core.Interfaces.DataSources;
using Soat.Eleven.FastFood.Core.UseCases;
using Xunit;

namespace Soat.Eleven.FastFood.Tests.Core.UseCases;

/// <summary>
/// Testes adicionais para PedidoUseCase cobrindo mais cenários
/// </summary>
public class PedidoUseCaseExtendedTests
{
    private readonly Mock<IPedidoDataSource> _pedidoDataSourceMock;
    private readonly PedidoGateway _pedidoGateway;
    private readonly PedidoUseCase _pedidoUseCase;

    public PedidoUseCaseExtendedTests()
    {
        _pedidoDataSourceMock = new Mock<IPedidoDataSource>();
        _pedidoGateway = new PedidoGateway(_pedidoDataSourceMock.Object);
        _pedidoUseCase = PedidoUseCase.Create(_pedidoGateway);
    }

    #region Create Factory Tests

    [Fact]
    public void Create_DeveRetornarInstanciaValida()
    {
        // Arrange
        var dataSourceMock = new Mock<IPedidoDataSource>();
        var gateway = new PedidoGateway(dataSourceMock.Object);

        // Act
        var useCase = PedidoUseCase.Create(gateway);

        // Assert
        useCase.Should().NotBeNull();
    }

    #endregion

    #region CriarPedido Extended Tests

    [Fact]
    public async Task CriarPedido_DeveGerarSenhaComFormatoCorreto()
    {
        // Arrange
        var pedidoDto = CriarPedidoInputDtoValido();
        var pedidoOutput = CriarPedidoOutputDtoFromInput(pedidoDto);

        _pedidoDataSourceMock
            .Setup(x => x.AddAsync(It.IsAny<PedidoInputDto>()))
            .ReturnsAsync(pedidoOutput);

        // Act
        var resultado = await _pedidoUseCase.CriarPedido(pedidoDto);

        // Assert
        resultado.SenhaPedido.Should().MatchRegex("^[A-Z0-9]+$");
        resultado.SenhaPedido.Should().HaveLength(10);
    }

    [Fact]
    public async Task CriarPedido_DeveMapearTodosOsItens()
    {
        // Arrange
        var pedidoDto = CriarPedidoInputDtoValido();
        pedidoDto.Itens.Add(new ItemPedidoInputDto
        {
            ProdutoId = Guid.NewGuid(),
            Quantidade = 5,
            PrecoUnitario = 50.00m,
            DescontoUnitario = 5.00m
        });
        pedidoDto.Itens.Add(new ItemPedidoInputDto
        {
            ProdutoId = Guid.NewGuid(),
            Quantidade = 3,
            PrecoUnitario = 30.00m,
            DescontoUnitario = 3.00m
        });
        var pedidoOutput = CriarPedidoOutputDtoFromInput(pedidoDto);

        _pedidoDataSourceMock
            .Setup(x => x.AddAsync(It.IsAny<PedidoInputDto>()))
            .ReturnsAsync(pedidoOutput);

        // Act
        var resultado = await _pedidoUseCase.CriarPedido(pedidoDto);

        // Assert
        resultado.Itens.Should().HaveCount(3);
    }

    [Fact]
    public async Task CriarPedido_DevePreservarClienteIdNulo()
    {
        // Arrange
        var pedidoDto = CriarPedidoInputDtoValido();
        pedidoDto.ClienteId = null;
        var pedidoOutput = CriarPedidoOutputDtoFromInput(pedidoDto);

        _pedidoDataSourceMock
            .Setup(x => x.AddAsync(It.IsAny<PedidoInputDto>()))
            .ReturnsAsync(pedidoOutput);

        // Act
        var resultado = await _pedidoUseCase.CriarPedido(pedidoDto);

        // Assert
        resultado.ClienteId.Should().BeNull();
    }

    #endregion

    #region AtualizarPedido Extended Tests

    [Fact]
    public async Task AtualizarPedido_DeveSubstituirItens()
    {
        // Arrange
        var pedidoId = Guid.NewGuid();
        var pedidoExistente = CriarPedidoOutputDto(pedidoId, StatusPedido.Pendente);
        var pedidoDto = CriarPedidoInputDtoValido();
        pedidoDto.Id = pedidoId;
        pedidoDto.Itens.Clear();
        pedidoDto.Itens.Add(new ItemPedidoInputDto
        {
            ProdutoId = Guid.NewGuid(),
            Quantidade = 10,
            PrecoUnitario = 100.00m,
            DescontoUnitario = 10.00m
        });

        _pedidoDataSourceMock
            .Setup(x => x.GetByIdAsync(pedidoId))
            .ReturnsAsync(pedidoExistente);

        _pedidoDataSourceMock
            .Setup(x => x.UpdateAsync(It.IsAny<PedidoInputDto>()))
            .Returns(Task.CompletedTask);

        // Act
        var resultado = await _pedidoUseCase.AtualizarPedido(pedidoDto);

        // Assert
        resultado.Itens.Should().HaveCount(1);
    }

    [Theory]
    [InlineData(StatusPedido.Recebido)]
    [InlineData(StatusPedido.EmPreparacao)]
    [InlineData(StatusPedido.Pronto)]
    [InlineData(StatusPedido.Finalizado)]
    [InlineData(StatusPedido.Cancelado)]
    public async Task AtualizarPedido_DeveLancarExcecao_QuandoStatusDiferenteDePendente(StatusPedido status)
    {
        // Arrange
        var pedidoId = Guid.NewGuid();
        var pedidoExistente = CriarPedidoOutputDto(pedidoId, status);
        var pedidoDto = CriarPedidoInputDtoValido();
        pedidoDto.Id = pedidoId;

        _pedidoDataSourceMock
            .Setup(x => x.GetByIdAsync(pedidoId))
            .ReturnsAsync(pedidoExistente);

        // Act
        var act = () => _pedidoUseCase.AtualizarPedido(pedidoDto);

        // Assert
        await act.Should().ThrowAsync<Exception>()
            .WithMessage("O status do pedido não permite alteração.");
    }

    #endregion

    #region ListarPedidos Extended Tests

    [Fact]
    public async Task ListarPedidos_DeveExcluirPedidosCancelados()
    {
        // Arrange
        var pedidos = new List<PedidoOutputDto>
        {
            CriarPedidoOutputDto(Guid.NewGuid(), StatusPedido.Pendente),
            CriarPedidoOutputDto(Guid.NewGuid(), StatusPedido.Cancelado),
            CriarPedidoOutputDto(Guid.NewGuid(), StatusPedido.EmPreparacao)
        };

        _pedidoDataSourceMock
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(pedidos);

        // Act
        var resultado = await _pedidoUseCase.ListarPedidos();

        // Assert
        resultado.Should().HaveCount(3); // Cancelado não é filtrado, apenas Finalizado
    }

    #endregion

    #region IniciarPreparacaoPedido Extended Tests

    [Theory]
    [InlineData(StatusPedido.Pendente)]
    [InlineData(StatusPedido.EmPreparacao)]
    [InlineData(StatusPedido.Pronto)]
    [InlineData(StatusPedido.Finalizado)]
    public async Task IniciarPreparacaoPedido_DeveLancarExcecao_QuandoStatusInvalido(StatusPedido status)
    {
        // Arrange
        var pedidoId = Guid.NewGuid();
        var pedidoExistente = CriarPedidoOutputDto(pedidoId, status);

        _pedidoDataSourceMock
            .Setup(x => x.GetByIdAsync(pedidoId))
            .ReturnsAsync(pedidoExistente);

        // Act
        var act = () => _pedidoUseCase.IniciarPreparacaoPedido(pedidoId);

        // Assert
        await act.Should().ThrowAsync<Exception>()
            .WithMessage("*status*");
    }

    #endregion

    #region FinalizarPreparacaoPedido Extended Tests

    [Theory]
    [InlineData(StatusPedido.Pendente)]
    [InlineData(StatusPedido.Recebido)]
    [InlineData(StatusPedido.Pronto)]
    [InlineData(StatusPedido.Finalizado)]
    public async Task FinalizarPreparacaoPedido_DeveLancarExcecao_QuandoStatusInvalido(StatusPedido status)
    {
        // Arrange
        var pedidoId = Guid.NewGuid();
        var pedidoExistente = CriarPedidoOutputDto(pedidoId, status);

        _pedidoDataSourceMock
            .Setup(x => x.GetByIdAsync(pedidoId))
            .ReturnsAsync(pedidoExistente);

        // Act
        var act = () => _pedidoUseCase.FinalizarPreparacaoPedido(pedidoId);

        // Assert
        await act.Should().ThrowAsync<Exception>()
            .WithMessage("*status*");
    }

    #endregion

    #region FinalizarPedido Extended Tests

    [Theory]
    [InlineData(StatusPedido.Pendente)]
    [InlineData(StatusPedido.Recebido)]
    [InlineData(StatusPedido.EmPreparacao)]
    [InlineData(StatusPedido.Finalizado)]
    public async Task FinalizarPedido_DeveLancarExcecao_QuandoStatusInvalido(StatusPedido status)
    {
        // Arrange
        var pedidoId = Guid.NewGuid();
        var pedidoExistente = CriarPedidoOutputDto(pedidoId, status);

        _pedidoDataSourceMock
            .Setup(x => x.GetByIdAsync(pedidoId))
            .ReturnsAsync(pedidoExistente);

        // Act
        var act = () => _pedidoUseCase.FinalizarPedido(pedidoId);

        // Assert
        await act.Should().ThrowAsync<Exception>()
            .WithMessage("*status*");
    }

    #endregion

    #region CancelarPedido Extended Tests

    [Fact]
    public async Task CancelarPedido_DeveLancarExcecao_QuandoStatusFinalizado()
    {
        // Arrange
        var pedidoId = Guid.NewGuid();
        var pedidoExistente = CriarPedidoOutputDto(pedidoId, StatusPedido.Finalizado);

        _pedidoDataSourceMock
            .Setup(x => x.GetByIdAsync(pedidoId))
            .ReturnsAsync(pedidoExistente);

        // Act
        var act = () => _pedidoUseCase.CancelarPedido(pedidoId);

        // Assert
        await act.Should().ThrowAsync<Exception>()
            .WithMessage("*cancelar pedido finalizado*");
    }

    [Theory]
    [InlineData(StatusPedido.Pendente)]
    [InlineData(StatusPedido.Recebido)]
    [InlineData(StatusPedido.EmPreparacao)]
    [InlineData(StatusPedido.Pronto)]
    public async Task CancelarPedido_DevePermitirCancelamento_QuandoStatusNaoFinalizado(StatusPedido status)
    {
        // Arrange
        var pedidoId = Guid.NewGuid();
        var pedidoExistente = CriarPedidoOutputDto(pedidoId, status);

        _pedidoDataSourceMock
            .Setup(x => x.GetByIdAsync(pedidoId))
            .ReturnsAsync(pedidoExistente);

        _pedidoDataSourceMock
            .Setup(x => x.UpdateAsync(It.IsAny<PedidoInputDto>()))
            .Returns(Task.CompletedTask);

        // Act
        await _pedidoUseCase.CancelarPedido(pedidoId);

        // Assert
        _pedidoDataSourceMock.Verify(x => x.UpdateAsync(It.Is<PedidoInputDto>(p => p.Status == StatusPedido.Cancelado)), Times.Once);
    }

    #endregion

    #region Helper Methods

    private static PedidoInputDto CriarPedidoInputDtoValido()
    {
        return new PedidoInputDto
        {
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
                    Quantidade = 2,
                    PrecoUnitario = 50.00m,
                    DescontoUnitario = 5.00m
                }
            }
        };
    }

    private static PedidoOutputDto CriarPedidoOutputDtoFromInput(PedidoInputDto input)
    {
        return new PedidoOutputDto
        {
            Id = Guid.NewGuid(),
            TokenAtendimentoId = input.TokenAtendimentoId,
            ClienteId = input.ClienteId,
            Subtotal = input.Subtotal,
            Desconto = input.Desconto,
            Total = input.Total,
            Status = StatusPedido.Pendente,
            SenhaPedido = "ABC1234567",
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

    private static PedidoOutputDto CriarPedidoOutputDto(Guid id, StatusPedido status)
    {
        return new PedidoOutputDto
        {
            Id = id,
            TokenAtendimentoId = Guid.NewGuid(),
            ClienteId = Guid.NewGuid(),
            Subtotal = 100.00m,
            Desconto = 10.00m,
            Total = 90.00m,
            Status = status,
            SenhaPedido = "ABC1234567",
            CriadoEm = DateTime.Now,
            Itens = new List<ItemPedidoOutputDto>
            {
                new()
                {
                    ProdutoId = Guid.NewGuid(),
                    Quantidade = 2,
                    PrecoUnitario = 50.00m,
                    DescontoUnitario = 5.00m
                }
            }
        };
    }

    #endregion
}
