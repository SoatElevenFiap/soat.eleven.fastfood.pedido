using Soat.Eleven.FastFood.Core.DTOs.Webhooks;
using Soat.Eleven.FastFood.Core.Enums;
using Soat.Eleven.FastFood.Core.Interfaces.Services;

namespace Soat.Eleven.FastFood.Adapter.Infra.Services;

public class MercadoPagoService : IMercadoPagoService
{
    public StatusPagamento GetStatusPagamento(MercadoPagoNotificationDto notification)
    {
        // TODO: Validação real com SDK do Mercado Pago.
        return StatusPagamento.Aprovado;
    }

    public bool ValidarNotificacao(string signature)
    {
        // TODO: Validação real com SDK do Mercado Pago.
        return true;
    }
}