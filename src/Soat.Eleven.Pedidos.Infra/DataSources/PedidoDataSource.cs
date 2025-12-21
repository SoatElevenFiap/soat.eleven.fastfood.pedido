using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Soat.Eleven.Pedidos.Adapter.Infra.EntityModel;
using Soat.Eleven.Pedidos.Core.DTOs.Pedidos;
using Soat.Eleven.Pedidos.Core.Enums;
using Soat.Eleven.Pedidos.Core.Interfaces.DataSources;
using Soat.Eleven.Pedidos.Infra.Data;

namespace Soat.Eleven.Pedidos.Adapter.Infra.DataSources
{
    public class PedidoDataSource : IPedidoDataSource
    {
        private readonly AppDbContext _context;
        private readonly DbSet<PedidoModel> _dbSet;
        private readonly IConfiguration _configuration;

        public PedidoDataSource(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _dbSet = _context.Set<PedidoModel>();
            _configuration = configuration;
        }

        public async Task<PedidoOutputDto> AddAsync(PedidoInputDto entity)
        {
            var model = Parse(entity);
            await _dbSet.AddAsync(model);
            await _context.SaveChangesAsync();

            return Parse(model);
        }

        public async Task<PedidoOutputDto?> GetByIdAsync(Guid id)
        {
            var result = await _dbSet
                .Include(p => p.Itens)
                .AsSplitQuery()
                .FirstOrDefaultAsync(e => e.Id == id);

            return result != null ? Parse(result) : null;
        }

        public async Task<IEnumerable<PedidoOutputDto>> GetAllAsync()
        {
            var result = await _dbSet
                .Include(p => p.Itens)
                .AsSplitQuery()
                .AsNoTracking()
                .ToListAsync();

            return result.Select(Parse);
        }

        public async Task UpdateAsync(PedidoInputDto entity)
        {
            var model = await _dbSet.FindAsync(entity.Id);

            if (model == null)
            {
                throw new ArgumentException($"Pedido with ID {entity.Id} not found.");
            }

            model.TokenAtendimentoId = entity.TokenAtendimentoId;
            model.ClienteId = entity.ClienteId;
            model.Subtotal = entity.Subtotal;
            model.Desconto = entity.Desconto;
            model.Total = entity.Total;
            model.Status = entity.Status;
            model.Itens = entity.Itens.Select(i => new ItemPedidoModel
            {
                ProdutoId = i.ProdutoId,
                Quantidade = i.Quantidade,
                DescontoUnitario = i.DescontoUnitario,
                PrecoUnitario = i.PrecoUnitario
            }).ToList();

            _dbSet.Update(model);
            await _context.SaveChangesAsync();
        }

        public async Task AtualizarStatusAsync(Guid pedidoId, StatusPedido status)
        {
            var model = await _dbSet.FindAsync(pedidoId);

            if (model == null)
            {
                throw new ArgumentException($"Pedido com ID {pedidoId} não encontrado.");
            }

            model.Status = status;
            await _context.SaveChangesAsync();
        }

        public string GetClientId()
        {
            return _configuration["PagamentoService:ClientId"] ?? string.Empty;
        }

        private static PedidoModel Parse(PedidoInputDto entity)
        {
            var model = new PedidoModel(entity.TokenAtendimentoId,
                                        entity.ClienteId,
                                        entity.Subtotal,
                                        entity.Desconto,
                                        entity.Total,
                                        entity.SenhaPedido);

            var itemModels = entity.Itens.Select(i => new ItemPedidoModel
            {
                ProdutoId = i.ProdutoId,
                Quantidade = i.Quantidade,
                DescontoUnitario = i.DescontoUnitario,
                PrecoUnitario = i.PrecoUnitario
            }).ToList();

            model.Itens = itemModels;

            return model;
        }

        private static PedidoOutputDto Parse(PedidoModel model)
        {
            return new PedidoOutputDto
            {
                Id = model.Id,
                TokenAtendimentoId = model.TokenAtendimentoId,
                ClienteId = model.ClienteId,
                Subtotal = model.Subtotal,
                Desconto = model.Desconto,
                Total = model.Total,
                SenhaPedido = model.SenhaPedido,
                Status = model.Status,
                CriadoEm = model.CriadoEm,
                Itens = model.Itens.Select(i => new ItemPedidoOutputDto
                {
                    ProdutoId = i.ProdutoId,
                    Quantidade = i.Quantidade,
                    DescontoUnitario = i.DescontoUnitario,
                    PrecoUnitario = i.PrecoUnitario
                }).ToList()
            };
        }
    }
}