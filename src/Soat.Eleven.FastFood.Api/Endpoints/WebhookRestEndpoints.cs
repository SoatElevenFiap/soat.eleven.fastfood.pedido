using Microsoft.AspNetCore.Mvc;
using Soat.Eleven.FastFood.Application.Controllers;
using Soat.Eleven.FastFood.Common.Interfaces.DataSources;
using Soat.Eleven.FastFood.Core.DTOs;
using Soat.Eleven.FastFood.Core.DTOs.Pagamentos;
using Soat.Eleven.FastFood.Core.DTOs.Webhooks;
using Soat.Eleven.FastFood.Core.Enums;
using Soat.Eleven.FastFood.Core.Gateways;
using Soat.Eleven.FastFood.Core.Interfaces.DataSources;

namespace Soat.Eleven.FastFood.Api.Controllers;

[ApiController]
[Route("api")]
public class WebhookRestEndpoints : ControllerBase
{
    private readonly PagamentoGateway _pagamentoGateway;
    private readonly PedidoController _pedidoController;

    public WebhookRestEndpoints(IPagamentoDataSource pagamentoDataSource,
                                IPedidoDataSource pedidoDataSource)
    {
        _pagamentoGateway = new PagamentoGateway(pagamentoDataSource);
        _pedidoController = new PedidoController(pedidoDataSource, pagamentoDataSource);
    }
    
    [HttpPost("Webhook/Pagamento/MercadoPago")]
    public async Task<IActionResult> ProcessarWebhookPagamentoMp(
        [FromQuery] String type, 
        [FromHeader(Name = "x-signature")] String signature,
        [FromBody] MercadoPagoNotificationDto request)
    {
        if (request.Id == "123")
        {
            await _pedidoController.PagarPedido(new SolicitacaoPagamento
            {
                PedidoId = Guid.Parse(request.Data.Id),
                Tipo = TipoPagamento.MercadoPago,
                Valor = 0
            }, _pagamentoGateway, new TipoPagamentoDto() { Signature = signature, Type = type });
        }
        else
        {
            await _pedidoController.RecusarPagamento(new SolicitacaoPagamento
            {
                PedidoId = Guid.Parse(request.Data.Id),
                Tipo = TipoPagamento.MercadoPago,
                Valor = 0
            }, _pagamentoGateway, new TipoPagamentoDto() { Signature = signature, Type = type });
        }

        return Ok();
    }
}
