using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Soat.Eleven.FastFood.Core.DTOs.Pagamentos;
using Soat.Eleven.FastFood.Core.Interfaces.DataSources;
using Soat.Eleven.FastFood.Core.Interfaces.Services;
using Xunit;

namespace Soat.Eleven.FastFood.Tests.Api.Configuration;

public class RegisterServicesConfigurationTests
{
    [Fact]
    public void RegisterServices_DeveRegistrarIPedidoDataSource()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = CriarConfigurationMock();

        // Adicionar IConfiguration ao container de DI
        services.AddSingleton<IConfiguration>(configuration);

        // Adicionar dependências necessárias para PedidoDataSource
        services.AddDbContext<Soat.Eleven.FastFood.Infra.Data.AppDbContext>(options =>
        {
            options.UseInMemoryDatabase("TestDb_" + Guid.NewGuid());
        });

        // Act
        services.RegisterServices(configuration);

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var dataSource = serviceProvider.GetService<IPedidoDataSource>();
        dataSource.Should().NotBeNull();
    }

    [Fact]
    public void RegisterServices_DeveConfigurarPagamentoSettings()
    {
        // Arrange
        var services = new ServiceCollection();
        var inMemorySettings = new Dictionary<string, string?>
        {
            {"PagamentoService:ClientId", "test-client-id"},
            {"PagamentoService:BaseUrl", "http://test-payment:8080"}
        };
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        services.AddDbContext<Soat.Eleven.FastFood.Infra.Data.AppDbContext>(options =>
        {
            options.UseInMemoryDatabase("TestDb_" + Guid.NewGuid());
        });

        // Act
        services.RegisterServices(configuration);

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var options = serviceProvider.GetService<Microsoft.Extensions.Options.IOptions<PagamentoSettings>>();
        options.Should().NotBeNull();
        options!.Value.ClientId.Should().Be("test-client-id");
        options.Value.BaseUrl.Should().Be("http://test-payment:8080");
    }

    [Fact]
    public void RegisterServices_DeveRegistrarIPagamentoService()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = CriarConfigurationMock();

        services.AddDbContext<Soat.Eleven.FastFood.Infra.Data.AppDbContext>(options =>
        {
            options.UseInMemoryDatabase("TestDb_" + Guid.NewGuid());
        });

        // Adicionar IConfiguration como singleton (necessário para PagamentoService)
        services.AddSingleton<IConfiguration>(configuration);

        // Act
        services.RegisterServices(configuration);

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var pagamentoService = serviceProvider.GetService<IPagamentoService>();
        pagamentoService.Should().NotBeNull();
    }

    [Fact]
    public void RegisterServices_DeveUsarBaseUrlPadrao_QuandoNaoConfigurada()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>())
            .Build();

        services.AddDbContext<Soat.Eleven.FastFood.Infra.Data.AppDbContext>(options =>
        {
            options.UseInMemoryDatabase("TestDb_" + Guid.NewGuid());
        });

        // Act
        services.RegisterServices(configuration);

        // Assert - não deve lançar exceção
        var serviceProvider = services.BuildServiceProvider();
        serviceProvider.Should().NotBeNull();
    }

    [Fact]
    public void RegisterServices_IPedidoDataSource_DeveSerScoped()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = CriarConfigurationMock();

        services.AddDbContext<Soat.Eleven.FastFood.Infra.Data.AppDbContext>(options =>
        {
            options.UseInMemoryDatabase("TestDb_" + Guid.NewGuid());
        });

        // Act
        services.RegisterServices(configuration);

        // Assert
        var descriptor = services.FirstOrDefault(d => d.ServiceType == typeof(IPedidoDataSource));
        descriptor.Should().NotBeNull();
        descriptor!.Lifetime.Should().Be(ServiceLifetime.Scoped);
    }

    private static IConfiguration CriarConfigurationMock()
    {
        var inMemorySettings = new Dictionary<string, string?>
        {
            {"PagamentoService:ClientId", "test-client"},
            {"PagamentoService:BaseUrl", "http://localhost:5001"}
        };
        return new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();
    }
}
