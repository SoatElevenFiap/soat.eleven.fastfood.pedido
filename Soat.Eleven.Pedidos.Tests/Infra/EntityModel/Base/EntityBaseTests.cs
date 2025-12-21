using FluentAssertions;
using Soat.Eleven.Pedidos.Adapter.Infra.EntityModel.Base;
using Xunit;

namespace Soat.Eleven.Pedidos.Tests.Infra.EntityModel.Base;

public class EntityBaseTests
{
    #region EntityBase Tests

    [Fact]
    public void EntityBase_DeveInicializarComValoresPadrao()
    {
        // Arrange & Act
        var entity = new TestEntity();

        // Assert
        entity.Id.Should().Be(Guid.Empty);
        entity.CriadoEm.Should().Be(default);
        entity.ModificadoEm.Should().Be(default);
    }

    [Fact]
    public void EntityBase_DevePermitirDefinirId()
    {
        // Arrange
        var entity = new TestEntity();
        var id = Guid.NewGuid();

        // Act
        entity.Id = id;

        // Assert
        entity.Id.Should().Be(id);
    }

    [Fact]
    public void EntityBase_DevePermitirDefinirCriadoEm()
    {
        // Arrange
        var entity = new TestEntity();
        var data = DateTime.UtcNow;

        // Act
        entity.CriadoEm = data;

        // Assert
        entity.CriadoEm.Should().Be(data);
    }

    [Fact]
    public void EntityBase_DevePermitirDefinirModificadoEm()
    {
        // Arrange
        var entity = new TestEntity();
        var data = DateTime.UtcNow;

        // Act
        entity.ModificadoEm = data;

        // Assert
        entity.ModificadoEm.Should().Be(data);
    }

    [Fact]
    public void EntityBase_DeveImplementarTEntity()
    {
        // Arrange & Act
        var entity = new TestEntity();

        // Assert
        entity.Should().BeAssignableTo<TEntity>();
    }

    [Fact]
    public void EntityBase_DeveImplementarTAuditable()
    {
        // Arrange & Act
        var entity = new TestEntity();

        // Assert
        entity.Should().BeAssignableTo<TAuditable>();
    }

    #endregion

    #region TEntity Interface Tests

    [Fact]
    public void TEntity_DeveDefinirPropriedadeId()
    {
        // Arrange
        TEntity entity = new TestEntity();
        var id = Guid.NewGuid();

        // Act
        entity.Id = id;

        // Assert
        entity.Id.Should().Be(id);
    }

    #endregion

    #region TAuditable Interface Tests

    [Fact]
    public void TAuditable_DeveDefinirPropriedadeCriadoEm()
    {
        // Arrange
        TAuditable entity = new TestEntity();
        var data = DateTime.UtcNow;

        // Act
        entity.CriadoEm = data;

        // Assert
        entity.CriadoEm.Should().Be(data);
    }

    [Fact]
    public void TAuditable_DeveDefinirPropriedadeModificadoEm()
    {
        // Arrange
        TAuditable entity = new TestEntity();
        var data = DateTime.UtcNow;

        // Act
        entity.ModificadoEm = data;

        // Assert
        entity.ModificadoEm.Should().Be(data);
    }

    #endregion

    #region Helper Classes

    private class TestEntity : EntityBase
    {
    }

    #endregion
}
