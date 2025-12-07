using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Soat.Eleven.FastFood.Application.Controllers;
using Soat.Eleven.FastFood.Common.Interfaces.DataSources;
using Soat.Eleven.FastFood.Core.Interfaces.DataSources;

namespace Soat.Eleven.FastFood.Api.Controllers
{
    [ApiController]
    [Route("api/StatusPagamento")]
    public class PagamentoRestEndpoint : ControllerBase
    {
        private readonly ILogger<PedidoRestEndpoints> _logger;
        private readonly IPedidoDataSource _pedidoDataSource;
        private readonly IPagamentoDataSource _pagamentoDataSource;

        public PagamentoRestEndpoint(ILogger<PedidoRestEndpoints> logger,
                                    IPedidoDataSource pedidoGateway, IPagamentoDataSource pagamentoDataSource)
        {
            _logger = logger;
            _pedidoDataSource = pedidoGateway;
            _pagamentoDataSource = pagamentoDataSource;
        }

        [HttpGet("{id:guid}")]
        [Authorize]
        public async Task<IActionResult>StatusPagamentoPedido(Guid id)
        {
            try
            {
                var controller = new PedidoController(_pedidoDataSource, _pagamentoDataSource);
                var pedidoCriado = await controller.StatusPagamentoPedido(id);
                return Ok(pedidoCriado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar pedido.");
                return StatusCode(500, "Erro interno do servidor.");
            }
        }
    }

}
