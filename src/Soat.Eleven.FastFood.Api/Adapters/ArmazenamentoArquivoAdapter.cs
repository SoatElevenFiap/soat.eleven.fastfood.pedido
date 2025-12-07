using Soat.Eleven.FastFood.Core.DTOs.Images;
using Soat.Eleven.FastFood.Core.Interfaces.Services;

namespace Soat.Eleven.FastFood.Api.Adapters
{
    public class ArmazenamentoArquivoAdapter : IArmazenamentoArquivoGateway
    {
        private readonly string _basePath;
        private readonly string _baseUrl;
        private readonly ILogger<ArmazenamentoArquivoAdapter> _logger;

        public ArmazenamentoArquivoAdapter(IConfiguration configuration, ILogger<ArmazenamentoArquivoAdapter> logger)
        {
            _basePath = configuration["FileStorage:BasePath"] ?? throw new ArgumentException("BasePath não configurado");
            _baseUrl = configuration["FileStorage:BaseUrl"] ?? throw new ArgumentException("BaseUrl não configurado");
            _logger = logger;
        }

        public async Task<string> UploadImagemAsync(string diretorio, string identificador, ImagemProdutoArquivo arquivo)
        {
            try
            {
                var diretorioCompleto = Path.Combine(_basePath, diretorio);
                Directory.CreateDirectory(diretorioCompleto);

                var nomeArquivo = $"{identificador}{Path.GetExtension(arquivo.Nome)}";
                var caminhoCompleto = Path.Combine(diretorioCompleto, nomeArquivo);

                using (var stream = new FileStream(caminhoCompleto, FileMode.Create))
                {
                    await arquivo.Conteudo.CopyToAsync(stream);
                }

                return nomeArquivo;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao salvar arquivo no diretório {Diretorio}", diretorio);
                throw;
            }
        }

        public async Task RemoverImagemAsync(string diretorio, string identificador)
        {
            try
            {
                var diretorioCompleto = Path.Combine(_basePath, diretorio);
                var arquivos = Directory.GetFiles(diretorioCompleto, $"{identificador}.*");

                if (arquivos.Length == 0)
                {
                    _logger.LogWarning("Nenhum arquivo encontrado para o identificador {Identificador} no diretório {Diretorio}", 
                        identificador, diretorio);
                    return;
                }

                foreach (var arquivo in arquivos)
                {
                    File.Delete(arquivo);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao remover arquivo do diretório {Diretorio}", diretorio);
                throw;
            }
        }

        public async Task<string> ObterUrlImagemAsync(string diretorio, string nomeArquivo)
        {
            return $"{_baseUrl}/{diretorio}/{nomeArquivo}";
        }
    }
} 