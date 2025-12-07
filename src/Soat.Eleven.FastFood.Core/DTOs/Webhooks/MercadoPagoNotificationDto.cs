using System.Text.Json.Serialization;

namespace Soat.Eleven.FastFood.Core.DTOs.Webhooks;

public class MercadoPagoNotificationDto
{
    public string Action { get; set; }
    
    [JsonPropertyName("api_version")]
    public string ApiVersion { get; set; }
    public NotificationData Data { get; set; }
    
    [JsonPropertyName("date_created")]
    public DateTime DateCreated { get; set; }
    public string Id { get; set; }
    
    [JsonPropertyName("live_mode")]
    public bool LiveMode { get; set; }
    public string Type { get; set; }
    
    [JsonPropertyName("user_id")]
    public long UserId { get; set; }
}

public class NotificationData
{
    public string Id { get; set; }
}