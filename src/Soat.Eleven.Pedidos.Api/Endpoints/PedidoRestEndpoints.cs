using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Soat.Eleven.Pedidos.Application.Controllers;
using Soat.Eleven.Pedidos.Core.DTOs.Pedidos;
using Soat.Eleven.Pedidos.Core.Entities;
using Soat.Eleven.Pedidos.Core.Enums;
using Soat.Eleven.Pedidos.Core.Interfaces.DataSources;
using Soat.Eleven.Pedidos.Core.Interfaces.Services;

namespace Soat.Eleven.Pedidos.Api.Controllers
{
    [ApiController]
    [Route("api/Pedido")]
    public class PedidoRestEndpoints : ControllerBase
    {
        private readonly ILogger<PedidoRestEndpoints> _logger;
        private readonly IPedidoDataSource _pedidoDataSource;
        private readonly IPagamentoService _pagamentoService;

        public PedidoRestEndpoints(ILogger<PedidoRestEndpoints> logger,
                                   IPedidoDataSource pedidoDataSource,
                                   IPagamentoService pagamentoService)
        {
            _logger = logger;
            _pedidoDataSource = pedidoDataSource;
            _pagamentoService = pagamentoService;
        }

        [HttpPost]
        [Authorize(PolicyRole.ClienteTotem)]
        public async Task<IActionResult> CriarPedido([FromBody] PedidoInputDto pedidoDto)
        {
            try
            {
                var controller = new PedidoController(_pedidoDataSource, _pagamentoService);
                var pedidoCriado = await controller.CriarPedido(pedidoDto);
                return Ok(pedidoCriado);
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
                var controller = new PedidoController(_pedidoDataSource);
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
                var controller = new PedidoController(_pedidoDataSource);
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
                var controller = new PedidoController(_pedidoDataSource);
                var pedidoAtualizado = await controller.AtualizarPedido(pedidoDto);
                return Ok(pedidoAtualizado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar pedido.");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("{id:guid}/iniciar-preparacao")]
        [Authorize(PolicyRole.Administrador)]
        public async Task<IActionResult> IniciarPreparacaoPedido(Guid id)
        {
            try
            {
                var controller = new PedidoController(_pedidoDataSource);
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
                var controller = new PedidoController(_pedidoDataSource);
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
                var controller = new PedidoController(_pedidoDataSource);
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
                var controller = new PedidoController(_pedidoDataSource);
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
