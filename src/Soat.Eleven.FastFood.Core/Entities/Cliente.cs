using Soat.Eleven.FastFood.Core.Enums;

namespace Soat.Eleven.FastFood.Core.Entities;

public class Cliente : Usuario
{

    public Cliente(string nome,
                   string email,
                   string senha,
                   string telefone,
                   PerfilUsuario perfil,
                   StatusUsuario status,
                   string cpf,
                   DateTime dataDeNascimento) : base(nome, email, senha, telefone, perfil, status)
    {
        Cpf = cpf;
        DataDeNascimento = dataDeNascimento;
    }

    public Cliente(string nome,
                   string email,
                   string senha,
                   string telefone,
                   PerfilUsuario perfil,
                   StatusUsuario status,
                   string cpf,
                   DateTime dataDeNascimento,
                   Guid clienteId) : base(nome, email, senha, telefone, perfil, status)
    {
        Cpf = cpf;
        DataDeNascimento = dataDeNascimento;
        ClienteId = clienteId;
    }

    public Cliente()
    {
    }

    public string Cpf { get; set; }
    public DateTime DataDeNascimento { get; set; }
    public Guid ClienteId { get; set; }
}
