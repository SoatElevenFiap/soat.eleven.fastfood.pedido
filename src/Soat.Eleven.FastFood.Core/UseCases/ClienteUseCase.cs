using Soat.Eleven.FastFood.Core.Entities;
using Soat.Eleven.FastFood.Core.Gateways;

namespace Soat.Eleven.FastFood.Core.UseCases;

public class ClienteUseCase
{
    private readonly ClienteGateway _clienteGateway;

    private ClienteUseCase(ClienteGateway clienteGateway)
    {
        _clienteGateway = clienteGateway;
    }

    public static ClienteUseCase Create(ClienteGateway clienteGateway)
    {
        return new ClienteUseCase(clienteGateway);
    }

    public async Task<Cliente> InserirCliente(Cliente request)
    {
        var existeEmail = await _clienteGateway.ExistEmail(request.Email);
        var existeCpf = await _clienteGateway.ExistCpf(request.Cpf);

        if (existeEmail || existeCpf)
        {
            throw new Exception("Usuário já existe");
        }

        var result = await _clienteGateway.CriarCliente(request);
        return result;
    }

    public async Task<Cliente> AtualizarCliente(Cliente request, Guid usuarioId)
    {
        var cliente = await _clienteGateway.GetByUsuarioId(usuarioId);

        if (cliente is null)
            throw new Exception("Usuário não encontrado");

        if (request.Email != cliente.Email)
        {
            var existeEmail = await _clienteGateway.ExistEmail(request.Email);

            if (existeEmail)
                throw new Exception("Endereço de e-mail já utilizado");
        }

        if (request.Cpf != cliente.Cpf)
        {
            var existeCpf = await _clienteGateway.ExistCpf(request.Cpf);

            if (existeCpf)
                throw new Exception("Já existe um usuário com este CPF");
        }

        cliente.Nome = request.Nome;
        cliente.Email = request.Email;
        cliente.Telefone = request.Telefone;
        cliente.Cpf = request.Cpf;
        cliente.DataDeNascimento = request.DataDeNascimento;

        var result = await _clienteGateway.AtualizarCliente(cliente);

        return result;
    }

    public async Task<Cliente> GetCliente(Guid usuarioId)
    {
        var cliente = await _clienteGateway.GetByUsuarioId(usuarioId);

        if (cliente is null)
            throw new Exception("Usuário não encontrado");

        return cliente;
    }

    public async Task<Cliente> GetClienteByCPF(string cpf)
    {
        var cliente = await _clienteGateway.GetByCPF(cpf);

        if (cliente is null)
            throw new Exception("Cliente não encontrado");

        return cliente;
    }
}
