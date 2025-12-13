using System.Text.Json.Serialization;

namespace Soat.Eleven.FastFood.Core.DTOs.Pagamentos;

public class CriarOrdemPagamentoRequest
{
    [JsonPropertyName("pedido_id")]
    public Guid PedidoId { get; set; }

    [JsonPropertyName("valor")]
    public decimal Valor { get; set; }

    [JsonPropertyName("descricao")]
    public string? Descricao { get; set; }
}
