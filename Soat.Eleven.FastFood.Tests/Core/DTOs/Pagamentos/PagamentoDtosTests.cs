using FluentAssertions;
using Soat.Eleven.FastFood.Core.DTOs.Pagamentos;
using Xunit;

namespace Soat.Eleven.FastFood.Tests.Core.DTOs.Pagamentos;

public class PagamentoDtosTests
{
    #region OrdemPagamentoResponse Tests

    [Fact]
    public void OrdemPagamentoResponse_DeveInicializarComValoresPadrao()
    {
        // Arrange & Act
        var response = new OrdemPagamentoResponse();

        // Assert
        response.Id.Should().BeNull();
        response.PedidoId.Should().Be(Guid.Empty);
        response.Valor.Should().Be(0);
        response.Status.Should().BeNull();
        response.RedirectUrl.Should().BeNull();
        response.CreatedAt.Should().BeNull();
    }

    [Fact]
    public void OrdemPagamentoResponse_DevePermitirDefinirPropriedades()
    {
        // Arrange
        var id = Guid.NewGuid().ToString();
        var pedidoId = Guid.NewGuid();
        var createdAt = DateTime.Now;

        // Act
        var response = new OrdemPagamentoResponse
        {
            Id = id,
            PedidoId = pedidoId,
            Valor = 150.00m,
            Status = "paid",
            RedirectUrl = "https://pagamento.com/redirect",
            CreatedAt = createdAt
        };

        // Assert
        response.Id.Should().Be(id);
        response.PedidoId.Should().Be(pedidoId);
        response.Valor.Should().Be(150.00m);
        response.Status.Should().Be("paid");
        response.RedirectUrl.Should().Be("https://pagamento.com/redirect");
        response.CreatedAt.Should().Be(createdAt);
    }

    #endregion

    #region CriarOrdemPagamentoRequest Tests

    [Fact]
    public void CriarOrdemPagamentoRequest_DevePermitirDefinirPropriedades()
    {
        // Arrange
        var endToEndId = Guid.NewGuid().ToString();
        var clientId = "client-123";
        var items = new List<ItemPagamento>
        {
            new() { Id = "1", Title = "Hamburguer", Quantity = 2, UnitPrice = 25.00m }
        };

        // Act
        var request = new CriarOrdemPagamentoRequest
        {
            EndToEndId = endToEndId,
            ClientId = clientId,
            Items = items
        };

        // Assert
        request.EndToEndId.Should().Be(endToEndId);
        request.ClientId.Should().Be(clientId);
        request.Items.Should().HaveCount(1);
    }

    [Fact]
    public void CriarOrdemPagamentoRequest_DevePermitirMultiplosItens()
    {
        // Arrange
        var items = new List<ItemPagamento>
        {
            new() { Id = "1", Title = "Item 1", Quantity = 1, UnitPrice = 10.00m },
            new() { Id = "2", Title = "Item 2", Quantity = 2, UnitPrice = 20.00m },
            new() { Id = "3", Title = "Item 3", Quantity = 3, UnitPrice = 30.00m }
        };

        // Act
        var request = new CriarOrdemPagamentoRequest
        {
            EndToEndId = "e2e-123",
            ClientId = "client",
            Items = items
        };

        // Assert
        request.Items.Should().HaveCount(3);
    }

    #endregion

    #region ItemPagamento Tests

    [Fact]
    public void ItemPagamento_DeveInicializarComValoresPadrao()
    {
        // Arrange & Act
        var item = new ItemPagamento();

        // Assert
        item.Id.Should().Be(string.Empty);
        item.Title.Should().Be(string.Empty);
        item.Quantity.Should().Be(0);
        item.UnitPrice.Should().Be(0);
    }

    [Fact]
    public void ItemPagamento_DevePermitirDefinirPropriedades()
    {
        // Arrange & Act
        var item = new ItemPagamento
        {
            Id = "item-123",
            Title = "Refrigerante",
            Quantity = 3,
            UnitPrice = 8.50m
        };

        // Assert
        item.Id.Should().Be("item-123");
        item.Title.Should().Be("Refrigerante");
        item.Quantity.Should().Be(3);
        item.UnitPrice.Should().Be(8.50m);
    }

    [Fact]
    public void ItemPagamento_DevePermitirCalcularValorTotal()
    {
        // Arrange
        var item = new ItemPagamento
        {
            Quantity = 5,
            UnitPrice = 12.50m
        };

        // Act
        var valorTotal = item.Quantity * item.UnitPrice;

        // Assert
        valorTotal.Should().Be(62.50m);
    }

    #endregion

    #region ConfirmacaoPagamento Tests

    [Fact]
    public void ConfirmacaoPagamento_DevePermitirDefinirPropriedades()
    {
        // Arrange & Act
        var confirmacao = new ConfirmacaoPagamento
        {
            Id = "pagamento-123",
            EndToEndId = "e2e-456",
            ExternalReferenceId = "ref-789",
            ClientId = "client-abc",
            Value = 99.99m,
            Provider = "mercadopago",
            Status = "paid",
            RedirectUrl = "https://redirect.com"
        };

        // Assert
        confirmacao.Id.Should().Be("pagamento-123");
        confirmacao.EndToEndId.Should().Be("e2e-456");
        confirmacao.ExternalReferenceId.Should().Be("ref-789");
        confirmacao.ClientId.Should().Be("client-abc");
        confirmacao.Value.Should().Be(99.99m);
        confirmacao.Provider.Should().Be("mercadopago");
        confirmacao.Status.Should().Be("paid");
        confirmacao.RedirectUrl.Should().Be("https://redirect.com");
    }

    [Theory]
    [InlineData("paid")]
    [InlineData("pending")]
    [InlineData("failed")]
    [InlineData("cancelled")]
    [InlineData("refunded")]
    public void ConfirmacaoPagamento_DeveAceitarDiferentesStatus(string status)
    {
        // Arrange & Act
        var confirmacao = new ConfirmacaoPagamento { Status = status };

        // Assert
        confirmacao.Status.Should().Be(status);
    }

    #endregion
}
