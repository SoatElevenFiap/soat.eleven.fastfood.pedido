using Soat.Eleven.FastFood.Core.Interfaces.Services;

namespace Soat.Eleven.FastFood.Adapter.Infra.Services
{
    public class ValidarPagamento : IValidarPagamento
    {
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task<bool> ValidarNotificacao(string signature, string type)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            if (type != "payment")
                throw new ArgumentException("Tipo de notificação inválido. Esperado 'payment'.");
            return true;
        }
    }
}
