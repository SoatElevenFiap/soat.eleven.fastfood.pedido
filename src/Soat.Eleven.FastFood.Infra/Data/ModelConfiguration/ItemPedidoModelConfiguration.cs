using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Soat.Eleven.FastFood.Adapter.Infra.EntityModel;
using Soat.Eleven.FastFood.Infra.Data.ModelConfiguration.Base;

namespace Soat.Eleven.FastFood.Infra.Data.ModelConfiguration;

public class ItemPedidoModelConfiguration : EntityBaseModelConfiguration<ItemPedidoModel>
{
    public override void Configure(EntityTypeBuilder<ItemPedidoModel> builder)
    {
        base.Configure(builder);

        builder.Property(c => c.PedidoId).IsRequired();
        builder.Property(c => c.ProdutoId).IsRequired();
        builder.Property(c => c.Quantidade).IsRequired();
        builder.Property(c => c.PrecoUnitario).HasPrecision(10, 2).IsRequired();
        builder.Property(c => c.DescontoUnitario).HasPrecision(10, 2).IsRequired();

        builder.HasOne(c => c.Pedido)
               .WithMany(p => p.Itens)
               .HasForeignKey(c => c.PedidoId);

        builder.HasOne(c => c.Produto)
               .WithMany()
               .HasForeignKey(c => c.ProdutoId);
    }
}
