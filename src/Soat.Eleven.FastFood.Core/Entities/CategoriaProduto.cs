using Soat.Eleven.FastFood.Core.ConditionRules;

namespace Soat.Eleven.FastFood.Core.Entities;

public class CategoriaProduto
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
    public string? Descricao { get; set; }
    public bool Ativo { get; set; } = true;

    public ICollection<Produto> Produtos { get; set; } = new List<Produto>();
}
