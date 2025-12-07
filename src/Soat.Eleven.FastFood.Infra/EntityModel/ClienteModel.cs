using Soat.Eleven.FastFood.Adapter.Infra.EntityModel.Base;

namespace Soat.Eleven.FastFood.Adapter.Infra.EntityModel;

public class ClienteModel : EntityBase
{
    public ClienteModel(string cpf, DateTime dataDeNascimento)
    {
        Cpf = cpf;
        DataDeNascimento = dataDeNascimento;
    }

    public string Cpf { get; set; }
    public DateTime DataDeNascimento { get; set; }
    public Guid UsuarioId { get; set; }
    public UsuarioModel Usuario { get; set; }
}
