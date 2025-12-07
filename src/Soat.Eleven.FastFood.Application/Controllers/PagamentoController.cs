using Soat.Eleven.FastFood.Application.UseCases;
using Soat.Eleven.FastFood.Core.DTOs.Pagamentos;
using Soat.Eleven.FastFood.Core.DTOs.Webhooks;
using Soat.Eleven.FastFood.Core.Gateways;

namespace Soat.Eleven.FastFood.Application.Controllers;

public class PagamentoController
{
    private readonly PagamentoGateway pagamentoGateway;

    public PagamentoController(PagamentoGateway pagamentoGateway)
    {
        this.pagamentoGateway = pagamentoGateway;
    }
    
    public async Task<ConfirmacaoPagamento> ConfirmarPagamento(NotificacaoPagamentoDto notificacaoPagamentoDto)
    {
        var useCase = new PagamentoUseCase(pagamentoGateway);
        var result = await useCase.ConfirmarPagamento(notificacaoPagamentoDto);
        return result;
    }
}
