using Soat.Eleven.FastFood.Core.Enums;

namespace Soat.Eleven.FastFood.Core.Entities
{
    public class PagamentoPedido
    {
        public PagamentoPedido(TipoPagamento tipo, decimal valor, StatusPagamento status, string autorizacao)
        {
            Tipo = tipo;
            Valor = valor;
            Status = status;
            Autorizacao = autorizacao;
        }
        public Guid Id { get; set; }
        public Guid PedidoId { get; set; }
        public TipoPagamento Tipo { get; set; }
        public decimal Valor { get; set; }
        public decimal Troco { get; set; }
        public StatusPagamento Status { get; set; }
        public string Autorizacao { get; set; }

        public Pedido Pedido { get; set; } = null!;
    }
}
