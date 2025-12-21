using Soat.Eleven.Pedidos.Core.DTOs.Pagamentos;
using Soat.Eleven.Pedidos.Core.DTOs.Pedidos;
using Soat.Eleven.Pedidos.Core.Interfaces.Gateways;

namespace Soat.Eleven.Pedidos.Core.UseCases;

/// <summary>
/// Caso de uso para operações de pagamento
/// </summary>
public class PagamentoUseCase
{
    private readonly IPagamentoGateway _pagamentoGateway;

    private PagamentoUseCase(IPagamentoGateway pagamentoGateway)
    {
        _pagamentoGateway = pagamentoGateway;
    }

    public static PagamentoUseCase Create(IPagamentoGateway pagamentoGateway)
    {
        return new PagamentoUseCase(pagamentoGateway);
    }

    /// <summary>
    /// Cria uma ordem de pagamento para um pedido
    /// </summary>
    /// <param name="pedido">Dados do pedido</param>
    /// <param name="clientId">ID do cliente para pagamento</param>
    /// <returns>Resposta da ordem de pagamento</returns>
    public async Task<OrdemPagamentoResponse?> CriarOrdemPagamento(PedidoOutputDto pedido, string clientId)
    {

        var itens = pedido.Itens.Select(item => new ItemPagamento
        {
            Id = item.ProdutoId.ToString(),
            Title = $"Produto {item.ProdutoId}",
            Quantity = item.Quantidade,
            UnitPrice = item.PrecoUnitario
        }).ToList();

        return await _pagamentoGateway.CriarOrdemPagamentoAsync(pedido.Id, clientId, itens);
    }
}
