using Soat.Eleven.FastFood.Core.Entities;

namespace Soat.Eleven.FastFood.Core.DTOs.Usuarios;

public class UsuarioAdmResponseDto
{
    public Guid Id { get; set; }
    public string Nome { get; set; }
    public string Email { get; set; }
    public string Telefone { get; set; }
}
