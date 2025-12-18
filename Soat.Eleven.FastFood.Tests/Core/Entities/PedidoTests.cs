using FluentAssertions;
using Soat.Eleven.FastFood.Core.Entities;
using Soat.Eleven.FastFood.Core.Enums;
using Xunit;

namespace Soat.Eleven.FastFood.Tests.Core.Entities;

public class PedidoTests
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

        // Act
        var pedido = new Pedido(tokenAtendimentoId, clienteId, subtotal, desconto, total);

        // Assert
        pedido.TokenAtendimentoId.Should().Be(tokenAtendimentoId);
        pedido.ClienteId.Should().Be(clienteId);
        pedido.Subtotal.Should().Be(subtotal);
        pedido.Desconto.Should().Be(desconto);
        pedido.Total.Should().Be(total);
        pedido.Status.Should().Be(StatusPedido.Pendente);
        pedido.CriadoEm.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Construtor_DeveAceitarClienteIdNulo()
    {
        // Arrange
        var tokenAtendimentoId = Guid.NewGuid();
        Guid? clienteId = null;

        // Act
        var pedido = new Pedido(tokenAtendimentoId, clienteId, 100.00m, 0.00m, 100.00m);

        // Assert
        pedido.ClienteId.Should().BeNull();
    }

    [Fact]
    public void Construtor_DeveInicializarColecaoItensVazia()
    {
        // Arrange & Act
        var pedido = new Pedido(Guid.NewGuid(), Guid.NewGuid(), 100.00m, 0.00m, 100.00m);

        // Assert
        pedido.Itens.Should().NotBeNull();
        pedido.Itens.Should().BeEmpty();
    }

    [Fact]
    public void ConstrutorVazio_DevePermitirInstanciacaoPeloORM()
    {
        // Arrange & Act
        var pedido = new Pedido();

        // Assert
        pedido.Should().NotBeNull();
        pedido.Id.Should().Be(Guid.Empty);
    }

    #endregion

    #region GerarSenha Tests

    [Fact]
    public void GerarSenha_DeveGerarSenhaComPrefixoDoToken()
    {
        // Arrange
        var tokenAtendimentoId = Guid.NewGuid();
        var pedido = new Pedido(tokenAtendimentoId, Guid.NewGuid(), 100.00m, 0.00m, 100.00m);

        // Act
        pedido.GerarSenha();

        // Assert
        pedido.SenhaPedido.Should().NotBeNullOrEmpty();
        pedido.SenhaPedido.Should().HaveLength(10); // 4 caracteres do token + 6 números
        pedido.SenhaPedido[..4].Should().Be(tokenAtendimentoId.ToString("N")[..4].ToUpper());
    }

    [Fact]
    public void GerarSenha_DeveGerarSenhaComNumerosAleatorios()
    {
        // Arrange
        var pedido = new Pedido(Guid.NewGuid(), Guid.NewGuid(), 100.00m, 0.00m, 100.00m);

        // Act
        pedido.GerarSenha();

        // Assert
        var partesNumericas = pedido.SenhaPedido[4..];
        partesNumericas.Should().MatchRegex(@"^\d{6}$");
    }

    [Fact]
    public void GerarSenha_DeveGerarSenhasDiferentes_QuandoChamadaMultiplasVezes()
    {
        // Arrange
        var pedido = new Pedido(Guid.NewGuid(), Guid.NewGuid(), 100.00m, 0.00m, 100.00m);
        var senhas = new HashSet<string>();

        // Act
        for (int i = 0; i < 10; i++)
        {
            pedido.GerarSenha();
            senhas.Add(pedido.SenhaPedido);
        }

        // Assert - Deve haver variação nas senhas geradas (pelo menos 2 diferentes)
        senhas.Count.Should().BeGreaterThan(1);
    }

    #endregion

    #region AdicionarItem Tests

    [Fact]
    public void AdicionarItem_DeveAdicionarItemAColecao()
    {
        // Arrange
        var pedido = new Pedido(Guid.NewGuid(), Guid.NewGuid(), 100.00m, 0.00m, 100.00m);
        var item = CriarItemPedidoValido();

        // Act
        pedido.AdicionarItem(item);

        // Assert
        pedido.Itens.Should().HaveCount(1);
        pedido.Itens.Should().Contain(item);
    }

    [Fact]
    public void AdicionarItem_DeveAdicionarMultiplosItens()
    {
        // Arrange
        var pedido = new Pedido(Guid.NewGuid(), Guid.NewGuid(), 100.00m, 0.00m, 100.00m);
        var item1 = CriarItemPedidoValido();
        var item2 = CriarItemPedidoValido();
        var item3 = CriarItemPedidoValido();

        // Act
        pedido.AdicionarItem(item1);
        pedido.AdicionarItem(item2);
        pedido.AdicionarItem(item3);

        // Assert
        pedido.Itens.Should().HaveCount(3);
    }

    [Fact]
    public void AdicionarItem_DeveLancarExcecao_QuandoItemNulo()
    {
        // Arrange
        var pedido = new Pedido(Guid.NewGuid(), Guid.NewGuid(), 100.00m, 0.00m, 100.00m);

        // Act
        var action = () => pedido.AdicionarItem(null!);

        // Assert
        action.Should().Throw<ArgumentNullException>()
            .WithParameterName("item");
    }

    #endregion

    #region AdicionarItens Tests

    [Fact]
    public void AdicionarItens_DeveAdicionarTodosItensAColecao()
    {
        // Arrange
        var pedido = new Pedido(Guid.NewGuid(), Guid.NewGuid(), 100.00m, 0.00m, 100.00m);
        var itens = new List<ItemPedido>
        {
            CriarItemPedidoValido(),
            CriarItemPedidoValido(),
            CriarItemPedidoValido()
        };

        // Act
        pedido.AdicionarItens(itens);

        // Assert
        pedido.Itens.Should().HaveCount(3);
    }

    [Fact]
    public void AdicionarItens_DeveLancarExcecao_QuandoColecaoNula()
    {
        // Arrange
        var pedido = new Pedido(Guid.NewGuid(), Guid.NewGuid(), 100.00m, 0.00m, 100.00m);

        // Act
        var action = () => pedido.AdicionarItens(null!);

        // Assert
        action.Should().Throw<ArgumentNullException>()
            .WithParameterName("itens");
    }

    [Fact]
    public void AdicionarItens_DeveNaoAlterarColecao_QuandoListaVazia()
    {
        // Arrange
        var pedido = new Pedido(Guid.NewGuid(), Guid.NewGuid(), 100.00m, 0.00m, 100.00m);
        var itens = new List<ItemPedido>();

        // Act
        pedido.AdicionarItens(itens);

        // Assert
        pedido.Itens.Should().BeEmpty();
    }

    #endregion

    #region RemoverItem Tests

    [Fact]
    public void RemoverItem_DeveRemoverItemDaColecao()
    {
        // Arrange
        var pedido = new Pedido(Guid.NewGuid(), Guid.NewGuid(), 100.00m, 0.00m, 100.00m);
        var item = CriarItemPedidoValido();
        var itemId = Guid.NewGuid();
        item.Id = itemId;
        pedido.AdicionarItem(item);

        // Act
        pedido.RemoverItem(itemId);

        // Assert
        pedido.Itens.Should().BeEmpty();
    }

    [Fact]
    public void RemoverItem_NaoDeveAlterarColecao_QuandoIdNaoExiste()
    {
        // Arrange
        var pedido = new Pedido(Guid.NewGuid(), Guid.NewGuid(), 100.00m, 0.00m, 100.00m);
        var item = CriarItemPedidoValido();
        pedido.AdicionarItem(item);

        // Act
        pedido.RemoverItem(Guid.NewGuid());

        // Assert
        pedido.Itens.Should().HaveCount(1);
    }

    [Fact]
    public void RemoverItem_DeveRemoverApenasItemEspecifico()
    {
        // Arrange
        var pedido = new Pedido(Guid.NewGuid(), Guid.NewGuid(), 100.00m, 0.00m, 100.00m);
        var item1 = CriarItemPedidoValido();
        var item2 = CriarItemPedidoValido();
        var itemId1 = Guid.NewGuid();
        item1.Id = itemId1;
        pedido.AdicionarItem(item1);
        pedido.AdicionarItem(item2);

        // Act
        pedido.RemoverItem(itemId1);

        // Assert
        pedido.Itens.Should().HaveCount(1);
        pedido.Itens.Should().Contain(item2);
    }

    #endregion

    #region AtualizarId Tests

    [Fact]
    public void AtualizarId_DeveAtualizarIdDoPedido()
    {
        // Arrange
        var pedido = new Pedido(Guid.NewGuid(), Guid.NewGuid(), 100.00m, 0.00m, 100.00m);
        var novoId = Guid.NewGuid();

        // Act
        pedido.AtualizarId(novoId);

        // Assert
        pedido.Id.Should().Be(novoId);
    }

    [Fact]
    public void AtualizarId_DeveLancarExcecao_QuandoIdVazio()
    {
        // Arrange
        var pedido = new Pedido(Guid.NewGuid(), Guid.NewGuid(), 100.00m, 0.00m, 100.00m);

        // Act
        var action = () => pedido.AtualizarId(Guid.Empty);

        // Assert
        action.Should().Throw<ArgumentException>()
            .WithMessage("O ID não pode ser vazio.*")
            .WithParameterName("id");
    }

    #endregion

    #region Status Tests

    [Fact]
    public void Status_DevePadronizarParaPendente_NaCriacao()
    {
        // Arrange & Act
        var pedido = new Pedido(Guid.NewGuid(), Guid.NewGuid(), 100.00m, 0.00m, 100.00m);

        // Assert
        pedido.Status.Should().Be(StatusPedido.Pendente);
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
        var pedido = new Pedido(Guid.NewGuid(), Guid.NewGuid(), 100.00m, 0.00m, 100.00m);

        // Act
        pedido.Status = novoStatus;

        // Assert
        pedido.Status.Should().Be(novoStatus);
    }

    #endregion

    #region Helper Methods

    private static ItemPedido CriarItemPedidoValido()
    {
        return new ItemPedido(
            produtoId: Guid.NewGuid(),
            quantidade: 2,
            precoUnitario: 25.00m,
            descontoUnitario: 0.00m
        );
    }

    #endregion
}
