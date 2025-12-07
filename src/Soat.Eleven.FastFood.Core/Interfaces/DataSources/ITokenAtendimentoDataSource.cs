using Soat.Eleven.FastFood.Core.DTOs.Usuarios;

namespace Soat.Eleven.FastFood.Core.Interfaces.DataSources;

public interface ITokenAtendimentoDataSource
{
    Task AddAsync(TokenAtendimentoDto tokenAtendimento);
    Task<TokenAtendimentoDto?> GetByIdAsync(Guid tokenId);
    Task<TokenAtendimentoDto?> GetMostRecentTokenByCpfAsync(string cpf);
}
