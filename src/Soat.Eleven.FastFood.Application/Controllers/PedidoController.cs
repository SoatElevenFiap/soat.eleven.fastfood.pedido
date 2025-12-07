using Soat.Eleven.FastFood.Common.Interfaces.DataSources;
using Soat.Eleven.FastFood.Core.DTOs;
using Soat.Eleven.FastFood.Core.DTOs.Pagamentos;
using Soat.Eleven.FastFood.Core.DTOs.Pedidos;
using Soat.Eleven.FastFood.Core.Gateways;
using Soat.Eleven.FastFood.Core.Interfaces.DataSources;
using Soat.Eleven.FastFood.Core.Presenters;
using Soat.Eleven.FastFood.Core.UseCases;

namespace Soat.Eleven.FastFood.Application.Controllers;

public class PedidoController
{
    private readonly IPedidoDataSource _pedidoDataSource;
    private readonly IPagamentoDataSource _pagamentoDataSource;

    public PedidoController(IPedidoDataSource pedidoGateway, IPagamentoDataSource pagamentoDataSource)
    {
        _pedidoDataSource = pedidoGateway;
        _pagamentoDataSource = pagamentoDataSource;
    }

    private PedidoUseCase FabricarUseCase()
    {
        var pedidoGateway = new PedidoGateway(_pedidoDataSource, _pagamentoDataSource);
        return PedidoUseCase.Create(pedidoGateway);
    }

    public async Task<PedidoOutputDto> CriarPedido(PedidoInputDto inputDto)
    {
        var useCase = FabricarUseCase();
        var result = await useCase.CriarPedido(inputDto);

        return PedidoPresenter.Output(result);
    }
    public async Task<StatusPagamentoPedidoDto> StatusPagamentoPedido(Guid idPedido)
    {
        var useCase = FabricarUseCase();
        return await useCase.StatusPagamentoPedido(idPedido);
    }

    public async Task<PedidoOutputDto> AtualizarPedido(PedidoInputDto inputDto)
    {
        var useCase = FabricarUseCase();
        var result = await useCase.AtualizarPedido(inputDto);

        return PedidoPresenter.Output(result);
    }

    public async Task<IEnumerable<PedidoOutputDto>> ListarPedidos()
    {
        var useCase = FabricarUseCase();
        var result = await useCase.ListarPedidos();

        return result.Select(PedidoPresenter.Output);
    }

    public async Task<PedidoOutputDto> ObterPedidoPorId(Guid id)
    {
        var useCase = FabricarUseCase();
        var result = await useCase.ObterPedidoPorId(id);

        return PedidoPresenter.Output(result);
    }

    public async Task<ConfirmacaoPagamento> PagarPedido(SolicitacaoPagamento solicitacaoPagamento, 
                                                        PagamentoGateway pagamentoGateway,
                                                        TipoPagamentoDto tipoPagamentoDto = default)
    {
       
        var useCase = FabricarUseCase();
        return await useCase.PagarPedido(solicitacaoPagamento, pagamentoGateway, tipoPagamentoDto);
    }

    public async Task<ConfirmacaoPagamento> RecusarPagamento(SolicitacaoPagamento solicitacaoPagamento,
                                                        PagamentoGateway pagamentoGateway,
                                                        TipoPagamentoDto tipoPagamentoDto = default)
    {

        var useCase = FabricarUseCase();
        return await useCase.RecusarPagamento(solicitacaoPagamento, pagamentoGateway, tipoPagamentoDto);
    }

    public async Task IniciarPreparacaoPedido(Guid id)
    {
        var useCase = FabricarUseCase();
        await useCase.IniciarPreparacaoPedido(id);
    }

    public async Task FinalizarPreparacao(Guid id)
    {
        var useCase = FabricarUseCase();
        await useCase.FinalizarPreparacaoPedido(id);
    }

    public async Task FinalizarPedido(Guid id)
    {
        var useCase = FabricarUseCase();
        await useCase.FinalizarPedido(id);
    }

    public async Task CancelarPedido(Guid id)
    {
        var useCase = FabricarUseCase();
        await useCase.CancelarPedido(id);
    }
}
