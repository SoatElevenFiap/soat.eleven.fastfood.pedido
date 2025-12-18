using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Soat.Eleven.FastFood.Adapter.Infra.EntityModel;
using Soat.Eleven.FastFood.Core.Enums;
using Soat.Eleven.FastFood.Infra.Data;
using Xunit;

namespace Soat.Eleven.FastFood.Tests.Infra.Data.ModelConfiguration;

public class ModelConfigurationTests
{
    private DbContextOptions<AppDbContext> CreateInMemoryOptions()
    {
        return new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
    }

    #region PedidoModelConfiguration Tests

    [Fact]
    public async Task PedidoConfiguration_DevePermitirSalvarPedidoCompleto()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new AppDbContext(options);
        var pedido = new PedidoModel
        {
            Id = Guid.NewGuid(),
            TokenAtendimentoId = Guid.NewGuid(),
            ClienteId = Guid.NewGuid(),
            SenhaPedido = "ABC12345XY",
            Subtotal = 100.00m,
            Desconto = 10.00m,
            Total = 90.00m,
            Status = StatusPedido.Pendente,
            CriadoEm = DateTime.UtcNow,
            ModificadoEm = DateTime.UtcNow
        };

        // Act
        await context.Pedidos.AddAsync(pedido);
        await context.SaveChangesAsync();

        // Assert
        var resultado = await context.Pedidos.FindAsync(pedido.Id);
        resultado.Should().NotBeNull();
        resultado!.TokenAtendimentoId.Should().Be(pedido.TokenAtendimentoId);
        resultado.SenhaPedido.Should().Be(pedido.SenhaPedido);
    }

    [Fact]
    public async Task PedidoConfiguration_DevePermitirValoresDecimaisComPrecisao()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new AppDbContext(options);
        var pedido = new PedidoModel
        {
            Id = Guid.NewGuid(),
            TokenAtendimentoId = Guid.NewGuid(),
            ClienteId = Guid.NewGuid(),
            SenhaPedido = "ABC123",
            Subtotal = 12345678.99m,
            Desconto = 1234567.89m,
            Total = 11111111.10m,
            Status = StatusPedido.Pendente,
            CriadoEm = DateTime.UtcNow,
            ModificadoEm = DateTime.UtcNow
        };

        // Act
        await context.Pedidos.AddAsync(pedido);
        await context.SaveChangesAsync();

        // Assert
        var resultado = await context.Pedidos.FindAsync(pedido.Id);
        resultado!.Subtotal.Should().Be(12345678.99m);
        resultado.Desconto.Should().Be(1234567.89m);
        resultado.Total.Should().Be(11111111.10m);
    }

    [Fact]
    public async Task PedidoConfiguration_DevePersistirStatusComoString()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new AppDbContext(options);
        
        foreach (StatusPedido status in Enum.GetValues(typeof(StatusPedido)))
        {
            var pedido = new PedidoModel
            {
                Id = Guid.NewGuid(),
                TokenAtendimentoId = Guid.NewGuid(),
                ClienteId = Guid.NewGuid(),
                SenhaPedido = "ABC123",
                Subtotal = 100.00m,
                Desconto = 0.00m,
                Total = 100.00m,
                Status = status,
                CriadoEm = DateTime.UtcNow,
                ModificadoEm = DateTime.UtcNow
            };

            await context.Pedidos.AddAsync(pedido);
            await context.SaveChangesAsync();

            // Assert
            var resultado = await context.Pedidos.FindAsync(pedido.Id);
            resultado!.Status.Should().Be(status);
        }
    }

    [Fact]
    public async Task PedidoConfiguration_DevePermitirClienteIdNulo()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new AppDbContext(options);
        var pedido = new PedidoModel
        {
            Id = Guid.NewGuid(),
            TokenAtendimentoId = Guid.NewGuid(),
            ClienteId = null,
            SenhaPedido = "ABC123",
            Subtotal = 100.00m,
            Desconto = 0.00m,
            Total = 100.00m,
            Status = StatusPedido.Pendente,
            CriadoEm = DateTime.UtcNow,
            ModificadoEm = DateTime.UtcNow
        };

        // Act
        await context.Pedidos.AddAsync(pedido);
        await context.SaveChangesAsync();

        // Assert
        var resultado = await context.Pedidos.FindAsync(pedido.Id);
        resultado!.ClienteId.Should().BeNull();
    }

    #endregion

    #region ItemPedidoModelConfiguration Tests

    [Fact]
    public async Task ItemPedidoConfiguration_DevePermitirSalvarItemCompleto()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new AppDbContext(options);
        var pedido = CriarPedidoModel();
        var item = new ItemPedidoModel
        {
            Id = Guid.NewGuid(),
            PedidoId = pedido.Id,
            ProdutoId = Guid.NewGuid(),
            Quantidade = 5,
            PrecoUnitario = 25.99m,
            DescontoUnitario = 2.50m,
            CriadoEm = DateTime.UtcNow,
            ModificadoEm = DateTime.UtcNow
        };

        await context.Pedidos.AddAsync(pedido);
        await context.ItensPedido.AddAsync(item);
        await context.SaveChangesAsync();

        // Assert
        var resultado = await context.ItensPedido.FindAsync(item.Id);
        resultado.Should().NotBeNull();
        resultado!.ProdutoId.Should().Be(item.ProdutoId);
        resultado.Quantidade.Should().Be(5);
    }

    [Fact]
    public async Task ItemPedidoConfiguration_DevePermitirValoresDecimaisComPrecisao()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new AppDbContext(options);
        var pedido = CriarPedidoModel();
        var item = new ItemPedidoModel
        {
            Id = Guid.NewGuid(),
            PedidoId = pedido.Id,
            ProdutoId = Guid.NewGuid(),
            Quantidade = 1,
            PrecoUnitario = 12345678.99m,
            DescontoUnitario = 1234567.89m,
            CriadoEm = DateTime.UtcNow,
            ModificadoEm = DateTime.UtcNow
        };

        await context.Pedidos.AddAsync(pedido);
        await context.ItensPedido.AddAsync(item);
        await context.SaveChangesAsync();

        // Assert
        var resultado = await context.ItensPedido.FindAsync(item.Id);
        resultado!.PrecoUnitario.Should().Be(12345678.99m);
        resultado.DescontoUnitario.Should().Be(1234567.89m);
    }

    [Fact]
    public async Task ItemPedidoConfiguration_DeveAssociarCorrementeAoPedido()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new AppDbContext(options);
        var pedido = CriarPedidoModel();
        var item = new ItemPedidoModel
        {
            Id = Guid.NewGuid(),
            PedidoId = pedido.Id,
            ProdutoId = Guid.NewGuid(),
            Quantidade = 2,
            PrecoUnitario = 50.00m,
            DescontoUnitario = 5.00m,
            CriadoEm = DateTime.UtcNow,
            ModificadoEm = DateTime.UtcNow
        };

        await context.Pedidos.AddAsync(pedido);
        await context.ItensPedido.AddAsync(item);
        await context.SaveChangesAsync();

        // Assert
        var resultado = await context.ItensPedido
            .Include(i => i.Pedido)
            .FirstOrDefaultAsync(i => i.Id == item.Id);
        
        resultado!.Pedido.Should().NotBeNull();
        resultado.Pedido!.Id.Should().Be(pedido.Id);
    }

    #endregion

    #region Relationship Configuration Tests

    [Fact]
    public async Task RelationshipConfiguration_DeveCarregarItensViaPedido()
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
        resultado!.Itens.Should().HaveCount(2);
        resultado.Itens.Should().Contain(i => i.Id == item1.Id);
        resultado.Itens.Should().Contain(i => i.Id == item2.Id);
    }

    [Fact]
    public async Task RelationshipConfiguration_DeveRemoverItensAoCascatear()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new AppDbContext(options);
        var pedido = CriarPedidoModel();
        var item = CriarItemPedidoModel(pedido.Id);

        await context.Pedidos.AddAsync(pedido);
        await context.ItensPedido.AddAsync(item);
        await context.SaveChangesAsync();

        // Act - remover o item diretamente
        context.ItensPedido.Remove(item);
        await context.SaveChangesAsync();

        // Assert
        context.ItensPedido.Should().BeEmpty();
        context.Pedidos.Should().HaveCount(1);
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
