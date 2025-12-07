using Soat.Eleven.FastFood.Common.Interfaces.DataSources;
using Soat.Eleven.FastFood.Core.DTOs.Pagamentos;
using Soat.Eleven.FastFood.Core.DTOs.Pedidos;
using Soat.Eleven.FastFood.Core.Entities;
using Soat.Eleven.FastFood.Core.Interfaces.DataSources;

namespace Soat.Eleven.FastFood.Core.Gateways
{
    public class PedidoGateway
    {
        private IPedidoDataSource _pedidoDataSource;
        private IPagamentoDataSource _pagamentoDataSource;
        public PedidoGateway(IPedidoDataSource dataSource, 
                             IPagamentoDataSource pagamentoDataSource)
        {
            _pedidoDataSource = dataSource;
            _pagamentoDataSource = pagamentoDataSource;
        }

       
        public async Task<Pedido> AtualizarPedido(Pedido pedido)
        {
            var dto = new PedidoInputDto
            {
                Id = pedido.Id,
                TokenAtendimentoId = pedido.TokenAtendimentoId,
                ClienteId = pedido.ClienteId,
                Subtotal = pedido.Subtotal,
                Desconto = pedido.Desconto,
                Total = pedido.Total,
                Status = pedido.Status,
                Itens = pedido.Itens.Select(i => new ItemPedidoInputDto
                {
                    ProdutoId = i.ProdutoId,
                    Quantidade = i.Quantidade,
                    PrecoUnitario = i.PrecoUnitario,
                    DescontoUnitario = i.DescontoUnitario
                }).ToList()
            };

            await _pedidoDataSource.UpdateAsync(dto);

            return pedido;
        }

        public async Task<Pedido> CriarPedido(Pedido pedido)
        {
            var dto = new PedidoInputDto
            {
                Id = pedido.Id,
                TokenAtendimentoId = pedido.TokenAtendimentoId,
                ClienteId = pedido.ClienteId,
                Subtotal = pedido.Subtotal,
                Desconto = pedido.Desconto,
                Total = pedido.Total,
                SenhaPedido = pedido.SenhaPedido,
                Itens = pedido.Itens.Select(i => new ItemPedidoInputDto
                {
                    ProdutoId = i.ProdutoId,
                    Quantidade = i.Quantidade,
                    PrecoUnitario = i.PrecoUnitario,
                    DescontoUnitario = i.DescontoUnitario
                }).ToList()
            };

            var dtoRetorno = await _pedidoDataSource.AddAsync(dto);

            pedido.AtualizarId(dtoRetorno.Id);

            return pedido;
        }

        public async Task<IEnumerable<Pedido>> ListarPedidos()
        {
            var pedidosDto = await _pedidoDataSource.GetAllAsync();

            return pedidosDto.Select(p => new Pedido
            {
                Id = p.Id,
                TokenAtendimentoId = p.TokenAtendimentoId,
                ClienteId = p.ClienteId,
                Subtotal = p.Subtotal,
                Desconto = p.Desconto,
                Total = p.Total,
                CriadoEm = p.CriadoEm,
                Status = p.Status,
                Itens = p.Itens.Select(i => new ItemPedido
                {
                    ProdutoId = i.ProdutoId,
                    Quantidade = i.Quantidade,
                    PrecoUnitario = i.PrecoUnitario,
                    DescontoUnitario = i.DescontoUnitario
                }).ToList()
            });
        }

        public async Task<Pedido?> ObterPedidoPorId(Guid id)
        {
            var pedido = await _pedidoDataSource.GetByIdAsync(id);

            return pedido != null ? new Pedido
            {
                Id = pedido.Id,
                TokenAtendimentoId = pedido.TokenAtendimentoId,
                ClienteId = pedido.ClienteId,
                Subtotal = pedido.Subtotal,
                Desconto = pedido.Desconto,
                Total = pedido.Total,
                Status = pedido.Status,
                SenhaPedido = pedido.SenhaPedido,
                CriadoEm = pedido.CriadoEm,
                Itens = pedido.Itens.Select(i => new ItemPedido
                {
                    ProdutoId = i.ProdutoId,
                    Quantidade = i.Quantidade,
                    PrecoUnitario = i.PrecoUnitario,
                    DescontoUnitario = i.DescontoUnitario
                }).ToList()
            } : null;
        }

        public Task<ConfirmacaoPagamento> PagarPedido(SolicitacaoPagamento solicitacaoPagamento, PagamentoGateway pagamentoGateway)
        {
            throw new NotImplementedException();
        }

        public async Task<StatusPagamentoPedidoDto> StatusPagamentoPedido(Guid idPedido)
        {
            return await _pagamentoDataSource.StatusPedido(idPedido);
        }
    }
}
