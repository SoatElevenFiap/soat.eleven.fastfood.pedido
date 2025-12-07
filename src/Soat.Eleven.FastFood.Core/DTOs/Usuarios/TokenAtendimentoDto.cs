using Soat.Eleven.FastFood.Core.Entities;

namespace Soat.Eleven.FastFood.Core.DTOs.Usuarios
{
    public class TokenAtendimentoDto
    {
        public Guid TokenId { get; set; }
        public Guid? ClienteId { get; set; }
        public string? Cpf { get; set; }
        public DateTime CriadoEm { get; set; }
        public Cliente? Cliente { get; set; }
        public string? CpfCliente { get; set; }
    }
}
