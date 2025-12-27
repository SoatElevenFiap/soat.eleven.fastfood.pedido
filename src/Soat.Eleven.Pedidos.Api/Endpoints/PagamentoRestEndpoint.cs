using Microsoft.AspNetCore.Mvc;
using Soat.Eleven.Pedidos.Application.Controllers;
using Soat.Eleven.Pedidos.Core.DTOs.Pagamentos;
using Soat.Eleven.Pedidos.Core.Interfaces.DataSources;

namespace Soat.Eleven.Pedidos.Api.Controllers
{
    [ApiController]
    [Route("api/pagamento")]
    public class PagamentoRestEndpoint : ControllerBase
    {
        private readonly ILogger<PagamentoRestEndpoint> _logger;
        private readonly IPedidoDataSource _pedidoDataSource;

        public PagamentoRestEndpoint(ILogger<PagamentoRestEndpoint> logger,
                                    IPedidoDataSource pedidoDataSource)
        {
            _logger = logger;
            _pedidoDataSource = pedidoDataSource;
        }

        /// <summary>
        /// Recebe notificação de status de pagamento de outro serviço
        /// </summary>
        [HttpPost("/notificacao")]
        public async Task<IActionResult> NotificacaoStatusPagamento([FromBody] ConfirmacaoPagamento notificacao)
        {
            try
            {
                var controller = new PagamentoController(_pedidoDataSource);
                await controller.AtualizarStatusPagamento(notificacao, new Guid(notificacao.EndToEndId));
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
