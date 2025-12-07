using Soat.Eleven.FastFood.Adapter.Infra.EntityModel.Base;

namespace Soat.Eleven.FastFood.Adapter.Infra.EntityModel
{
    public class ItemPedidoModel : EntityBase
    {
        public ItemPedidoModel()
        {
            //Construtor vazio para o ORM
        }

        public ItemPedidoModel(Guid produtoId, int quantidade, decimal precoUnitario, decimal descontoUnitario)
        {
            ProdutoId = produtoId;
            Quantidade = quantidade;
            PrecoUnitario = precoUnitario;
            DescontoUnitario = descontoUnitario;
        }

        public Guid PedidoId { get; set; }
        public Guid ProdutoId { get; set; }
        public int Quantidade { get; set; }        
        public decimal PrecoUnitario { get; set; }
        public decimal DescontoUnitario { get; set; }

        public PedidoModel Pedido { get; set; } = null!;
        public ProdutoModel Produto { get; set; } = null!;
    }

}
