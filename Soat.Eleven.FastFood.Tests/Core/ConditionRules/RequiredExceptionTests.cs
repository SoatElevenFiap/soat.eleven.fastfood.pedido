using FluentAssertions;
using Soat.Eleven.FastFood.Core.ConditionRules;
using Xunit;

namespace Soat.Eleven.FastFood.Tests.Core.ConditionRules;

public class RequiredExceptionTests
{
    #region Constructor Tests

    [Fact]
    public void Constructor_DeveCriarInstancia_ComStringTarget()
    {
        // Arrange
        var target = "test value";
        var argumentName = "testArg";

        // Act
        var exception = new RequiredException<string>(target, argumentName);

        // Assert
        exception.Should().NotBeNull();
    }

    [Fact]
    public void Constructor_DeveCriarInstancia_ComIntTarget()
    {
        // Arrange
        var target = 42;
        var argumentName = "intArg";

        // Act
        var exception = new RequiredException<int>(target, argumentName);

        // Assert
        exception.Should().NotBeNull();
    }

    [Fact]
    public void Constructor_DeveCriarInstancia_ComGuidTarget()
    {
        // Arrange
        var target = Guid.NewGuid();
        var argumentName = "guidArg";

        // Act
        var exception = new RequiredException<Guid>(target, argumentName);

        // Assert
        exception.Should().NotBeNull();
    }

    [Fact]
    public void Constructor_DeveCriarInstancia_ComDecimalTarget()
    {
        // Arrange
        var target = 99.99m;
        var argumentName = "decimalArg";

        // Act
        var exception = new RequiredException<decimal>(target, argumentName);

        // Assert
        exception.Should().NotBeNull();
    }

    [Fact]
    public void Constructor_DeveCriarInstancia_ComObjectTarget()
    {
        // Arrange
        var target = new { Name = "Test", Value = 123 };
        var argumentName = "objectArg";

        // Act
        var exception = new RequiredException<object>(target, argumentName);

        // Assert
        exception.Should().NotBeNull();
    }

    [Fact]
    public void Constructor_DeveCriarInstancia_ComNullTarget()
    {
        // Arrange
        string? target = null;
        var argumentName = "nullArg";

        // Act
        var exception = new RequiredException<string?>(target, argumentName);

        // Assert
        exception.Should().NotBeNull();
    }

    [Fact]
    public void Constructor_DeveCriarInstancia_ComEmptyArgumentName()
    {
        // Arrange
        var target = "value";
        var argumentName = string.Empty;

        // Act
        var exception = new RequiredException<string>(target, argumentName);

        // Assert
        exception.Should().NotBeNull();
    }

    [Fact]
    public void Constructor_DeveCriarInstancia_ComListTarget()
    {
        // Arrange
        var target = new List<int> { 1, 2, 3 };
        var argumentName = "listArg";

        // Act
        var exception = new RequiredException<List<int>>(target, argumentName);

        // Assert
        exception.Should().NotBeNull();
    }

    [Fact]
    public void Constructor_DeveCriarInstancia_ComDateTimeTarget()
    {
        // Arrange
        var target = DateTime.Now;
        var argumentName = "dateArg";

        // Act
        var exception = new RequiredException<DateTime>(target, argumentName);

        // Assert
        exception.Should().NotBeNull();
    }

    [Fact]
    public void Constructor_DeveCriarInstancia_ComBoolTarget()
    {
        // Arrange
        var target = true;
        var argumentName = "boolArg";

        // Act
        var exception = new RequiredException<bool>(target, argumentName);

        // Assert
        exception.Should().NotBeNull();
    }

    #endregion
}
