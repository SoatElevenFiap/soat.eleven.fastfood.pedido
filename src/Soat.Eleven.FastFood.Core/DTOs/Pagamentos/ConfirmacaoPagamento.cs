using Soat.Eleven.FastFood.Core.Enums;

namespace Soat.Eleven.FastFood.Core.DTOs.Pagamentos
{
    public class ConfirmacaoPagamento
    {
        public ConfirmacaoPagamento(StatusPagamento status, string autorizacao)
        {
            Status = status;
            Autorizacao = autorizacao;
        }

        public StatusPagamento Status { get; set; }
        public string Autorizacao { get; set; }
    }
}
