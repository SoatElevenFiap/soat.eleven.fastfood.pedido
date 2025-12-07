using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Soat.Eleven.FastFood.Application.Controllers;
using Soat.Eleven.FastFood.Common.Interfaces.DataSources;
using Soat.Eleven.FastFood.Core.DTOs.Pagamentos;
using Soat.Eleven.FastFood.Core.DTOs.Pedidos;
using Soat.Eleven.FastFood.Core.Enums;
using Soat.Eleven.FastFood.Core.Gateways;
using Soat.Eleven.FastFood.Core.Interfaces.DataSources;

namespace Soat.Eleven.FastFood.Api.Controllers
{
    [ApiController]
    [Route("api/Pedido")]
    public class PedidoRestEndpoints : ControllerBase
    {
        private readonly ILogger<PedidoRestEndpoints> _logger;
        private readonly IPedidoDataSource _pedidoDataSource;
        private readonly IPagamentoDataSource _pagamentoDataSource;

        public PedidoRestEndpoints(ILogger<PedidoRestEndpoints> logger,
                                   IPedidoDataSource pedidoGateway, 
                                   IPagamentoDataSource pagamentoDataSource)
        {
            _logger = logger;
            _pedidoDataSource = pedidoGateway;
            _pagamentoDataSource = pagamentoDataSource;
        }

        [HttpPost]
        [Authorize(PolicyRole.ClienteTotem)]
        public async Task<IActionResult> CriarPedido([FromBody] PedidoInputDto pedidoDto)
        {
            try
            {
                var controller = new PedidoController(_pedidoDataSource, _pagamentoDataSource);
                var pedidoCriado = await controller.CriarPedido(pedidoDto);
                return CreatedAtAction(nameof(CriarPedido), pedidoCriado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar pedido.");
                return StatusCode(500, "Erro interno do servidor.");
            }
        }

        [HttpGet]
        [Authorize(PolicyRole.Administrador)]
        public async Task<IActionResult> ListarPedidos()
        {
            try
            {
                var controller = new PedidoController(_pedidoDataSource, _pagamentoDataSource);
                var pedidos = await controller.ListarPedidos();
                return Ok(pedidos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao listar pedidos.");
                return StatusCode(500, "Erro interno do servidor.");
            }
        }

        [HttpGet("{id:guid}")]
        [Authorize]
        public async Task<IActionResult> ObterPedidoPorId(Guid id)
        {
            try
            {
                var controller = new PedidoController(_pedidoDataSource, _pagamentoDataSource);
                var pedido = await controller.ObterPedidoPorId(id);
                return Ok(pedido);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter pedido por id.");
                return StatusCode(500, "Erro interno do servidor.");
            }
        }

        [HttpPut("{id:guid}")]
        [Authorize]
        public async Task<IActionResult> AtualizarPedido(Guid id, [FromBody] PedidoInputDto pedidoDto)
        {
            try
            {
                pedidoDto.Id = id;
                var controller = new PedidoController(_pedidoDataSource, _pagamentoDataSource);
                var pedidoAtualizado = await controller.AtualizarPedido(pedidoDto);
                return Ok(pedidoAtualizado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar pedido.");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("{id:guid}/pagar")]
        [Authorize(PolicyRole.ClienteTotem)]
        public async Task<IActionResult> PagarPedido(Guid id, [FromBody] SolicitacaoPagamento pagamento)
        {
            try
            {
                pagamento.PedidoId = id;
                var controller = new PedidoController(_pedidoDataSource, _pagamentoDataSource);
                var _pagamentoGateway = new PagamentoGateway(_pagamentoDataSource);
                var pagamentoProcessado = await controller.PagarPedido(pagamento, _pagamentoGateway);
                return Ok(pagamentoProcessado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao pagar o pedido");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("{id:guid}/iniciar-preparacao")]
        [Authorize(PolicyRole.Administrador)]
        public async Task<IActionResult> IniciarPreparacaoPedido(Guid id)
        {
            try
            {
                var controller = new PedidoController(_pedidoDataSource, _pagamentoDataSource);
                await controller.IniciarPreparacaoPedido(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao iniciar preparação do pedido.");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("{id:guid}/finalizar-preparacao")]
        [Authorize(PolicyRole.Administrador)]
        public async Task<IActionResult> FinalizarPreparacaoPedido(Guid id)
        {
            try
            {
                var controller = new PedidoController(_pedidoDataSource, _pagamentoDataSource);
                await controller.FinalizarPreparacao(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao finalizar preparação do pedido.");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("{id:guid}/finalizar")]
        [Authorize(PolicyRole.Administrador)]
        public async Task<IActionResult> FinalizarPedido(Guid id)
        {
            try
            {
                var controller = new PedidoController(_pedidoDataSource, _pagamentoDataSource);
                await controller.FinalizarPedido(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao finalizar pedido.");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("{id:guid}/cancelar")]
        [Authorize]
        public async Task<IActionResult> CancelarPedido(Guid id)
        {
            try
            {
                var controller = new PedidoController(_pedidoDataSource, _pagamentoDataSource);
                await controller.CancelarPedido(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao cancelar pedido.");
                return StatusCode(500, ex.Message);
            }
        }
    }
}
