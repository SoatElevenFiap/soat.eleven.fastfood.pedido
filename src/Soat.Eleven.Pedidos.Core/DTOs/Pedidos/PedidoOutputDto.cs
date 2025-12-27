using Soat.Eleven.Pedidos.Core.Enums;

namespace Soat.Eleven.Pedidos.Core.DTOs.Pedidos
{
    public class PedidoOutputDto
    {
        public Guid Id { get; set; }
        public Guid TokenAtendimentoId { get; set; }
        public Guid? ClienteId { get; set; }
        public StatusPedido Status { get; set; }
        public string StatusNome => Status.ToString();
        public string SenhaPedido { get; set; } = null!;
        public decimal Subtotal { get; set; }
        public decimal Desconto { get; set; }
        public decimal Total { get; set; }

        public DateTime CriadoEm { get; set; }

        public ICollection<ItemPedidoOutputDto> Itens { get; set; } = [];
    }
}
