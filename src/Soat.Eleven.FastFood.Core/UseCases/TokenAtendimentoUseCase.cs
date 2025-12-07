using Soat.Eleven.FastFood.Core.Entities;
using Soat.Eleven.FastFood.Core.Gateways;
using Soat.Eleven.FastFood.Core.Interfaces.DataSources;

namespace Soat.Eleven.FastFood.Core.UseCases
{
    public class TokenAtendimentoUseCase
    {
        private readonly TokenAtendimentoGateway _tokenGateway;

        private TokenAtendimentoUseCase(
            TokenAtendimentoGateway tokenGateway)
        {
            _tokenGateway = tokenGateway;
        }

        public static TokenAtendimentoUseCase Create(
            TokenAtendimentoGateway tokenGateway)
        {
            return new TokenAtendimentoUseCase(tokenGateway);
        }

        public async Task<string> GetTokenPorCPF(string cpf, IUsuarioDataSource usuarioGateway)
        {
            var tokenAtendimentoDTO = await GerarToken(null, cpf);

            if (tokenAtendimentoDTO != null)
            {

                if (tokenAtendimentoDTO.ClienteId is null)
                    return tokenAtendimentoDTO.TokenId.ToString();

                var usuario = await usuarioGateway.GetByIdAsync(tokenAtendimentoDTO.ClienteId.Value);
                return tokenAtendimentoDTO.TokenId.ToString();
            }

            throw new Exception("Token não gerado");
        }

        public async Task<string> GetTokenAnonimo()
        {
            var token = await GerarToken();
            return token.TokenId.ToString();
        }

        /// <summary>
        /// Gera o Token de atendimento para o cliente.
        /// com ou sem identificação do cliente.
        /// </summary>
        /// <param name="clienteId"></param>
        /// <param name="cpf"></param>
        /// <returns></returns>
        private async Task<TokenAtendimento> TokenGen(Guid? clienteId = default, string? cpf = default)
        {
            try
            {
                var token = new TokenAtendimento
                {
                    TokenId = Guid.NewGuid(),
                    ClienteId = clienteId,
                    CpfCliente = cpf,
                    CriadoEm = DateTime.UtcNow,
                };

                await _tokenGateway.AddAsync(token);

                return token;
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "Erro ao gerar token de atendimento");
                throw;
            }
        }

        public async Task<TokenAtendimento> GerarToken(Guid? clienteId = default, string? cpf = default)
        {
            return await TokenGen(clienteId, cpf);
        }

        public TokenAtendimento RecuperarTokenAtendimento(Guid tokenId)
        {
            try
            {
                var token = _tokenGateway.GetByIdAsync(tokenId).Result;

                return token ?? throw new Exception("Token não encontrado");
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "Erro ao recuperar token de atendimento");
                throw;
            }
        }

        public async Task<TokenAtendimento?> RecuperarTokenMaisNovoPorCpfAsync(string cpf)
        {
            try
            {
                var token = await _tokenGateway.GetMostRecentTokenByCpfAsync(cpf);

                return token;
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "Erro ao recuperar token mais novo por CPF");
                throw;
            }
        }

        public async Task<TokenAtendimento> GerarToken(Cliente cliente)
        {
            try
            {
                return await GerarToken(cliente.Id, cliente.Cpf);
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "Erro ao gerar token para cliente");
                throw;
            }
        }
    }
}
