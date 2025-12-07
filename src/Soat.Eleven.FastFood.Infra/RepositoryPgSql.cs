//using Microsoft.EntityFrameworkCore;
//using Soat.Eleven.FastFood.Infra.Data;
//using Soat.Eleven.FastFood.Domain.Interfaces;
//using System.Linq.Expressions;

//namespace Soat.Eleven.FastFood.Infra.Repositories
//{
//    public class RepositoryPgSql<T> : IRepository<T> where T : class
//    {
//        private readonly AppDbContext _context;
//        private readonly DbSet<T> _dbSet;

//        public RepositoryPgSql(AppDbContext context)
//        {
//            _context = context;
//            _dbSet = _context.Set<T>();
//        }

//        public async Task<T> AddAsync(T entity)
//        {
//            await _dbSet.AddAsync(entity);
//            await _context.SaveChangesAsync();
//            return entity;
//        }

//        public async Task<T?> GetByIdAsync(Guid id, params Expression<Func<T, object>>[] includes)
//        {
//            IQueryable<T> query = _dbSet;

//            foreach (var include in includes)
//            {
//                query = query.Include(include);
//            }

//            return await query
//                .AsSplitQuery()
//                .FirstOrDefaultAsync(e => EF.Property<Guid>(e, "Id") == id);
//        }

//        public async Task<IEnumerable<T>> GetAllAsync(params Expression<Func<T, object>>[] includes)
//        {
//            IQueryable<T> query = _dbSet;

//            foreach (var include in includes)
//            {
//                query = query.Include(include);
//            }

//            return await query
//                .AsSplitQuery()
//                .AsNoTracking()
//                .ToListAsync();
//        }

//        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, Func<IQueryable<T>, IQueryable<T>>? include = null)
//        {
//            IQueryable<T> query = _dbSet;

//            if (include != null)
//            {
//                query = include(query);
//            }

//            return await query
//                .Where(predicate)
//                .AsSplitQuery()
//                .AsNoTracking()
//                .ToListAsync();
//        }

//        public async Task UpdateAsync(T entity)
//        {
//            _dbSet.Update(entity);
//            await _context.SaveChangesAsync();
//        }

//        public async Task DeleteAsync(T entity)
//        {
//            _dbSet.Remove(entity);
//            await _context.SaveChangesAsync();
//        }

//        public async Task<T> SaveBatchAsync(T masterEntity, params Expression<Func<T, IEnumerable<object>>>[] detailSelectors)
//        {
//            // Verificar se a entidade master existe no banco
//            var masterEntry = _context.Entry(masterEntity);

//            if (masterEntry.IsKeySet)
//            {
//                masterEntry.State = EntityState.Modified;
//            }
//            else
//            {
//                await _dbSet.AddAsync(masterEntity);
//            }

//            // Processar cada detail relacionado
//            foreach (var detailSelector in detailSelectors)
//            {
//                // Obter a propriedade de navegação (detalhes)
//                var detailCollection = detailSelector.Compile().Invoke(masterEntity);
//                var propertyName = ((MemberExpression)detailSelector.Body).Member.Name;

//                if (detailCollection == null) continue;

//                var detailCollectionOrigemList = detailCollection.Cast<object>().ToList();

//                // Carregar details existentes relacionados no banco
//                var dbDetails = await _context.Entry(masterEntity)
//                    .Collection(propertyName)
//                    .Query()
//                    .Cast<object>()
//                    .AsNoTracking()
//                    .ToListAsync();

//                // Inserir ou atualizar details do JSON
//                foreach (var detail in detailCollectionOrigemList)
//                {
//                    var detailEntry = _context.Entry(detail);

//                    if (detailEntry.IsKeySet)
//                    {
//                        detailEntry.State = EntityState.Modified;
//                    }
//                    else
//                    {
//                        await _context.AddAsync(detail);
//                    }
//                }

//                // Remover details que não estão no JSON
//                var jsonKeys = detailCollectionOrigemList
//                    .Select(d => _context.Entry(d).Property("Id").CurrentValue)
//                    .ToHashSet();

//                foreach (var dbDetail in dbDetails)
//                {
//                    var dbKey = _context.Entry(dbDetail).Property("Id").CurrentValue;
//                    if (!jsonKeys.Contains(dbKey))
//                    {
//                        _context.Remove(dbDetail);
//                    }
//                }
//            }

//            await _context.SaveChangesAsync();

//            //Retorna dado atualizado
//            var keyValues = _context.Model.FindEntityType(typeof(T))?.FindPrimaryKey()
//                ?.Properties.Select(p => _context.Entry(masterEntity).Property(p.Name).CurrentValue)
//                .ToArray();

//            if (keyValues != null)
//            {
//                return await _dbSet.FindAsync(keyValues) ?? masterEntity;
//            }

//            return masterEntity;
//        }

//        public IQueryable<T> Queryable()
//        {
//            return _dbSet;
//        }

//        public async Task DeleteRangeAsync(List<T> entities)
//        {
//            _dbSet.RemoveRange(entities);
//            await _context.SaveChangesAsync();
//        }

//        public async Task AddRangeAsync(List<T> entities)
//        {
//            _dbSet.AddRange(entities);
//            await _context.SaveChangesAsync();
//        }
//    }
//}
