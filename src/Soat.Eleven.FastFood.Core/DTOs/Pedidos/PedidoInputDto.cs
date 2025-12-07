using Soat.Eleven.FastFood.Core.Entities;
using Soat.Eleven.FastFood.Core.Enums;

namespace Soat.Eleven.FastFood.Core.DTOs.Pedidos
{
    public class PedidoInputDto
    {
        public Guid Id { get; set; }
        public Guid TokenAtendimentoId { get; set; }
        public Guid? ClienteId { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Desconto { get; set; }
        public decimal Total { get; set; }
        public string SenhaPedido { get; set; } = string.Empty;
        public StatusPedido Status { get; set; }
        public ICollection<ItemPedidoInputDto> Itens { get; set; } = [];
    }
}
