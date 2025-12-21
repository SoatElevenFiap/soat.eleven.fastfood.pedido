using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Soat.Eleven.Pedidos.Api.Configuration;
using System.IO;
using System.Net;
using Xunit;

namespace Soat.Eleven.Pedidos.Tests.Api.Configuration;

public class ErrorExceptionHandlingMiddlewareTests
{
    private readonly Mock<ILogger> _loggerMock;
    private readonly DefaultHttpContext _httpContext;

    public ErrorExceptionHandlingMiddlewareTests()
    {
        _loggerMock = new Mock<ILogger>();
        _httpContext = new DefaultHttpContext();
        _httpContext.Response.Body = new MemoryStream();
    }

    #region Invoke Tests - Success

    [Fact]
    public async Task Invoke_DevePassarParaProximoMiddleware_QuandoNaoHaExcecao()
    {
        // Arrange
        var nextCalled = false;
        RequestDelegate next = (ctx) =>
        {
            nextCalled = true;
            return Task.CompletedTask;
        };

        var middleware = new ErrorExceptionHandlingMiddleware(next, _loggerMock.Object);

        // Act
        await middleware.Invoke(_httpContext);

        // Assert
        nextCalled.Should().BeTrue();
    }

    #endregion

    #region Invoke Tests - Exception Handling

    [Fact]
    public async Task Invoke_DeveRetornarInternalServerError_QuandoExcecaoGenerica()
    {
        // Arrange
        RequestDelegate next = (ctx) => throw new Exception("Erro genérico");
        var middleware = new ErrorExceptionHandlingMiddleware(next, _loggerMock.Object);

        // Act
        await middleware.Invoke(_httpContext);

        // Assert
        _httpContext.Response.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
        _httpContext.Response.ContentType.Should().Be("application/json");
    }

    [Fact]
    public async Task Invoke_DeveRetornarNotFound_QuandoKeyNotFoundException()
    {
        // Arrange
        var innerException = new KeyNotFoundException("Não encontrado");
        RequestDelegate next = (ctx) => throw new Exception("Erro", innerException);
        var middleware = new ErrorExceptionHandlingMiddleware(next, _loggerMock.Object);

        // Act
        await middleware.Invoke(_httpContext);

        // Assert
        _httpContext.Response.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Invoke_DeveRetornarNotFound_QuandoFileNotFoundException()
    {
        // Arrange
        var innerException = new FileNotFoundException("Arquivo não encontrado");
        RequestDelegate next = (ctx) => throw new Exception("Erro", innerException);
        var middleware = new ErrorExceptionHandlingMiddleware(next, _loggerMock.Object);

        // Act
        await middleware.Invoke(_httpContext);

        // Assert
        _httpContext.Response.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Invoke_DeveRetornarUnauthorized_QuandoUnauthorizedAccessException()
    {
        // Arrange
        var innerException = new UnauthorizedAccessException("Acesso negado");
        RequestDelegate next = (ctx) => throw new Exception("Erro", innerException);
        var middleware = new ErrorExceptionHandlingMiddleware(next, _loggerMock.Object);

        // Act
        await middleware.Invoke(_httpContext);

        // Assert
        _httpContext.Response.StatusCode.Should().Be((int)HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Invoke_DeveRetornarBadRequest_QuandoArgumentException()
    {
        // Arrange
        var innerException = new ArgumentException("Argumento inválido");
        RequestDelegate next = (ctx) => throw new Exception("Erro", innerException);
        var middleware = new ErrorExceptionHandlingMiddleware(next, _loggerMock.Object);

        // Act
        await middleware.Invoke(_httpContext);

        // Assert
        _httpContext.Response.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Invoke_DeveRetornarBadRequest_QuandoInvalidOperationException()
    {
        // Arrange
        var innerException = new InvalidOperationException("Operação inválida");
        RequestDelegate next = (ctx) => throw new Exception("Erro", innerException);
        var middleware = new ErrorExceptionHandlingMiddleware(next, _loggerMock.Object);

        // Act
        await middleware.Invoke(_httpContext);

        // Assert
        _httpContext.Response.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Invoke_DeveRetornarBadRequest_QuandoFormatException()
    {
        // Arrange
        var innerException = new FormatException("Formato inválido");
        RequestDelegate next = (ctx) => throw new Exception("Erro", innerException);
        var middleware = new ErrorExceptionHandlingMiddleware(next, _loggerMock.Object);

        // Act
        await middleware.Invoke(_httpContext);

        // Assert
        _httpContext.Response.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Invoke_DeveEscreverMensagemDeErroNoResponse()
    {
        // Arrange
        RequestDelegate next = (ctx) => throw new Exception("Mensagem de erro teste");
        var middleware = new ErrorExceptionHandlingMiddleware(next, _loggerMock.Object);

        // Act
        await middleware.Invoke(_httpContext);

        // Assert
        _httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
        using var reader = new StreamReader(_httpContext.Response.Body);
        var responseBody = await reader.ReadToEndAsync();

        responseBody.Should().Contain("Mensagem de erro teste");
        responseBody.Should().Contain("status_code");
        responseBody.Should().Contain("message_error");
    }

    [Fact]
    public async Task Invoke_DeveLogarErro_QuandoExcecaoOcorre()
    {
        // Arrange
        RequestDelegate next = (ctx) => throw new Exception("Erro para logar");
        var middleware = new ErrorExceptionHandlingMiddleware(next, _loggerMock.Object);

        // Act
        await middleware.Invoke(_httpContext);

        // Assert
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)),
            Times.Once);
    }

    #endregion

    #region Content Type Tests

    [Fact]
    public async Task Invoke_DeveDefinirContentTypeComoJson_QuandoExcecao()
    {
        // Arrange
        RequestDelegate next = (ctx) => throw new Exception("Erro");
        var middleware = new ErrorExceptionHandlingMiddleware(next, _loggerMock.Object);

        // Act
        await middleware.Invoke(_httpContext);

        // Assert
        _httpContext.Response.ContentType.Should().Be("application/json");
    }

    #endregion
}
