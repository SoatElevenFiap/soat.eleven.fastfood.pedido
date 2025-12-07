using Microsoft.EntityFrameworkCore;
using Soat.Eleven.FastFood.Adapter.Infra.EntityModel;
using Soat.Eleven.FastFood.Core.DTOs.Usuarios;
using Soat.Eleven.FastFood.Core.Interfaces.DataSources;
using Soat.Eleven.FastFood.Infra.Data;

namespace Soat.Eleven.FastFood.Adapter.Infra.DataSources
{
    public class TokenAtendimentoDataSource : ITokenAtendimentoDataSource
    {
        private readonly AppDbContext _context;
        private readonly DbSet<TokenAtendimentoModel> _dbSet;

        public TokenAtendimentoDataSource(AppDbContext context)
        {
            _context = context;
            _dbSet = context.Set<TokenAtendimentoModel>();
        }

        public async Task AddAsync(TokenAtendimentoDto entity)
        {
            var model = Parse(entity);

            await _dbSet.AddAsync(model);
            await _context.SaveChangesAsync();
        }

        public async Task<TokenAtendimentoDto?> GetByIdAsync(Guid id)
        {
            var tokenAtendimento = await _dbSet.FindAsync(id);

            return tokenAtendimento != null ? Parse(tokenAtendimento) : null;
        }

        public async Task<TokenAtendimentoDto?> GetMostRecentTokenByCpfAsync(string cpf)
        {
            var result = await _dbSet
                .Where(t => t.Cpf == cpf)
                .ToListAsync();

            var token = result.OrderByDescending(t => t.CriadoEm)
                .FirstOrDefault();

            var entity = token != null ? Parse(token) : null;

            return entity;
        }

        private static TokenAtendimentoModel Parse(TokenAtendimentoDto entity)
        {
            var model = new TokenAtendimentoModel()
            {
                TokenId = entity.TokenId,
                ClienteId = entity.ClienteId,
                Cpf = entity.Cpf
            };
            return model;
        }

        private static TokenAtendimentoDto Parse(TokenAtendimentoModel model)
        {
            return new TokenAtendimentoDto()
            {
                TokenId = model.TokenId,
                ClienteId = model.ClienteId,
                Cpf = model.Cpf,
            };
        }
    }
}
