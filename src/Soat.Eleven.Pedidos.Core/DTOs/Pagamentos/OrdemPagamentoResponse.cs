using System.Text.Json.Serialization;

namespace Soat.Eleven.Pedidos.Core.DTOs.Pagamentos;

public class OrdemPagamentoResponse
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("pedido_id")]
    public Guid PedidoId { get; set; }

    [JsonPropertyName("valor")]
    public decimal Valor { get; set; }

    [JsonPropertyName("status")]
    public string? Status { get; set; }

    [JsonPropertyName("redirect_url")]
    public string? RedirectUrl { get; set; }

    [JsonPropertyName("created_at")]
    public DateTime? CreatedAt { get; set; }
}
