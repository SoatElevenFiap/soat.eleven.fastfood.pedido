using Soat.Eleven.FastFood.Core.Entities;

namespace Soat.Eleven.FastFood.Core.DTOs.Pedidos
{
    public class PagamentoPedidoOutputDto
    {
        public Guid Id { get; set; }
        public string Tipo { get; set; }
        public decimal Valor { get; set; }
        public decimal Troco { get; set; }
        public string Status { get; set; }
        public string Autorizacao { get; set; }

        public static explicit operator PagamentoPedidoOutputDto(PagamentoPedido pagamentoPedido)
        {
            return new PagamentoPedidoOutputDto()
            {
                Id = pagamentoPedido.Id,
                Tipo = pagamentoPedido.Tipo.ToString(),
                Valor = pagamentoPedido.Valor,
                Troco = pagamentoPedido.Troco,
                Status = pagamentoPedido.Status.ToString(),
                Autorizacao = pagamentoPedido.Autorizacao
            };
        }
    }
}
