using Soat.Eleven.FastFood.Core.Enums;

namespace Soat.Eleven.FastFood.Core.DTOs.Pagamentos;

public class PagamentoStatusDto
{
    public Guid PedidoId { get; set; }
    public StatusPagamento Status { get; set; }
    public string? Autorizacao { get; set; }
}
