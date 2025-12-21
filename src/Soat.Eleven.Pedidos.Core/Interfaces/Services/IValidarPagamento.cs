using Soat.Eleven.Pedidos.Core.DTOs.Webhooks;
using Soat.Eleven.Pedidos.Core.Enums;

namespace Soat.Eleven.Pedidos.Core.Interfaces.Services
{
    public interface IValidarPagamento
    {
        public Task<bool> ValidarNotificacao(string signature, string type);
        
    }
}