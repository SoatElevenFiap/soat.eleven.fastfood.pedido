using Soat.Eleven.FastFood.Core.DTOs.Usuarios;
using Soat.Eleven.FastFood.Core.Entities;
using Soat.Eleven.FastFood.Core.Enums;
using Soat.Eleven.FastFood.Core.Interfaces.DataSources;

namespace Soat.Eleven.FastFood.Core.Gateways
{
    public class UsuarioGateway
    {
        private readonly IUsuarioDataSource _usuarioDataSource;

        public UsuarioGateway(IUsuarioDataSource usuarioDataSource)
        {
            _usuarioDataSource = usuarioDataSource;
        }

        public async Task<Usuario> ObterUsuarioPodId(Guid id)
        {
            var usuarioDto = await _usuarioDataSource.GetByIdAsync(id);

            if (usuarioDto == null)
                throw new KeyNotFoundException($"Usuário com Id {id} não encontrado.");

            return new Usuario
            {
                Id = usuarioDto.Id,
                Nome = usuarioDto.Nome,
                Email = usuarioDto.Email,
                Senha = usuarioDto.Senha
            };
        }

        public async Task<Usuario?> ValidarLoginEObterUsuario(string email, string senha)
        {
            var dto = await _usuarioDataSource.GetByEmailAndPassword(email, senha);

            if (dto == null)
                return null;

            return new Usuario
            (
                dto.Id,
                dto.Nome,
                dto.Email,
                dto.Senha,
                dto.Telefone,
                dto.Perfil,
                dto.Status
            );
        }

        public async Task AtualizarSenha(Guid id, string senha)
        {
            await _usuarioDataSource.UpdatePasswordAsync(id, senha);
        }

        public async Task<bool> ExistEmail(string email)
        {
            var existe = await _usuarioDataSource.ExistEmail(email);
            return existe;
        }

        public async Task<Usuario> CriarAdministrador(Usuario entity)
        {
            var dto = new UsuarioDto
            {
                Id = entity.Id,
                Nome = entity.Nome,
                Email = entity.Email,
                Senha = entity.Senha,
                Telefone = entity.Telefone,
                Perfil = entity.Perfil,
                Status = entity.Status
            };

            var u = await _usuarioDataSource.AddAsync(dto);

            if (u == null)
                throw new Exception("Erro ao inserir usuário");

            return new Usuario
            (
                u.Id,
                u.Nome,
                u.Email,
                u.Senha,
                u.Telefone,
                u.Perfil,
                u.Status
            );
        }

        public async Task<Usuario> GetByIdAsync(Guid usuarioId)
        {
            var usuarioDto = await _usuarioDataSource.GetByIdAsync(usuarioId);

            if (usuarioDto == null)
                throw new KeyNotFoundException($"Usuário com Id {usuarioId} não encontrado.");

            return new Usuario
            (
                usuarioDto.Id,
                usuarioDto.Nome,
                usuarioDto.Email,
                usuarioDto.Senha,
                usuarioDto.Telefone,
                usuarioDto.Perfil,
                usuarioDto.Status
            );
        }

        public async Task<Usuario> AtualizarAdministrador(Usuario entity)
        {
            var dto = new UsuarioDto
            {
                Id = entity.Id,
                Nome = entity.Nome,
                Email = entity.Email,
                Senha = entity.Senha,
                Telefone = entity.Telefone,
                Perfil = entity.Perfil,
                Status = entity.Status
            };

            var u = await _usuarioDataSource.UpdateAsync(dto);

            return new Usuario
            (
                u.Id,
                u.Nome,
                u.Email,
                u.Senha,
                u.Telefone,
                u.Perfil,
                u.Status
            );
        }

        public async Task<IEnumerable<Usuario>> ObterUsuarios()
        {
            var usuariosDto = await _usuarioDataSource.GetAllAsync();

            return usuariosDto.Select(dto => new Usuario
            (
                dto.Id,
                dto.Nome,
                dto.Email,
                dto.Senha,
                dto.Telefone,
                dto.Perfil,
                dto.Status
            )).ToList();
        }
    }
}
