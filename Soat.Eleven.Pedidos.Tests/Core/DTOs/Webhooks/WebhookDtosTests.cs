using FluentAssertions;
using Soat.Eleven.Pedidos.Core.DTOs.Webhooks;
using Xunit;

namespace Soat.Eleven.Pedidos.Tests.Core.DTOs.Webhooks;

public class WebhookDtosTests
{
    #region MercadoPagoNotificationDto Tests

    [Fact]
    public void MercadoPagoNotificationDto_DeveInicializarComValoresPadrao()
    {
        // Arrange & Act
        var dto = new MercadoPagoNotificationDto();

        // Assert
        dto.Action.Should().BeNull();
        dto.ApiVersion.Should().BeNull();
        dto.Data.Should().BeNull();
        dto.DateCreated.Should().Be(default);
        dto.Id.Should().BeNull();
        dto.LiveMode.Should().BeFalse();
        dto.Type.Should().BeNull();
        dto.UserId.Should().Be(0);
    }

    [Fact]
    public void MercadoPagoNotificationDto_DevePermitirDefinirAction()
    {
        // Arrange
        var dto = new MercadoPagoNotificationDto();

        // Act
        dto.Action = "payment.created";

        // Assert
        dto.Action.Should().Be("payment.created");
    }

    [Fact]
    public void MercadoPagoNotificationDto_DevePermitirDefinirApiVersion()
    {
        // Arrange
        var dto = new MercadoPagoNotificationDto();

        // Act
        dto.ApiVersion = "v1";

        // Assert
        dto.ApiVersion.Should().Be("v1");
    }

    [Fact]
    public void MercadoPagoNotificationDto_DevePermitirDefinirData()
    {
        // Arrange
        var dto = new MercadoPagoNotificationDto();
        var data = new NotificationData { Id = "12345" };

        // Act
        dto.Data = data;

        // Assert
        dto.Data.Should().NotBeNull();
        dto.Data.Id.Should().Be("12345");
    }

    [Fact]
    public void MercadoPagoNotificationDto_DevePermitirDefinirDateCreated()
    {
        // Arrange
        var dto = new MercadoPagoNotificationDto();
        var date = DateTime.UtcNow;

        // Act
        dto.DateCreated = date;

        // Assert
        dto.DateCreated.Should().Be(date);
    }

    [Fact]
    public void MercadoPagoNotificationDto_DevePermitirDefinirId()
    {
        // Arrange
        var dto = new MercadoPagoNotificationDto();

        // Act
        dto.Id = "notification-123";

        // Assert
        dto.Id.Should().Be("notification-123");
    }

    [Fact]
    public void MercadoPagoNotificationDto_DevePermitirDefinirLiveMode()
    {
        // Arrange
        var dto = new MercadoPagoNotificationDto();

        // Act
        dto.LiveMode = true;

        // Assert
        dto.LiveMode.Should().BeTrue();
    }

    [Fact]
    public void MercadoPagoNotificationDto_DevePermitirDefinirType()
    {
        // Arrange
        var dto = new MercadoPagoNotificationDto();

        // Act
        dto.Type = "payment";

        // Assert
        dto.Type.Should().Be("payment");
    }

    [Fact]
    public void MercadoPagoNotificationDto_DevePermitirDefinirUserId()
    {
        // Arrange
        var dto = new MercadoPagoNotificationDto();

        // Act
        dto.UserId = 123456789;

        // Assert
        dto.UserId.Should().Be(123456789);
    }

    [Fact]
    public void MercadoPagoNotificationDto_DeveCriarComTodosOsCampos()
    {
        // Arrange
        var dateCreated = DateTime.UtcNow;
        var data = new NotificationData { Id = "payment-id" };

        // Act
        var dto = new MercadoPagoNotificationDto
        {
            Action = "payment.created",
            ApiVersion = "v1",
            Data = data,
            DateCreated = dateCreated,
            Id = "notification-001",
            LiveMode = true,
            Type = "payment",
            UserId = 987654321
        };

        // Assert
        dto.Action.Should().Be("payment.created");
        dto.ApiVersion.Should().Be("v1");
        dto.Data.Should().Be(data);
        dto.DateCreated.Should().Be(dateCreated);
        dto.Id.Should().Be("notification-001");
        dto.LiveMode.Should().BeTrue();
        dto.Type.Should().Be("payment");
        dto.UserId.Should().Be(987654321);
    }

    #endregion

    #region NotificationData Tests

    [Fact]
    public void NotificationData_DeveInicializarComValorPadrao()
    {
        // Arrange & Act
        var data = new NotificationData();

        // Assert
        data.Id.Should().BeNull();
    }

    [Fact]
    public void NotificationData_DevePermitirDefinirId()
    {
        // Arrange
        var data = new NotificationData();

        // Act
        data.Id = "payment-12345";

        // Assert
        data.Id.Should().Be("payment-12345");
    }

    [Fact]
    public void NotificationData_DeveCriarComId()
    {
        // Arrange & Act
        var data = new NotificationData { Id = "test-id" };

        // Assert
        data.Id.Should().Be("test-id");
    }

    #endregion

    #region NotificacaoPagamentoDto Tests

    [Fact]
    public void NotificacaoPagamentoDto_DeveInicializarComValoresPadrao()
    {
        // Arrange & Act
        var dto = new NotificacaoPagamentoDto();

        // Assert
        dto.ExternalId.Should().BeNull();
        dto.Type.Should().BeNull();
        dto.Signature.Should().BeNull();
    }

    [Fact]
    public void NotificacaoPagamentoDto_DevePermitirDefinirExternalId()
    {
        // Arrange
        var dto = new NotificacaoPagamentoDto();

        // Act
        dto.ExternalId = "external-123";

        // Assert
        dto.ExternalId.Should().Be("external-123");
    }

    [Fact]
    public void NotificacaoPagamentoDto_DevePermitirDefinirType()
    {
        // Arrange
        var dto = new NotificacaoPagamentoDto();

        // Act
        dto.Type = "approved";

        // Assert
        dto.Type.Should().Be("approved");
    }

    [Fact]
    public void NotificacaoPagamentoDto_DevePermitirDefinirSignature()
    {
        // Arrange
        var dto = new NotificacaoPagamentoDto();

        // Act
        dto.Signature = "abc123signature";

        // Assert
        dto.Signature.Should().Be("abc123signature");
    }

    [Fact]
    public void NotificacaoPagamentoDto_DeveCriarComTodosOsCampos()
    {
        // Arrange & Act
        var dto = new NotificacaoPagamentoDto
        {
            ExternalId = "ext-001",
            Type = "payment_approved",
            Signature = "signature-xyz"
        };

        // Assert
        dto.ExternalId.Should().Be("ext-001");
        dto.Type.Should().Be("payment_approved");
        dto.Signature.Should().Be("signature-xyz");
    }

    #endregion
}
