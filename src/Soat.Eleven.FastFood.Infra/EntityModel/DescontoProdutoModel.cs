using Soat.Eleven.FastFood.Adapter.Infra.EntityModel.Base;

namespace Soat.Eleven.FastFood.Adapter.Infra.EntityModel
{
    public class DescontoProdutoModel : EntityBase
    {
        public Guid DescontoProdutoId { get; set; }
        public string Nome { get; set; } = null!;
        public Guid ProdutoId { get; set; }
        public decimal Valor { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime? DataFim { get; set; }
        public bool Ativo { get; set; }
        public ProdutoModel Produto { get; set; } = null!;
    }
}
