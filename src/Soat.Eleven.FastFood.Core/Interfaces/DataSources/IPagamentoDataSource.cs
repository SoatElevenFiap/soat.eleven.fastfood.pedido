using Soat.Eleven.FastFood.Core.DTOs.Pagamentos;
using Soat.Eleven.FastFood.Core.DTOs.Pedidos;
using Soat.Eleven.FastFood.Core.Entities;

namespace Soat.Eleven.FastFood.Common.Interfaces.DataSources;

public interface IPagamentoDataSource
{
    Task<PagamentoPedido> AddAsync(PagamentoPedido pagamento);
    Task UpdateAsync(Guid pedidoId, ConfirmacaoPagamento confirmacao);
    Task<StatusPagamentoPedidoDto> StatusPedido(Guid pedidoId);
}