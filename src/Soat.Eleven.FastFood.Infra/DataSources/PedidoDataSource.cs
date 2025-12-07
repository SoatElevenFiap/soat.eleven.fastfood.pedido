using Microsoft.EntityFrameworkCore;
using Soat.Eleven.FastFood.Adapter.Infra.EntityModel;
using Soat.Eleven.FastFood.Core.DTOs.Pedidos;
using Soat.Eleven.FastFood.Core.Entities;
using Soat.Eleven.FastFood.Core.Interfaces.DataSources;
using Soat.Eleven.FastFood.Infra.Data;

namespace Soat.Eleven.FastFood.Adapter.Infra.DataSources
{
    public class PedidoDataSource : IPedidoDataSource
    {
        private readonly AppDbContext _context;
        private readonly DbSet<PedidoModel> _dbSet;

        public PedidoDataSource(AppDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<PedidoModel>();
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
                .Include(p => p.Pagamentos)
                .AsSplitQuery()
                .FirstOrDefaultAsync(e => e.Id == id);

            return result != null ? Parse(result) : null;
        }

        public async Task<IEnumerable<PedidoOutputDto>> GetAllAsync()
        {
            var result = await _dbSet
                .Include(p => p.Itens)
                .Include(p => p.Pagamentos)
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
                }).ToList(),
                Pagamentos = model.Pagamentos.Select(p => new PagamentoPedidoOutputDto
                {
                    Id = p.Id,
                    Tipo = p.Tipo.ToString(),
                    Valor = p.Valor,
                    Status = p.Status.ToString(),
                    Autorizacao = p.Autorizacao,
                    Troco = p.Troco
                }).ToList()
            };
        }
    }
}