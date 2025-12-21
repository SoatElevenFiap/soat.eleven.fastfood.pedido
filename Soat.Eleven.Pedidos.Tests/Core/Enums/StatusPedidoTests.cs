using FluentAssertions;
using Soat.Eleven.Pedidos.Core.Enums;
using Xunit;

namespace Soat.Eleven.Pedidos.Tests.Core.Enums;

public class StatusPedidoTests
{
    #region Valores do Enum Tests

    [Fact]
    public void StatusPedido_DeveConterTodosOsStatusEsperados()
    {
        // Assert
        Enum.GetValues(typeof(StatusPedido)).Length.Should().Be(6);
    }

    [Theory]
    [InlineData(StatusPedido.Pendente, 1)]
    [InlineData(StatusPedido.Recebido, 2)]
    [InlineData(StatusPedido.EmPreparacao, 3)]
    [InlineData(StatusPedido.Pronto, 4)]
    [InlineData(StatusPedido.Finalizado, 5)]
    [InlineData(StatusPedido.Cancelado, 6)]
    public void StatusPedido_DeveConterValoresCorretos(StatusPedido status, int valorEsperado)
    {
        // Act
        var valor = (int)status;

        // Assert
        valor.Should().Be(valorEsperado);
    }

    [Theory]
    [InlineData("Pendente", StatusPedido.Pendente)]
    [InlineData("Recebido", StatusPedido.Recebido)]
    [InlineData("EmPreparacao", StatusPedido.EmPreparacao)]
    [InlineData("Pronto", StatusPedido.Pronto)]
    [InlineData("Finalizado", StatusPedido.Finalizado)]
    [InlineData("Cancelado", StatusPedido.Cancelado)]
    public void StatusPedido_DeveParsearCorretamente(string nome, StatusPedido statusEsperado)
    {
        // Act
        var resultado = Enum.Parse<StatusPedido>(nome);

        // Assert
        resultado.Should().Be(statusEsperado);
    }

    [Theory]
    [InlineData(StatusPedido.Pendente, "Pendente")]
    [InlineData(StatusPedido.Recebido, "Recebido")]
    [InlineData(StatusPedido.EmPreparacao, "EmPreparacao")]
    [InlineData(StatusPedido.Pronto, "Pronto")]
    [InlineData(StatusPedido.Finalizado, "Finalizado")]
    [InlineData(StatusPedido.Cancelado, "Cancelado")]
    public void StatusPedido_DeveConverterParaStringCorretamente(StatusPedido status, string nomeEsperado)
    {
        // Act
        var nome = status.ToString();

        // Assert
        nome.Should().Be(nomeEsperado);
    }

    #endregion

    #region Conversao de Inteiro Tests

    [Theory]
    [InlineData(1, StatusPedido.Pendente)]
    [InlineData(2, StatusPedido.Recebido)]
    [InlineData(3, StatusPedido.EmPreparacao)]
    [InlineData(4, StatusPedido.Pronto)]
    [InlineData(5, StatusPedido.Finalizado)]
    [InlineData(6, StatusPedido.Cancelado)]
    public void StatusPedido_DeveConverterDeInteiroCorretamente(int valor, StatusPedido statusEsperado)
    {
        // Act
        var status = (StatusPedido)valor;

        // Assert
        status.Should().Be(statusEsperado);
    }

    [Fact]
    public void StatusPedido_DeveSerDefinidoCorretamente()
    {
        // Assert
        Enum.IsDefined(typeof(StatusPedido), 1).Should().BeTrue();
        Enum.IsDefined(typeof(StatusPedido), 2).Should().BeTrue();
        Enum.IsDefined(typeof(StatusPedido), 3).Should().BeTrue();
        Enum.IsDefined(typeof(StatusPedido), 4).Should().BeTrue();
        Enum.IsDefined(typeof(StatusPedido), 5).Should().BeTrue();
        Enum.IsDefined(typeof(StatusPedido), 6).Should().BeTrue();
        Enum.IsDefined(typeof(StatusPedido), 0).Should().BeFalse();
        Enum.IsDefined(typeof(StatusPedido), 7).Should().BeFalse();
    }

    #endregion

    #region Comparacao Tests

    [Theory]
    [InlineData(StatusPedido.Pendente, StatusPedido.Recebido)]
    [InlineData(StatusPedido.Recebido, StatusPedido.EmPreparacao)]
    [InlineData(StatusPedido.EmPreparacao, StatusPedido.Pronto)]
    [InlineData(StatusPedido.Pronto, StatusPedido.Finalizado)]
    public void StatusPedido_DevePermitirComparacaoDeProgresso(StatusPedido anterior, StatusPedido posterior)
    {
        // Act & Assert
        ((int)anterior).Should().BeLessThan((int)posterior);
    }

    [Fact]
    public void StatusPedido_Cancelado_DeveSerUltimoNaOrdem()
    {
        // Assert
        ((int)StatusPedido.Cancelado).Should().Be(6);
    }

    #endregion
}
