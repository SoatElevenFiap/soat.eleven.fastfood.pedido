using Microsoft.EntityFrameworkCore;
using Soat.Eleven.FastFood.Infra.Data;
using System.Linq.Expressions;

namespace Soat.Eleven.FastFood.Adapter.Infra.Gateways;

public abstract class GatewayBase<TModel> where TModel : class
{
    private readonly AppDbContext _context;
    private readonly DbSet<TModel> _dbSet;

    public GatewayBase(AppDbContext context)
    {
        _context = context;
        _dbSet = _context.Set<TModel>();
    }

    protected async Task<TModel> AddModelAsync(TModel entity)
    {
        await _dbSet.AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    protected async Task<TModel?> GetModelByIdAsync(Guid id, params Expression<Func<TModel, object>>[] includes)
    {
        IQueryable<TModel> query = _dbSet;

        foreach (var include in includes)
        {
            query = query.Include(include);
        }

        return await query
            .AsSplitQuery()
            .FirstOrDefaultAsync(e => EF.Property<Guid>(e, "Id") == id);
    }

    protected async Task<IEnumerable<TModel>> GetAllModelAsync(params Expression<Func<TModel, object>>[] includes)
    {
        IQueryable<TModel> query = _dbSet;

        foreach (var include in includes)
        {
            query = query.Include(include);
        }

        return await query
            .AsSplitQuery()
            .AsNoTracking()
            .ToListAsync();
    }

    protected async Task<IEnumerable<TModel>> FindModelAsync(Expression<Func<TModel, bool>> predicate, Func<IQueryable<TModel>, IQueryable<TModel>>? include = null)
    {
        IQueryable<TModel> query = _dbSet;

        if (include != null)
        {
            query = include(query);
        }

        return await query
            .Where(predicate)
            .AsSplitQuery()
            .AsNoTracking()
            .ToListAsync();
    }

    protected async Task UpdateModelAsync(TModel entity)
    {
        _dbSet.Update(entity);
        await _context.SaveChangesAsync();
    }

    protected async Task DeleteModelAsync(TModel entity)
    {
        _dbSet.Remove(entity);
        await _context.SaveChangesAsync();
    }
}
