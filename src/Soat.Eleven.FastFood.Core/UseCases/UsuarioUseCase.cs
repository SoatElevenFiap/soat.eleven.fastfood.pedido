using Soat.Eleven.FastFood.Core.Entities;
using Soat.Eleven.FastFood.Core.Gateways;

namespace Soat.Eleven.FastFood.Core.UseCases;
public class UsuarioUseCase
{
    private readonly UsuarioGateway _usuarioGateway;

    private UsuarioUseCase(UsuarioGateway usuarioGateway)
    {
        _usuarioGateway = usuarioGateway;
    }

    public static UsuarioUseCase Create(UsuarioGateway usuarioDataSource)
    {
        return new UsuarioUseCase(usuarioDataSource);
    }

    public Task<Usuario> ObterUsuarioPodId(Guid id)
    {
        return _usuarioGateway.ObterUsuarioPodId(id);
    }
    

    public async Task<Usuario> AlterarSenha(string newPassword, string currentPassword, Guid usuarioId)
    {
        var usuario = await _usuarioGateway.ObterUsuarioPodId(usuarioId);

        if (usuario is null)
            throw new Exception("Usuário não encontrado");

        if (Usuario.GeneratePassword(currentPassword) != usuario.Senha)
            throw new Exception("Senha atual está incorreta");

        usuario.Senha = Usuario.GeneratePassword(newPassword);

        await _usuarioGateway.AtualizarSenha(usuarioId, usuario.Senha);

        return usuario;
    }

    public async Task<Usuario> InserirAdministrador(Usuario entity)
    {
        var existeEmail = await _usuarioGateway.ExistEmail(entity.Email);

        if (existeEmail)
            throw new Exception("Usuário já existe");

        var administrador = await _usuarioGateway.CriarAdministrador(entity);

        return administrador;
    }

    public async Task<Usuario> AtualizarAdministrador(Usuario request, Guid id)
    {
        var adminstrador = await _usuarioGateway.GetByIdAsync(id);

        if (adminstrador is null)
            throw new Exception("Usuário não encontrado");

        if (request.Email != adminstrador.Email)
        {
            var existeEmail = await _usuarioGateway.ExistEmail(request.Email);

            if (existeEmail)
                throw new Exception("Endereço de e-mail já utilizado");
        }

        adminstrador.Id = id;
        adminstrador.Nome = request.Nome;
        adminstrador.Email = request.Email;
        adminstrador.Telefone = request.Telefone;

        var result = await _usuarioGateway.AtualizarAdministrador(adminstrador);

        return result;
    }

    public async Task<Usuario> Login(string email, string senha)
    {
        var usuario = await _usuarioGateway.ValidarLoginEObterUsuario(email, Usuario.GeneratePassword(senha));

        return usuario ?? throw new ArgumentException("E-mail e/ou Senha estão incorretos");
    }

    public async Task<IEnumerable<Usuario>> ObterUsuarios()
    {
        return await _usuarioGateway.ObterUsuarios();
    }
}
