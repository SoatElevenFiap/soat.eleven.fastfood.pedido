using Soat.Eleven.FastFood.Core.DTOs.Usuarios;

namespace Soat.Eleven.FastFood.Core.Interfaces.DataSources;

public interface IUsuarioDataSource
{
    Task UpdatePasswordAsync(Guid id, string password);
    Task<UsuarioDto?> GetByIdAsync(Guid id);
    Task<UsuarioDto?> GetByEmailAndPassword(string email, string senha);
    Task<bool> ExistEmail(string email);
    Task<UsuarioDto?> AddAsync(UsuarioDto dto);
    Task<UsuarioDto?> UpdateAsync(UsuarioDto dto);
    Task<IEnumerable<UsuarioDto>> GetAllAsync();
}
