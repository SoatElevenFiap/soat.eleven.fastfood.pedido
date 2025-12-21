using Soat.Eleven.Pedidos.Core.DTOs.Pedidos;
using Soat.Eleven.Pedidos.Core.Gateways;
using Soat.Eleven.Pedidos.Core.Interfaces.DataSources;
using Soat.Eleven.Pedidos.Core.Interfaces.Services;
using Soat.Eleven.Pedidos.Core.Presenters;
using Soat.Eleven.Pedidos.Core.UseCases;

namespace Soat.Eleven.Pedidos.Application.Controllers;

public class PedidoController
{
    private readonly IPedidoDataSource _pedidoDataSource;
    private readonly IPagamentoService? _pagamentoService;

    public PedidoController(IPedidoDataSource pedidoDataSource)
    {
        _pedidoDataSource = pedidoDataSource;
    }

    public PedidoController(IPedidoDataSource pedidoDataSource, IPagamentoService pagamentoService)
    {
        _pedidoDataSource = pedidoDataSource;
        _pagamentoService = pagamentoService;
    }

    private PedidoUseCase FabricarPedidoUseCase()
    {
        var pedidoGateway = new PedidoGateway(_pedidoDataSource);
        return PedidoUseCase.Create(pedidoGateway);
    }

    private PagamentoUseCase? FabricarPagamentoUseCase()
    {
        if (_pagamentoService == null)
            return null;

        var pagamentoGateway = new PagamentoGateway(_pagamentoService);
        return PagamentoUseCase.Create(pagamentoGateway);
    }

    public async Task<CriarPedidoOutputDto> CriarPedido(PedidoInputDto inputDto)
    {
        var pedidoUseCase = FabricarPedidoUseCase();
        var result = await pedidoUseCase.CriarPedido(inputDto);
        var pedidoOutput = PedidoPresenter.Output(result);

        string? redirectUrl = null;

        var pagamentoUseCase = FabricarPagamentoUseCase();
        if (pagamentoUseCase != null)
        {
            var clientId = _pedidoDataSource.GetClientId();
            var ordemPagamento = await pagamentoUseCase.CriarOrdemPagamento(pedidoOutput, clientId);
            redirectUrl = ordemPagamento?.RedirectUrl;
        }

        return CriarPedidoOutputDto.FromPedidoOutputDto(pedidoOutput, redirectUrl);
    }

    public async Task<PedidoOutputDto> AtualizarPedido(PedidoInputDto inputDto)
    {
        var useCase = FabricarPedidoUseCase();
        var result = await useCase.AtualizarPedido(inputDto);

        return PedidoPresenter.Output(result);
    }

    public async Task<IEnumerable<PedidoOutputDto>> ListarPedidos()
    {
        var useCase = FabricarPedidoUseCase();
        var result = await useCase.ListarPedidos();

        return result.Select(PedidoPresenter.Output);
    }

    public async Task<PedidoOutputDto> ObterPedidoPorId(Guid id)
    {
        var useCase = FabricarPedidoUseCase();
        var result = await useCase.ObterPedidoPorId(id);

        return PedidoPresenter.Output(result);
    }

    public async Task IniciarPreparacaoPedido(Guid id)
    {
        var useCase = FabricarPedidoUseCase();
        await useCase.IniciarPreparacaoPedido(id);
    }

    public async Task FinalizarPreparacao(Guid id)
    {
        var useCase = FabricarPedidoUseCase();
        await useCase.FinalizarPreparacaoPedido(id);
    }

    public async Task FinalizarPedido(Guid id)
    {
        var useCase = FabricarPedidoUseCase();
        await useCase.FinalizarPedido(id);
    }

    public async Task CancelarPedido(Guid id)
    {
        var useCase = FabricarPedidoUseCase();
        await useCase.CancelarPedido(id);
    }
}
