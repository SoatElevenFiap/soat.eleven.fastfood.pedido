namespace Soat.Eleven.FastFood.Core.Entities;
public class TokenAtendimento
{
    public Guid TokenId { get; set; }
    public Guid? ClienteId { get; set; }
    public string? Cpf { get; set; }
    public DateTime CriadoEm { get; set; }
    public Cliente? Cliente { get; set; }
    public string? CpfCliente { get; set; }
}
