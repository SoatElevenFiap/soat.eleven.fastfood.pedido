using Soat.Eleven.Pedidos.Core.AntiCorruption;
using Soat.Eleven.Pedidos.Core.DTOs.Pagamentos;
using Soat.Eleven.Pedidos.Core.Interfaces.DataSources;

namespace Soat.Eleven.Pedidos.Application.Controllers;

public class PagamentoController
{
    private readonly IPedidoDataSource _pedidoDataSource;

    public PagamentoController(IPedidoDataSource pedidoDataSource)
    {
        _pedidoDataSource = pedidoDataSource;
    }

    public async Task AtualizarStatusPagamento(ConfirmacaoPagamento notificacao, Guid pedidoId)
    {
        var novoStatus = PagamentoStatusTranslator.ToStatusPedido(notificacao.Status);

        if (novoStatus.HasValue)
        {
            await _pedidoDataSource.AtualizarStatusAsync(pedidoId, novoStatus.Value);
        }
    }
}
