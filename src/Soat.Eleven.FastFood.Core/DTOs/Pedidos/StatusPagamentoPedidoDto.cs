using Soat.Eleven.FastFood.Core.Enums;

namespace Soat.Eleven.FastFood.Core.DTOs.Pedidos
{
    public class StatusPagamentoPedidoDto
    {
        public StatusPagamento Status { get; set; }
        public string Descricao
        {
            get
            {
                return Status switch
                {
                    StatusPagamento.Pendente => "Pagamento Pendente",
                    StatusPagamento.Aprovado => "Pagamento Aprovado",
                    StatusPagamento.Rejeitado => "Pagamento Rejeitado",
                    StatusPagamento.NaoEncontrado => "Status do Pagamento Não Encontrado",
                    _ => "Status Desconhecido"
                };
            }
        }
        public Guid PedidoId { get; set; }
    }
}
