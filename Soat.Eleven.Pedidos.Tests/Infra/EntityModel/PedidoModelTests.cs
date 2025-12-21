using FluentAssertions;
using Soat.Eleven.Pedidos.Adapter.Infra.EntityModel;
using Soat.Eleven.Pedidos.Core.Enums;
using Xunit;

namespace Soat.Eleven.Pedidos.Tests.Infra.EntityModel;

public class PedidoModelTests
{
    #region Constructor Tests

    [Fact]
    public void Construtor_DeveInicializarPropriedadesCorretamente()
    {
        // Arrange
        var tokenAtendimentoId = Guid.NewGuid();
        var clienteId = Guid.NewGuid();
        var subtotal = 100.00m;
        var desconto = 10.00m;
        var total = 90.00m;
        var senhaPedido = "ABCD123456";

        // Act
        var pedidoModel = new PedidoModel(tokenAtendimentoId, clienteId, subtotal, desconto, total, senhaPedido);

        // Assert
        pedidoModel.TokenAtendimentoId.Should().Be(tokenAtendimentoId);
        pedidoModel.ClienteId.Should().Be(clienteId);
        pedidoModel.Subtotal.Should().Be(subtotal);
        pedidoModel.Desconto.Should().Be(desconto);
        pedidoModel.Total.Should().Be(total);
        pedidoModel.SenhaPedido.Should().Be(senhaPedido);
        pedidoModel.Status.Should().Be(StatusPedido.Pendente);
    }

    [Fact]
    public void Construtor_DeveAceitarClienteIdNulo()
    {
        // Arrange & Act
        var pedidoModel = new PedidoModel(Guid.NewGuid(), null, 100.00m, 0.00m, 100.00m, "SENHA123");

        // Assert
        pedidoModel.ClienteId.Should().BeNull();
    }

    [Fact]
    public void ConstrutorVazio_DevePermitirInstanciacaoPeloORM()
    {
        // Arrange & Act
        var pedidoModel = new PedidoModel();

        // Assert
        pedidoModel.Should().NotBeNull();
        pedidoModel.Itens.Should().NotBeNull();
        pedidoModel.Itens.Should().BeEmpty();
    }

    #endregion

    #region AdicionarItem Tests

    [Fact]
    public void AdicionarItem_DeveAdicionarItemAColecao()
    {
        // Arrange
        var pedidoModel = CriarPedidoModelValido();
        var item = CriarItemPedidoModelValido();

        // Act
        pedidoModel.AdicionarItem(item);

        // Assert
        pedidoModel.Itens.Should().HaveCount(1);
        pedidoModel.Itens.Should().Contain(item);
    }

    [Fact]
    public void AdicionarItem_DeveLancarExcecao_QuandoItemNulo()
    {
        // Arrange
        var pedidoModel = CriarPedidoModelValido();

        // Act
        var action = () => pedidoModel.AdicionarItem(null!);

        // Assert
        action.Should().Throw<ArgumentNullException>()
            .WithParameterName("item");
    }

    [Fact]
    public void AdicionarItem_DeveAdicionarMultiplosItens()
    {
        // Arrange
        var pedidoModel = CriarPedidoModelValido();

        // Act
        pedidoModel.AdicionarItem(CriarItemPedidoModelValido());
        pedidoModel.AdicionarItem(CriarItemPedidoModelValido());
        pedidoModel.AdicionarItem(CriarItemPedidoModelValido());

        // Assert
        pedidoModel.Itens.Should().HaveCount(3);
    }

    #endregion

    #region AdicionarItens Tests

    [Fact]
    public void AdicionarItens_DeveAdicionarTodosItensAColecao()
    {
        // Arrange
        var pedidoModel = CriarPedidoModelValido();
        var itens = new List<ItemPedidoModel>
        {
            CriarItemPedidoModelValido(),
            CriarItemPedidoModelValido(),
            CriarItemPedidoModelValido()
        };

        // Act
        pedidoModel.AdicionarItens(itens);

        // Assert
        pedidoModel.Itens.Should().HaveCount(3);
    }

    [Fact]
    public void AdicionarItens_DeveLancarExcecao_QuandoColecaoNula()
    {
        // Arrange
        var pedidoModel = CriarPedidoModelValido();

        // Act
        var action = () => pedidoModel.AdicionarItens(null!);

        // Assert
        action.Should().Throw<ArgumentNullException>()
            .WithParameterName("itens");
    }

    [Fact]
    public void AdicionarItens_DeveNaoAlterarColecao_QuandoListaVazia()
    {
        // Arrange
        var pedidoModel = CriarPedidoModelValido();
        var itens = new List<ItemPedidoModel>();

        // Act
        pedidoModel.AdicionarItens(itens);

        // Assert
        pedidoModel.Itens.Should().BeEmpty();
    }

    #endregion

    #region RemoverItem Tests

    [Fact]
    public void RemoverItem_DeveRemoverItemDaColecao()
    {
        // Arrange
        var pedidoModel = CriarPedidoModelValido();
        var item = CriarItemPedidoModelValido();
        var itemId = Guid.NewGuid();
        item.Id = itemId;
        pedidoModel.AdicionarItem(item);

        // Act
        pedidoModel.RemoverItem(itemId);

        // Assert
        pedidoModel.Itens.Should().BeEmpty();
    }

    [Fact]
    public void RemoverItem_NaoDeveAlterarColecao_QuandoIdNaoExiste()
    {
        // Arrange
        var pedidoModel = CriarPedidoModelValido();
        var item = CriarItemPedidoModelValido();
        pedidoModel.AdicionarItem(item);

        // Act
        pedidoModel.RemoverItem(Guid.NewGuid());

        // Assert
        pedidoModel.Itens.Should().HaveCount(1);
    }

    [Fact]
    public void RemoverItem_DeveRemoverApenasItemEspecifico()
    {
        // Arrange
        var pedidoModel = CriarPedidoModelValido();
        var item1 = CriarItemPedidoModelValido();
        var item2 = CriarItemPedidoModelValido();
        var itemId1 = Guid.NewGuid();
        item1.Id = itemId1;
        pedidoModel.AdicionarItem(item1);
        pedidoModel.AdicionarItem(item2);

        // Act
        pedidoModel.RemoverItem(itemId1);

        // Assert
        pedidoModel.Itens.Should().HaveCount(1);
        pedidoModel.Itens.Should().Contain(item2);
    }

    #endregion

    #region Status Tests

    [Fact]
    public void Status_DevePadronizarParaPendente_NaCriacao()
    {
        // Arrange & Act
        var pedidoModel = CriarPedidoModelValido();

        // Assert
        pedidoModel.Status.Should().Be(StatusPedido.Pendente);
    }

    [Theory]
    [InlineData(StatusPedido.Recebido)]
    [InlineData(StatusPedido.EmPreparacao)]
    [InlineData(StatusPedido.Pronto)]
    [InlineData(StatusPedido.Finalizado)]
    [InlineData(StatusPedido.Cancelado)]
    public void Status_DevePermitirAlteracaoParaQualquerStatus(StatusPedido novoStatus)
    {
        // Arrange
        var pedidoModel = CriarPedidoModelValido();

        // Act
        pedidoModel.Status = novoStatus;

        // Assert
        pedidoModel.Status.Should().Be(novoStatus);
    }

    #endregion

    #region Helper Methods

    private static PedidoModel CriarPedidoModelValido()
    {
        return new PedidoModel(
            tokenAtendimentoId: Guid.NewGuid(),
            clienteId: Guid.NewGuid(),
            subtotal: 100.00m,
            desconto: 10.00m,
            total: 90.00m,
            senhaPedido: "ABCD123456"
        );
    }

    private static ItemPedidoModel CriarItemPedidoModelValido()
    {
        return new ItemPedidoModel(
            produtoId: Guid.NewGuid(),
            quantidade: 2,
            precoUnitario: 25.00m,
            descontoUnitario: 0.00m
        );
    }

    #endregion
}
