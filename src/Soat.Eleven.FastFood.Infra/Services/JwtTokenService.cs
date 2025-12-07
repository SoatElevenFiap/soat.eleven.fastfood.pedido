using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Soat.Eleven.FastFood.Application.Services;
using System.IdentityModel.Tokens.Jwt;

namespace Soat.Eleven.FastFood.Adapter.Infra.Services;

public class JwtTokenService : IJwtTokenService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private const string _keyTokenAtendimento = "TokenAtendimento";

    public JwtTokenService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid GetIdUsuario()
    {
        var id = ReadToken(JwtRegisteredClaimNames.Sub);

        return Guid.Parse(id);
    }

    public string GetTokenAtendimento()
    {
        return ReadToken(_keyTokenAtendimento);
    }

    private string ReadToken(string typeClaim)
    {
        var user = _httpContextAccessor.HttpContext?.User;
        var token = _httpContextAccessor.HttpContext?.Request.Headers.Authorization.First() ?? throw new AuthenticationFailureException("Usuário não autenticado");

        var handler = new JwtSecurityTokenHandler();
        var jsonToken = handler.ReadToken(token.Replace("Bearer ", string.Empty)) ?? throw new AuthenticationFailureException("Usuário não autenticado");

        return (jsonToken as JwtSecurityToken)!.Claims.First(x => x.Type == typeClaim).Value;
    }
}
