using FluentAssertions;
using Moq;
using Soat.Eleven.Pedidos.Core.DTOs.Pedidos;
using Soat.Eleven.Pedidos.Core.Entities;
using Soat.Eleven.Pedidos.Core.Enums;
using Soat.Eleven.Pedidos.Core.Gateways;
using Soat.Eleven.Pedidos.Core.Interfaces.DataSources;
using Xunit;

namespace Soat.Eleven.Pedidos.Tests.Core.Gateways;

public class PedidoGatewayTests
{
    private readonly Mock<IPedidoDataSource> _pedidoDataSourceMock;
    private readonly PedidoGateway _gateway;

    public PedidoGatewayTests()
    {
        _pedidoDataSourceMock = new Mock<IPedidoDataSource>();
        _gateway = new PedidoGateway(_pedidoDataSourceMock.Object);
    }

    #region CriarPedido Tests

    [Fact]
    public async Task CriarPedido_DeveRetornarPedidoComIdAtualizado()
    {
        // Arrange
        var pedido = CriarPedidoValido();
        var outputDto = CriarPedidoOutputDto(pedido);
        outputDto.Id = Guid.NewGuid();

        _pedidoDataSourceMock
            .Setup(x => x.AddAsync(It.IsAny<PedidoInputDto>()))
            .ReturnsAsync(outputDto);

        // Act
        var resultado = await _gateway.CriarPedido(pedido);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Id.Should().Be(outputDto.Id);
    }

    [Fact]
    public async Task CriarPedido_DeveChamarAddAsyncComDadosCorretos()
    {
        // Arrange
        var pedido = CriarPedidoValido();
        var outputDto = CriarPedidoOutputDto(pedido);

        _pedidoDataSourceMock
            .Setup(x => x.AddAsync(It.IsAny<PedidoInputDto>()))
            .ReturnsAsync(outputDto);

        // Act
        await _gateway.CriarPedido(pedido);

        // Assert
        _pedidoDataSourceMock.Verify(x => x.AddAsync(It.Is<PedidoInputDto>(dto =>
            dto.TokenAtendimentoId == pedido.TokenAtendimentoId &&
            dto.ClienteId == pedido.ClienteId &&
            dto.Subtotal == pedido.Subtotal &&
            dto.Desconto == pedido.Desconto &&
            dto.Total == pedido.Total
        )), Times.Once);
    }

    [Fact]
    public async Task CriarPedido_DeveMapearItensCorretamente()
    {
        // Arrange
        var pedido = CriarPedidoValido();
        pedido.AdicionarItem(new ItemPedido(Guid.NewGuid(), 2, 50.00m, 5.00m));
        var outputDto = CriarPedidoOutputDto(pedido);

        _pedidoDataSourceMock
            .Setup(x => x.AddAsync(It.IsAny<PedidoInputDto>()))
            .ReturnsAsync(outputDto);

        // Act
        await _gateway.CriarPedido(pedido);

        // Assert
        _pedidoDataSourceMock.Verify(x => x.AddAsync(It.Is<PedidoInputDto>(dto =>
            dto.Itens.Count == pedido.Itens.Count
        )), Times.Once);
    }

    #endregion

    #region AtualizarPedido Tests

    [Fact]
    public async Task AtualizarPedido_DeveRetornarPedidoAtualizado()
    {
        // Arrange
        var pedido = CriarPedidoValido();
        pedido.Status = StatusPedido.EmPreparacao;

        _pedidoDataSourceMock
            .Setup(x => x.UpdateAsync(It.IsAny<PedidoInputDto>()))
            .Returns(Task.CompletedTask);

        // Act
        var resultado = await _gateway.AtualizarPedido(pedido);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Status.Should().Be(StatusPedido.EmPreparacao);
    }

    [Fact]
    public async Task AtualizarPedido_DeveChamarUpdateAsyncComDadosCorretos()
    {
        // Arrange
        var pedido = CriarPedidoValido();
        pedido.Status = StatusPedido.Pronto;

        _pedidoDataSourceMock
            .Setup(x => x.UpdateAsync(It.IsAny<PedidoInputDto>()))
            .Returns(Task.CompletedTask);

        // Act
        await _gateway.AtualizarPedido(pedido);

        // Assert
        _pedidoDataSourceMock.Verify(x => x.UpdateAsync(It.Is<PedidoInputDto>(dto =>
            dto.Id == pedido.Id &&
            dto.Status == StatusPedido.Pronto
        )), Times.Once);
    }

    [Fact]
    public async Task AtualizarPedido_DeveMapearItensCorretamente()
    {
        // Arrange
        var pedido = CriarPedidoValido();

        _pedidoDataSourceMock
            .Setup(x => x.UpdateAsync(It.IsAny<PedidoInputDto>()))
            .Returns(Task.CompletedTask);

        // Act
        await _gateway.AtualizarPedido(pedido);

        // Assert
        _pedidoDataSourceMock.Verify(x => x.UpdateAsync(It.Is<PedidoInputDto>(dto =>
            dto.Itens.Count == pedido.Itens.Count
        )), Times.Once);
    }

    #endregion

    #region ListarPedidos Tests

    [Fact]
    public async Task ListarPedidos_DeveRetornarListaDePedidos()
    {
        // Arrange
        var pedidosDto = new List<PedidoOutputDto>
        {
            CriarPedidoOutputDto(CriarPedidoValido()),
            CriarPedidoOutputDto(CriarPedidoValido())
        };

        _pedidoDataSourceMock
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(pedidosDto);

        // Act
        var resultado = await _gateway.ListarPedidos();

        // Assert
        resultado.Should().HaveCount(2);
    }

    [Fact]
    public async Task ListarPedidos_DeveRetornarListaVazia_QuandoNaoExistemPedidos()
    {
        // Arrange
        _pedidoDataSourceMock
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(new List<PedidoOutputDto>());

        // Act
        var resultado = await _gateway.ListarPedidos();

        // Assert
        resultado.Should().BeEmpty();
    }

    [Fact]
    public async Task ListarPedidos_DeveMapearPropriedadesCorretamente()
    {
        // Arrange
        var pedido = CriarPedidoValido();
        var pedidoDto = CriarPedidoOutputDto(pedido);
        var pedidosDto = new List<PedidoOutputDto> { pedidoDto };

        _pedidoDataSourceMock
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(pedidosDto);

        // Act
        var resultado = (await _gateway.ListarPedidos()).ToList();

        // Assert
        resultado.Should().HaveCount(1);
        resultado[0].TokenAtendimentoId.Should().Be(pedidoDto.TokenAtendimentoId);
        resultado[0].ClienteId.Should().Be(pedidoDto.ClienteId);
        resultado[0].Subtotal.Should().Be(pedidoDto.Subtotal);
    }

    [Fact]
    public async Task ListarPedidos_DeveMapearItensCorretamente()
    {
        // Arrange
        var pedido = CriarPedidoValido();
        var pedidoDto = CriarPedidoOutputDto(pedido);
        pedidoDto.Itens.Add(new ItemPedidoOutputDto
        {
            ProdutoId = Guid.NewGuid(),
            Quantidade = 2,
            PrecoUnitario = 30.00m,
            DescontoUnitario = 0
        });

        _pedidoDataSourceMock
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(new List<PedidoOutputDto> { pedidoDto });

        // Act
        var resultado = (await _gateway.ListarPedidos()).ToList();

        // Assert
        resultado[0].Itens.Should().HaveCount(pedidoDto.Itens.Count);
    }

    #endregion

    #region ObterPedidoPorId Tests

    [Fact]
    public async Task ObterPedidoPorId_DeveRetornarPedido_QuandoEncontrado()
    {
        // Arrange
        var pedidoId = Guid.NewGuid();
        var pedido = CriarPedidoValido();
        pedido.Id = pedidoId;
        var pedidoDto = CriarPedidoOutputDto(pedido);
        pedidoDto.Id = pedidoId;

        _pedidoDataSourceMock
            .Setup(x => x.GetByIdAsync(pedidoId))
            .ReturnsAsync(pedidoDto);

        // Act
        var resultado = await _gateway.ObterPedidoPorId(pedidoId);

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
        var resultado = await _gateway.ObterPedidoPorId(Guid.NewGuid());

        // Assert
        resultado.Should().BeNull();
    }

    [Fact]
    public async Task ObterPedidoPorId_DeveMapearPropriedadesCorretamente()
    {
        // Arrange
        var pedidoId = Guid.NewGuid();
        var pedido = CriarPedidoValido();
        var pedidoDto = CriarPedidoOutputDto(pedido);
        pedidoDto.Id = pedidoId;
        pedidoDto.Status = StatusPedido.EmPreparacao;
        pedidoDto.SenhaPedido = "SENHA123";

        _pedidoDataSourceMock
            .Setup(x => x.GetByIdAsync(pedidoId))
            .ReturnsAsync(pedidoDto);

        // Act
        var resultado = await _gateway.ObterPedidoPorId(pedidoId);

        // Assert
        resultado.Should().NotBeNull();
        resultado!.TokenAtendimentoId.Should().Be(pedidoDto.TokenAtendimentoId);
        resultado.Status.Should().Be(StatusPedido.EmPreparacao);
        resultado.SenhaPedido.Should().Be("SENHA123");
    }

    [Fact]
    public async Task ObterPedidoPorId_DeveMapearItensCorretamente()
    {
        // Arrange
        var pedidoId = Guid.NewGuid();
        var pedido = CriarPedidoValido();
        var pedidoDto = CriarPedidoOutputDto(pedido);
        pedidoDto.Id = pedidoId;

        _pedidoDataSourceMock
            .Setup(x => x.GetByIdAsync(pedidoId))
            .ReturnsAsync(pedidoDto);

        // Act
        var resultado = await _gateway.ObterPedidoPorId(pedidoId);

        // Assert
        resultado.Should().NotBeNull();
        resultado!.Itens.Should().HaveCount(pedidoDto.Itens.Count);
    }

    #endregion

    #region Helper Methods

    private static Pedido CriarPedidoValido()
    {
        var pedido = new Pedido(Guid.NewGuid(), Guid.NewGuid(), 100.00m, 10.00m, 90.00m)
        {
            Id = Guid.NewGuid()
        };
        pedido.GerarSenha();
        pedido.AdicionarItem(new ItemPedido(Guid.NewGuid(), 1, 100.00m, 10.00m));
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

    #endregion
}
