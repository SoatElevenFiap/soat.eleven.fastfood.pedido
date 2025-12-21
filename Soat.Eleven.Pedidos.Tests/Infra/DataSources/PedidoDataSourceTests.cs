using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using Soat.Eleven.Pedidos.Adapter.Infra.DataSources;
using Soat.Eleven.Pedidos.Adapter.Infra.EntityModel;
using Soat.Eleven.Pedidos.Core.DTOs.Pedidos;
using Soat.Eleven.Pedidos.Core.Enums;
using Soat.Eleven.Pedidos.Infra.Data;
using Xunit;

namespace Soat.Eleven.Pedidos.Tests.Infra.DataSources;

public class PedidoDataSourceTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly PedidoDataSource _dataSource;
    private readonly Mock<IConfiguration> _configurationMock;

    public PedidoDataSourceTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        _configurationMock = new Mock<IConfiguration>();
        _configurationMock.Setup(x => x["PagamentoService:ClientId"]).Returns("test-client-id");
        _dataSource = new PedidoDataSource(_context, _configurationMock.Object);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    #region AddAsync Tests

    [Fact]
    public async Task AddAsync_DeveAdicionarPedido_ERetornarOutputDto()
    {
        // Arrange
        var inputDto = CriarPedidoInputDtoValido();

        // Act
        var resultado = await _dataSource.AddAsync(inputDto);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Id.Should().NotBe(Guid.Empty);
        resultado.TokenAtendimentoId.Should().Be(inputDto.TokenAtendimentoId);
    }

    [Fact]
    public async Task AddAsync_DevePersistirPedidoNoBanco()
    {
        // Arrange
        var inputDto = CriarPedidoInputDtoValido();

        // Act
        var resultado = await _dataSource.AddAsync(inputDto);

        // Assert
        var pedidoNoBanco = await _context.Pedidos.FindAsync(resultado.Id);
        pedidoNoBanco.Should().NotBeNull();
    }

    [Fact]
    public async Task AddAsync_DevePersistirItens()
    {
        // Arrange
        var inputDto = CriarPedidoInputDtoValido();
        inputDto.Itens.Add(new ItemPedidoInputDto
        {
            ProdutoId = Guid.NewGuid(),
            Quantidade = 2,
            PrecoUnitario = 50.00m,
            DescontoUnitario = 5.00m
        });

        // Act
        var resultado = await _dataSource.AddAsync(inputDto);

        // Assert
        var pedidoNoBanco = await _context.Pedidos
            .Include(p => p.Itens)
            .FirstOrDefaultAsync(p => p.Id == resultado.Id);

        pedidoNoBanco.Should().NotBeNull();
        pedidoNoBanco!.Itens.Should().HaveCount(2);
    }

    [Fact]
    public async Task AddAsync_DeveMapearPropriedadesCorretamente()
    {
        // Arrange
        var inputDto = CriarPedidoInputDtoValido();

        // Act
        var resultado = await _dataSource.AddAsync(inputDto);

        // Assert
        resultado.ClienteId.Should().Be(inputDto.ClienteId);
        resultado.Subtotal.Should().Be(inputDto.Subtotal);
        resultado.Desconto.Should().Be(inputDto.Desconto);
        resultado.Total.Should().Be(inputDto.Total);
        resultado.SenhaPedido.Should().Be(inputDto.SenhaPedido);
        resultado.Status.Should().Be(StatusPedido.Pendente);
    }

    #endregion

    #region GetByIdAsync Tests

    [Fact]
    public async Task GetByIdAsync_DeveRetornarPedido_QuandoExiste()
    {
        // Arrange
        var pedidoModel = await AdicionarPedidoNoBanco();

        // Act
        var resultado = await _dataSource.GetByIdAsync(pedidoModel.Id);

        // Assert
        resultado.Should().NotBeNull();
        resultado!.Id.Should().Be(pedidoModel.Id);
    }

    [Fact]
    public async Task GetByIdAsync_DeveRetornarNull_QuandoNaoExiste()
    {
        // Act
        var resultado = await _dataSource.GetByIdAsync(Guid.NewGuid());

        // Assert
        resultado.Should().BeNull();
    }

    [Fact]
    public async Task GetByIdAsync_DeveIncluirItens()
    {
        // Arrange
        var pedidoModel = await AdicionarPedidoNoBanco(incluirItens: true);

        // Act
        var resultado = await _dataSource.GetByIdAsync(pedidoModel.Id);

        // Assert
        resultado.Should().NotBeNull();
        resultado!.Itens.Should().NotBeEmpty();
    }

    #endregion

    #region GetAllAsync Tests

    [Fact]
    public async Task GetAllAsync_DeveRetornarTodosPedidos()
    {
        // Arrange
        await AdicionarPedidoNoBanco();
        await AdicionarPedidoNoBanco();
        await AdicionarPedidoNoBanco();

        // Act
        var resultado = await _dataSource.GetAllAsync();

        // Assert
        resultado.Should().HaveCount(3);
    }

    [Fact]
    public async Task GetAllAsync_DeveRetornarListaVazia_QuandoNaoExistemPedidos()
    {
        // Act
        var resultado = await _dataSource.GetAllAsync();

        // Assert
        resultado.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllAsync_DeveIncluirItens()
    {
        // Arrange
        await AdicionarPedidoNoBanco(incluirItens: true);

        // Act
        var resultado = (await _dataSource.GetAllAsync()).ToList();

        // Assert
        resultado.Should().HaveCount(1);
        resultado[0].Itens.Should().NotBeEmpty();
    }

    #endregion

    #region UpdateAsync Tests

    [Fact]
    public async Task UpdateAsync_DeveAtualizarPedido()
    {
        // Arrange
        var pedidoModel = await AdicionarPedidoNoBanco();
        var updateDto = new PedidoInputDto
        {
            Id = pedidoModel.Id,
            TokenAtendimentoId = pedidoModel.TokenAtendimentoId,
            ClienteId = Guid.NewGuid(),
            Subtotal = 200.00m,
            Desconto = 20.00m,
            Total = 180.00m,
            Status = StatusPedido.EmPreparacao,
            Itens = new List<ItemPedidoInputDto>()
        };

        // Act
        await _dataSource.UpdateAsync(updateDto);

        // Assert
        var pedidoAtualizado = await _context.Pedidos.FindAsync(pedidoModel.Id);
        pedidoAtualizado.Should().NotBeNull();
        pedidoAtualizado!.Subtotal.Should().Be(200.00m);
        pedidoAtualizado.Status.Should().Be(StatusPedido.EmPreparacao);
    }

    [Fact]
    public async Task UpdateAsync_DeveLancarExcecao_QuandoPedidoNaoEncontrado()
    {
        // Arrange
        var updateDto = new PedidoInputDto
        {
            Id = Guid.NewGuid(),
            TokenAtendimentoId = Guid.NewGuid(),
            Itens = new List<ItemPedidoInputDto>()
        };

        // Act
        var act = () => _dataSource.UpdateAsync(updateDto);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage($"*Pedido with ID {updateDto.Id} not found*");
    }

    [Fact]
    public async Task UpdateAsync_DeveAtualizarStatus()
    {
        // Arrange
        var pedidoModel = await AdicionarPedidoNoBanco();
        var updateDto = new PedidoInputDto
        {
            Id = pedidoModel.Id,
            TokenAtendimentoId = pedidoModel.TokenAtendimentoId,
            ClienteId = pedidoModel.ClienteId,
            Subtotal = pedidoModel.Subtotal,
            Desconto = pedidoModel.Desconto,
            Total = pedidoModel.Total,
            Status = StatusPedido.Finalizado,
            Itens = new List<ItemPedidoInputDto>()
        };

        // Act
        await _dataSource.UpdateAsync(updateDto);

        // Assert
        var pedidoAtualizado = await _context.Pedidos.FindAsync(pedidoModel.Id);
        pedidoAtualizado!.Status.Should().Be(StatusPedido.Finalizado);
    }

    #endregion

    #region AtualizarStatusAsync Tests

    [Fact]
    public async Task AtualizarStatusAsync_DeveAtualizarApenasStatus()
    {
        // Arrange
        var pedidoModel = await AdicionarPedidoNoBanco();
        var subtotalOriginal = pedidoModel.Subtotal;

        // Act
        await _dataSource.AtualizarStatusAsync(pedidoModel.Id, StatusPedido.Recebido);

        // Assert
        var pedidoAtualizado = await _context.Pedidos.FindAsync(pedidoModel.Id);
        pedidoAtualizado!.Status.Should().Be(StatusPedido.Recebido);
        pedidoAtualizado.Subtotal.Should().Be(subtotalOriginal);
    }

    [Fact]
    public async Task AtualizarStatusAsync_DeveLancarExcecao_QuandoPedidoNaoEncontrado()
    {
        // Act
        var act = () => _dataSource.AtualizarStatusAsync(Guid.NewGuid(), StatusPedido.Recebido);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("*Pedido com ID*não encontrado*");
    }

    [Theory]
    [InlineData(StatusPedido.Pendente)]
    [InlineData(StatusPedido.Recebido)]
    [InlineData(StatusPedido.EmPreparacao)]
    [InlineData(StatusPedido.Pronto)]
    [InlineData(StatusPedido.Finalizado)]
    [InlineData(StatusPedido.Cancelado)]
    public async Task AtualizarStatusAsync_DeveAtualizarParaQualquerStatus(StatusPedido novoStatus)
    {
        // Arrange
        var pedidoModel = await AdicionarPedidoNoBanco();

        // Act
        await _dataSource.AtualizarStatusAsync(pedidoModel.Id, novoStatus);

        // Assert
        var pedidoAtualizado = await _context.Pedidos.FindAsync(pedidoModel.Id);
        pedidoAtualizado!.Status.Should().Be(novoStatus);
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
            SenhaPedido = "TEST123456",
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

    private async Task<PedidoModel> AdicionarPedidoNoBanco(bool incluirItens = false)
    {
        var pedidoModel = new PedidoModel(
            Guid.NewGuid(),
            Guid.NewGuid(),
            100.00m,
            10.00m,
            90.00m,
            "TEST123456"
        );

        if (incluirItens)
        {
            pedidoModel.Itens = new List<ItemPedidoModel>
            {
                new()
                {
                    ProdutoId = Guid.NewGuid(),
                    Quantidade = 1,
                    PrecoUnitario = 100.00m,
                    DescontoUnitario = 10.00m
                }
            };
        }

        await _context.Pedidos.AddAsync(pedidoModel);
        await _context.SaveChangesAsync();

        return pedidoModel;
    }

    #endregion
}
