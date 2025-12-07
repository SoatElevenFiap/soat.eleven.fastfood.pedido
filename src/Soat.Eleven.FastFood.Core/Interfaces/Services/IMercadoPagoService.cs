using Soat.Eleven.FastFood.Core.DTOs.Webhooks;
using Soat.Eleven.FastFood.Core.Enums;

namespace Soat.Eleven.FastFood.Core.Interfaces.Services;

public interface IMercadoPagoService
{
    bool ValidarNotificacao(string signature);
    StatusPagamento GetStatusPagamento(MercadoPagoNotificationDto notification);
}