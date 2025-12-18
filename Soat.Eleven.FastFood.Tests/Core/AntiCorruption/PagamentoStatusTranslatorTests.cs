using FluentAssertions;
using Soat.Eleven.FastFood.Core.AntiCorruption;
using Soat.Eleven.FastFood.Core.Enums;
using Xunit;

namespace Soat.Eleven.FastFood.Tests.Core.AntiCorruption;

public class PagamentoStatusTranslatorTests
{
    #region ToStatusPedido Tests

    [Theory]
    [InlineData("paid", StatusPedido.Recebido)]
    [InlineData("PAID", StatusPedido.Recebido)]
    [InlineData("Paid", StatusPedido.Recebido)]
    public void ToStatusPedido_DeveRetornarRecebido_QuandoPaid(string status, StatusPedido esperado)
    {
        // Act
        var resultado = PagamentoStatusTranslator.ToStatusPedido(status);

        // Assert
        resultado.Should().Be(esperado);
    }

    [Theory]
    [InlineData("failed", StatusPedido.Cancelado)]
    [InlineData("FAILED", StatusPedido.Cancelado)]
    [InlineData("cancelled", StatusPedido.Cancelado)]
    [InlineData("CANCELLED", StatusPedido.Cancelado)]
    [InlineData("refunded", StatusPedido.Cancelado)]
    [InlineData("REFUNDED", StatusPedido.Cancelado)]
    public void ToStatusPedido_DeveRetornarCancelado_QuandoFalhaOuCancelamento(string status, StatusPedido esperado)
    {
        // Act
        var resultado = PagamentoStatusTranslator.ToStatusPedido(status);

        // Assert
        resultado.Should().Be(esperado);
    }

    [Theory]
    [InlineData("pending")]
    [InlineData("PENDING")]
    [InlineData("refund_requested")]
    [InlineData("REFUND_REQUESTED")]
    [InlineData("error")]
    [InlineData("ERROR")]
    public void ToStatusPedido_DeveRetornarNull_QuandoStatusNaoAlteraEstadoPedido(string status)
    {
        // Act
        var resultado = PagamentoStatusTranslator.ToStatusPedido(status);

        // Assert
        resultado.Should().BeNull();
    }

    [Theory]
    [InlineData("unknown")]
    [InlineData("invalid")]
    [InlineData("random_status")]
    [InlineData("")]
    public void ToStatusPedido_DeveRetornarNull_QuandoStatusDesconhecido(string status)
    {
        // Act
        var resultado = PagamentoStatusTranslator.ToStatusPedido(status);

        // Assert
        resultado.Should().BeNull();
    }

    [Fact]
    public void ToStatusPedido_DeveRetornarNull_QuandoStatusNulo()
    {
        // Act
        var resultado = PagamentoStatusTranslator.ToStatusPedido(null);

        // Assert
        resultado.Should().BeNull();
    }

    #endregion

    #region IsPagamentoAprovado Tests

    [Theory]
    [InlineData("paid", true)]
    [InlineData("PAID", true)]
    [InlineData("Paid", true)]
    public void IsPagamentoAprovado_DeveRetornarTrue_QuandoPaid(string status, bool esperado)
    {
        // Act
        var resultado = PagamentoStatusTranslator.IsPagamentoAprovado(status);

        // Assert
        resultado.Should().Be(esperado);
    }

    [Theory]
    [InlineData("pending")]
    [InlineData("failed")]
    [InlineData("cancelled")]
    [InlineData("refunded")]
    [InlineData("error")]
    [InlineData("unknown")]
    [InlineData("")]
    public void IsPagamentoAprovado_DeveRetornarFalse_QuandoNaoPaid(string status)
    {
        // Act
        var resultado = PagamentoStatusTranslator.IsPagamentoAprovado(status);

        // Assert
        resultado.Should().BeFalse();
    }

    [Fact]
    public void IsPagamentoAprovado_DeveRetornarFalse_QuandoNulo()
    {
        // Act
        var resultado = PagamentoStatusTranslator.IsPagamentoAprovado(null);

        // Assert
        resultado.Should().BeFalse();
    }

    #endregion

    #region IsPagamentoRejeitado Tests

    [Theory]
    [InlineData("failed", true)]
    [InlineData("FAILED", true)]
    [InlineData("cancelled", true)]
    [InlineData("CANCELLED", true)]
    [InlineData("refunded", true)]
    [InlineData("REFUNDED", true)]
    [InlineData("error", true)]
    [InlineData("ERROR", true)]
    public void IsPagamentoRejeitado_DeveRetornarTrue_QuandoStatusRejeitado(string status, bool esperado)
    {
        // Act
        var resultado = PagamentoStatusTranslator.IsPagamentoRejeitado(status);

        // Assert
        resultado.Should().Be(esperado);
    }

    [Theory]
    [InlineData("paid")]
    [InlineData("pending")]
    [InlineData("unknown")]
    [InlineData("")]
    public void IsPagamentoRejeitado_DeveRetornarFalse_QuandoStatusNaoRejeitado(string status)
    {
        // Act
        var resultado = PagamentoStatusTranslator.IsPagamentoRejeitado(status);

        // Assert
        resultado.Should().BeFalse();
    }

    [Fact]
    public void IsPagamentoRejeitado_DeveRetornarFalse_QuandoNulo()
    {
        // Act
        var resultado = PagamentoStatusTranslator.IsPagamentoRejeitado(null);

        // Assert
        resultado.Should().BeFalse();
    }

    #endregion
}
