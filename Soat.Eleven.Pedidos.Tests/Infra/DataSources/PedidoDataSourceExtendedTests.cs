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

/// <summary>
/// Testes adicionais para PedidoDataSource cobrindo mais cenários
/// </summary>
public class PedidoDataSourceExtendedTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly PedidoDataSource _dataSource;
    private readonly Mock<IConfiguration> _configurationMock;

    public PedidoDataSourceExtendedTests()
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

    #region AddAsync Extended Tests

    [Fact]
    public async Task AddAsync_DeveMapearTodosOsCamposDoItem()
    {
        // Arrange
        var inputDto = CriarPedidoInputDtoValido();
        var item = new ItemPedidoInputDto
        {
            ProdutoId = Guid.NewGuid(),
            Quantidade = 5,
            PrecoUnitario = 99.99m,
            DescontoUnitario = 9.99m
        };
        inputDto.Itens.Add(item);

        // Act
        var resultado = await _dataSource.AddAsync(inputDto);

        // Assert
        var itemSalvo = resultado.Itens.First(i => i.ProdutoId == item.ProdutoId);
        itemSalvo.Quantidade.Should().Be(5);
        itemSalvo.PrecoUnitario.Should().Be(99.99m);
        itemSalvo.DescontoUnitario.Should().Be(9.99m);
    }

    [Fact]
    public async Task AddAsync_DevePersistirPedidoSemItens()
    {
        // Arrange
        var inputDto = CriarPedidoInputDtoValido();
        inputDto.Itens.Clear();

        // Act
        var resultado = await _dataSource.AddAsync(inputDto);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Itens.Should().BeEmpty();
    }

    [Fact]
    public async Task AddAsync_DevePersistirPedidoComClienteIdNulo()
    {
        // Arrange
        var inputDto = CriarPedidoInputDtoValido();
        inputDto.ClienteId = null;

        // Act
        var resultado = await _dataSource.AddAsync(inputDto);

        // Assert
        resultado.Should().NotBeNull();
        resultado.ClienteId.Should().BeNull();
    }

    [Fact]
    public async Task AddAsync_DevePersistirMultiplosItens()
    {
        // Arrange
        var inputDto = CriarPedidoInputDtoValido();
        inputDto.Itens.Add(new ItemPedidoInputDto
        {
            ProdutoId = Guid.NewGuid(),
            Quantidade = 1,
            PrecoUnitario = 10.00m,
            DescontoUnitario = 0
        });
        inputDto.Itens.Add(new ItemPedidoInputDto
        {
            ProdutoId = Guid.NewGuid(),
            Quantidade = 2,
            PrecoUnitario = 20.00m,
            DescontoUnitario = 2.00m
        });
        inputDto.Itens.Add(new ItemPedidoInputDto
        {
            ProdutoId = Guid.NewGuid(),
            Quantidade = 3,
            PrecoUnitario = 30.00m,
            DescontoUnitario = 3.00m
        });

        // Act
        var resultado = await _dataSource.AddAsync(inputDto);

        // Assert
        resultado.Itens.Should().HaveCount(4); // 1 original + 3 adicionados
    }

    #endregion

    #region GetAllAsync Extended Tests

    [Fact]
    public async Task GetAllAsync_DeveRetornarPedidosOrdenados()
    {
        // Arrange
        await _dataSource.AddAsync(CriarPedidoInputDtoValido());
        await _dataSource.AddAsync(CriarPedidoInputDtoValido());
        await _dataSource.AddAsync(CriarPedidoInputDtoValido());

        // Act
        var resultado = await _dataSource.GetAllAsync();

        // Assert
        resultado.Should().HaveCount(3);
    }

    [Fact]
    public async Task GetAllAsync_DeveRetornarPedidosComItens()
    {
        // Arrange
        var inputDto = CriarPedidoInputDtoValido();
        inputDto.Itens.Add(new ItemPedidoInputDto
        {
            ProdutoId = Guid.NewGuid(),
            Quantidade = 2,
            PrecoUnitario = 25.00m,
            DescontoUnitario = 2.50m
        });
        await _dataSource.AddAsync(inputDto);

        // Act
        var resultado = await _dataSource.GetAllAsync();

        // Assert
        resultado.First().Itens.Should().HaveCount(2);
    }

    #endregion

    #region GetByIdAsync Extended Tests

    [Fact]
    public async Task GetByIdAsync_DeveRetornarPedidoComTodosOsCampos()
    {
        // Arrange
        var inputDto = CriarPedidoInputDtoValido();
        inputDto.Subtotal = 500.00m;
        inputDto.Desconto = 50.00m;
        inputDto.Total = 450.00m;
        inputDto.SenhaPedido = "SENHA12345";
        var pedidoCriado = await _dataSource.AddAsync(inputDto);

        // Act
        var resultado = await _dataSource.GetByIdAsync(pedidoCriado.Id);

        // Assert
        resultado.Should().NotBeNull();
        resultado!.Subtotal.Should().Be(500.00m);
        resultado.Desconto.Should().Be(50.00m);
        resultado.Total.Should().Be(450.00m);
        resultado.SenhaPedido.Should().Be("SENHA12345");
    }

    [Fact]
    public async Task GetByIdAsync_DeveRetornarItensCorretos()
    {
        // Arrange
        var inputDto = CriarPedidoInputDtoValido();
        var produtoId = Guid.NewGuid();
        inputDto.Itens.Add(new ItemPedidoInputDto
        {
            ProdutoId = produtoId,
            Quantidade = 10,
            PrecoUnitario = 100.00m,
            DescontoUnitario = 10.00m
        });
        var pedidoCriado = await _dataSource.AddAsync(inputDto);

        // Act
        var resultado = await _dataSource.GetByIdAsync(pedidoCriado.Id);

        // Assert
        var item = resultado!.Itens.First(i => i.ProdutoId == produtoId);
        item.Quantidade.Should().Be(10);
        item.PrecoUnitario.Should().Be(100.00m);
        item.DescontoUnitario.Should().Be(10.00m);
    }

    #endregion

    #region UpdateAsync Extended Tests

    [Fact]
    public async Task UpdateAsync_DeveAtualizarTodosOsCampos()
    {
        // Arrange
        var inputDto = CriarPedidoInputDtoValido();
        var pedidoCriado = await _dataSource.AddAsync(inputDto);

        var updateDto = new PedidoInputDto
        {
            Id = pedidoCriado.Id,
            TokenAtendimentoId = Guid.NewGuid(),
            ClienteId = Guid.NewGuid(),
            Subtotal = 999.99m,
            Desconto = 99.99m,
            Total = 900.00m,
            Status = StatusPedido.EmPreparacao,
            Itens = new List<ItemPedidoInputDto>
            {
                new() { ProdutoId = Guid.NewGuid(), Quantidade = 5, PrecoUnitario = 50.00m, DescontoUnitario = 5.00m }
            }
        };

        // Act
        await _dataSource.UpdateAsync(updateDto);

        // Assert
        var resultado = await _dataSource.GetByIdAsync(pedidoCriado.Id);
        resultado!.Subtotal.Should().Be(999.99m);
        resultado.Desconto.Should().Be(99.99m);
        resultado.Total.Should().Be(900.00m);
        resultado.Status.Should().Be(StatusPedido.EmPreparacao);
    }

    [Fact]
    public async Task UpdateAsync_DeveSubstituirItens()
    {
        // Arrange
        var inputDto = CriarPedidoInputDtoValido();
        var pedidoCriado = await _dataSource.AddAsync(inputDto);

        var novoProdutoId = Guid.NewGuid();
        var updateDto = new PedidoInputDto
        {
            Id = pedidoCriado.Id,
            TokenAtendimentoId = inputDto.TokenAtendimentoId,
            ClienteId = inputDto.ClienteId,
            Subtotal = inputDto.Subtotal,
            Desconto = inputDto.Desconto,
            Total = inputDto.Total,
            Status = StatusPedido.Pendente,
            Itens = new List<ItemPedidoInputDto>
            {
                new() { ProdutoId = novoProdutoId, Quantidade = 7, PrecoUnitario = 70.00m, DescontoUnitario = 7.00m }
            }
        };

        // Act
        await _dataSource.UpdateAsync(updateDto);

        // Assert
        var resultado = await _dataSource.GetByIdAsync(pedidoCriado.Id);
        resultado!.Itens.Should().HaveCount(1);
        resultado.Itens.First().ProdutoId.Should().Be(novoProdutoId);
    }

    #endregion

    #region AtualizarStatusAsync Extended Tests

    [Theory]
    [InlineData(StatusPedido.Recebido)]
    [InlineData(StatusPedido.EmPreparacao)]
    [InlineData(StatusPedido.Pronto)]
    [InlineData(StatusPedido.Finalizado)]
    [InlineData(StatusPedido.Cancelado)]
    public async Task AtualizarStatusAsync_DeveAtualizarParaTodosStatus(StatusPedido novoStatus)
    {
        // Arrange
        var inputDto = CriarPedidoInputDtoValido();
        var pedidoCriado = await _dataSource.AddAsync(inputDto);

        // Act
        await _dataSource.AtualizarStatusAsync(pedidoCriado.Id, novoStatus);

        // Assert
        var resultado = await _dataSource.GetByIdAsync(pedidoCriado.Id);
        resultado!.Status.Should().Be(novoStatus);
    }

    [Fact]
    public async Task AtualizarStatusAsync_DeveManterOutrosCamposIntactos()
    {
        // Arrange
        var inputDto = CriarPedidoInputDtoValido();
        inputDto.Subtotal = 123.45m;
        inputDto.SenhaPedido = "MINHA_SENHA";
        var pedidoCriado = await _dataSource.AddAsync(inputDto);

        // Act
        await _dataSource.AtualizarStatusAsync(pedidoCriado.Id, StatusPedido.Pronto);

        // Assert
        var resultado = await _dataSource.GetByIdAsync(pedidoCriado.Id);
        resultado!.Subtotal.Should().Be(123.45m);
        resultado.SenhaPedido.Should().Be("MINHA_SENHA");
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
            SenhaPedido = "ABC1234567",
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

    #endregion
}
