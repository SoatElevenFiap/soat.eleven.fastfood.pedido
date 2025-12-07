using Soat.Eleven.FastFood.Core.Entities;

namespace Soat.Eleven.FastFood.Core.DTOs.Usuarios;

public class UsuarioClienteResponseDto
{
    public Guid Id { get; set; }
    public Guid ClientId { get; set; }
    public string Nome { get; set; }
    public string Email { get; set; }
    public string Telefone { get; set; }
    public string Senha { get; set; }
    public string Cpf { get; set; }
    public DateTime DataDeNascimento { get; set; }
}
