using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Soat.Eleven.FastFood.Application.Controllers;
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

        public PagamentoRestEndpoint(ILogger<PagamentoRestEndpoint> logger,
                                    IPedidoDataSource pedidoDataSource)
        {
            _logger = logger;
            _pedidoDataSource = pedidoDataSource;
        }

        /// <summary>
        /// Recebe notificação de status de pagamento de outro serviço
        /// </summary>
        [HttpPost("{pedidoId:guid}/notificacao")]
        public async Task<IActionResult> NotificacaoStatusPagamento(Guid pedidoId, [FromBody] ConfirmacaoPagamento notificacao)
        {
            try
            {
                var controller = new PagamentoController(_pedidoDataSource);
                await controller.AtualizarStatusPagamento(notificacao, pedidoId);
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
