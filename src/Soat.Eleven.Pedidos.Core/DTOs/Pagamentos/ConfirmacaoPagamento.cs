using System.Text.Json.Serialization;

namespace Soat.Eleven.Pedidos.Core.DTOs.Pagamentos
{
    public class ConfirmacaoPagamento
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("end_to_end_id")]
        public string EndToEndId { get; set; }

        [JsonPropertyName("external_reference_id")]
        public string ExternalReferenceId { get; set; }

        [JsonPropertyName("client_id")]
        public string ClientId { get; set; }

        [JsonPropertyName("value")]
        public decimal Value { get; set; }

        [JsonPropertyName("provider")]
        public string Provider { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("redirect_url")]
        public string RedirectUrl { get; set; }

        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("updated_at")]
        public DateTime UpdatedAt { get; set; }
    }
}
