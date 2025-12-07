using Soat.Eleven.FastFood.Core.DTOs.Pagamentos;
using Soat.Eleven.FastFood.Core.DTOs.Pedidos;

namespace Soat.Eleven.FastFood.Common.Interfaces.DataSources;

public interface IPagamentoDataSource
{
    Task UpdateAsync(Guid pedidoId, ConfirmacaoPagamento produto);
    Task<StatusPagamentoPedidoDto> StatusPedido(Guid pedidoId);

}