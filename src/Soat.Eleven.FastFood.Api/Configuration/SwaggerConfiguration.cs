using Microsoft.OpenApi;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Swagger;

namespace Soat.Eleven.FastFood.Api.Configuration;

public static class SwaggerConfiguration
{
    public static void AddSwaggerConfiguration(this IServiceCollection services)
    {
        services.AddSwaggerGen(option =>
        {
            option.SwaggerDoc("v1", new OpenApiInfo()
            {
                Title = "FastFood Api",
                Description = @"
Projeto acadêmico desenvolvido para a disciplina de Arquitetura de Software (FIAP - Pós-graduação)

# Autenticação e Autorização

- **JWT Bearer Token**: Todas as rotas (exceto geração de token de atendimento) exigem autenticação via JWT.
- **Perfis de Usuário**:
    - `Administrador`: gerenciamento de categorias, produtos e operações de cozinha (preparação de pedidos).
    - `Cliente`: acesso aos próprios dados e pode cancelar próprios pedidos.
    - `ClienteTotem`: pode criar e pagar pedidos via totem.
    - `Commom`: acesso básico para visualização de dados gerais.
- **Como obter token**:
    1. Use o endpoint `/api/Auth` para autenticar com usuário e senha.
    2. Ou gere um token de atendimento anônimo via `/api/Atendimento/token/anonimo` ou por CPF via `/api/Atendimento/token/porCpf/{cpf}`.

- **Usuário e senha padrão (admin)**:
    - Usuário: `sistema@fastfood.com`
    - Senha: `Senha@123`

# Ordem recomendada de execução dos endpoints

1. **Autenticação**
    - `/api/Auth` (POST) — Login com usuário e senha
    - `/api/Atendimento/token/anonimo` (GET) — Token anônimo (sem auth)
    - `/api/Atendimento/token/porCpf/{cpf}` (GET) — Token por CPF (sem auth)

2. **Cadastro de Cliente (opcional)**
    - `/api/Usuario/Cliente` (POST) — Criação de cliente (sem auth necessária)

3. **Cadastro de Categorias e Produtos (obrigatório - apenas admin)**
    - `/api/Categoria` (POST) — Criação de categoria (PolicyRole.Administrador)
    - `/api/Produto` (POST) — Criação de produto (PolicyRole.Administrador)
    - `/api/Produto` (GET) — Listagem de produtos (requer auth)
    - `/api/Categoria` (GET) — Listagem de categorias (requer auth)

4. **Criação e Pagamento de Pedido (ClienteTotem)**
    - `/api/Pedido` (POST) — Criação de pedido (PolicyRole.ClienteTotem)
    - `/api/Pedido/{id}/pagar` (POST) — Pagamento (PolicyRole.ClienteTotem)

5. **Acompanhamento do Pedido**
    - `/api/Pedido/{id}` (GET) — Visualizar pedido (requer auth)
    - `/api/Pedido/{id}` (PUT) — Atualizar pedido (requer auth)

6. **Operações de Cozinha (apenas admin)**
    - `/api/Pedido` (GET) — Listar todos os pedidos (PolicyRole.Administrador)
    - `/api/Pedido/{id}/iniciar-preparacao` (POST) — (PolicyRole.Administrador)
    - `/api/Pedido/{id}/finalizar-preparacao` (POST) — (PolicyRole.Administrador)
    - `/api/Pedido/{id}/finalizar` (POST) — (PolicyRole.Administrador)

7. **Cancelamento de Pedido**
    - `/api/Pedido/{id}/cancelar` (POST) — Cancelar pedido (requer auth)

> **Importante:** Sempre envie o token JWT no header `Authorization: Bearer {token}` para acessar os endpoints protegidos.
> 
> **Observação:** Para criar um pedido, é obrigatório que exista pelo menos um produto cadastrado no sistema.

"
            });
            option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
            {
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n " +
                    "Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 1safsfsdfdfd\"",
            });
            option.AddSecurityRequirement(new OpenApiSecurityRequirement {
                {
                    new OpenApiSecurityScheme {
                        Reference = new OpenApiReference {
                            Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                        }
                    },
                    new string[] {}
                }
            });
        });
    }

    public static void UseSwaggerConfiguration(this WebApplication applicationBuilder)
    {
        if (applicationBuilder.Environment.IsDevelopment())
        {
            applicationBuilder.UseSwagger();
            applicationBuilder.UseSwaggerUI();
        }
    }
}
