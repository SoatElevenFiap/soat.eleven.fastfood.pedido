using FluentAssertions;
using Soat.Eleven.FastFood.Adapter.Infra.Services;
using Xunit;

namespace Soat.Eleven.FastFood.Tests.Infra.Services;

public class ValidarPagamentoTests
{
    private readonly ValidarPagamento _validarPagamento;

    public ValidarPagamentoTests()
    {
        _validarPagamento = new ValidarPagamento();
    }

    #region ValidarNotificacao Tests

    [Fact]
    public async Task ValidarNotificacao_DeveRetornarTrue_QuandoTipoPayment()
    {
        // Arrange
        var signature = "assinatura-valida";
        var type = "payment";

        // Act
        var resultado = await _validarPagamento.ValidarNotificacao(signature, type);

        // Assert
        resultado.Should().BeTrue();
    }

    [Fact]
    public async Task ValidarNotificacao_DeveLancarExcecao_QuandoTipoInvalido()
    {
        // Arrange
        var signature = "assinatura-valida";
        var type = "refund";

        // Act
        var action = async () => await _validarPagamento.ValidarNotificacao(signature, type);

        // Assert
        await action.Should().ThrowAsync<ArgumentException>()
            .WithMessage("Tipo de notificação inválido. Esperado 'payment'.");
    }

    [Theory]
    [InlineData("order")]
    [InlineData("notification")]
    [InlineData("webhook")]
    [InlineData("")]
    public async Task ValidarNotificacao_DeveLancarExcecao_QuandoTipoDiferenteDePayment(string tipo)
    {
        // Arrange
        var signature = "assinatura-qualquer";

        // Act
        var action = async () => await _validarPagamento.ValidarNotificacao(signature, tipo);

        // Assert
        await action.Should().ThrowAsync<ArgumentException>()
            .WithMessage("Tipo de notificação inválido. Esperado 'payment'.");
    }

    [Fact]
    public async Task ValidarNotificacao_DeveRetornarTrue_IndependenteDaSignature()
    {
        // Arrange
        var type = "payment";

        // Act
        var resultadoComSignature = await _validarPagamento.ValidarNotificacao("assinatura", type);
        var resultadoSemSignature = await _validarPagamento.ValidarNotificacao("", type);
        var resultadoSignatureNula = await _validarPagamento.ValidarNotificacao(null!, type);

        // Assert
        resultadoComSignature.Should().BeTrue();
        resultadoSemSignature.Should().BeTrue();
        resultadoSignatureNula.Should().BeTrue();
    }

    [Fact]
    public async Task ValidarNotificacao_DeveTratarTipoCaseSensitive()
    {
        // Arrange
        var signature = "assinatura";
        var typeUpperCase = "PAYMENT";

        // Act
        var action = async () => await _validarPagamento.ValidarNotificacao(signature, typeUpperCase);

        // Assert
        await action.Should().ThrowAsync<ArgumentException>();
    }

    #endregion
}
