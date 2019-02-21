using ExpectedObjects;
using Poker.TexasHoldem.Test._Base;
using System;
using System.Collections.Generic;
using System.Text;
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
            Assert.Equal(Ressource.CartaNaipeInvalido, mensagem);
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

    public class Naipe
    {
        public string Id { get; private set; }
        public string Nome { get; private set; }
        public string Simbolo { get; private set; }

        public Naipe(string id)
        {
            var deParaNaipe = new DeParaNaipe(id);

            if (deParaNaipe.Id == null)
                throw new Exception(Ressource.CartaNaipeInvalido);

            Id = deParaNaipe.Id;
            Nome = deParaNaipe.Nome;
            Simbolo = deParaNaipe.Simbolo;
        }

        internal class DeParaNaipe
        {
            public string Id { get; private set; }
            public string Nome { get; private set; }
            public string Simbolo { get; private set; }

            public DeParaNaipe(string id)
            {
                switch (id)
                {
                    case "P":
                        Id = id;
                        Nome = Ressource.NaipePausNome;
                        Simbolo = Ressource.NaipePausSimbolo;
                        break;
                    case "C":
                        Id = id;
                        Nome = Ressource.NaipeCopasNome;
                        Simbolo = Ressource.NaipeCopasSimbolo;
                        break;
                    case "E":
                        Id = id;
                        Nome = Ressource.NaipeEspadasNome;
                        Simbolo = Ressource.NaipeEspadasSimbolo;
                        break;
                    case "O":
                        Id = id;
                        Nome = Ressource.NaipeOurosNome;
                        Simbolo = Ressource.NaipeOurosSimbolo;
                        break;
                }
            }
        }
    }
}
