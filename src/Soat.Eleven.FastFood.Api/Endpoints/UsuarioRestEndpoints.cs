using Microsoft.AspNetCore.Mvc;
using Soat.Eleven.FastFood.Application.Controllers;
using Soat.Eleven.FastFood.Application.Services;
using Soat.Eleven.FastFood.Core.DTOs.Usuarios;
using Soat.Eleven.FastFood.Core.Interfaces.DataSources;

namespace Soat.Eleven.FastFood.Api.Controllers;

[ApiController]
[Route("api")]
public class UsuarioRestEndpoints : ControllerBase
{
    private readonly IUsuarioDataSource _usuarioGateway;
    private readonly IJwtTokenService _jwtTokenService;

    public UsuarioRestEndpoints(IUsuarioDataSource usuarioGateway, IJwtTokenService jwtTokenService)
    {
        _usuarioGateway = usuarioGateway;
        _jwtTokenService = jwtTokenService;
    }

    [HttpPut("Usuario/Password")]
    public async Task<IActionResult> AtualizarSenha([FromBody] AtualizarSenhaRequestDto request)
    {
        var controller = new UsuarioController(_usuarioGateway);
        return Ok(await controller.AtualizarSenha(request, _jwtTokenService.GetIdUsuario()));
    }
}
