using Soat.Eleven.Pedidos.Core.Enums;

namespace Soat.Eleven.Pedidos.Core.DTOs.Pedidos
{
    /// <summary>
    /// DTO de resposta para criação de pedido, incluindo URL de redirecionamento para pagamento
    /// </summary>
    public class CriarPedidoOutputDto
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

        /// <summary>
        /// URL de redirecionamento para o pagamento
        /// </summary>
        public string? RedirectUrl { get; set; }

        /// <summary>
        /// Cria um CriarPedidoOutputDto a partir de um PedidoOutputDto
        /// </summary>
        public static CriarPedidoOutputDto FromPedidoOutputDto(PedidoOutputDto pedido, string? redirectUrl = null)
        {
            return new CriarPedidoOutputDto
            {
                Id = pedido.Id,
                TokenAtendimentoId = pedido.TokenAtendimentoId,
                ClienteId = pedido.ClienteId,
                Status = pedido.Status,
                SenhaPedido = pedido.SenhaPedido,
                Subtotal = pedido.Subtotal,
                Desconto = pedido.Desconto,
                Total = pedido.Total,
                CriadoEm = pedido.CriadoEm,
                Itens = pedido.Itens,
                RedirectUrl = redirectUrl
            };
        }
    }
}
