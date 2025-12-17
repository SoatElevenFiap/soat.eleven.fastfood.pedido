using Soat.Eleven.FastFood.Core.AntiCorruption;
using Soat.Eleven.FastFood.Core.DTOs.Pagamentos;
using Soat.Eleven.FastFood.Core.Interfaces.DataSources;
using Soat.Eleven.FastFood.Core.Interfaces.Services;

namespace Soat.Eleven.FastFood.Application.Controllers;

public class PagamentoController
{
    private readonly IPedidoDataSource _pedidoDataSource;
    private readonly IPagamentoService? _pagamentoService;

    public PagamentoController(IPedidoDataSource pedidoDataSource)
    {
        _pedidoDataSource = pedidoDataSource;
    }

    public PagamentoController(IPedidoDataSource pedidoDataSource, IPagamentoService pagamentoService)
    {
        _pedidoDataSource = pedidoDataSource;
        _pagamentoService = pagamentoService;
    }

    public async Task<OrdemPagamentoResponse> CriarOrdemPagamento(CriarOrdemPagamentoRequest request)
    {
        if (_pagamentoService == null)
            throw new InvalidOperationException("Serviço de pagamento não configurado");

        return await _pagamentoService.CriarOrdemPagamentoAsync(request);
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
