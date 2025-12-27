using Soat.Eleven.Pedidos.Adapter.Infra.EntityModel.Base;
using Soat.Eleven.Pedidos.Core.Enums;

namespace Soat.Eleven.Pedidos.Adapter.Infra.EntityModel
{
    public class PedidoModel : EntityBase
    {
        public PedidoModel()
        {
            //Construtor vazio para o ORM
        }

        public PedidoModel(Guid tokenAtendimentoId, Guid? clienteId, decimal subtotal, decimal desconto, decimal total, string senhaPedido)
        {
            TokenAtendimentoId = tokenAtendimentoId;
            ClienteId = clienteId;
            Subtotal = subtotal;
            Desconto = desconto;
            Total = total;
            Status = StatusPedido.Pendente;
            SenhaPedido = senhaPedido;
        }

        public Guid TokenAtendimentoId { get; set; }
        public Guid? ClienteId { get; set; }
        public StatusPedido Status { get; set; }
        public string SenhaPedido { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Desconto { get; set; }
        public decimal Total { get; set; }

        public ICollection<ItemPedidoModel> Itens { get; set; } = [];

        public void AdicionarItem(ItemPedidoModel item)
        {
            ArgumentNullException.ThrowIfNull(item, nameof(item));

            Itens.Add(item);
        }

        public void AdicionarItens(ICollection<ItemPedidoModel> itens)
        {
            ArgumentNullException.ThrowIfNull(itens, nameof(itens));

            foreach (var item in itens)
            {
                AdicionarItem(item);
            }
        }

        public void RemoverItem(Guid itemId)
        {
            var item = Itens.FirstOrDefault(i => i.Id == itemId);

            if (item == null)
            {
                return;
            }

            Itens.Remove(item);
        }
    }
}
