using System.Text.Json.Serialization;

namespace Soat.Eleven.Pedidos.Core.DTOs.Pagamentos;

/// <summary>
/// Request para criação de ordem de pagamento
/// </summary>
public class CriarOrdemPagamentoRequest
{
    /// <summary>
    /// Id externo para identificação posterior
    /// </summary>
    [JsonPropertyName("end_to_end_id")]
    public required string EndToEndId { get; set; }

    /// <summary>
    /// ID do cliente (recuperado das configurações/variáveis de ambiente)
    /// </summary>
    [JsonPropertyName("client_id")]
    public required string ClientId { get; set; }

    /// <summary>
    /// Itens relacionados ao pagamento
    /// </summary>
    [JsonPropertyName("items")]
    public required List<ItemPagamento> Items { get; set; }
}
