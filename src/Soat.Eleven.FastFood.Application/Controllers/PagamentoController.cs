using Soat.Eleven.FastFood.Application.UseCases;
using Soat.Eleven.FastFood.Core.DTOs.Pagamentos;
using Soat.Eleven.FastFood.Core.DTOs.Webhooks;
using Soat.Eleven.FastFood.Core.Entities;
using Soat.Eleven.FastFood.Core.Enums;
using Soat.Eleven.FastFood.Core.Gateways;
using Soat.Eleven.FastFood.Common.Interfaces.DataSources;

namespace Soat.Eleven.FastFood.Application.Controllers;

public class PagamentoController
{
    private readonly IPagamentoDataSource _pagamentoDataSource;
    private readonly PagamentoGateway? _pagamentoGateway;

    public PagamentoController(IPagamentoDataSource pagamentoDataSource)
    {
        _pagamentoDataSource = pagamentoDataSource;
        _pagamentoGateway = new PagamentoGateway(pagamentoDataSource);
    }

    public PagamentoController(PagamentoGateway pagamentoGateway)
    {
        _pagamentoGateway = pagamentoGateway;
    }
    
    public async Task<ConfirmacaoPagamento> ConfirmarPagamento(NotificacaoPagamentoDto notificacaoPagamentoDto)
    {
        var useCase = new PagamentoUseCase(_pagamentoGateway!);
        var result = await useCase.ConfirmarPagamento(notificacaoPagamentoDto);
        return result;
    }

    public async Task<PagamentoPedido> CriarOrdemPagamento(CriarOrdemPagamentoDto request)
    {
        var pagamento = new PagamentoPedido(
            request.Tipo,
            request.Valor,
            StatusPagamento.Pendente,
            string.Empty)
        {
            PedidoId = request.PedidoId
        };

        return await _pagamentoDataSource.AddAsync(pagamento);
    }

    public async Task AtualizarStatusPagamento(PagamentoStatusDto notificacao)
    {
        var confirmacao = new ConfirmacaoPagamento(notificacao.Status, notificacao.Autorizacao ?? string.Empty);
        await _pagamentoDataSource.UpdateAsync(notificacao.PedidoId, confirmacao);
    }
}
