using Azure.Core;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using System.Diagnostics.CodeAnalysis;

namespace Soat.Eleven.FastFood.Pedidos.Api.Configuration;

[SuppressMessage(
    "Security",
    "S4502",
    Justification = "Configuração de acesso ao Azure Key Vault"
)]
public static class KeyVaultConfiguration
{
    public static WebApplicationBuilder ConfigureKeyVault(this WebApplicationBuilder builder)
    {
        var keyVaultName = builder.Configuration["KeyVault:Name"];

        var isDevelopmentEnvironment = builder.Environment.IsDevelopment();
        TokenCredential credential;

        if (isDevelopmentEnvironment)
        {
            Console.WriteLine("🔧 Ambiente de desenvolvimento detectado - usando configuração local");

            credential = new ChainedTokenCredential(
                new AzureCliCredential(),
                new EnvironmentCredential(),
                new ManagedIdentityCredential()
            );
        }
        else
        {
            Console.WriteLine("🔧 Ambiente de producao detectado - usando configuração de Produção");

            credential = new DefaultAzureCredential();
        }

        if (!string.IsNullOrEmpty(keyVaultName))
        {
            SetKeyVault(builder, keyVaultName, credential);
            Console.WriteLine("Configuração do Key Vault para SecretKey carregada com sucesso.");
        }

        return builder;
    }

    private static void SetKeyVault(WebApplicationBuilder builder, string keyVaultName, TokenCredential credential)
    {
        try
        {
            builder.Configuration.AddAzureKeyVault(
                new Uri($"https://{keyVaultName}.vault.azure.net/"),
                credential,
                new KeyVaultSecretManager());
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ERRO : ao configurar o Key Vault: {ex.Message}");
            throw;
        }
    }
}

public class KeyVaultSecretManager : Azure.Extensions.AspNetCore.Configuration.Secrets.KeyVaultSecretManager
{
    public override string GetKey(KeyVaultSecret secret)
    {
        return secret.Name.Replace("--", ":");
    }
}
