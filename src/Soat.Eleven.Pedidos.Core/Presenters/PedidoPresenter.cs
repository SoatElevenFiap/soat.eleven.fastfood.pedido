using Soat.Eleven.Pedidos.Core.DTOs.Pedidos;
using Soat.Eleven.Pedidos.Core.Entities;

namespace Soat.Eleven.Pedidos.Core.Presenters;

public class PedidoPresenter
{
    public static PedidoOutputDto Output(Pedido output)
    {
        return new PedidoOutputDto
        {
            Id = output?.Id ?? Guid.Empty,
            TokenAtendimentoId = output?.TokenAtendimentoId ?? Guid.Empty,
            ClienteId = output?.ClienteId ?? Guid.Empty,
            Itens = output?.Itens.Select(i => new ItemPedidoOutputDto
            {
                ProdutoId = i.ProdutoId,
                Quantidade = i.Quantidade,
                DescontoUnitario = i.DescontoUnitario,
                PrecoUnitario = i.PrecoUnitario
            }).ToList() ?? [],
            SenhaPedido = output?.SenhaPedido ?? string.Empty,
            Subtotal = output?.Subtotal ?? 0,
            Desconto = output?.Desconto ?? 0,
            Total = output?.Total ?? 0,
            Status = output?.Status ?? default,
            CriadoEm = output?.CriadoEm ?? DateTime.MinValue
        };
    }
}
