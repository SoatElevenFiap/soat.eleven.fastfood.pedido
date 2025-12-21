using Soat.Eleven.Pedidos.Core.DTOs.Pedidos;
using Soat.Eleven.Pedidos.Core.Entities;
using Soat.Eleven.Pedidos.Core.Enums;

namespace Soat.Eleven.Pedidos.Core.Interfaces.DataSources;

public interface IPedidoDataSource
{
    Task<PedidoOutputDto> AddAsync(PedidoInputDto pedido);
    Task<IEnumerable<PedidoOutputDto>> GetAllAsync();
    Task<PedidoOutputDto?> GetByIdAsync(Guid id);
    Task UpdateAsync(PedidoInputDto pedido);
    Task AtualizarStatusAsync(Guid pedidoId, StatusPedido status);
    string GetClientId();
}
