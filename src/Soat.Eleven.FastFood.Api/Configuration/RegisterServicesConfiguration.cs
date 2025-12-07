using Soat.Eleven.FastFood.Adapter.Infra.DataSources;
using Soat.Eleven.FastFood.Adapter.Infra.Services;
using Soat.Eleven.FastFood.Core.Interfaces.DataSources;
using Soat.Eleven.FastFood.Core.Interfaces.Services;
using Soat.Eleven.FastFood.Common.Interfaces.DataSources;

namespace Microsoft.Extensions.DependencyInjection;

public static class RegisterServicesConfiguration
{
    public static void RegisterServices(this IServiceCollection serviceCollection)
    {
        #region Data Sources
        serviceCollection.AddScoped<IPedidoDataSource, PedidoDataSource>();
        serviceCollection.AddScoped<IPagamentoDataSource, PagamentoDataSource>();
        #endregion       

        // Services
        serviceCollection.AddScoped<IMercadoPagoService, MercadoPagoService>();
    }
}
