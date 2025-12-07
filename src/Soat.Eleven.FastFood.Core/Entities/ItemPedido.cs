namespace Soat.Eleven.FastFood.Core.Entities;

public class ItemPedido
{
    public ItemPedido()
    {
        //Construtor vazio para o ORM
    }

    public ItemPedido(Guid produtoId, int quantidade, decimal precoUnitario, decimal descontoUnitario)
    {
        ProdutoId = produtoId;
        Quantidade = quantidade;
        PrecoUnitario = precoUnitario;
        DescontoUnitario = descontoUnitario;
    }

    public Guid Id { get; set; }
    public Guid PedidoId { get; set; }
    public Guid ProdutoId { get; set; }
    public int Quantidade { get; set; }        
    public decimal PrecoUnitario { get; set; }
    public decimal DescontoUnitario { get; set; }

    public Pedido Pedido { get; set; } = null!;
    public Produto Produto { get; set; } = null!;
}
