using Soat.Eleven.FastFood.Core.Entities;
using Soat.Eleven.FastFood.Core.Enums;

namespace Soat.Eleven.FastFood.Core.DTOs.Usuarios;

public class CriarClienteRequestDto
{
    public string Nome { get; set; }
    public string Email { get; set; }
    public string Senha { get; set; }
    public string Telefone { get; set; }
    public string Cpf { get; set; }
    public DateTime DataDeNascimento { get; set; }

    public static implicit operator Cliente(CriarClienteRequestDto dto)
    {
        var cliente = new Cliente(dto.Nome,
                                  dto.Email,
                                  dto.Senha,
                                  dto.Telefone,
                                  PerfilUsuario.Cliente,
                                  StatusUsuario.Ativo,
                                  dto.Cpf,
                                  dto.DataDeNascimento);

        return cliente;
    }
}
