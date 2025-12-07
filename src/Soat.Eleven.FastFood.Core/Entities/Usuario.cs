using Soat.Eleven.FastFood.Core.ConditionRules;
using Soat.Eleven.FastFood.Core.Enums;
using System.Security.Cryptography;
using System.Text;

namespace Soat.Eleven.FastFood.Core.Entities;

public class Usuario
{
    private static string Salt => "LhC2w472LWXN0/RMkp65Yw==";

    public Usuario(Guid id, string nome, string email, string senha, string telefone, PerfilUsuario perfil, StatusUsuario status)
    {
        Id = id;
        Nome = nome;
        Email = email;
        Senha = senha;
        Telefone = telefone;
        Perfil = perfil;
        Status = status;
    }

    public Usuario(string nome, string email, string senha, string telefone, PerfilUsuario perfil, StatusUsuario status)
    {
        Nome = nome;
        Email = email;
        Senha = senha;
        Telefone = telefone;
        Perfil = perfil;
        Status = status;
    }

    public Usuario(Guid id, string nome, string email, string senha)
    {
        Id = id;
        Nome = nome;
        Email = email;
        Senha = senha;
    }

    public Usuario()
    {
    }

    public Guid Id { get; set; }

    public string Nome
    {
        get { return nome; }
        set {
            Condition.Require(value, "Nome").IsNullOrEmpty();
            nome = value; 
        }
    }

    public string Email
    {
        get { return email; }
        set {
            Condition.Require(value, "Email").IsEmail();
            email = value; 
        }
    }

    public string Senha { get; set; }
    public string Telefone { get; set; }
    public PerfilUsuario Perfil { get; set; }
    public DateTime CriadoEm { get; set; }
    public DateTime ModificadoEm { get; set; }
    public StatusUsuario Status { get; set; }

    private string nome;
    private string email;


    public static string GeneratePassword(string password)
    {
        var saltByte = Encoding.UTF8.GetBytes(Salt);
        var hmacMD5 = new HMACMD5(saltByte);
        var passwordConvert = Encoding.UTF8.GetBytes(password!);
        return Convert.ToBase64String(hmacMD5.ComputeHash(passwordConvert));
    }

    public static bool ItIsMyPassword(string currentPassword, string beforePassword)
    {
        var password = GeneratePassword(currentPassword);
        return password == beforePassword;
    }
}
