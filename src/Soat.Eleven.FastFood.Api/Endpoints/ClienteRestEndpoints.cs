using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Soat.Eleven.FastFood.Application.Controllers;
using Soat.Eleven.FastFood.Application.Services;
using Soat.Eleven.FastFood.Core.DTOs.Usuarios;
using Soat.Eleven.FastFood.Core.Enums;
using Soat.Eleven.FastFood.Core.Interfaces.DataSources;

namespace Soat.Eleven.FastFood.Adapter.WebApi.Controllers;

[ApiController]
[Route("api/Cliente")]
public class ClienteRestEndpoints : ControllerBase
{
    private readonly IClienteDataSource _clienteDataSource;
    private readonly IJwtTokenService _jwtTokenService;

    public ClienteRestEndpoints(IClienteDataSource clienteGateway, IJwtTokenService jwtTokenService)
    {
        _clienteDataSource = clienteGateway;
        _jwtTokenService = jwtTokenService;
    }

    [HttpPost]
    public async Task<IActionResult> InserirCliente([FromBody] CriarClienteRequestDto request)
    {
        var controller = new ClienteController(_clienteDataSource, _jwtTokenService);
        return Ok(await controller.InserirClienteAsync(request));
    }

    [HttpPut("{id}")]
    [Authorize(PolicyRole.Cliente)]
    public async Task<IActionResult> AtualizarCliente([FromRoute] Guid id, [FromBody] AtualizarClienteRequestDto request)
    {
        request.Id = id;
        var controller = new ClienteController(_clienteDataSource, _jwtTokenService);
        return Ok(await controller.AtualizarClienteAsync(request));
    }

    [HttpGet]
    [Authorize(PolicyRole.Cliente)]
    public async Task<IActionResult> GetUsuario()
    {
        var controller = new ClienteController(_clienteDataSource, _jwtTokenService);
        return Ok(await controller.GetClienteAsync());
    }

    [HttpGet("PorCpf/{cpf}")]
    [Authorize(PolicyRole.Cliente)]
    public async Task<IActionResult> GetUsuario([FromRoute] string cpf)
    {
        var controller = new ClienteController(_clienteDataSource, _jwtTokenService);
        return Ok(await controller.GetByCPF(cpf));
    }
}
