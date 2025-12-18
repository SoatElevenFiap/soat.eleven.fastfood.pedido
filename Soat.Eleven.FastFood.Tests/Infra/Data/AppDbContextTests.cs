using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Soat.Eleven.FastFood.Adapter.Infra.EntityModel;
using Soat.Eleven.FastFood.Core.Enums;
using Soat.Eleven.FastFood.Infra.Data;
using Xunit;

namespace Soat.Eleven.FastFood.Tests.Infra.Data;

public class AppDbContextTests
{
    private DbContextOptions<AppDbContext> CreateInMemoryOptions()
    {
        return new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
    }

    #region Constructor Tests

    [Fact]
    public void Constructor_DeveCriarContextoCorretamente()
    {
        // Arrange
        var options = CreateInMemoryOptions();

        // Act
        using var context = new AppDbContext(options);

        // Assert
        context.Should().NotBeNull();
    }

    [Fact]
    public void Pedidos_DeveRetornarDbSet()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new AppDbContext(options);

        // Act
        var pedidos = context.Pedidos;

        // Assert
        pedidos.Should().NotBeNull();
    }

    [Fact]
    public void ItensPedido_DeveRetornarDbSet()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new AppDbContext(options);

        // Act
        var itens = context.ItensPedido;

        // Assert
        itens.Should().NotBeNull();
    }

    #endregion

    #region Pedido CRUD Tests

    [Fact]
    public async Task Pedidos_DeveAdicionarPedido()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new AppDbContext(options);
        var pedido = CriarPedidoModel();

        // Act
        await context.Pedidos.AddAsync(pedido);
        await context.SaveChangesAsync();

        // Assert
        context.Pedidos.Should().HaveCount(1);
    }

    [Fact]
    public async Task Pedidos_DeveBuscarPedidoPorId()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new AppDbContext(options);
        var pedido = CriarPedidoModel();
        await context.Pedidos.AddAsync(pedido);
        await context.SaveChangesAsync();

        // Act
        var resultado = await context.Pedidos.FindAsync(pedido.Id);

        // Assert
        resultado.Should().NotBeNull();
        resultado!.Id.Should().Be(pedido.Id);
    }

    [Fact]
    public async Task Pedidos_DeveAtualizarPedido()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new AppDbContext(options);
        var pedido = CriarPedidoModel();
        await context.Pedidos.AddAsync(pedido);
        await context.SaveChangesAsync();

        // Act
        pedido.Status = StatusPedido.EmPreparacao;
        context.Pedidos.Update(pedido);
        await context.SaveChangesAsync();

        // Assert
        var resultado = await context.Pedidos.FindAsync(pedido.Id);
        resultado!.Status.Should().Be(StatusPedido.EmPreparacao);
    }

    [Fact]
    public async Task Pedidos_DeveRemoverPedido()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new AppDbContext(options);
        var pedido = CriarPedidoModel();
        await context.Pedidos.AddAsync(pedido);
        await context.SaveChangesAsync();

        // Act
        context.Pedidos.Remove(pedido);
        await context.SaveChangesAsync();

        // Assert
        context.Pedidos.Should().BeEmpty();
    }

    #endregion

    #region ItemPedido CRUD Tests

    [Fact]
    public async Task ItensPedido_DeveAdicionarItem()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new AppDbContext(options);
        var pedido = CriarPedidoModel();
        var item = CriarItemPedidoModel(pedido.Id);
        await context.Pedidos.AddAsync(pedido);
        await context.ItensPedido.AddAsync(item);
        await context.SaveChangesAsync();

        // Assert
        context.ItensPedido.Should().HaveCount(1);
    }

    [Fact]
    public async Task ItensPedido_DeveBuscarItemPorId()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new AppDbContext(options);
        var pedido = CriarPedidoModel();
        var item = CriarItemPedidoModel(pedido.Id);
        await context.Pedidos.AddAsync(pedido);
        await context.ItensPedido.AddAsync(item);
        await context.SaveChangesAsync();

        // Act
        var resultado = await context.ItensPedido.FindAsync(item.Id);

        // Assert
        resultado.Should().NotBeNull();
        resultado!.Id.Should().Be(item.Id);
    }

    [Fact]
    public async Task ItensPedido_DeveAtualizarItem()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new AppDbContext(options);
        var pedido = CriarPedidoModel();
        var item = CriarItemPedidoModel(pedido.Id);
        await context.Pedidos.AddAsync(pedido);
        await context.ItensPedido.AddAsync(item);
        await context.SaveChangesAsync();

        // Act
        item.Quantidade = 5;
        context.ItensPedido.Update(item);
        await context.SaveChangesAsync();

        // Assert
        var resultado = await context.ItensPedido.FindAsync(item.Id);
        resultado!.Quantidade.Should().Be(5);
    }

    [Fact]
    public async Task ItensPedido_DeveRemoverItem()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new AppDbContext(options);
        var pedido = CriarPedidoModel();
        var item = CriarItemPedidoModel(pedido.Id);
        await context.Pedidos.AddAsync(pedido);
        await context.ItensPedido.AddAsync(item);
        await context.SaveChangesAsync();

        // Act
        context.ItensPedido.Remove(item);
        await context.SaveChangesAsync();

        // Assert
        context.ItensPedido.Should().BeEmpty();
    }

    #endregion

    #region Relationship Tests

    [Fact]
    public async Task Pedido_DeveIncluirItensRelacionados()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new AppDbContext(options);
        var pedido = CriarPedidoModel();
        var item1 = CriarItemPedidoModel(pedido.Id);
        var item2 = CriarItemPedidoModel(pedido.Id);
        
        await context.Pedidos.AddAsync(pedido);
        await context.ItensPedido.AddRangeAsync(item1, item2);
        await context.SaveChangesAsync();

        // Act
        var resultado = await context.Pedidos
            .Include(p => p.Itens)
            .FirstOrDefaultAsync(p => p.Id == pedido.Id);

        // Assert
        resultado.Should().NotBeNull();
        resultado!.Itens.Should().HaveCount(2);
    }

    [Fact]
    public async Task Item_DeveReferenciarPedido()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new AppDbContext(options);
        var pedido = CriarPedidoModel();
        var item = CriarItemPedidoModel(pedido.Id);
        
        await context.Pedidos.AddAsync(pedido);
        await context.ItensPedido.AddAsync(item);
        await context.SaveChangesAsync();

        // Act
        var resultado = await context.ItensPedido
            .Include(i => i.Pedido)
            .FirstOrDefaultAsync(i => i.Id == item.Id);

        // Assert
        resultado.Should().NotBeNull();
        resultado!.Pedido.Should().NotBeNull();
        resultado.Pedido!.Id.Should().Be(pedido.Id);
    }

    #endregion

    #region Query Tests

    [Fact]
    public async Task Pedidos_DeveListarTodos()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new AppDbContext(options);
        await context.Pedidos.AddRangeAsync(
            CriarPedidoModel(),
            CriarPedidoModel(),
            CriarPedidoModel()
        );
        await context.SaveChangesAsync();

        // Act
        var resultado = await context.Pedidos.ToListAsync();

        // Assert
        resultado.Should().HaveCount(3);
    }

    [Fact]
    public async Task Pedidos_DeveFiltrarPorStatus()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new AppDbContext(options);
        var pedido1 = CriarPedidoModel();
        pedido1.Status = StatusPedido.Pendente;
        var pedido2 = CriarPedidoModel();
        pedido2.Status = StatusPedido.EmPreparacao;
        var pedido3 = CriarPedidoModel();
        pedido3.Status = StatusPedido.Pendente;
        
        await context.Pedidos.AddRangeAsync(pedido1, pedido2, pedido3);
        await context.SaveChangesAsync();

        // Act
        var resultado = await context.Pedidos
            .Where(p => p.Status == StatusPedido.Pendente)
            .ToListAsync();

        // Assert
        resultado.Should().HaveCount(2);
    }

    [Fact]
    public async Task ItensPedido_DeveFiltrarPorPedidoId()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new AppDbContext(options);
        var pedido1 = CriarPedidoModel();
        var pedido2 = CriarPedidoModel();
        var item1 = CriarItemPedidoModel(pedido1.Id);
        var item2 = CriarItemPedidoModel(pedido1.Id);
        var item3 = CriarItemPedidoModel(pedido2.Id);
        
        await context.Pedidos.AddRangeAsync(pedido1, pedido2);
        await context.ItensPedido.AddRangeAsync(item1, item2, item3);
        await context.SaveChangesAsync();

        // Act
        var resultado = await context.ItensPedido
            .Where(i => i.PedidoId == pedido1.Id)
            .ToListAsync();

        // Assert
        resultado.Should().HaveCount(2);
    }

    #endregion

    #region Helper Methods

    private static PedidoModel CriarPedidoModel()
    {
        return new PedidoModel
        {
            Id = Guid.NewGuid(),
            TokenAtendimentoId = Guid.NewGuid(),
            ClienteId = Guid.NewGuid(),
            SenhaPedido = "ABC123",
            Subtotal = 100.00m,
            Desconto = 10.00m,
            Total = 90.00m,
            Status = StatusPedido.Pendente,
            CriadoEm = DateTime.UtcNow,
            ModificadoEm = DateTime.UtcNow
        };
    }

    private static ItemPedidoModel CriarItemPedidoModel(Guid pedidoId)
    {
        return new ItemPedidoModel
        {
            Id = Guid.NewGuid(),
            PedidoId = pedidoId,
            ProdutoId = Guid.NewGuid(),
            Quantidade = 2,
            PrecoUnitario = 25.00m,
            DescontoUnitario = 2.50m,
            CriadoEm = DateTime.UtcNow,
            ModificadoEm = DateTime.UtcNow
        };
    }

    #endregion
}
