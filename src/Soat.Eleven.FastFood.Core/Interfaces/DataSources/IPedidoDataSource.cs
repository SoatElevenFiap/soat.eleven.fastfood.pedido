using Soat.Eleven.FastFood.Core.DTOs.Pedidos;
using Soat.Eleven.FastFood.Core.Entities;

namespace Soat.Eleven.FastFood.Core.Interfaces.DataSources;

public interface IPedidoDataSource
{
    Task<PedidoOutputDto> AddAsync(PedidoInputDto pedido);
    Task<IEnumerable<PedidoOutputDto>> GetAllAsync();
    Task<PedidoOutputDto?> GetByIdAsync(Guid id);
    Task UpdateAsync(PedidoInputDto pedido);
}
