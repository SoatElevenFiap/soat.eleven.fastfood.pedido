using Microsoft.EntityFrameworkCore;
using Soat.Eleven.FastFood.Adapter.Infra.EntityModel;
using Soat.Eleven.FastFood.Infra.Data.ModelConfiguration;

namespace Soat.Eleven.FastFood.Infra.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<PedidoModel> Pedidos { get; set; }
        public DbSet<ItemPedidoModel> ItensPedido { get; set; }
        public DbSet<PagamentoPedidoModel> PagamentosPedido { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region Configuration Pedido
            modelBuilder.ApplyConfiguration(new PedidoModelConfiguration());
            modelBuilder.ApplyConfiguration(new ItemPedidoModelConfiguration());
            modelBuilder.ApplyConfiguration(new PagamentoPedidoModelConfiguration());
            #endregion
        }
    }

}
