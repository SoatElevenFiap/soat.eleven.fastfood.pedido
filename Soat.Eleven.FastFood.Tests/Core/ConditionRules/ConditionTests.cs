using FluentAssertions;
using Soat.Eleven.FastFood.Core.ConditionRules;
using Xunit;

namespace Soat.Eleven.FastFood.Tests.Core.ConditionRules;

public class ConditionTests
{
    #region Require<T> Tests

    [Fact]
    public void Require_DeveRetornarRequiredException_ComValorPadrao()
    {
        // Arrange
        var target = "valor de teste";

        // Act
        var resultado = Condition.Require(target);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Should().BeOfType<RequiredException<string>>();
    }

    [Fact]
    public void Require_DeveAceitarTiposGenericos_String()
    {
        // Arrange & Act
        var resultado = Condition.Require("teste");

        // Assert
        resultado.Should().BeOfType<RequiredException<string>>();
    }

    [Fact]
    public void Require_DeveAceitarTiposGenericos_Int()
    {
        // Arrange & Act
        var resultado = Condition.Require(42);

        // Assert
        resultado.Should().BeOfType<RequiredException<int>>();
    }

    [Fact]
    public void Require_DeveAceitarTiposGenericos_Decimal()
    {
        // Arrange & Act
        var resultado = Condition.Require(99.99m);

        // Assert
        resultado.Should().BeOfType<RequiredException<decimal>>();
    }

    [Fact]
    public void Require_ComArgumentName_DeveRetornarRequiredException()
    {
        // Arrange
        var target = "valor";
        var argumentName = "meuArgumento";

        // Act
        var resultado = Condition.Require(target, argumentName);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Should().BeOfType<RequiredException<string>>();
    }

    [Fact]
    public void Require_DeveAceitarValorNulo()
    {
        // Arrange
        string? target = null;

        // Act
        var resultado = Condition.Require(target);

        // Assert
        resultado.Should().NotBeNull();
    }

    [Fact]
    public void Require_ComValorNulo_DevePermitirValidacaoIsNullOrEmpty()
    {
        // Arrange
        string? target = null;

        // Act
        var condition = Condition.Require(target!, "campo");
        var action = () => condition.IsNullOrEmpty();

        // Assert
        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Require_ComValorValido_NaoDeveLancarExcecaoNaValidacao()
    {
        // Arrange
        var target = "valor valido";

        // Act
        var condition = Condition.Require(target, "campo");
        var action = () => condition.IsNullOrEmpty();

        // Assert
        action.Should().NotThrow();
    }

    #endregion
}
