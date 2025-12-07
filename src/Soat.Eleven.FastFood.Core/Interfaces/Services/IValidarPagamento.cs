using Soat.Eleven.FastFood.Core.DTOs.Webhooks;
using Soat.Eleven.FastFood.Core.Enums;

namespace Soat.Eleven.FastFood.Core.Interfaces.Services
{
    public interface IValidarPagamento
    {
        public Task<bool> ValidarNotificacao(string signature, string type);
        
    }
}