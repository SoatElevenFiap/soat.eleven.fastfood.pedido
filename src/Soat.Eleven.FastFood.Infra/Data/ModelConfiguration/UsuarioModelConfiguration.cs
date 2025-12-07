using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Soat.Eleven.FastFood.Adapter.Infra.EntityModel;
using Soat.Eleven.FastFood.Core.Enums;
using Soat.Eleven.FastFood.Infra.Data.ModelConfiguration.Base;

namespace Soat.Eleven.FastFood.Infra.Data.ModelConfiguration;

public class UsuarioModelConfiguration : EntityBaseModelConfiguration<UsuarioModel>
{
    private UsuarioModel usuarioAdmDefault
    {
        get
        {
            //Password = Senha@123
            var u = new UsuarioModel("Sistema Fast Food", "sistema@fastfood.com", "3+wuaNtvoRoxLxP7qPmYrg==", "11985203641", PerfilUsuario.Administrador);
            u.Id = Guid.Parse("3b31ada8-b56a-466d-a1a6-75fe92a36552");
            return u;
        }
    }
    public override void Configure(EntityTypeBuilder<UsuarioModel> builder)
    {
        base.Configure(builder);

        builder.Property(c => c.Nome)
               .IsRequired();

        builder.Property(c => c.Email)
               .IsRequired();

        builder.Property(c => c.Perfil)
               .IsRequired();

        builder.Property(c => c.Perfil)
               .HasConversion<string>();

        builder.Property(c => c.Status)
               .HasDefaultValue(StatusUsuario.Ativo)
               .HasConversion<string>();

        builder.HasData([usuarioAdmDefault]);
    }
}
