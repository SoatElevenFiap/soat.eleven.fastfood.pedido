using Soat.Eleven.FastFood.Core.Enums;

namespace Soat.Eleven.FastFood.Application.Dtos;

public class UsuarioDto
{
    public UsuarioDto(Guid id, string nome, string email, PerfilUsuario perfil)
    {
        Id = id;
        Nome = nome;
        Email = email;
        Perfil = perfil;
    }
    public UsuarioDto()
    {
            
    }
    public Guid Id { get; set; }
    public string Nome { get; set; }
    public string Email { get; set; }
    public PerfilUsuario Perfil { get; set; }

}
