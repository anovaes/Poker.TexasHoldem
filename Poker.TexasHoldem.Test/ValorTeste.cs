using ExpectedObjects;
using Poker.TexasHoldem.Lib;
using Poker.TexasHoldem.Lib._Base;
using System;
using Xunit;

namespace Poker.TexasHoldem.Test
{
    public class ValorTeste
    {
        [Fact]
        public void DeveGerarValor()
        {
            var valorEsperado = new
            {
                Id = "A",
                Nome = "ÁS",
                Plural = "ASES",
                Peso = 14
            };

            var valorGerado = new Valor(valorEsperado.Id);

            valorEsperado.ToExpectedObject().ShouldMatch(valorGerado);
        }

        [Theory(DisplayName = "NaoDevePermitirValorInvalido")]
        [InlineData("1")]
        [InlineData("-10")]
        [InlineData("0")]
        [InlineData("15")]
        [InlineData("10000000000000000000000")]
        [InlineData("Z")]
        [InlineData(null)]
        public void NaoDevePermitirValorInvalido(string valorInvalido)
        {
            var mensagem = Assert.Throws<Exception>(() => new Valor(valorInvalido)).Message;
            Assert.Equal(Ressource.CartaValorInvalido, mensagem);
        }

        [Fact]
        public void PermitirTrocaDoPesoDoValorAsParaBaixo()
        {
            var pesoEsperado = 1;

            var valor = new Valor("A");
            valor.TrocarPeso();

            Assert.Equal(pesoEsperado, valor.Peso);
        }

        [Fact]
        public void PermitirTrocaDoPesoDoValorAsParaCima()
        {
            var pesoEsperado = 14;

            var valor = new Valor("A");
            valor.TrocarPeso();
            valor.TrocarPeso();

            Assert.Equal(pesoEsperado, valor.Peso);
        }

        [Theory(DisplayName = "NaoDeveTrocarPesoDeValoresNaoPermitidos")]
        [InlineData("3", 3)]
        [InlineData("8", 8)]
        [InlineData("10", 10)]
        [InlineData("Q", 12)]
        [InlineData("K", 13)]
        public void NaoDeveTrocarPesoDeValoresNaoPermitidos(string valorNaoPermitidoParaTrocaDePeso, int pesoEsperado)
        {
            var valor = new Valor(valorNaoPermitidoParaTrocaDePeso);
            valor.TrocarPeso();

            Assert.Equal(pesoEsperado, valor.Peso);
        }
    }
}
