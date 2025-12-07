using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Soat.Eleven.FastFood.Application.Controllers;
using Soat.Eleven.FastFood.Core.DTOs.Usuarios;
using Soat.Eleven.FastFood.Core.Enums;
using Soat.Eleven.FastFood.Core.Interfaces.DataSources;

namespace Soat.Eleven.FastFood.Adapter.WebApi.Controllers;

[ApiController]
[Route("api/Administrador")]
public class AdministradorEndpoints : ControllerBase
{
    private readonly IUsuarioDataSource _usuarioDataSource;

    public AdministradorEndpoints(IUsuarioDataSource usuarioDataSource)
    {
        _usuarioDataSource = usuarioDataSource;
    }

    [HttpPost]
    [Authorize(PolicyRole.Administrador)]
    public async Task<IActionResult> InserirAdministrador([FromBody] CriarAdmRequestDto request)
    {
        var controller = new UsuarioController(_usuarioDataSource);
        return Ok(await controller.InserirAdministradorAsync(request));
    }

    [HttpPut("{id}")]
    [Authorize(PolicyRole.Administrador)]
    public async Task<IActionResult> AtualizarAdministrador([FromBody] AtualizarAdmRequestDto request, Guid id)
    {
        var controller = new UsuarioController(_usuarioDataSource);
        return Ok(await controller.AtualizarAdministradorAsync(request, id));
    }

    [HttpGet("{id}")]
    [Authorize(PolicyRole.Administrador)]
    public async Task<IActionResult> GetAdministrador(Guid id)
    {
        var controller = new UsuarioController(_usuarioDataSource);
        return Ok(await controller.GetAdministradorAsync(id));
    }

    [HttpGet]
    [Authorize(PolicyRole.Administrador)]
    public async Task<IActionResult> GetAdministradores()
    {
        var controller = new UsuarioController(_usuarioDataSource);
        return Ok(await controller.GetAdministradores());
    }
}
