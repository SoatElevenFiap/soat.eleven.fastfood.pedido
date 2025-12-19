using Moq;
using Soat.Eleven.FastFood.Core.DTOs.Pedidos;
using Soat.Eleven.FastFood.Core.Entities;
using Soat.Eleven.FastFood.Core.Enums;
using Soat.Eleven.FastFood.Core.Gateways;
using Soat.Eleven.FastFood.Core.Interfaces.DataSources;
using Soat.Eleven.FastFood.Core.UseCases;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;
using FluentAssertions;

namespace Soat.Eleven.FastFood.Tests.BDD.StepDefinitions;

[Binding]
public class CriacaoPedidoStepDefinitions
{
    private readonly Mock<IPedidoDataSource> _pedidoDataSourceMock;
    private readonly PedidoGateway _pedidoGateway;
    private readonly PedidoUseCase _pedidoUseCase;

    private Guid _tokenAtendimentoId;
    private Guid? _clienteId;
    private decimal _subtotal;
    private decimal _desconto;
    private decimal _total;
    private List<ItemPedidoInputDto> _itens;
    private Pedido? _pedidoCriado;
    private Exception? _exception;

    public CriacaoPedidoStepDefinitions()
    {
        _pedidoDataSourceMock = new Mock<IPedidoDataSource>();
        _pedidoGateway = new PedidoGateway(_pedidoDataSourceMock.Object);
        _pedidoUseCase = PedidoUseCase.Create(_pedidoGateway);
        _itens = new List<ItemPedidoInputDto>();
        _subtotal = 0;
        _desconto = 0;
        _total = 0;
    }

    [Given(@"que eu tenho um token de atendimento válido")]
    public void DadoQueEuTenhoUmTokenDeAtendimentoValido()
    {
        _tokenAtendimentoId = Guid.NewGuid();
    }

    [Given(@"que eu tenho um cliente identificado")]
    public void DadoQueEuTenhoUmClienteIdentificado()
    {
        _clienteId = Guid.NewGuid();
    }

    [Given(@"que eu não tenho um cliente identificado")]
    public void DadoQueEuNaoTenhoUmClienteIdentificado()
    {
        _clienteId = null;
    }

    [Given(@"que eu tenho os seguintes itens no pedido:")]
    public void DadoQueEuTenhoOsSeguintesItensNoPedido(Table table)
    {
        _itens = table.CreateSet<ItemPedidoInputDto>().ToList();
        
        // Calcula subtotal e total baseado nos itens se não foram definidos manualmente
        if (_subtotal == 0)
        {
            _subtotal = _itens.Sum(i => i.PrecoUnitario * i.Quantidade);
            _desconto = _itens.Sum(i => i.DescontoUnitario * i.Quantidade);
            _total = _subtotal - _desconto;
        }
    }

    [Given(@"que o pedido tem subtotal de (.*)")]
    public void DadoQueOPedidoTemSubtotalDe(decimal subtotal)
    {
        _subtotal = subtotal;
    }

    [Given(@"que o pedido tem desconto de (.*)")]
    public void DadoQueOPedidoTemDescontoDe(decimal desconto)
    {
        _desconto = desconto;
    }

    [Given(@"que o pedido tem total de (.*)")]
    public void DadoQueOPedidoTemTotalDe(decimal total)
    {
        _total = total;
    }

    [When(@"eu criar o pedido")]
    public async Task QuandoEuCriarOPedido()
    {
        var pedidoInputDto = new PedidoInputDto
        {
            TokenAtendimentoId = _tokenAtendimentoId,
            ClienteId = _clienteId,
            Subtotal = _subtotal,
            Desconto = _desconto,
            Total = _total,
            Itens = _itens
        };

        // Configura o mock para retornar um PedidoOutputDto quando AddAsync for chamado
        _pedidoDataSourceMock
            .Setup(x => x.AddAsync(It.IsAny<PedidoInputDto>()))
            .ReturnsAsync((PedidoInputDto dto) => new PedidoOutputDto
            {
                Id = Guid.NewGuid(),
                TokenAtendimentoId = dto.TokenAtendimentoId,
                ClienteId = dto.ClienteId,
                Subtotal = dto.Subtotal,
                Desconto = dto.Desconto,
                Total = dto.Total,
                Status = StatusPedido.Pendente,
                SenhaPedido = dto.SenhaPedido,
                CriadoEm = DateTime.Now,
                Itens = dto.Itens.Select(i => new ItemPedidoOutputDto
                {
                    ProdutoId = i.ProdutoId,
                    Quantidade = i.Quantidade,
                    PrecoUnitario = i.PrecoUnitario,
                    DescontoUnitario = i.DescontoUnitario
                }).ToList()
            });

        try
        {
            _pedidoCriado = await _pedidoUseCase.CriarPedido(pedidoInputDto);
        }
        catch (Exception ex)
        {
            _exception = ex;
        }
    }

    [Then(@"o pedido deve ser criado com sucesso")]
    public void EntaoOPedidoDeveSerCriadoComSucesso()
    {
        _exception.Should().BeNull("o pedido deveria ter sido criado sem erros");
        _pedidoCriado.Should().NotBeNull("o pedido deveria ter sido criado");
    }

    [Then(@"o pedido deve ter status ""(.*)""")]
    public void EntaoOPedidoDeveTerStatus(string status)
    {
        var statusEsperado = Enum.Parse<StatusPedido>(status);
        _pedidoCriado!.Status.Should().Be(statusEsperado);
    }

    [Then(@"o pedido deve ter uma senha gerada")]
    public void EntaoOPedidoDeveTerUmaSenhaGerada()
    {
        _pedidoCriado!.SenhaPedido.Should().NotBeNullOrEmpty("o pedido deveria ter uma senha gerada");
        _pedidoCriado.SenhaPedido.Length.Should().Be(10, "a senha deveria ter 10 caracteres");
    }

    [Then(@"o pedido deve conter (.*) itens")]
    public void EntaoOPedidoDeveConterItens(int quantidadeItens)
    {
        _pedidoCriado!.Itens.Should().HaveCount(quantidadeItens);
    }

    [Then(@"o cliente do pedido deve ser nulo")]
    public void EntaoOClienteDoPedidoDeveSerNulo()
    {
        _pedidoCriado!.ClienteId.Should().BeNull();
    }

    [Then(@"o total de itens no pedido deve ser (.*)")]
    public void EntaoOTotalDeItensNoPedidoDeveSer(int totalItens)
    {
        var somaQuantidade = _pedidoCriado!.Itens.Sum(i => i.Quantidade);
        somaQuantidade.Should().Be(totalItens);
    }

    [Then(@"o subtotal do pedido deve ser (.*)")]
    public void EntaoOSubtotalDoPedidoDeveSer(decimal subtotal)
    {
        _pedidoCriado!.Subtotal.Should().Be(subtotal);
    }

    [Then(@"o desconto do pedido deve ser (.*)")]
    public void EntaoODescontoDoPedidoDeveSer(decimal desconto)
    {
        _pedidoCriado!.Desconto.Should().Be(desconto);
    }

    [Then(@"o total do pedido deve ser (.*)")]
    public void EntaoOTotalDoPedidoDeveSer(decimal total)
    {
        _pedidoCriado!.Total.Should().Be(total);
    }
}
