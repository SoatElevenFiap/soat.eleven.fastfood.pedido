using Microsoft.EntityFrameworkCore;
using Soat.Eleven.FastFood.Adapter.Infra.EntityModel;
using Soat.Eleven.FastFood.Common.Interfaces.DataSources;
using Soat.Eleven.FastFood.Core.DTOs.Pagamentos;
using Soat.Eleven.FastFood.Core.DTOs.Pedidos;
using Soat.Eleven.FastFood.Core.Entities;
using Soat.Eleven.FastFood.Core.Enums;
using Soat.Eleven.FastFood.Infra.Data;

namespace Soat.Eleven.FastFood.Adapter.Infra.DataSources;

public class PagamentoDataSource : IPagamentoDataSource
{
    private readonly AppDbContext _context;
    private readonly DbSet<PagamentoPedidoModel> _dbSet;
    
    public PagamentoDataSource(AppDbContext context)
    {
        _context = context;
        _dbSet = _context.Set<PagamentoPedidoModel>();
    }
    
    public async Task<PagamentoPedido> AddAsync(PagamentoPedido pagamento)
    {
        var model = Parse(pagamento);
        await _dbSet.AddAsync(model);
        await _context.SaveChangesAsync();
        return Parse(model);
    }
    
    public async Task UpdateAsync(Guid pedidoId, ConfirmacaoPagamento confirmacaoPagamento)
    {
        var model = await _dbSet.FirstOrDefaultAsync(e => e.PedidoId == pedidoId);

        if (model == null)
        {
            // Create a new payment record if it doesn't exist
            var newPagamento = new PagamentoPedido(
                TipoPagamento.MercadoPago, // Default type for webhook payments
                0, // Default value, should be retrieved from order
                confirmacaoPagamento.Status,
                confirmacaoPagamento.Autorizacao)
            {
                PedidoId = pedidoId
            };
            
            await AddAsync(newPagamento);
            return;
        }

        model.Status = confirmacaoPagamento.Status;
        model.Autorizacao = confirmacaoPagamento.Autorizacao;

        _dbSet.Update(model);
        await _context.SaveChangesAsync();
    }
    
    private static PagamentoPedidoModel Parse(PagamentoPedido entity)
    {
        return new PagamentoPedidoModel(entity.Tipo, entity.Valor, entity.Status, entity.Autorizacao)
        {
            Id = entity.Id,
            PedidoId = entity.PedidoId,
            Troco = entity.Troco
        };
    }
    
    private static PagamentoPedido Parse(PagamentoPedidoModel model)
    {
        var entity = new PagamentoPedido(model.Tipo, model.Valor, model.Status, model.Autorizacao)
        {
            Id = model.Id,
            PedidoId = model.PedidoId,
            Troco = model.Troco
        };
        return entity;
    }

    public async Task<StatusPagamentoPedidoDto> StatusPedido(Guid pedidoId)
    {
        var model = await _dbSet.FirstOrDefaultAsync(e => e.PedidoId == pedidoId);

        if (model == null) return default;

        return new StatusPagamentoPedidoDto
        {
            PedidoId = pedidoId,
            Status = model.Status,
        };

    }
}