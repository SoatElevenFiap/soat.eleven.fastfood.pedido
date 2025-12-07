using Soat.Eleven.FastFood.Core.DTOs.Produtos;

namespace Soat.Eleven.FastFood.Common.Interfaces.DataSources;

public interface IProdutoDataSource
{
    Task<IEnumerable<ProdutoDto>> GetAllAsync();
    Task<ProdutoDto?> GetByIdAsync(Guid id);
    Task<IEnumerable<ProdutoDto>> FindAsync(Func<ProdutoDto, bool> predicate);
    Task<IEnumerable<ProdutoDto>> GetByCategoriaAsync(Guid categoriaId);
    Task AddAsync(ProdutoDto produto);
    Task UpdateAsync(ProdutoDto produto);
}
