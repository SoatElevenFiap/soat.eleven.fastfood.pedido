namespace Soat.Eleven.FastFood.Core.Interfaces.Gateways;

public interface IGateway<T>
{
    Task<T> AddAsync(T entity);
    Task<IEnumerable<T>> GetAllAsync();
    Task<T?> GetByIdAsync(Guid id);
    Task UpdateAsync(T entity);
    Task DeleteAsync(T entity);
}
