using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using Soat.Eleven.Pedidos.Adapter.Infra.Services;
using Soat.Eleven.Pedidos.Core.DTOs.Pagamentos;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

namespace Soat.Eleven.Pedidos.Tests.Infra.Services;

public class PagamentoServiceTests
{
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly Mock<IOptions<PagamentoSettings>> _pagamentoSettingsMock;
    private readonly Mock<ILogger<PagamentoService>> _loggerMock;
    private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;

    public PagamentoServiceTests()
    {
        _configurationMock = new Mock<IConfiguration>();
        _pagamentoSettingsMock = new Mock<IOptions<PagamentoSettings>>();
        _loggerMock = new Mock<ILogger<PagamentoService>>();
        _httpMessageHandlerMock = new Mock<HttpMessageHandler>();

        _configurationMock.Setup(x => x["PagamentoService:BaseUrl"]).Returns("http://localhost:5001");
        _pagamentoSettingsMock.Setup(x => x.Value).Returns(new PagamentoSettings { ClientId = "test-client-id" });
    }

    #region Constructor Tests

    [Fact]
    public void Constructor_DeveInicializarCorretamente()
    {
        // Arrange
        var httpClient = new HttpClient(_httpMessageHandlerMock.Object);

        // Act
        var service = new PagamentoService(
            httpClient,
            _configurationMock.Object,
            _pagamentoSettingsMock.Object,
            _loggerMock.Object
        );

        // Assert
        service.Should().NotBeNull();
    }

    [Fact]
    public void GetClientId_DeveRetornarClientIdConfigurado()
    {
        // Arrange
        var httpClient = new HttpClient(_httpMessageHandlerMock.Object);
        var service = new PagamentoService(
            httpClient,
            _configurationMock.Object,
            _pagamentoSettingsMock.Object,
            _loggerMock.Object
        );

        // Act
        var clientId = service.GetClientId();

        // Assert
        clientId.Should().Be("test-client-id");
    }

    [Fact]
    public void Constructor_DeveUsarBaseUrlPadrao_QuandoNaoConfigurada()
    {
        // Arrange
        var configMock = new Mock<IConfiguration>();
        configMock.Setup(x => x["PagamentoService:BaseUrl"]).Returns((string?)null);
        var httpClient = new HttpClient(_httpMessageHandlerMock.Object);

        // Act
        var service = new PagamentoService(
            httpClient,
            configMock.Object,
            _pagamentoSettingsMock.Object,
            _loggerMock.Object
        );

        // Assert
        service.Should().NotBeNull();
    }

    #endregion

    #region CriarOrdemPagamentoAsync Tests

    [Fact]
    public async Task CriarOrdemPagamentoAsync_DeveRetornarResponse_QuandoSucesso()
    {
        // Arrange
        var expectedResponse = new OrdemPagamentoResponse
        {
            Id = Guid.NewGuid().ToString(),
            PedidoId = Guid.NewGuid(),
            Status = "pending",
            Valor = 100.00m
        };

        var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = JsonContent.Create(expectedResponse)
        };

        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(responseMessage);

        var httpClient = new HttpClient(_httpMessageHandlerMock.Object);
        var service = new PagamentoService(
            httpClient,
            _configurationMock.Object,
            _pagamentoSettingsMock.Object,
            _loggerMock.Object
        );

        var request = new CriarOrdemPagamentoRequest
        {
            EndToEndId = Guid.NewGuid().ToString(),
            ClientId = "client-123",
            Items = new List<ItemPagamento>
            {
                new() { Id = "1", Title = "Item", Quantity = 1, UnitPrice = 100.00m }
            }
        };

        // Act
        var resultado = await service.CriarOrdemPagamentoAsync(request);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Status.Should().Be("pending");
    }

    [Fact]
    public async Task CriarOrdemPagamentoAsync_DeveRetornarResponsePadrao_QuandoRespostaNula()
    {
        // Arrange
        var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("null", System.Text.Encoding.UTF8, "application/json")
        };

        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(responseMessage);

        var httpClient = new HttpClient(_httpMessageHandlerMock.Object);
        var service = new PagamentoService(
            httpClient,
            _configurationMock.Object,
            _pagamentoSettingsMock.Object,
            _loggerMock.Object
        );

        var request = CriarRequestValido();

        // Act
        var resultado = await service.CriarOrdemPagamentoAsync(request);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Status.Should().Be("pending");
    }

    [Fact]
    public async Task CriarOrdemPagamentoAsync_DeveLancarExcecao_QuandoErroHttp()
    {
        // Arrange
        var responseMessage = new HttpResponseMessage(HttpStatusCode.InternalServerError);

        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(responseMessage);

        var httpClient = new HttpClient(_httpMessageHandlerMock.Object);
        var service = new PagamentoService(
            httpClient,
            _configurationMock.Object,
            _pagamentoSettingsMock.Object,
            _loggerMock.Object
        );

        var request = CriarRequestValido();

        // Act
        var action = async () => await service.CriarOrdemPagamentoAsync(request);

        // Assert
        await action.Should().ThrowAsync<HttpRequestException>();
    }

    [Fact]
    public async Task CriarOrdemPagamentoAsync_DeveLancarExcecao_QuandoFalhaConexao()
    {
        // Arrange
        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ThrowsAsync(new HttpRequestException("Falha na conexão"));

        var httpClient = new HttpClient(_httpMessageHandlerMock.Object);
        var service = new PagamentoService(
            httpClient,
            _configurationMock.Object,
            _pagamentoSettingsMock.Object,
            _loggerMock.Object
        );

        var request = CriarRequestValido();

        // Act
        var action = async () => await service.CriarOrdemPagamentoAsync(request);

        // Assert
        await action.Should().ThrowAsync<HttpRequestException>();
    }

    [Fact]
    public async Task CriarOrdemPagamentoAsync_DeveFazerChamadaParaUrlCorreta()
    {
        // Arrange
        HttpRequestMessage? capturedRequest = null;

        var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = JsonContent.Create(new OrdemPagamentoResponse { Status = "pending" })
        };

        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .Callback<HttpRequestMessage, CancellationToken>((req, _) => capturedRequest = req)
            .ReturnsAsync(responseMessage);

        var httpClient = new HttpClient(_httpMessageHandlerMock.Object);
        var service = new PagamentoService(
            httpClient,
            _configurationMock.Object,
            _pagamentoSettingsMock.Object,
            _loggerMock.Object
        );

        var request = CriarRequestValido();

        // Act
        await service.CriarOrdemPagamentoAsync(request);

        // Assert
        capturedRequest.Should().NotBeNull();
        capturedRequest!.RequestUri!.ToString().Should().Be("http://localhost:5001/api/pagamento");
        capturedRequest.Method.Should().Be(HttpMethod.Post);
    }

    #endregion

    #region Helper Methods

    private static CriarOrdemPagamentoRequest CriarRequestValido()
    {
        return new CriarOrdemPagamentoRequest
        {
            EndToEndId = Guid.NewGuid().ToString(),
            ClientId = "client-123",
            Items = new List<ItemPagamento>
            {
                new()
                {
                    Id = Guid.NewGuid().ToString(),
                    Title = "Hamburguer",
                    Quantity = 1,
                    UnitPrice = 25.00m
                }
            }
        };
    }

    #endregion
}
