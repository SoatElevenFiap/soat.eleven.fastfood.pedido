using Soat.Eleven.Pedidos.Core.DTOs.Pagamentos;
using Soat.Eleven.Pedidos.Core.Interfaces.Gateways;
using Soat.Eleven.Pedidos.Core.Interfaces.Services;

namespace Soat.Eleven.Pedidos.Core.Gateways;

/// <summary>
/// Gateway para operações de pagamento
/// </summary>
public class PagamentoGateway : IPagamentoGateway
{
    private readonly IPagamentoService _pagamentoService;

    public PagamentoGateway(IPagamentoService pagamentoService)
    {
        _pagamentoService = pagamentoService;
    }

    public async Task<OrdemPagamentoResponse?> CriarOrdemPagamentoAsync(Guid pedidoId, string clientId, List<ItemPagamento> itens)
    {
        if (string.IsNullOrEmpty(clientId))
            return null;

        var request = new CriarOrdemPagamentoRequest
        {
            EndToEndId = pedidoId.ToString(),
            ClientId = clientId,
            Items = itens
        };

        return await _pagamentoService.CriarOrdemPagamentoAsync(request);
    }
}
