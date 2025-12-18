using FluentAssertions;
using Soat.Eleven.FastFood.Core.Entities;
using Soat.Eleven.FastFood.Core.Enums;
using Xunit;

namespace Soat.Eleven.FastFood.Tests.Core.Entities;

/// <summary>
/// Testes adicionais para Pedido cobrindo mais cenários
/// </summary>
public class PedidoExtendedTests
{
    #region GerarSenha Extended Tests

    [Fact]
    public void GerarSenha_DeveGerarSenhasDiferentes_ParaPedidosDiferentes()
    {
        // Arrange
        var pedido1 = CriarPedidoValido();
        var pedido2 = CriarPedidoValido();

        // Act
        pedido1.GerarSenha();
        pedido2.GerarSenha();

        // Assert
        pedido1.SenhaPedido.Should().NotBe(pedido2.SenhaPedido);
    }

    [Fact]
    public void GerarSenha_DeveGerarSenhaComApenasCaracteresValidos()
    {
        // Arrange
        var pedido = CriarPedidoValido();

        // Act
        pedido.GerarSenha();

        // Assert
        pedido.SenhaPedido.Should().MatchRegex("^[A-Z0-9]+$");
    }

    [Fact]
    public void GerarSenha_DeveSobrescreverSenhaAnterior()
    {
        // Arrange
        var pedido = CriarPedidoValido();
        pedido.GerarSenha();
        var senhaAnterior = pedido.SenhaPedido;

        // Act
        pedido.GerarSenha();

        // Assert
        pedido.SenhaPedido.Should().NotBe(senhaAnterior);
    }

    #endregion

    #region AdicionarItem Extended Tests

    [Fact]
    public void AdicionarItem_DeveAdicionarMuitosItens()
    {
        // Arrange
        var pedido = CriarPedidoValido();
        var quantidadeItens = 100;

        // Act
        for (int i = 0; i < quantidadeItens; i++)
        {
            pedido.AdicionarItem(new ItemPedido(Guid.NewGuid(), i + 1, 10.00m, 1.00m));
        }

        // Assert
        pedido.Itens.Should().HaveCount(quantidadeItens);
    }

    [Fact]
    public void AdicionarItem_DevePermitirItensComMesmoProdutoId()
    {
        // Arrange
        var pedido = CriarPedidoValido();
        var produtoId = Guid.NewGuid();

        // Act
        pedido.AdicionarItem(new ItemPedido(produtoId, 1, 10.00m, 0));
        pedido.AdicionarItem(new ItemPedido(produtoId, 2, 10.00m, 0));

        // Assert
        pedido.Itens.Should().HaveCount(2);
        pedido.Itens.Count(i => i.ProdutoId == produtoId).Should().Be(2);
    }

    [Fact]
    public void AdicionarItem_DeveManterOrdemDeInsercao()
    {
        // Arrange
        var pedido = CriarPedidoValido();
        var produtoId1 = Guid.NewGuid();
        var produtoId2 = Guid.NewGuid();
        var produtoId3 = Guid.NewGuid();

        // Act
        pedido.AdicionarItem(new ItemPedido(produtoId1, 1, 10.00m, 0));
        pedido.AdicionarItem(new ItemPedido(produtoId2, 2, 20.00m, 0));
        pedido.AdicionarItem(new ItemPedido(produtoId3, 3, 30.00m, 0));

        // Assert
        var itens = pedido.Itens.ToList();
        itens[0].ProdutoId.Should().Be(produtoId1);
        itens[1].ProdutoId.Should().Be(produtoId2);
        itens[2].ProdutoId.Should().Be(produtoId3);
    }

    #endregion

    #region AdicionarItens Extended Tests

    [Fact]
    public void AdicionarItens_DeveAdicionarAosItensExistentes()
    {
        // Arrange
        var pedido = CriarPedidoValido();
        pedido.AdicionarItem(new ItemPedido(Guid.NewGuid(), 1, 10.00m, 0));
        var novosItens = new List<ItemPedido>
        {
            new(Guid.NewGuid(), 2, 20.00m, 0),
            new(Guid.NewGuid(), 3, 30.00m, 0)
        };

        // Act
        pedido.AdicionarItens(novosItens);

        // Assert
        pedido.Itens.Should().HaveCount(3);
    }

    #endregion

    #region AtualizarId Extended Tests

    [Fact]
    public void AtualizarId_DeveSobrescreverIdAnterior()
    {
        // Arrange
        var pedido = CriarPedidoValido();
        var primeiroId = Guid.NewGuid();
        var segundoId = Guid.NewGuid();
        pedido.AtualizarId(primeiroId);

        // Act
        pedido.AtualizarId(segundoId);

        // Assert
        pedido.Id.Should().Be(segundoId);
    }

    [Fact]
    public void AtualizarId_DeveLancarExcecao_QuandoGuidEmpty()
    {
        // Arrange
        var pedido = CriarPedidoValido();
        pedido.AtualizarId(Guid.NewGuid());

        // Act
        var action = () => pedido.AtualizarId(Guid.Empty);

        // Assert
        action.Should().Throw<ArgumentException>()
            .WithMessage("O ID não pode ser vazio.*");
    }

    #endregion

    #region Status Transitions Extended Tests

    [Fact]
    public void Status_DeveSerPendenteAoCriar()
    {
        // Arrange & Act
        var pedido = CriarPedidoValido();

        // Assert
        pedido.Status.Should().Be(StatusPedido.Pendente);
    }

    [Theory]
    [InlineData(StatusPedido.Pendente)]
    [InlineData(StatusPedido.Recebido)]
    [InlineData(StatusPedido.EmPreparacao)]
    [InlineData(StatusPedido.Pronto)]
    [InlineData(StatusPedido.Finalizado)]
    [InlineData(StatusPedido.Cancelado)]
    public void Status_DevePermitirAlteracaoParaQualquerStatus(StatusPedido novoStatus)
    {
        // Arrange
        var pedido = CriarPedidoValido();

        // Act
        pedido.Status = novoStatus;

        // Assert
        pedido.Status.Should().Be(novoStatus);
    }

    #endregion

    #region Properties Extended Tests

    [Fact]
    public void Subtotal_DeveAceitarValorZero()
    {
        // Arrange
        var pedido = new Pedido(Guid.NewGuid(), Guid.NewGuid(), 0.00m, 0.00m, 0.00m);

        // Assert
        pedido.Subtotal.Should().Be(0.00m);
    }

    [Fact]
    public void Total_DeveAceitarValorNegativo()
    {
        // Arrange
        var pedido = new Pedido(Guid.NewGuid(), Guid.NewGuid(), 100.00m, 150.00m, -50.00m);

        // Assert
        pedido.Total.Should().Be(-50.00m);
    }

    [Fact]
    public void CriadoEm_DeveSerDefinido_AoCriar()
    {
        // Arrange
        var antes = DateTime.Now.AddSeconds(-1);
        
        // Act
        var pedido = CriarPedidoValido();
        var depois = DateTime.Now.AddSeconds(1);

        // Assert
        pedido.CriadoEm.Should().BeAfter(antes);
        pedido.CriadoEm.Should().BeBefore(depois);
    }

    [Fact]
    public void CriadoEm_DevePermitirAlteracao()
    {
        // Arrange
        var pedido = CriarPedidoValido();
        var data = new DateTime(2025, 12, 18, 12, 0, 0);

        // Act
        pedido.CriadoEm = data;

        // Assert
        pedido.CriadoEm.Should().Be(data);
    }

    [Fact]
    public void TokenAtendimentoId_DevePermitirAlteracao()
    {
        // Arrange
        var pedido = CriarPedidoValido();
        var novoToken = Guid.NewGuid();

        // Act
        pedido.TokenAtendimentoId = novoToken;

        // Assert
        pedido.TokenAtendimentoId.Should().Be(novoToken);
    }

    [Fact]
    public void ClienteId_DevePermitirAlteracaoParaNulo()
    {
        // Arrange
        var pedido = CriarPedidoValido();

        // Act
        pedido.ClienteId = null;

        // Assert
        pedido.ClienteId.Should().BeNull();
    }

    #endregion

    #region Helper Methods

    private static Pedido CriarPedidoValido()
    {
        return new Pedido(
            tokenAtendimentoId: Guid.NewGuid(),
            clienteId: Guid.NewGuid(),
            subtotal: 100.00m,
            desconto: 10.00m,
            total: 90.00m
        );
    }

    #endregion
}
