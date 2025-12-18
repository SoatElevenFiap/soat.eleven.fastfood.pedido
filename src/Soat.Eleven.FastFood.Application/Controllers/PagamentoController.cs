using Soat.Eleven.FastFood.Core.AntiCorruption;
using Soat.Eleven.FastFood.Core.DTOs.Pagamentos;
using Soat.Eleven.FastFood.Core.Interfaces.DataSources;

namespace Soat.Eleven.FastFood.Application.Controllers;

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
