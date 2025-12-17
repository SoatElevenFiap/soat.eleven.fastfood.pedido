using Soat.Eleven.FastFood.Core.DTOs.Pagamentos;

namespace Soat.Eleven.FastFood.Core.Interfaces.Services;

/// <summary>
/// Interface para comunicação com o serviço externo de pagamento
/// </summary>
public interface IPagamentoService
{
    Task<OrdemPagamentoResponse> CriarOrdemPagamentoAsync(CriarOrdemPagamentoRequest request);
}
