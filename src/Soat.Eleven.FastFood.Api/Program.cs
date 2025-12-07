using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Soat.Eleven.FastFood.Api.Configuration;
using Soat.Eleven.FastFood.Infra.Data;
using Soat.Eleven.FastFood.Api.Adapters;
using Soat.Eleven.FastFood.Core.Enums;
using Soat.Eleven.FastFood.Core.Interfaces.Services;

const string SECRET_KEY_PASS = "5180e58ff93cef142763fdf3cc11f36c16335292a69bf201a4f72a834e625038032d04823966b02ff320564a0bc677c4bdcf3d67be724879b33711b04ba3e337";

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpContextAccessor();

builder.Services.AddLogging(loggingBuilder =>
{
    loggingBuilder.AddConsole();
    loggingBuilder.AddDebug();
});

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgresConnectionString")));

builder.Services.AddCors();

builder.Services.AddAuthentication(option =>
{
    option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(option =>
    {
        option.RequireHttpsMetadata = false;
        option.SaveToken = true;
        option.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration["SECRET_KEY_PASSWORK"] ?? SECRET_KEY_PASS)),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });

builder.Services.AddAuthorization(option =>
{
    option.AddPolicy("Cliente", policy => policy.RequireRole(RolesAuthorization.Cliente));
    option.AddPolicy("Administrador", policy => policy.RequireRole(RolesAuthorization.Administrador));
    option.AddPolicy("ClienteTotem", policy => policy.RequireRole([RolesAuthorization.Cliente, RolesAuthorization.IdentificacaoTotem]));
    option.AddPolicy("Commom", policy => policy.RequireRole([RolesAuthorization.Cliente, RolesAuthorization.Administrador]));
});

// Add Health Checks
builder.Services.AddHealthChecks()
    .AddCheck("self", () => Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy());

builder.Services.RegisterServices();
builder.Services.AddScoped<IArmazenamentoArquivoGateway, ArmazenamentoArquivoAdapter>();

builder.Services.AddSwaggerConfiguration();

var app = builder.Build();

app.UseMiddleware<ErrorExceptionHandlingMiddleware>(app.Logger);

// Configure the HTTP request pipeline.
app.UseSwaggerConfiguration();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseCors(x => x.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader());

app.UseAuthentication();
app.UseAuthorization();

// Map Health Check endpoints
app.MapHealthChecks("/health");

app.MapControllers();

app.Run();