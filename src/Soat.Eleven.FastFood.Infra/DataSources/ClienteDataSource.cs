using Microsoft.EntityFrameworkCore;
using Soat.Eleven.FastFood.Adapter.Infra.EntityModel;
using Soat.Eleven.FastFood.Core.DTOs.Usuarios;
using Soat.Eleven.FastFood.Core.Enums;
using Soat.Eleven.FastFood.Core.Interfaces.DataSources;
using Soat.Eleven.FastFood.Infra.Data;

namespace Soat.Eleven.FastFood.Adapter.Infra.DataSources
{
    public class ClienteDataSource : IClienteDataSource
    {
        private readonly AppDbContext _context;
        private readonly DbSet<UsuarioModel> _dbSet;

        public ClienteDataSource(AppDbContext context)
        {
            _context = context;
            _dbSet = context.Set<UsuarioModel>();
        }

        public async Task<UsuarioClienteResponseDto> AddAsync(CriarClienteRequestDto dto)
        {
            var model = Parse(dto);
            await _dbSet.AddAsync(model);
            await _context.SaveChangesAsync();

            return Parse(model);
        }

        public async Task<UsuarioClienteResponseDto> UpdateAsync(AtualizarClienteRequestDto dto)
        {
            var model = Parse(dto);
            _dbSet.Update(model);
            await _context.SaveChangesAsync();

            return Parse(model);
        }

        public async Task<UsuarioClienteResponseDto> GetClienteByCPF(string cpf)
        {
            var exist = await _dbSet
                .AsNoTracking()
                .Where(u => u.Cliente.Cpf == cpf)
                .ToListAsync();

            return exist.Any() ? Parse(exist.First()) : null;
        }

        public async Task<bool> ExistCpf(string cpf)
        {
            var exist = await _dbSet
                .AsNoTracking()
                .Where(u => u.Cliente.Cpf == cpf)
                .ToListAsync();

            return exist.Any();
        }

        public async Task<bool> ExistEmail(string email)
        {
            var exist = await _dbSet
                .AsNoTracking()
                .Where(u => u.Email == email)
                .ToListAsync();

            return exist.Any();
        }

        public async Task<UsuarioClienteResponseDto> GetByUsuario(Guid usuarioId)
        {
            var result = await _dbSet
                .AsNoTracking()
                .Include(u => u.Cliente)
                .FirstOrDefaultAsync(e => e.Id == usuarioId);
            return result != null ? Parse(result) : null;
        }

        private static UsuarioClienteResponseDto Parse(UsuarioModel model)
        {
            return new UsuarioClienteResponseDto
            {
                Id = model.Id,
                Nome = model.Nome,
                Senha = model.Senha,
                Email = model.Email,
                Telefone = model.Telefone,
                ClientId = model.Cliente.Id,
                Cpf = model.Cliente.Cpf,
                DataDeNascimento = model.Cliente.DataDeNascimento,
            };
        }

        private static UsuarioModel Parse(CriarClienteRequestDto dto)
        {
            var cliente = new UsuarioModel(dto.Nome, dto.Email, dto.Senha, dto.Telefone, PerfilUsuario.Cliente);
            cliente.CriarCliente(dto.Cpf, dto.DataDeNascimento);

            return cliente;
        }

        private static UsuarioModel Parse(AtualizarClienteRequestDto dto)
        {
            var cliente = new UsuarioModel(dto.Nome, dto.Email, dto.Senha, dto.Telefone, PerfilUsuario.Cliente);
            cliente.Id = dto.Id;
            cliente.CriarCliente(dto.Cpf, dto.DataDeNascimento);
            cliente.Cliente.Id = dto.ClienteId;

            return cliente;
        }
    }
}
