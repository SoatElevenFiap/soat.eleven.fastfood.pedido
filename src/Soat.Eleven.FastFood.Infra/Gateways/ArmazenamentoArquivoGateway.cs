using Microsoft.Extensions.Configuration;
using Soat.Eleven.FastFood.Core.DTOs.Images;
using Soat.Eleven.FastFood.Core.Interfaces.Services;

namespace Soat.Eleven.FastFood.Infra.Gateways
{
    public class ArmazenamentoArquivoGateway : IArmazenamentoArquivoGateway
    {
        private readonly string _basePath;
        private readonly string _baseUrl;

        public ArmazenamentoArquivoGateway(IConfiguration configuration)
        {
            _basePath = configuration["FileStorage:BasePath"] ?? throw new ArgumentException("BasePath não configurado");
            _baseUrl = configuration["FileStorage:BaseUrl"] ?? throw new ArgumentException("BaseUrl não configurado");
        }

        public async Task<string> UploadImagemAsync(string diretorio, string identificador, ImagemProdutoArquivo arquivo)
        {
            var diretorioCompleto = Path.Combine(_basePath, diretorio);
            Directory.CreateDirectory(diretorioCompleto);

            var extensao = ObterExtensaoDoContentType(arquivo.ContentType);
            var nomeArquivo = $"{identificador}.{extensao}";
            var caminhoArquivo = Path.Combine(diretorioCompleto, nomeArquivo);

            using (var fileStream = new FileStream(caminhoArquivo, FileMode.Create))
            {
                await arquivo.Conteudo.CopyToAsync(fileStream);
            }

            return nomeArquivo;
        }

        private string ObterExtensaoDoContentType(string contentType)
        {
            return contentType switch
            {
                "image/jpeg" => "jpg",
                "image/jpg" => "jpg", 
                "image/png" => "png",
                "image/gif" => "gif",
                "image/webp" => "webp",
                _ => "jpg"
            };
        }

        public async Task RemoverImagemAsync(string diretorio, string identificador)
        {
            try
            {
                var diretorioCompleto = Path.Combine(_basePath, diretorio);
                var arquivos = Directory.GetFiles(diretorioCompleto, $"{identificador}.*");

                foreach (var arquivo in arquivos)
                {
                    File.Delete(arquivo);
                }
            }
            catch (Exception)
            {
                // Log error but don't throw
            }
        }

        public async Task<string> ObterUrlImagemAsync(string diretorio, string nomeArquivo)
        {
            return $"{_baseUrl}/{diretorio}/{nomeArquivo}";
        }
    }
}
