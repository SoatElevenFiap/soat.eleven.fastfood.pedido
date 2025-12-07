using Microsoft.EntityFrameworkCore;
using Soat.Eleven.FastFood.Adapter.Infra.Data.ModelConfiguration;
using Soat.Eleven.FastFood.Adapter.Infra.EntityModel;
using Soat.Eleven.FastFood.Infra.Data.ModelConfiguration;

namespace Soat.Eleven.FastFood.Infra.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<UsuarioModel> Usuarios { get; set; }
        public DbSet<ClienteModel> Clientes { get; set; }
        public DbSet<CategoriaProdutoModel> CategoriasProduto { get; set; }
        public DbSet<ProdutoModel> Produtos { get; set; }
        public DbSet<DescontoProdutoModel> DescontosProduto { get; set; }
        public DbSet<PedidoModel> Pedidos { get; set; }
        public DbSet<ItemPedidoModel> ItensPedido { get; set; }
        public DbSet<PagamentoPedidoModel> PagamentosPedido { get; set; }
        public DbSet<TokenAtendimentoModel> TokensAtendimento { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Relacionamento opcional Cliente.UsuarioId
            modelBuilder.ApplyConfiguration(new ClienteModelConfiguration());
            modelBuilder.ApplyConfiguration(new UsuarioModelConfiguration());
            modelBuilder.ApplyConfiguration(new DescontoProdutoModelConfiguration());

            #region Configuration Pedido
            modelBuilder.ApplyConfiguration(new PedidoModelConfiguration());
            modelBuilder.ApplyConfiguration(new ItemPedidoModelConfiguration());
            modelBuilder.ApplyConfiguration(new PagamentoPedidoModelConfiguration());
            #endregion

            modelBuilder.Entity<ProdutoModel>()
                .Property(p => p.Preco)
                .HasPrecision(10, 2);

            modelBuilder.Entity<DescontoProdutoModel>()
                .Property(d => d.Valor)
                .HasPrecision(10, 2);

            modelBuilder.Entity<TokenAtendimentoModel>()
             .HasKey(t=>t.TokenId);
        }
    }

}
