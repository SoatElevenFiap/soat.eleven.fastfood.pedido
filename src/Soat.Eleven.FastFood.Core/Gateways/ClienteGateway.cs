using Soat.Eleven.FastFood.Core.DTOs.Usuarios;
using Soat.Eleven.FastFood.Core.Entities;
using Soat.Eleven.FastFood.Core.Interfaces.DataSources;

namespace Soat.Eleven.FastFood.Core.Gateways
{
    public class ClienteGateway
    {
        IClienteDataSource _clienteDataSource;

        public ClienteGateway(IClienteDataSource clienteDataSource)
        {
            _clienteDataSource = clienteDataSource;
        }

        public async Task<Cliente> CriarCliente(Cliente entity)
        {
            var dto = new CriarClienteRequestDto
            {
                Nome = entity.Nome,
                Email = entity.Email,
                Telefone = entity.Telefone,
                Cpf = entity.Cpf,
                DataDeNascimento = entity.DataDeNascimento
            };

            var novoClienteDto = await _clienteDataSource.AddAsync(dto);

            return new Cliente
            {
                Id = novoClienteDto.Id,
                Nome = novoClienteDto.Nome,
                Email = novoClienteDto.Email,
                Telefone = novoClienteDto.Telefone,
                Cpf = novoClienteDto.Cpf,
                DataDeNascimento = novoClienteDto.DataDeNascimento
            };
        }

        public async Task<Cliente> AtualizarCliente(Cliente entity)
        {
            var dto = new AtualizarClienteRequestDto
            {
                Id = entity.Id,
                Nome = entity.Nome,
                Email = entity.Email,
                Telefone = entity.Telefone,
                ClienteId = entity.ClienteId,
                Senha = entity.Senha,
                Cpf = entity.Cpf,
                DataDeNascimento = entity.DataDeNascimento
            };

            var clienteDto = await _clienteDataSource.UpdateAsync(dto);

            return new Cliente
            {
                Id = clienteDto.Id,
                ClienteId = clienteDto.ClientId,
                Nome = clienteDto.Nome,
                Email = clienteDto.Email,
                Telefone = clienteDto.Telefone,
                Cpf = clienteDto.Cpf,
                DataDeNascimento = clienteDto.DataDeNascimento
            };
        }

        public async Task<bool> ExistCpf(string cpf)
        {
            return await _clienteDataSource.ExistCpf(cpf);
        }

        public async Task<bool> ExistEmail(string email)
        {
            return await _clienteDataSource.ExistEmail(email);
        }

        public async Task<Cliente?> GetByCPF(string cpf)
        {
            var result = await _clienteDataSource.GetClienteByCPF(cpf);

            if (result == null)
                return null;

            return new Cliente
            {
                Id = result.Id,
                Nome = result.Nome,
                Email = result.Email,
                Telefone = result.Telefone,
                Cpf = result.Cpf,
                DataDeNascimento = result.DataDeNascimento
            };
        }

        public async Task<Cliente?> GetByUsuarioId(Guid usuarioId)
        {
            var result = await _clienteDataSource.GetByUsuario(usuarioId);

            if (result == null)
                return null;

            return new Cliente
            {
                Id = result.Id,
                Nome = result.Nome,
                Email = result.Email,
                Telefone = result.Telefone,
                Senha = result.Senha,
                Cpf = result.Cpf,
                DataDeNascimento = result.DataDeNascimento,
                ClienteId = result.ClientId
            };
        }
    }
}
