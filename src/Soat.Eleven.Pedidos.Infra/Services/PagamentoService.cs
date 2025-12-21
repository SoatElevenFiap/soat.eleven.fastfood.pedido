using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Soat.Eleven.Pedidos.Core.DTOs.Pagamentos;
using Soat.Eleven.Pedidos.Core.Interfaces.Services;

namespace Soat.Eleven.Pedidos.Adapter.Infra.Services;

/// <summary>
/// Implementação do serviço de pagamento que faz chamada REST para o serviço externo
/// </summary>
public class PagamentoService : IPagamentoService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<PagamentoService> _logger;
    private readonly string _baseUrl;
    private readonly string _clientId;

    public PagamentoService(HttpClient httpClient, IConfiguration configuration, IOptions<PagamentoSettings> pagamentoSettings, ILogger<PagamentoService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _baseUrl = configuration["PagamentoService:BaseUrl"] ?? "http://localhost:5001";
        _clientId = pagamentoSettings.Value.ClientId;
    }

    /// <summary>
    /// Obtém o ClientId configurado
    /// </summary>
    public string GetClientId() => _clientId;

    public async Task<OrdemPagamentoResponse> CriarOrdemPagamentoAsync(CriarOrdemPagamentoRequest request)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/api/pagamento", request);
            
            response.EnsureSuccessStatusCode();
            
            var result = await response.Content.ReadFromJsonAsync<OrdemPagamentoResponse>();
            
            return result ?? new OrdemPagamentoResponse 
            { 
                Status = "pending"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar ordem de pagamento para pedido {EndToEndId}", request.EndToEndId);
            throw;
        }
    }
}
