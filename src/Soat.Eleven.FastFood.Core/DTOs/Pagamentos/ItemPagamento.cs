using System.Text.Json.Serialization;

namespace Soat.Eleven.FastFood.Core.DTOs.Pagamentos;

/// <summary>
/// Item relacionado ao pagamento
/// </summary>
public class ItemPagamento
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("quantity")]
    public int Quantity { get; set; }

    [JsonPropertyName("unit_price")]
    public decimal UnitPrice { get; set; }
}
