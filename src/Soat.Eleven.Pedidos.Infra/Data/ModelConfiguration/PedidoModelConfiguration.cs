using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Soat.Eleven.Pedidos.Adapter.Infra.EntityModel;
using Soat.Eleven.Pedidos.Infra.Data.ModelConfiguration.Base;

namespace Soat.Eleven.Pedidos.Infra.Data.ModelConfiguration;

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

        builder.HasMany(c => c.Itens)
               .WithOne(i => i.Pedido)
               .HasForeignKey(i => i.PedidoId);
    }
}
