using Soat.Eleven.FastFood.Core.Entities;

namespace Soat.Eleven.FastFood.Core.DTOs.Pedidos
{
    public class ItemPedidoInputDto
    {
        public Guid ProdutoId { get; set; }
        public int Quantidade { get; set; }
        public decimal PrecoUnitario { get; set; }
        public decimal DescontoUnitario { get; set; }

        public static implicit operator ItemPedido(ItemPedidoInputDto inputDto)
        {
            return new ItemPedido(inputDto.ProdutoId, inputDto.Quantidade, inputDto.PrecoUnitario, inputDto.DescontoUnitario);
        }
    }
}
