using Soat.Eleven.FastFood.Core.Enums;

namespace Soat.Eleven.FastFood.Core.DTOs;

public partial class TipoPagamentoDto
{
    public TipoPagamento Tipo { get; set; }
    public string Signature { get; set; }
    public string Type { get; set; }

    public TipoPagamentoDto()
    {
        this.Tipo = TipoPagamento.Totem;
    }

}