using FluentAssertions;
using Soat.Eleven.FastFood.Core.ConditionRules;
using Xunit;

namespace Soat.Eleven.FastFood.Tests.Core.ConditionRules;

public class TypesConditionExceptionTests
{
    #region String - IsNullOrEmpty Tests

    [Fact]
    public void IsNullOrEmpty_DeveLancarExcecao_QuandoStringVazia()
    {
        // Arrange
        var condition = Condition.Require(string.Empty, "campoTeste");

        // Act
        var action = () => condition.IsNullOrEmpty();

        // Assert
        action.Should().Throw<ArgumentException>()
            .WithMessage("campoTeste cannot be Null or Empty");
    }

    [Fact]
    public void IsNullOrEmpty_DeveLancarExcecao_QuandoStringNula()
    {
        // Arrange
        string? valor = null;
        var condition = Condition.Require(valor!, "campoTeste");

        // Act
        var action = () => condition.IsNullOrEmpty();

        // Assert
        action.Should().Throw<ArgumentException>()
            .WithMessage("campoTeste cannot be Null or Empty");
    }

    [Fact]
    public void IsNullOrEmpty_NaoDeveLancarExcecao_QuandoStringValida()
    {
        // Arrange
        var condition = Condition.Require("valor valido", "campoTeste");

        // Act
        var action = () => condition.IsNullOrEmpty();

        // Assert
        action.Should().NotThrow();
    }

    #endregion

    #region String - IsEmail Tests

    [Theory]
    [InlineData("email@exemplo.com")]
    [InlineData("usuario.nome@dominio.com.br")]
    [InlineData("test@test.co")]
    public void IsEmail_NaoDeveLancarExcecao_QuandoEmailValido(string email)
    {
        // Arrange
        var condition = Condition.Require(email, "email");

        // Act
        var action = () => condition.IsEmail();

        // Assert
        action.Should().NotThrow();
    }

    [Theory]
    [InlineData("email_invalido")]
    [InlineData("@dominio.com")]
    [InlineData("usuario@")]
    [InlineData("")]
    public void IsEmail_DeveLancarExcecao_QuandoEmailInvalido(string email)
    {
        // Arrange
        var condition = Condition.Require(email, "email");

        // Act
        var action = () => condition.IsEmail();

        // Assert
        action.Should().Throw<ArgumentException>()
            .WithMessage("email is not valid email");
    }

    [Fact]
    public void IsEmail_DeveLancarExcecao_QuandoEmailNulo()
    {
        // Arrange
        string? email = null;
        var condition = Condition.Require(email!, "email");

        // Act
        var action = () => condition.IsEmail();

        // Assert
        action.Should().Throw<ArgumentException>()
            .WithMessage("email is not valid email");
    }

    #endregion

    #region String - MinimumCharacter Tests

    [Fact]
    public void MinimumCharacter_DeveLancarExcecao_QuandoStringMaiorQueMinimo()
    {
        // Arrange
        var condition = Condition.Require("abcdef", "campo");

        // Act
        var action = () => condition.MinimumCharacter(5);

        // Assert
        action.Should().Throw<ArgumentException>()
            .WithMessage("campo cannot be less than 5");
    }

    [Fact]
    public void MinimumCharacter_NaoDeveLancarExcecao_QuandoStringMenorQueMinimo()
    {
        // Arrange
        var condition = Condition.Require("abc", "campo");

        // Act
        var action = () => condition.MinimumCharacter(5);

        // Assert
        action.Should().NotThrow();
    }

    #endregion

    #region String - MaximumCharacter Tests

    [Fact]
    public void MaximumCharacter_DeveLancarExcecao_QuandoStringMenorQueMaximo()
    {
        // Arrange
        var condition = Condition.Require("ab", "campo");

        // Act
        var action = () => condition.MaximumCharacter(5);

        // Assert
        action.Should().Throw<ArgumentException>()
            .WithMessage("campo cannot be greater than 5");
    }

    [Fact]
    public void MaximumCharacter_NaoDeveLancarExcecao_QuandoStringMaiorQueMaximo()
    {
        // Arrange
        var condition = Condition.Require("abcdefghij", "campo");

        // Act
        var action = () => condition.MaximumCharacter(5);

        // Assert
        action.Should().NotThrow();
    }

    #endregion

    #region Int - IsGreaterThan Tests

    [Fact]
    public void IsGreaterThan_Int_DeveLancarExcecao_QuandoMaior()
    {
        // Arrange
        var condition = Condition.Require(10, "valor");

        // Act
        var action = () => condition.IsGreaterThan(5);

        // Assert
        action.Should().Throw<ArgumentException>()
            .WithMessage("valor must be greater than 5");
    }

    [Fact]
    public void IsGreaterThan_Int_NaoDeveLancarExcecao_QuandoMenorOuIgual()
    {
        // Arrange
        var condition = Condition.Require(3, "valor");

        // Act
        var action = () => condition.IsGreaterThan(5);

        // Assert
        action.Should().NotThrow();
    }

    #endregion

    #region Int - IsLessThan Tests

    [Fact]
    public void IsLessThan_Int_DeveLancarExcecao_QuandoMenor()
    {
        // Arrange
        var condition = Condition.Require(3, "valor");

        // Act
        var action = () => condition.IsLessThan(5);

        // Assert
        action.Should().Throw<ArgumentException>()
            .WithMessage("valor must be less than 5");
    }

    [Fact]
    public void IsLessThan_Int_NaoDeveLancarExcecao_QuandoMaiorOuIgual()
    {
        // Arrange
        var condition = Condition.Require(10, "valor");

        // Act
        var action = () => condition.IsLessThan(5);

        // Assert
        action.Should().NotThrow();
    }

    #endregion

    #region Int - IsGreaterThanOrEqualTo Tests

    [Fact]
    public void IsGreaterThanOrEqualTo_Int_DeveLancarExcecao_QuandoMaiorOuIgual()
    {
        // Arrange
        var condition = Condition.Require(5, "valor");

        // Act
        var action = () => condition.IsGreaterThanOrEqualTo(5);

        // Assert
        action.Should().Throw<ArgumentException>()
            .WithMessage("valor must be greater or equal than 5");
    }

    [Fact]
    public void IsGreaterThanOrEqualTo_Int_NaoDeveLancarExcecao_QuandoMenor()
    {
        // Arrange
        var condition = Condition.Require(3, "valor");

        // Act
        var action = () => condition.IsGreaterThanOrEqualTo(5);

        // Assert
        action.Should().NotThrow();
    }

    #endregion

    #region Int - IsLessThanOrEqualTo Tests

    [Fact]
    public void IsLessThanOrEqualTo_Int_DeveLancarExcecao_QuandoMenor()
    {
        // Arrange
        var condition = Condition.Require(3, "valor");

        // Act
        var action = () => condition.IsLessThanOrEqualTo(5);

        // Assert
        action.Should().Throw<ArgumentException>()
            .WithMessage("valor must be less or equal than 5");
    }

    [Fact]
    public void IsLessThanOrEqualTo_Int_NaoDeveLancarExcecao_QuandoMaiorOuIgual()
    {
        // Arrange
        var condition = Condition.Require(10, "valor");

        // Act
        var action = () => condition.IsLessThanOrEqualTo(5);

        // Assert
        action.Should().NotThrow();
    }

    #endregion

    #region Decimal - IsGreaterThan Tests

    [Fact]
    public void IsGreaterThan_Decimal_DeveLancarExcecao_QuandoMaior()
    {
        // Arrange
        var condition = Condition.Require(10.5m, "valor");

        // Act
        var action = () => condition.IsGreaterThan(5.0m);

        // Assert
        action.Should().Throw<ArgumentException>()
            .WithMessage("valor must be greater than *");
    }

    [Fact]
    public void IsGreaterThan_Decimal_NaoDeveLancarExcecao_QuandoMenorOuIgual()
    {
        // Arrange
        var condition = Condition.Require(3.5m, "valor");

        // Act
        var action = () => condition.IsGreaterThan(5.0m);

        // Assert
        action.Should().NotThrow();
    }

    #endregion

    #region Decimal - IsLessThan Tests

    [Fact]
    public void IsLessThan_Decimal_DeveLancarExcecao_QuandoMenor()
    {
        // Arrange
        var condition = Condition.Require(3.5m, "valor");

        // Act
        var action = () => condition.IsLessThan(5.0m);

        // Assert
        action.Should().Throw<ArgumentException>()
            .WithMessage("valor must be less than *");
    }

    [Fact]
    public void IsLessThan_Decimal_NaoDeveLancarExcecao_QuandoMaiorOuIgual()
    {
        // Arrange
        var condition = Condition.Require(10.5m, "valor");

        // Act
        var action = () => condition.IsLessThan(5.0m);

        // Assert
        action.Should().NotThrow();
    }

    #endregion

    #region Decimal - IsGreaterThanOrEqualTo Tests

    [Fact]
    public void IsGreaterThanOrEqualTo_Decimal_DeveLancarExcecao_QuandoMaiorOuIgual()
    {
        // Arrange
        var condition = Condition.Require(5.0m, "valor");

        // Act
        var action = () => condition.IsGreaterThanOrEqualTo(5.0m);

        // Assert
        action.Should().Throw<ArgumentException>()
            .WithMessage("valor must be greater or equal than *");
    }

    [Fact]
    public void IsGreaterThanOrEqualTo_Decimal_NaoDeveLancarExcecao_QuandoMenor()
    {
        // Arrange
        var condition = Condition.Require(3.5m, "valor");

        // Act
        var action = () => condition.IsGreaterThanOrEqualTo(5.0m);

        // Assert
        action.Should().NotThrow();
    }

    #endregion

    #region Decimal - IsLessThanOrEqualTo Tests

    [Fact]
    public void IsLessThanOrEqualTo_Decimal_DeveLancarExcecao_QuandoMenor()
    {
        // Arrange
        var condition = Condition.Require(3.5m, "valor");

        // Act
        var action = () => condition.IsLessThanOrEqualTo(5.0m);

        // Assert
        action.Should().Throw<ArgumentException>()
            .WithMessage("valor must be less or equal than *");
    }

    [Fact]
    public void IsLessThanOrEqualTo_Decimal_NaoDeveLancarExcecao_QuandoMaiorOuIgual()
    {
        // Arrange
        var condition = Condition.Require(10.5m, "valor");

        // Act
        var action = () => condition.IsLessThanOrEqualTo(5.0m);

        // Assert
        action.Should().NotThrow();
    }

    #endregion
}
