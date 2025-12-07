using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Soat.Eleven.FastFood.Application.Controllers;
using Soat.Eleven.FastFood.Common.Interfaces.DataSources;
using Soat.Eleven.FastFood.Core.DTOs.Images;
using Soat.Eleven.FastFood.Core.DTOs.Produtos;
using Soat.Eleven.FastFood.Core.Enums;

namespace Soat.Eleven.FastFood.Api.Controllers
{
    [ApiController]
    [Route("api/Produto")]
    public class ProdutoRestEndpoints : ControllerBase
    {        
        private readonly IProdutoDataSource _produtoDataSource;
        private readonly ICategoriaProdutoDataSource _categoriaSource;
        private readonly ILogger<ProdutoRestEndpoints> _logger;

        public ProdutoRestEndpoints(IProdutoDataSource produtoDataSource, ICategoriaProdutoDataSource categoriaGateway, ILogger<ProdutoRestEndpoints> logger)
        {
            _produtoDataSource = produtoDataSource;
            _logger = logger;
            _categoriaSource = categoriaGateway;
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<ProdutoDto>>> GetProdutos(
            [FromQuery] bool incluirInativos = false,
            [FromQuery] Guid? categoriaId = null)
        {
            try
            {
                var controller = new ProdutoController(_produtoDataSource, _categoriaSource);
                var produtos = await controller.ListarProdutos(categoriaId, incluirInativos);
                return Ok(produtos);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<ProdutoDto>> GetProduto(Guid id)
        {
            var controller = new ProdutoController(_produtoDataSource, _categoriaSource);
            var produto = await controller.GetProduto(id);

            return Ok(produto);
        }

        [HttpPost]
        [Authorize(PolicyRole.Administrador)]
        public async Task<ActionResult<ProdutoDto>> PostProduto(CriarProdutoDto produto)
        {
            try
            {
                var controller = new ProdutoController(_produtoDataSource, _categoriaSource);

                var result = await controller.CriarProduto(produto);
                return CreatedAtAction(nameof(PostProduto), result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        [Authorize(PolicyRole.Administrador)]
        public async Task<IActionResult> PutProduto(Guid id, AtualizarProdutoDto produto)
        {
            try
            {
                produto.Id = id;
                var controller = new ProdutoController(_produtoDataSource, _categoriaSource);
                var produtoAtualizado = await controller.AtualizarProduto(produto);
                return Ok(produtoAtualizado);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        [Authorize(PolicyRole.Administrador)]
        public async Task<IActionResult> DeleteProduto(Guid id)
        {
            try
            {
                var controller = new ProdutoController(_produtoDataSource, _categoriaSource);
                await controller.DesativarProduto(id);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost("{id}/reativar")]
        [Authorize(PolicyRole.Administrador)]
        public async Task<IActionResult> ReativarProduto(Guid id)
        {
            try
            {
                var controller = new ProdutoController(_produtoDataSource, _categoriaSource);
                await controller.ReativarProduto(id);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost("{id}/imagem")]
        public async Task<IActionResult> UploadImagem(Guid id, [FromForm] IFormFile imagem)
        {
            try
            {
                if (imagem == null || imagem.Length == 0)
                    return BadRequest(new { mensagem = "Nenhuma imagem enviada." });

                var IMAGE_MAX_SIZE = 2 * 1024 * 1024; // 2MB
                if (imagem.Length > IMAGE_MAX_SIZE)
                    return BadRequest(new { mensagem = "A imagem deve ter no máximo 2MB." });

                var imagemDto = new ImagemProdutoArquivo
                {
                    Nome = imagem.FileName,
                    ContentType = imagem.ContentType,
                    Conteudo = imagem.OpenReadStream()
                };

                //await _produtoUseCase.UploadImagemAsync(id, imagemDto);
                return Ok(new { mensagem = "Imagem de produto alterada com sucesso." });
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Erro ao fazer upload da imagem para o produto {Id}", id);
                return BadRequest(new { mensagem = ex.Message });
            }
        }

        [HttpDelete("{id}/imagem")]
        public async Task<IActionResult> RemoverImagem(Guid id)
        {
            //await _produtoUseCase.RemoverImagemAsync(id);
            return NoContent();
        }
    }
} 
