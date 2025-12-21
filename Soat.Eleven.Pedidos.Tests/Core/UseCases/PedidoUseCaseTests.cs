using FluentAssertions;
using Moq;
using Soat.Eleven.Pedidos.Core.DTOs.Pedidos;
using Soat.Eleven.Pedidos.Core.Entities;
using Soat.Eleven.Pedidos.Core.Enums;
using Soat.Eleven.Pedidos.Core.Gateways;
using Soat.Eleven.Pedidos.Core.Interfaces.DataSources;
using Soat.Eleven.Pedidos.Core.UseCases;
using Xunit;

namespace Soat.Eleven.Pedidos.Tests.Core.UseCases;

public class PedidoUseCaseTests
{
    private readonly Mock<IPedidoDataSource> _pedidoDataSourceMock;
    private readonly PedidoGateway _pedidoGateway;
    private readonly PedidoUseCase _pedidoUseCase;

    public PedidoUseCaseTests()
    {
        _pedidoDataSourceMock = new Mock<IPedidoDataSource>();
        _pedidoGateway = new PedidoGateway(_pedidoDataSourceMock.Object);
        _pedidoUseCase = PedidoUseCase.Create(_pedidoGateway);
    }

    #region CriarPedido Tests

    [Fact]
    public async Task CriarPedido_DeveRetornarPedidoCriado_QuandoDadosValidos()
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
        resultado.Should().NotBeNull();
        resultado.TokenAtendimentoId.Should().Be(pedidoDto.TokenAtendimentoId);
        resultado.ClienteId.Should().Be(pedidoDto.ClienteId);
        resultado.Subtotal.Should().Be(pedidoDto.Subtotal);
        resultado.Desconto.Should().Be(pedidoDto.Desconto);
        resultado.Total.Should().Be(pedidoDto.Total);
        resultado.Status.Should().Be(StatusPedido.Pendente);
        resultado.SenhaPedido.Should().NotBeNullOrEmpty();
        resultado.Itens.Should().HaveCount(pedidoDto.Itens.Count);
    }

    [Fact]
    public async Task CriarPedido_DeveGerarSenha_QuandoPedidoCriado()
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
        resultado.SenhaPedido.Should().NotBeNullOrEmpty();
        resultado.SenhaPedido.Length.Should().Be(10);
    }

    [Fact]
    public async Task CriarPedido_DeveAdicionarItens_QuandoPedidoTemItens()
    {
        // Arrange
        var pedidoDto = CriarPedidoInputDtoValido();
        pedidoDto.Itens.Add(new ItemPedidoInputDto
        {
            ProdutoId = Guid.NewGuid(),
            Quantidade = 2,
            PrecoUnitario = 15.00m,
            DescontoUnitario = 0
        });
        var pedidoOutput = CriarPedidoOutputDtoFromInput(pedidoDto);
        
        _pedidoDataSourceMock
            .Setup(x => x.AddAsync(It.IsAny<PedidoInputDto>()))
            .ReturnsAsync(pedidoOutput);

        // Act
        var resultado = await _pedidoUseCase.CriarPedido(pedidoDto);

        // Assert
        resultado.Itens.Should().HaveCount(2);
    }

    #endregion

    #region AtualizarPedido Tests

    [Fact]
    public async Task AtualizarPedido_DeveRetornarPedidoAtualizado_QuandoStatusPendente()
    {
        // Arrange
        var pedidoId = Guid.NewGuid();
        var pedidoExistente = CriarPedidoExistente(pedidoId, StatusPedido.Pendente);
        var pedidoDto = CriarPedidoInputDtoValido();
        pedidoDto.Id = pedidoId;
        pedidoDto.Subtotal = 200.00m;
        pedidoDto.Total = 180.00m;

        _pedidoDataSourceMock
            .Setup(x => x.GetByIdAsync(pedidoId))
            .ReturnsAsync(CriarPedidoOutputDto(pedidoExistente));

        _pedidoDataSourceMock
            .Setup(x => x.UpdateAsync(It.IsAny<PedidoInputDto>()))
            .Returns(Task.CompletedTask);

        // Act
        var resultado = await _pedidoUseCase.AtualizarPedido(pedidoDto);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Subtotal.Should().Be(200.00m);
        resultado.Total.Should().Be(180.00m);
    }

    [Fact]
    public async Task AtualizarPedido_DeveLancarExcecao_QuandoStatusNaoPendente()
    {
        // Arrange
        var pedidoId = Guid.NewGuid();
        var pedidoExistente = CriarPedidoExistente(pedidoId, StatusPedido.EmPreparacao);
        var pedidoDto = CriarPedidoInputDtoValido();
        pedidoDto.Id = pedidoId;

        _pedidoDataSourceMock
            .Setup(x => x.GetByIdAsync(pedidoId))
            .ReturnsAsync(CriarPedidoOutputDto(pedidoExistente));

        // Act
        var act = () => _pedidoUseCase.AtualizarPedido(pedidoDto);

        // Assert
        await act.Should().ThrowAsync<Exception>()
            .WithMessage("O status do pedido não permite alteração.");
    }

    [Fact]
    public async Task AtualizarPedido_DeveLancarExcecao_QuandoPedidoNaoEncontrado()
    {
        // Arrange
        var pedidoDto = CriarPedidoInputDtoValido();
        pedidoDto.Id = Guid.NewGuid();

        _pedidoDataSourceMock
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((PedidoOutputDto?)null);

        // Act
        var act = () => _pedidoUseCase.AtualizarPedido(pedidoDto);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("Pedido não encontrado.");
    }

    #endregion

    #region ListarPedidos Tests

    [Fact]
    public async Task ListarPedidos_DeveRetornarPedidosFiltrados_ExcluindoFinalizados()
    {
        // Arrange
        var pedidos = new List<PedidoOutputDto>
        {
            CriarPedidoOutputDto(CriarPedidoExistente(Guid.NewGuid(), StatusPedido.Pendente)),
            CriarPedidoOutputDto(CriarPedidoExistente(Guid.NewGuid(), StatusPedido.EmPreparacao)),
            CriarPedidoOutputDto(CriarPedidoExistente(Guid.NewGuid(), StatusPedido.Finalizado)),
            CriarPedidoOutputDto(CriarPedidoExistente(Guid.NewGuid(), StatusPedido.Pronto))
        };

        _pedidoDataSourceMock
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(pedidos);

        // Act
        var resultado = await _pedidoUseCase.ListarPedidos();

        // Assert
        resultado.Should().HaveCount(3);
        resultado.Should().NotContain(p => p.Status == StatusPedido.Finalizado);
    }

    [Fact]
    public async Task ListarPedidos_DeveRetornarListaVazia_QuandoNaoExistemPedidos()
    {
        // Arrange
        _pedidoDataSourceMock
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(new List<PedidoOutputDto>());

        // Act
        var resultado = await _pedidoUseCase.ListarPedidos();

        // Assert
        resultado.Should().BeEmpty();
    }

    [Fact]
    public async Task ListarPedidos_DeveRetornarListaVazia_QuandoTodosPedidosFinalizados()
    {
        // Arrange
        var pedidos = new List<PedidoOutputDto>
        {
            CriarPedidoOutputDto(CriarPedidoExistente(Guid.NewGuid(), StatusPedido.Finalizado)),
            CriarPedidoOutputDto(CriarPedidoExistente(Guid.NewGuid(), StatusPedido.Finalizado))
        };

        _pedidoDataSourceMock
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(pedidos);

        // Act
        var resultado = await _pedidoUseCase.ListarPedidos();

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
        var pedidoExistente = CriarPedidoExistente(pedidoId, StatusPedido.Pendente);

        _pedidoDataSourceMock
            .Setup(x => x.GetByIdAsync(pedidoId))
            .ReturnsAsync(CriarPedidoOutputDto(pedidoExistente));

        // Act
        var resultado = await _pedidoUseCase.ObterPedidoPorId(pedidoId);

        // Assert
        resultado.Should().NotBeNull();
        resultado!.Id.Should().Be(pedidoId);
    }

    [Fact]
    public async Task ObterPedidoPorId_DeveRetornarNull_QuandoNaoEncontrado()
    {
        // Arrange
        _pedidoDataSourceMock
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((PedidoOutputDto?)null);

        // Act
        var resultado = await _pedidoUseCase.ObterPedidoPorId(Guid.NewGuid());

        // Assert
        resultado.Should().BeNull();
    }

    #endregion

    #region IniciarPreparacaoPedido Tests

    [Fact]
    public async Task IniciarPreparacaoPedido_DeveAlterarStatusParaEmPreparacao_QuandoStatusRecebido()
    {
        // Arrange
        var pedidoId = Guid.NewGuid();
        var pedidoExistente = CriarPedidoExistente(pedidoId, StatusPedido.Recebido);

        _pedidoDataSourceMock
            .Setup(x => x.GetByIdAsync(pedidoId))
            .ReturnsAsync(CriarPedidoOutputDto(pedidoExistente));

        _pedidoDataSourceMock
            .Setup(x => x.UpdateAsync(It.IsAny<PedidoInputDto>()))
            .Returns(Task.CompletedTask);

        // Act
        await _pedidoUseCase.IniciarPreparacaoPedido(pedidoId);

        // Assert
        _pedidoDataSourceMock.Verify(x => x.UpdateAsync(It.Is<PedidoInputDto>(p => p.Status == StatusPedido.EmPreparacao)), Times.Once);
    }

    [Fact]
    public async Task IniciarPreparacaoPedido_DeveLancarExcecao_QuandoStatusNaoRecebido()
    {
        // Arrange
        var pedidoId = Guid.NewGuid();
        var pedidoExistente = CriarPedidoExistente(pedidoId, StatusPedido.Pendente);

        _pedidoDataSourceMock
            .Setup(x => x.GetByIdAsync(pedidoId))
            .ReturnsAsync(CriarPedidoOutputDto(pedidoExistente));

        // Act
        var act = () => _pedidoUseCase.IniciarPreparacaoPedido(pedidoId);

        // Assert
        await act.Should().ThrowAsync<Exception>()
            .WithMessage("*O status do pedido não permite iniciar a preparação*");
    }

    [Fact]
    public async Task IniciarPreparacaoPedido_DeveLancarExcecao_QuandoPedidoNaoEncontrado()
    {
        // Arrange
        _pedidoDataSourceMock
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((PedidoOutputDto?)null);

        // Act
        var act = () => _pedidoUseCase.IniciarPreparacaoPedido(Guid.NewGuid());

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("Pedido não encontrado.");
    }

    #endregion

    #region FinalizarPreparacaoPedido Tests

    [Fact]
    public async Task FinalizarPreparacaoPedido_DeveAlterarStatusParaPronto_QuandoStatusEmPreparacao()
    {
        // Arrange
        var pedidoId = Guid.NewGuid();
        var pedidoExistente = CriarPedidoExistente(pedidoId, StatusPedido.EmPreparacao);

        _pedidoDataSourceMock
            .Setup(x => x.GetByIdAsync(pedidoId))
            .ReturnsAsync(CriarPedidoOutputDto(pedidoExistente));

        _pedidoDataSourceMock
            .Setup(x => x.UpdateAsync(It.IsAny<PedidoInputDto>()))
            .Returns(Task.CompletedTask);

        // Act
        await _pedidoUseCase.FinalizarPreparacaoPedido(pedidoId);

        // Assert
        _pedidoDataSourceMock.Verify(x => x.UpdateAsync(It.Is<PedidoInputDto>(p => p.Status == StatusPedido.Pronto)), Times.Once);
    }

    [Fact]
    public async Task FinalizarPreparacaoPedido_DeveLancarExcecao_QuandoStatusNaoEmPreparacao()
    {
        // Arrange
        var pedidoId = Guid.NewGuid();
        var pedidoExistente = CriarPedidoExistente(pedidoId, StatusPedido.Recebido);

        _pedidoDataSourceMock
            .Setup(x => x.GetByIdAsync(pedidoId))
            .ReturnsAsync(CriarPedidoOutputDto(pedidoExistente));

        // Act
        var act = () => _pedidoUseCase.FinalizarPreparacaoPedido(pedidoId);

        // Assert
        await act.Should().ThrowAsync<Exception>()
            .WithMessage("*O status do pedido não está permite finalizar a preparação*");
    }

    [Fact]
    public async Task FinalizarPreparacaoPedido_DeveLancarExcecao_QuandoPedidoNaoEncontrado()
    {
        // Arrange
        _pedidoDataSourceMock
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((PedidoOutputDto?)null);

        // Act
        var act = () => _pedidoUseCase.FinalizarPreparacaoPedido(Guid.NewGuid());

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("Pedido não encontrado.");
    }

    #endregion

    #region FinalizarPedido Tests

    [Fact]
    public async Task FinalizarPedido_DeveAlterarStatusParaFinalizado_QuandoStatusPronto()
    {
        // Arrange
        var pedidoId = Guid.NewGuid();
        var pedidoExistente = CriarPedidoExistente(pedidoId, StatusPedido.Pronto);

        _pedidoDataSourceMock
            .Setup(x => x.GetByIdAsync(pedidoId))
            .ReturnsAsync(CriarPedidoOutputDto(pedidoExistente));

        _pedidoDataSourceMock
            .Setup(x => x.UpdateAsync(It.IsAny<PedidoInputDto>()))
            .Returns(Task.CompletedTask);

        // Act
        await _pedidoUseCase.FinalizarPedido(pedidoId);

        // Assert
        _pedidoDataSourceMock.Verify(x => x.UpdateAsync(It.Is<PedidoInputDto>(p => p.Status == StatusPedido.Finalizado)), Times.Once);
    }

    [Fact]
    public async Task FinalizarPedido_DeveLancarExcecao_QuandoStatusNaoPronto()
    {
        // Arrange
        var pedidoId = Guid.NewGuid();
        var pedidoExistente = CriarPedidoExistente(pedidoId, StatusPedido.EmPreparacao);

        _pedidoDataSourceMock
            .Setup(x => x.GetByIdAsync(pedidoId))
            .ReturnsAsync(CriarPedidoOutputDto(pedidoExistente));

        // Act
        var act = () => _pedidoUseCase.FinalizarPedido(pedidoId);

        // Assert
        await act.Should().ThrowAsync<Exception>()
            .WithMessage("*O status do pedido não permite finalização*");
    }

    [Fact]
    public async Task FinalizarPedido_DeveLancarExcecao_QuandoPedidoNaoEncontrado()
    {
        // Arrange
        _pedidoDataSourceMock
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((PedidoOutputDto?)null);

        // Act
        var act = () => _pedidoUseCase.FinalizarPedido(Guid.NewGuid());

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("Pedido não encontrado.");
    }

    #endregion

    #region CancelarPedido Tests

    [Fact]
    public async Task CancelarPedido_DeveAlterarStatusParaCancelado_QuandoStatusPendente()
    {
        // Arrange
        var pedidoId = Guid.NewGuid();
        var pedidoExistente = CriarPedidoExistente(pedidoId, StatusPedido.Pendente);

        _pedidoDataSourceMock
            .Setup(x => x.GetByIdAsync(pedidoId))
            .ReturnsAsync(CriarPedidoOutputDto(pedidoExistente));

        _pedidoDataSourceMock
            .Setup(x => x.UpdateAsync(It.IsAny<PedidoInputDto>()))
            .Returns(Task.CompletedTask);

        // Act
        await _pedidoUseCase.CancelarPedido(pedidoId);

        // Assert
        _pedidoDataSourceMock.Verify(x => x.UpdateAsync(It.Is<PedidoInputDto>(p => p.Status == StatusPedido.Cancelado)), Times.Once);
    }

    [Fact]
    public async Task CancelarPedido_DeveAlterarStatusParaCancelado_QuandoStatusRecebido()
    {
        // Arrange
        var pedidoId = Guid.NewGuid();
        var pedidoExistente = CriarPedidoExistente(pedidoId, StatusPedido.Recebido);

        _pedidoDataSourceMock
            .Setup(x => x.GetByIdAsync(pedidoId))
            .ReturnsAsync(CriarPedidoOutputDto(pedidoExistente));

        _pedidoDataSourceMock
            .Setup(x => x.UpdateAsync(It.IsAny<PedidoInputDto>()))
            .Returns(Task.CompletedTask);

        // Act
        await _pedidoUseCase.CancelarPedido(pedidoId);

        // Assert
        _pedidoDataSourceMock.Verify(x => x.UpdateAsync(It.Is<PedidoInputDto>(p => p.Status == StatusPedido.Cancelado)), Times.Once);
    }

    [Fact]
    public async Task CancelarPedido_DeveAlterarStatusParaCancelado_QuandoStatusEmPreparacao()
    {
        // Arrange
        var pedidoId = Guid.NewGuid();
        var pedidoExistente = CriarPedidoExistente(pedidoId, StatusPedido.EmPreparacao);

        _pedidoDataSourceMock
            .Setup(x => x.GetByIdAsync(pedidoId))
            .ReturnsAsync(CriarPedidoOutputDto(pedidoExistente));

        _pedidoDataSourceMock
            .Setup(x => x.UpdateAsync(It.IsAny<PedidoInputDto>()))
            .Returns(Task.CompletedTask);

        // Act
        await _pedidoUseCase.CancelarPedido(pedidoId);

        // Assert
        _pedidoDataSourceMock.Verify(x => x.UpdateAsync(It.Is<PedidoInputDto>(p => p.Status == StatusPedido.Cancelado)), Times.Once);
    }

    [Fact]
    public async Task CancelarPedido_DeveAlterarStatusParaCancelado_QuandoStatusPronto()
    {
        // Arrange
        var pedidoId = Guid.NewGuid();
        var pedidoExistente = CriarPedidoExistente(pedidoId, StatusPedido.Pronto);

        _pedidoDataSourceMock
            .Setup(x => x.GetByIdAsync(pedidoId))
            .ReturnsAsync(CriarPedidoOutputDto(pedidoExistente));

        _pedidoDataSourceMock
            .Setup(x => x.UpdateAsync(It.IsAny<PedidoInputDto>()))
            .Returns(Task.CompletedTask);

        // Act
        await _pedidoUseCase.CancelarPedido(pedidoId);

        // Assert
        _pedidoDataSourceMock.Verify(x => x.UpdateAsync(It.Is<PedidoInputDto>(p => p.Status == StatusPedido.Cancelado)), Times.Once);
    }

    [Fact]
    public async Task CancelarPedido_DeveLancarExcecao_QuandoStatusFinalizado()
    {
        // Arrange
        var pedidoId = Guid.NewGuid();
        var pedidoExistente = CriarPedidoExistente(pedidoId, StatusPedido.Finalizado);

        _pedidoDataSourceMock
            .Setup(x => x.GetByIdAsync(pedidoId))
            .ReturnsAsync(CriarPedidoOutputDto(pedidoExistente));

        // Act
        var act = () => _pedidoUseCase.CancelarPedido(pedidoId);

        // Assert
        await act.Should().ThrowAsync<Exception>()
            .WithMessage("Não é permitido cancelar pedido finalizado");
    }

    [Fact]
    public async Task CancelarPedido_DeveLancarExcecao_QuandoPedidoNaoEncontrado()
    {
        // Arrange
        _pedidoDataSourceMock
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((PedidoOutputDto?)null);

        // Act
        var act = () => _pedidoUseCase.CancelarPedido(Guid.NewGuid());

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("Pedido não encontrado.");
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

    private static Pedido CriarPedidoExistente(Guid id, StatusPedido status)
    {
        var pedido = new Pedido(Guid.NewGuid(), Guid.NewGuid(), 100.00m, 10.00m, 90.00m)
        {
            Id = id,
            Status = status
        };
        pedido.GerarSenha();
        return pedido;
    }

    private static PedidoOutputDto CriarPedidoOutputDto(Pedido pedido)
    {
        return new PedidoOutputDto
        {
            Id = pedido.Id,
            TokenAtendimentoId = pedido.TokenAtendimentoId,
            ClienteId = pedido.ClienteId,
            Subtotal = pedido.Subtotal,
            Desconto = pedido.Desconto,
            Total = pedido.Total,
            Status = pedido.Status,
            SenhaPedido = pedido.SenhaPedido,
            CriadoEm = pedido.CriadoEm,
            Itens = pedido.Itens.Select(i => new ItemPedidoOutputDto
            {
                ProdutoId = i.ProdutoId,
                Quantidade = i.Quantidade,
                PrecoUnitario = i.PrecoUnitario,
                DescontoUnitario = i.DescontoUnitario
            }).ToList()
        };
    }

    private static PedidoOutputDto CriarPedidoOutputDtoFromInput(PedidoInputDto input)
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

    #endregion
}
