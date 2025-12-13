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
        var confirmacaoPagamento = new ConfirmacaoPagamento
        {
            Id = new Random().Next(100000, 999999).ToString(),
            Status = "pending",
            Value = 0
        };
        await _pagamentoDataSource.UpdateAsync(pedidoId, confirmacaoPagamento);
        return confirmacaoPagamento;
    }
    private async Task<bool> ValidarPagamento(TipoPagamentoDto tipoPagamentoDto) => tipoPagamentoDto?.Tipo == TipoPagamento.MercadoPago ? await _validarPagamento.ValidarNotificacao(tipoPagamentoDto.Signature, tipoPagamentoDto.Type) : true;

    public async Task<ConfirmacaoPagamento> AprovarPagamento(Guid pedidoId, TipoPagamentoDto tipoPagamentoDto)
    {
        await ValidarPagamento(tipoPagamentoDto);
        
        var confirmacaoPagamento = new ConfirmacaoPagamento
        {
            Id = new Random().Next(100000, 999999).ToString(),
            Status = "approved",
            Value = 0
        };
        await _pagamentoDataSource.UpdateAsync(pedidoId, confirmacaoPagamento);
        return confirmacaoPagamento;
    }

    public async Task<ConfirmacaoPagamento> RejeitarPagamento(Guid pedidoId, TipoPagamentoDto tipoPagamentoDto)
    {
        await ValidarPagamento(tipoPagamentoDto);

        var confirmacaoPagamento = new ConfirmacaoPagamento
        {
            Id = "0",
            Status = "rejected",
            Value = 0
        };

        await _pagamentoDataSource.UpdateAsync(pedidoId, confirmacaoPagamento);
        return confirmacaoPagamento;
    }
}