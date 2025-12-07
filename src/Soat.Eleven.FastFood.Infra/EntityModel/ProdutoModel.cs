namespace Soat.Eleven.FastFood.Adapter.Infra.EntityModel
{
    public class ProdutoModel
    {
        public Guid Id { get; set; }
        public string Nome { get; set; } = null!;
        public string SKU { get; set; } = null!;
        public string? Descricao { get; set; }
        public decimal Preco { get; set; }
        public Guid CategoriaId { get; set; }
        public bool Ativo { get; set; }
        public DateTime CriadoEm { get; set; }
        public string? Imagem { get; set; }
        public CategoriaProdutoModel Categoria { get; set; } = null!;
    }

}
