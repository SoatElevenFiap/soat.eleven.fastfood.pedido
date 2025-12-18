using FluentAssertions;
using Soat.Eleven.FastFood.Core.DTOs.Pagamentos;
using Xunit;

namespace Soat.Eleven.FastFood.Tests.Core.DTOs.Pagamentos;

public class PagamentoSettingsTests
{
    [Fact]
    public void SectionName_DeveSerPagamentoService()
    {
        // Assert
        PagamentoSettings.SectionName.Should().Be("PagamentoService");
    }

    [Fact]
    public void ClientId_DeveInicializarComStringVazia()
    {
        // Arrange & Act
        var settings = new PagamentoSettings();

        // Assert
        settings.ClientId.Should().Be(string.Empty);
    }

    [Fact]
    public void BaseUrl_DeveInicializarComValorPadrao()
    {
        // Arrange & Act
        var settings = new PagamentoSettings();

        // Assert
        settings.BaseUrl.Should().Be("http://localhost:5001");
    }

    [Fact]
    public void ClientId_DevePermitirDefinirValor()
    {
        // Arrange
        var settings = new PagamentoSettings();

        // Act
        settings.ClientId = "client-123";

        // Assert
        settings.ClientId.Should().Be("client-123");
    }

    [Fact]
    public void BaseUrl_DevePermitirDefinirValor()
    {
        // Arrange
        var settings = new PagamentoSettings();

        // Act
        settings.BaseUrl = "http://payment-service:8080";

        // Assert
        settings.BaseUrl.Should().Be("http://payment-service:8080");
    }

    [Fact]
    public void PagamentoSettings_DevePermitirCriarComInicializadorDeObjeto()
    {
        // Arrange & Act
        var settings = new PagamentoSettings
        {
            ClientId = "my-client-id",
            BaseUrl = "https://api.payment.com"
        };

        // Assert
        settings.ClientId.Should().Be("my-client-id");
        settings.BaseUrl.Should().Be("https://api.payment.com");
    }

    [Fact]
    public void ClientId_DevePermitirValorNulo()
    {
        // Arrange
        var settings = new PagamentoSettings();

        // Act
        settings.ClientId = null!;

        // Assert
        settings.ClientId.Should().BeNull();
    }

    [Fact]
    public void BaseUrl_DevePermitirValorNulo()
    {
        // Arrange
        var settings = new PagamentoSettings();

        // Act
        settings.BaseUrl = null!;

        // Assert
        settings.BaseUrl.Should().BeNull();
    }
}
