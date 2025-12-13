using Soat.Eleven.FastFood.Core.Enums;

namespace Soat.Eleven.FastFood.Core.DTOs.Pagamentos;

public class CriarOrdemPagamentoDto
{
    public Guid PedidoId { get; set; }
    public decimal Valor { get; set; }
    public TipoPagamento Tipo { get; set; } = TipoPagamento.MercadoPago;
}
