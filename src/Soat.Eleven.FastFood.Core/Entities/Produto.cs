using Soat.Eleven.FastFood.Core.ConditionRules;

namespace Soat.Eleven.FastFood.Core.Entities;

public class Produto
{
    public Guid Id { get; set; }
    private string nome;

    public string Nome
    {
        get { return nome; }
        set
        {
            Condition.Require(value, "Nome").IsNullOrEmpty();
            nome = value;
        }
    }
    private string sku;

    public string SKU
    {
        get { return sku; }
        set
        {
            sku = value;
        }
    }
    public string? Descricao { get; set; }
    public decimal Preco { get; set; }
    public Guid CategoriaId { get; set; }
    public bool Ativo { get; set; }
    public DateTime CriadoEm { get; set; }
    public string? Imagem { get; set; }
    public CategoriaProduto Categoria { get; set; } = null!;
}
