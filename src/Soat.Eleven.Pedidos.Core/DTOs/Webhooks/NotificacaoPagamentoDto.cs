using Soat.Eleven.Pedidos.Core.Enums;

namespace Soat.Eleven.Pedidos.Core.DTOs.Webhooks;

public class NotificacaoPagamentoDto
{
    public string ExternalId { get; set; }
    public string Type { get; set; }
    public string Signature { get; set; }
}