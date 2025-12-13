using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Soat.Eleven.FastFood.Application.Controllers;
using Soat.Eleven.FastFood.Common.Interfaces.DataSources;
using Soat.Eleven.FastFood.Core.DTOs.Pagamentos;
using Soat.Eleven.FastFood.Core.Interfaces.DataSources;

namespace Soat.Eleven.FastFood.Api.Controllers
{
    [ApiController]
    [Route("api/Pagamento")]
    public class PagamentoRestEndpoint : ControllerBase
    {
        private readonly ILogger<PagamentoRestEndpoint> _logger;
        private readonly IPedidoDataSource _pedidoDataSource;
        private readonly IPagamentoDataSource _pagamentoDataSource;

        public PagamentoRestEndpoint(ILogger<PagamentoRestEndpoint> logger,
                                    IPedidoDataSource pedidoGateway, 
                                    IPagamentoDataSource pagamentoDataSource)
        {
            _logger = logger;
            _pedidoDataSource = pedidoGateway;
            _pagamentoDataSource = pagamentoDataSource;
        }

        [HttpGet("{id:guid}/status")]
        [Authorize]
        public async Task<IActionResult> StatusPagamentoPedido(Guid id)
        {
            try
            {
                var controller = new PedidoController(_pedidoDataSource, _pagamentoDataSource);
                var status = await controller.StatusPagamentoPedido(id);
                return Ok(status);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter status do pagamento.");
                return StatusCode(500, "Erro interno do servidor.");
            }
        }

        /// <summary>
        /// Cria uma ordem de pagamento para o pedido
        /// </summary>
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CriarOrdemPagamento([FromBody] CriarOrdemPagamentoDto request)
        {
            try
            {
                var controller = new PagamentoController(_pagamentoDataSource);
                var resultado = await controller.CriarOrdemPagamento(request);
                return CreatedAtAction(nameof(StatusPagamentoPedido), new { id = request.PedidoId }, resultado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar ordem de pagamento.");
                return StatusCode(500, "Erro interno do servidor.");
            }
        }

        /// <summary>
        /// Recebe notificação de status de pagamento de outro serviço
        /// </summary>
        [HttpPost("notificacao")]
        public async Task<IActionResult> NotificacaoStatusPagamento([FromBody] PagamentoStatusDto notificacao)
        {
            try
            {
                var controller = new PagamentoController(_pagamentoDataSource);
                await controller.AtualizarStatusPagamento(notificacao);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao processar notificação de pagamento.");
                return StatusCode(500, "Erro interno do servidor.");
            }
        }
    }
}
