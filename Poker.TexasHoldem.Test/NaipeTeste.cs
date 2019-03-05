using ExpectedObjects;
using Poker.TexasHoldem.Lib;
using Poker.TexasHoldem.Lib._Base;
using System;
using Xunit;

namespace Poker.TexasHoldem.Test
{
    public class NaipeTeste
    {
        [Fact]
        public void DeveGerarNaipe()
        {
            var naipeEsperado = new
            {
                Id = "C",
                Nome = "COPAS",
                Simbolo = "♥"
            };

            var naipeGerado = new Naipe(naipeEsperado.Id);

            naipeEsperado.ToExpectedObject().ShouldMatch(naipeGerado);
        }

        [Theory(DisplayName = "NaoDevePermitirNaipeInvalido")]
        [InlineData("A")]
        [InlineData("Z")]
        [InlineData("Ç")]
        [InlineData("//")]
        [InlineData("$")]
        [InlineData("&")]
        [InlineData("8")]
        [InlineData("      ")]
        [InlineData("     C")]
        [InlineData(null)]
        public void NaoDevePermitirNaipeInvalido(string naipeInvalido)
        {
            var mensagem = Assert.Throws<Exception>(() => new Naipe(naipeInvalido)).Message;
            Assert.Equal(Ressource.CartaMsgNaipeInvalido, mensagem);
        }

        [Theory(DisplayName = "DeveGerarSimboloEsperado")]
        [InlineData("P", "♣")]
        [InlineData("C", "♥")]
        [InlineData("E", "♠")]
        [InlineData("O", "♦")]
        public void DeveGerarSimboloEsperado(string idNaipe, string simboloEsperado)
        {
            var naipeGerado = new Naipe(idNaipe);
            Assert.Equal(simboloEsperado, naipeGerado.Simbolo);
        }
    }
}
