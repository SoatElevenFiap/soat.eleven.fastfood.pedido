using Soat.Eleven.FastFood.Core.Enums;

namespace Soat.Eleven.FastFood.Core.DTOs.Pagamentos
{
    public class SolicitacaoPagamento
    {
        public Guid PedidoId { get; set; }
        public TipoPagamento Tipo { get; set; }
        public decimal Valor { get; set; }
    }
}
