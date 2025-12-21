namespace Soat.Eleven.Pedidos.Core.DTOs.Pagamentos;

/// <summary>
/// Configurações do serviço de pagamento
/// </summary>
public class PagamentoSettings
{
    public const string SectionName = "PagamentoService";

    /// <summary>
    /// ID do cliente para identificação no serviço de pagamento
    /// </summary>
    public string ClientId { get; set; } = string.Empty;

    /// <summary>
    /// URL base do serviço de pagamento
    /// </summary>
    public string BaseUrl { get; set; } = "http://localhost:5001";
}
