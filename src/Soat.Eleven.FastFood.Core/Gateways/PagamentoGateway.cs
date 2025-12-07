using Soat.Eleven.FastFood.Common.Interfaces.DataSources;
using Soat.Eleven.FastFood.Core.DTOs;
using Soat.Eleven.FastFood.Core.DTOs.Pagamentos;
using Soat.Eleven.FastFood.Core.Enums;
using Soat.Eleven.FastFood.Core.Interfaces.Services;

namespace Soat.Eleven.FastFood.Core.Gateways;

public partial class PagamentoGateway
{
    private IPagamentoDataSource _pagamentoDataSource;
    private IValidarPagamento _validarPagamento;

    public PagamentoGateway(IPagamentoDataSource pagamentoDataSource)
    {
        _pagamentoDataSource = pagamentoDataSource;
    }
    
    public async Task<ConfirmacaoPagamento> ProcessarPagamento(Guid pedidoId)
    {
        ConfirmacaoPagamento confirmacaoPagamento = new ConfirmacaoPagamento(
            StatusPagamento.Pendente, 
            new Random().Next(100000, 999999).ToString());
        await _pagamentoDataSource.UpdateAsync(pedidoId, confirmacaoPagamento);
        return confirmacaoPagamento;
    }
    private async Task<bool> ValidarPagamento(TipoPagamentoDto tipoPagamentoDto) => tipoPagamentoDto?.Tipo == TipoPagamento.MercadoPago ? await _validarPagamento.ValidarNotificacao(tipoPagamentoDto.Signature, tipoPagamentoDto.Type) : true;

    public async Task<ConfirmacaoPagamento> AprovarPagamento(Guid pedidoId, TipoPagamentoDto tipoPagamentoDto)
    {
        await ValidarPagamento(tipoPagamentoDto);
        
        ConfirmacaoPagamento confirmacaoPagamento = new ConfirmacaoPagamento(
            StatusPagamento.Aprovado, 
            new Random().Next(100000, 999999).ToString());
        await _pagamentoDataSource.UpdateAsync(pedidoId, confirmacaoPagamento);
        return confirmacaoPagamento;
    }

    public async Task<ConfirmacaoPagamento> RejeitarPagamento(Guid pedidoId, TipoPagamentoDto tipoPagamentoDto)
    {
        await ValidarPagamento(tipoPagamentoDto);

        ConfirmacaoPagamento confirmacaoPagamento = new ConfirmacaoPagamento(
            StatusPagamento.Rejeitado,
            "0");

        await _pagamentoDataSource.UpdateAsync(pedidoId, confirmacaoPagamento);
        return confirmacaoPagamento;
    }
}