namespace Soat.Eleven.FastFood.Adapter.Infra.EntityModel
{
    public class CategoriaProdutoModel
    {
        public Guid Id { get; set; }
        public string Nome { get; set; } = null!;
        public string? Descricao { get; set; }
        public bool Ativo { get; set; } = true;

        public ICollection<ProdutoModel> Produtos { get; set; } = new List<ProdutoModel>();
    }

}
