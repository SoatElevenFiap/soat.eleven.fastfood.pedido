using Soat.Eleven.FastFood.Adapter.Infra.EntityModel.Base;
using Soat.Eleven.FastFood.Core.Enums;

namespace Soat.Eleven.FastFood.Adapter.Infra.EntityModel
{
    public class PagamentoPedidoModel : EntityBase
    {
        public PagamentoPedidoModel()
        {
            //Construtor vazio para o ORM
        }

        public PagamentoPedidoModel(TipoPagamento tipo, decimal valor, StatusPagamento status, string autorizacao)
        {
            Tipo = tipo;
            Valor = valor;
            Status = status;
            Autorizacao = autorizacao;
        }

        public Guid PedidoId { get; set; }
        public TipoPagamento Tipo { get; set; }
        public decimal Valor { get; set; }
        public decimal Troco { get; set; }
        public StatusPagamento Status { get; set; }
        public string Autorizacao { get; set; }

        public PedidoModel Pedido { get; set; } = null!;
    }
}
