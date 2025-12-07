using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Soat.Eleven.FastFood.Adapter.Infra.EntityModel;

namespace Soat.Eleven.FastFood.Infra.Data.ModelConfiguration;

public class ClienteModelConfiguration : IEntityTypeConfiguration<ClienteModel>
{
    public void Configure(EntityTypeBuilder<ClienteModel> builder)
    {
        builder.HasOne(c => c.Usuario)
               .WithOne(u => u.Cliente)
               .HasForeignKey<ClienteModel>(c => c.UsuarioId)
               .OnDelete(DeleteBehavior.NoAction)
               .IsRequired();

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id).HasDefaultValueSql("gen_random_uuid()");

        builder.Property(c => c.Cpf)
               .IsRequired()
               .HasMaxLength(11);

        builder.Property(c => c.DataDeNascimento)
               .IsRequired()
               .HasColumnType("timestamp");

        builder.Property(c => c.CriadoEm)
               .HasColumnType("timestamp")
               .HasDefaultValueSql("NOW()");

        builder.Property(c => c.ModificadoEm)
               .HasColumnType("timestamp")
               .HasDefaultValueSql("NOW()")
               .ValueGeneratedOnAddOrUpdate();
    }
}
