using Soat.Eleven.FastFood.Adapter.Infra.EntityModel.Base;
using Soat.Eleven.FastFood.Core.Enums;

namespace Soat.Eleven.FastFood.Adapter.Infra.EntityModel
{
    public class UsuarioModel : EntityBase
    {
        public UsuarioModel(string nome, string email, string senha, string telefone, PerfilUsuario perfil)
        {
            Nome = nome;
            Email = email;
            Senha = senha;
            Telefone = telefone;
            Perfil = perfil;
        }

        public UsuarioModel(string nome, string email, string telefone, PerfilUsuario perfil)
        {
            Nome = nome;
            Email = email;
            Telefone = telefone;
            Perfil = perfil;
        }

        public string Nome { get; set; }
        public string Email { get; set; }
        public string Senha { get; set; }
        public string Telefone { get; set; }
        public PerfilUsuario Perfil { get; set; }
        public StatusUsuario Status { get; set; }
        public ClienteModel Cliente { get; set; }

        public void CriarCliente(string cpf, DateTime dataNascimento)
        {
            Cliente = new ClienteModel(cpf, dataNascimento);
        }
    }
}
