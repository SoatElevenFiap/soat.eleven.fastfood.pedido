using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Soat.Eleven.FastFood.Adapter.Infra.EntityModel;
using Soat.Eleven.FastFood.Infra.Data.ModelConfiguration.Base;

namespace Soat.Eleven.FastFood.Infra.Data.ModelConfiguration;

public class PagamentoPedidoModelConfiguration : EntityBaseModelConfiguration<PagamentoPedidoModel>
{
    public override void Configure(EntityTypeBuilder<PagamentoPedidoModel> builder)
    {
        base.Configure(builder);

        builder.Property(c => c.PedidoId).IsRequired();

        builder.Property(c => c.Tipo)
               .IsRequired()
               .HasConversion<string>();

        builder.Property(c => c.Valor).HasPrecision(10, 2).IsRequired();
        builder.Property(c => c.Troco).HasPrecision(10, 2).IsRequired();

        builder.Property(c=> c.Status)
               .IsRequired()
               .HasConversion<string>();

        builder.Property(c => c.Autorizacao).IsRequired();

        builder.HasOne(c => c.Pedido)
               .WithMany(p => p.Pagamentos)
               .HasForeignKey(c => c.PedidoId);
    }
}
