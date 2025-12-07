using Soat.Eleven.FastFood.Adapter.Infra.DataSources;
using Soat.Eleven.FastFood.Adapter.Infra.Services;
using Soat.Eleven.FastFood.Application.Services;
using Soat.Eleven.FastFood.Common.Interfaces.DataSources;
using Soat.Eleven.FastFood.Core.Interfaces.DataSources;
using Soat.Eleven.FastFood.Core.Interfaces.Gateways;
using Soat.Eleven.FastFood.Core.Interfaces.Services;
using Soat.Eleven.FastFood.Infra.Gateways;

namespace Microsoft.Extensions.DependencyInjection;

public static class RegisterServicesConfiguration
{
    public static void RegisterServices(this IServiceCollection serviceCollection)
    {
        #region Data Sources
        serviceCollection.AddScoped<ICategoriaProdutoDataSource, CategoriaProdutoDataSource>();
        serviceCollection.AddScoped<IProdutoDataSource, ProdutoDataSource>();
        serviceCollection.AddScoped<IPedidoDataSource, PedidoDataSource>();
        serviceCollection.AddScoped<IUsuarioDataSource, UsuarioDataSource>();
        serviceCollection.AddScoped<IClienteDataSource, ClienteDataSource>();
        serviceCollection.AddScoped<ITokenAtendimentoDataSource, TokenAtendimentoDataSource>();
        serviceCollection.AddScoped<IPagamentoDataSource, PagamentoDataSource>();
        #endregion       

        // Services
        //serviceCollection.AddScoped<IImagemService, ImagemService>();
        serviceCollection.AddScoped<IJwtTokenService, JwtTokenService>();
        serviceCollection.AddScoped<IMercadoPagoService, MercadoPagoService>();
    }
}
