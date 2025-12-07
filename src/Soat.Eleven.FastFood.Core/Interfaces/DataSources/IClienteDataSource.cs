using Soat.Eleven.FastFood.Core.DTOs.Usuarios;
namespace Soat.Eleven.FastFood.Core.Interfaces.DataSources;

public interface IClienteDataSource
{
    Task<UsuarioClienteResponseDto> AddAsync(CriarClienteRequestDto dto);
    Task<UsuarioClienteResponseDto> UpdateAsync(AtualizarClienteRequestDto dto);
    Task<bool> ExistCpf(string cpf);
    Task<bool> ExistEmail(string email);
    Task<UsuarioClienteResponseDto> GetClienteByCPF(string cpf);
    Task<UsuarioClienteResponseDto> GetByUsuario(Guid usuarioId);
}
