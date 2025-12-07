using Soat.Eleven.FastFood.Core.DTOs.Usuarios;
using Soat.Eleven.FastFood.Core.Entities;
using Soat.Eleven.FastFood.Core.Interfaces.DataSources;

namespace Soat.Eleven.FastFood.Core.Gateways
{
    public class TokenAtendimentoGateway
    {
        private readonly ITokenAtendimentoDataSource _tokenAtendimentoDataSource;

        public TokenAtendimentoGateway(ITokenAtendimentoDataSource tokenAtendimentoDataSource)
        {
            _tokenAtendimentoDataSource = tokenAtendimentoDataSource;
        }

        public async Task AddAsync(TokenAtendimento tokenAtendimento)
        {
            var dto = new TokenAtendimentoDto
            {
                TokenId = tokenAtendimento.TokenId,
                ClienteId = tokenAtendimento.ClienteId,
                Cpf = tokenAtendimento.CpfCliente                   

            };

            await _tokenAtendimentoDataSource.AddAsync(dto);
        }

        public async Task<TokenAtendimento?> GetByIdAsync(Guid tokenId)
        {
            var dto = await _tokenAtendimentoDataSource.GetByIdAsync(tokenId);

            var entity = dto != null ? new TokenAtendimento
            {
                TokenId = dto.TokenId,
                ClienteId = dto.ClienteId,
                CpfCliente = dto.Cpf
            } : null;

            return entity;
        }

        public async Task<TokenAtendimento> GetMostRecentTokenByCpfAsync(string cpf)
        {
            var dto = await _tokenAtendimentoDataSource.GetMostRecentTokenByCpfAsync(cpf);

            var token = dto != null ? new TokenAtendimento
            {
                TokenId = dto.TokenId,
                ClienteId = dto.ClienteId,
                CpfCliente = dto.Cpf
            } : new TokenAtendimento();

            return token;
        }
    }
}
