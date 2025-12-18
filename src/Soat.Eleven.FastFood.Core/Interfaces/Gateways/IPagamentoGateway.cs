using Soat.Eleven.FastFood.Core.DTOs.Pagamentos;

namespace Soat.Eleven.FastFood.Core.Interfaces.Gateways;

/// <summary>
/// Interface de gateway para operações de pagamento
/// </summary>
public interface IPagamentoGateway
{
    /// <summary>
    /// Cria uma ordem de pagamento
    /// </summary>
    /// <param name="pedidoId">ID do pedido</param>
    /// <param name="clientId">ID do cliente para pagamento</param>
    /// <param name="itens">Itens do pagamento</param>
    /// <returns>Resposta da ordem de pagamento</returns>
    Task<OrdemPagamentoResponse?> CriarOrdemPagamentoAsync(Guid pedidoId, string clientId, List<ItemPagamento> itens);
}
