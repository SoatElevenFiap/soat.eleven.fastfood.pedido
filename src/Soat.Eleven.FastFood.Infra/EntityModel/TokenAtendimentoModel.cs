namespace Soat.Eleven.FastFood.Adapter.Infra.EntityModel
{

    public class TokenAtendimentoModel
    {
        public Guid TokenId { get; set; }
        public Guid? ClienteId { get; set; }
        public string? Cpf { get; set; }
        public DateTime CriadoEm { get; set; }
        public ClienteModel? Cliente { get; set; }
    }
}
