using Soat.Eleven.FastFood.Application.Services;
using Soat.Eleven.FastFood.Core.DTOs.Categorias;
using Soat.Eleven.FastFood.Core.DTOs.Usuarios;
using Soat.Eleven.FastFood.Core.Gateways;
using Soat.Eleven.FastFood.Core.Interfaces.DataSources;
using Soat.Eleven.FastFood.Core.Presenters;
using Soat.Eleven.FastFood.Core.UseCases;

namespace Soat.Eleven.FastFood.Application.Controllers;

public class UsuarioController
{
    private readonly IUsuarioDataSource _usuarioDataSource;

    public UsuarioController(IUsuarioDataSource usuarioDataSource)
    {
        _usuarioDataSource = usuarioDataSource;
    }

    private UsuarioUseCase FabricarUseCase()
    {
        var usuarioGateway = new UsuarioGateway(_usuarioDataSource);
        return UsuarioUseCase.Create(usuarioGateway);
    }

    public async Task<UsuarioAdmResponseDto> InserirAdministradorAsync(CriarAdmRequestDto request)
    {
        var entity = UsuarioPresenter.Input(request);
        var useCase = FabricarUseCase();
        var result = await useCase.InserirAdministrador(entity);

        return UsuarioPresenter.Output(result);
    }

    public async Task<bool> AtualizarSenha(AtualizarSenhaRequestDto dto,Guid usuarioId)
    {
        var useCase = FabricarUseCase();
        await useCase.AlterarSenha(dto.NewPassword, dto.CurrentPassword, usuarioId);

        return true;
    }

    public async Task<object?> AtualizarAdministradorAsync(AtualizarAdmRequestDto request, Guid id)
    {
        var useCase = FabricarUseCase();

        var entity = UsuarioPresenter.Input(request);

        var result = await useCase.AtualizarAdministrador(entity, id);

        return result;
    }

    public async Task<object?> GetAdministradorAsync(Guid id)
    {
        var useCase = FabricarUseCase();        
        var result = await useCase.ObterUsuarioPodId(id);

        return result;
    }

    public async Task<IEnumerable<object>> GetAdministradores()
    {
        var useCase = FabricarUseCase();
        var result = await useCase.ObterUsuarios();

        return result;
    }
}
