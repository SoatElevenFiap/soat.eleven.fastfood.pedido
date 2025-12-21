using Soat.Eleven.Pedidos.Core.Enums;

namespace Soat.Eleven.Pedidos.Core.AntiCorruption;

/// <summary>
/// Camada de anticorrupção para traduzir status de pagamento externo para status do pedido
/// </summary>
public static class PagamentoStatusTranslator
{
    /// <summary>
    /// Converte o status de pagamento recebido do serviço externo para StatusPedido
    /// </summary>
    public static StatusPedido? ToStatusPedido(string? statusPagamento)
    {
        return statusPagamento?.ToLower() switch
        {
            "pending" => null, // Mantém status atual do pedido
            "paid" => StatusPedido.Recebido,
            "failed" => StatusPedido.Cancelado,
            "cancelled" => StatusPedido.Cancelado,
            "refund_requested" => null, // Mantém status atual
            "refunded" => StatusPedido.Cancelado,
            "error" => null, // Mantém status atual
            _ => null
        };
    }

    /// <summary>
    /// Verifica se o pagamento foi aprovado
    /// </summary>
    public static bool IsPagamentoAprovado(string? statusPagamento)
    {
        return statusPagamento?.ToLower() == "paid";
    }

    /// <summary>
    /// Verifica se o pagamento foi rejeitado/cancelado
    /// </summary>
    public static bool IsPagamentoRejeitado(string? statusPagamento)
    {
        return statusPagamento?.ToLower() is "failed" or "cancelled" or "refunded" or "error";
    }
}
