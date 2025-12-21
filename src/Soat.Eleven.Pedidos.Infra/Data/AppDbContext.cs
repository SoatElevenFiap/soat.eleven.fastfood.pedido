using Microsoft.EntityFrameworkCore;
using Soat.Eleven.Pedidos.Adapter.Infra.EntityModel;
using Soat.Eleven.Pedidos.Infra.Data.ModelConfiguration;

namespace Soat.Eleven.Pedidos.Infra.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<PedidoModel> Pedidos { get; set; }
        public DbSet<ItemPedidoModel> ItensPedido { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region Configuration Pedido
            modelBuilder.ApplyConfiguration(new PedidoModelConfiguration());
            modelBuilder.ApplyConfiguration(new ItemPedidoModelConfiguration());
            #endregion
        }
    }

}
