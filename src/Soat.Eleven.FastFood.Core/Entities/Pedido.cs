using Soat.Eleven.FastFood.Core.ConditionRules;
using Soat.Eleven.FastFood.Core.Enums;

namespace Soat.Eleven.FastFood.Core.Entities;

public class Pedido
{
    public Pedido()
    {
        //Construtor vazio para o ORM
    }

    public Pedido(Guid tokenAtendimentoId, Guid? clienteId, decimal subtotal, decimal desconto, decimal total)
    {
        TokenAtendimentoId = tokenAtendimentoId;
        ClienteId = clienteId;                    
        Subtotal = subtotal;
        Desconto = desconto;
        Total = total;
        Status = StatusPedido.Pendente; //O pedido nasce com Status Pendente
        CriadoEm = DateTime.Now;
    }
    public Guid Id { get; set; }
    public Guid TokenAtendimentoId { get; set; }
    public Guid? ClienteId { get; set; }
    public StatusPedido Status { get; set; }

    private string senhaPedido;

    public string SenhaPedido
    {
        get { return senhaPedido; }
        set 
        {
            Condition.Require(value, "SenhaPedido").IsNullOrEmpty();
            senhaPedido = value; 
        }
    }

    public decimal Subtotal { get; set; }
    public decimal Desconto { get; set; }
    public decimal Total { get; set; }

    public DateTime CriadoEm { get; set; }

    public Cliente Cliente { get; set; } = null!;
    public ICollection<ItemPedido> Itens { get; set; } = [];
    public ICollection<PagamentoPedido> Pagamentos { get; set; } = [];

    public void GerarSenha()
    {
        // Gera a senha baseada no TokenAtendimentoId, garantindo que não se repita  
        var random = new Random();
        var uniquePart = random.Next(100000, 999999).ToString();
        SenhaPedido = $"{TokenAtendimentoId.ToString("N")[..4].ToUpper()}{uniquePart}";
    }

    public void AdicionarItem(ItemPedido item)
    {
        ArgumentNullException.ThrowIfNull(item, nameof(item));

        Itens.Add(item);
    }

    public void AdicionarItens(ICollection<ItemPedido> itens)
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

    public void AdicionarPagamento(PagamentoPedido pagamento)
    {
        ArgumentNullException.ThrowIfNull(pagamento, nameof(pagamento));

        Pagamentos.Add(pagamento);
    }

    public void AtualizarId(Guid id)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("O ID não pode ser vazio.", nameof(id));
        }
        Id = id;
    }
}
