using Soat.Eleven.FastFood.Adapter.Infra.DataSources;
using Soat.Eleven.FastFood.Adapter.Infra.Services;
using Soat.Eleven.FastFood.Core.DTOs.Pagamentos;
using Soat.Eleven.FastFood.Core.Interfaces.DataSources;
using Soat.Eleven.FastFood.Core.Interfaces.Services;

namespace Microsoft.Extensions.DependencyInjection;

public static class RegisterServicesConfiguration
{
    public static void RegisterServices(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        #region Data Sources
        serviceCollection.AddScoped<IPedidoDataSource, PedidoDataSource>();
        #endregion       

        #region Settings
        // Registrar PagamentoSettings para injeção de dependência
        serviceCollection.Configure<PagamentoSettings>(configuration.GetSection(PagamentoSettings.SectionName));
        #endregion

        #region Services
        // Configurar HttpClient para o serviço de pagamento
        var pagamentoBaseUrl = configuration["PagamentoService:BaseUrl"] ?? "http://localhost:5001";
        serviceCollection.AddHttpClient<IPagamentoService, PagamentoService>(client =>
        {
            client.BaseAddress = new Uri(pagamentoBaseUrl);
        });
        #endregion
    }
}
