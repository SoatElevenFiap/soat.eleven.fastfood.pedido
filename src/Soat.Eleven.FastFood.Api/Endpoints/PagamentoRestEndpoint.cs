using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Soat.Eleven.FastFood.Application.Controllers;
using Soat.Eleven.FastFood.Core.DTOs.Pagamentos;
using Soat.Eleven.FastFood.Core.Interfaces.DataSources;
using Soat.Eleven.FastFood.Core.Interfaces.Services;

namespace Soat.Eleven.FastFood.Api.Controllers
{
    [ApiController]
    [Route("api/Pagamento")]
    public class PagamentoRestEndpoint : ControllerBase
    {
        private readonly ILogger<PagamentoRestEndpoint> _logger;
        private readonly IPedidoDataSource _pedidoDataSource;
        private readonly IPagamentoService _pagamentoService;

        public PagamentoRestEndpoint(ILogger<PagamentoRestEndpoint> logger,
                                    IPedidoDataSource pedidoDataSource,
                                    IPagamentoService pagamentoService)
        {
            _logger = logger;
            _pedidoDataSource = pedidoDataSource;
            _pagamentoService = pagamentoService;
        }

        /// <summary>
        /// Cria uma ordem de pagamento no serviço externo
        /// </summary>
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CriarOrdemPagamento([FromBody] CriarOrdemPagamentoRequest request)
        {
            try
            {
                var controller = new PagamentoController(_pedidoDataSource, _pagamentoService);
                var resultado = await controller.CriarOrdemPagamento(request);
                return Ok(resultado);
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
