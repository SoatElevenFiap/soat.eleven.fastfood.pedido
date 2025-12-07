using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Soat.Eleven.FastFood.Adapter.Infra.EntityModel;
using Soat.Eleven.FastFood.Infra.Data.ModelConfiguration.Base;

namespace Soat.Eleven.FastFood.Infra.Data.ModelConfiguration;

public class PedidoModelConfiguration : EntityBaseModelConfiguration<PedidoModel>
{
    public override void Configure(EntityTypeBuilder<PedidoModel> builder)
    {
        base.Configure(builder);

        builder.Property(c => c.TokenAtendimentoId).IsRequired();

        builder.Property(c => c.Status)
               .IsRequired()
               .HasConversion<string>();

        builder.Property(c => c.SenhaPedido).IsRequired();

        builder.Property(c => c.Subtotal)
            .HasPrecision(10, 2)
            .IsRequired();

        builder.Property(c => c.Desconto)
            .HasPrecision(10, 2)
            .IsRequired();

        builder.Property(c => c.Total)
            .HasPrecision(10, 2)
            .IsRequired();

        builder.HasOne(c => c.Cliente)
               .WithMany()
               .HasForeignKey(c => c.ClienteId)
               .IsRequired(false);

        builder.HasMany(c => c.Itens)
               .WithOne(i => i.Pedido)
               .HasForeignKey(i => i.PedidoId);

        builder.HasMany(c => c.Pagamentos)
               .WithOne(p => p.Pedido)
               .HasForeignKey(p => p.PedidoId);
    }
}
