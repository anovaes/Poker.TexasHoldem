using ExpectedObjects;
using Poker.TexasHoldem.Test._Base;
using System;
using System.Collections.Generic;
using System.Text;
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
        [InlineData("Q", 11)]
        [InlineData("K", 13)]
        public void NaoDeveTrocarPesoDeValoresNaoPermitidos(string valorNaoPermitidoParaTrocaDePeso, int pesoEsperado)
        {
            var valor = new Valor(valorNaoPermitidoParaTrocaDePeso);
            valor.TrocarPeso();

            Assert.Equal(pesoEsperado, valor.Peso);
        }
    }

    public class Valor
    {
        public string Id { get; private set; }
        public string Nome { get; private set; }
        public string Plural { get; private set; }
        public int Peso { get; private set; }

        public Valor(string id)
        {
            var deParaValor = new DeParaValor(id);

            if (deParaValor.Id == null)
                throw new Exception(Ressource.CartaValorInvalido);

            Id = deParaValor.Id;
            Nome = deParaValor.Nome;
            Plural = deParaValor.Plural;
            Peso = deParaValor.Peso;
        }

        public void TrocarPeso()
        {
            if (Id == "A")
                Peso = Peso == Ressource.ValorAsAltoPeso ? Ressource.ValorAsBaixoPeso : Ressource.ValorAsAltoPeso;
        }

        internal class DeParaValor
        {
            public string Id { get; private set; }
            public string Nome { get; private set; }
            public string Plural { get; private set; }
            public int Peso { get; private set; }

            public DeParaValor(string id)
            {
                switch (id)
                {
                    case "2":
                        Id = id;
                        Nome = Ressource.ValorDoisNome;
                        Plural = Ressource.ValorDoisNome;
                        Peso = Ressource.ValorDoisPeso;
                        break;
                    case "3":
                        Id = id;
                        Nome = Ressource.ValorTresNome;
                        Plural = Ressource.ValorTresNome;
                        Peso = Ressource.ValorTresPeso;
                        break;
                    case "4":
                        Id = id;
                        Nome = Ressource.ValorQuatroNome;
                        Plural = Ressource.ValorQuatroNome;
                        Peso = Ressource.ValorQuatroPeso;
                        break;
                    case "5":
                        Id = id;
                        Nome = Ressource.ValorCincoNome;
                        Plural = Ressource.ValorCincoNome;
                        Peso = Ressource.ValorCincoPeso;
                        break;
                    case "6":
                        Id = id;
                        Nome = Ressource.ValorSeisNome;
                        Plural = Ressource.ValorSeisNome;
                        Peso = Ressource.ValorSeisPeso;
                        break;
                    case "7":
                        Id = id;
                        Nome = Ressource.ValorSeteNome;
                        Plural = Ressource.ValorSeteNome;
                        Peso = Ressource.ValorSetePeso;
                        break;
                    case "8":
                        Id = id;
                        Nome = Ressource.ValorOitoNome;
                        Plural = Ressource.ValorOitoNome;
                        Peso = Ressource.ValorOitoPeso;
                        break;
                    case "9":
                        Id = id;
                        Nome = Ressource.ValorNoveNome;
                        Plural = Ressource.ValorNoveNome;
                        Peso = Ressource.ValorNovePeso;
                        break;
                    case "10":
                        Id = id;
                        Nome = Ressource.ValorDezNome;
                        Plural = Ressource.ValorDezNome;
                        Peso = Ressource.ValorDezPeso;
                        break;
                    case "Q":
                        Id = id;
                        Nome = Ressource.ValorDamaNome;
                        Plural = Ressource.ValorDamaPlural;
                        Peso = Ressource.ValorDamaPeso;
                        break;
                    case "J":
                        Id = id;
                        Nome = Ressource.ValorValeteNome;
                        Plural = Ressource.ValorValetePlural;
                        Peso = Ressource.ValorValetePeso;
                        break;
                    case "K":
                        Id = id;
                        Nome = Ressource.ValorReiNome;
                        Plural = Ressource.ValorReiPlural;
                        Peso = Ressource.ValorReiPeso;
                        break;
                    case "A":
                        Id = id;
                        Nome = Ressource.ValorAsNome;
                        Plural = Ressource.ValorAsPlural;
                        Peso = Ressource.ValorAsAltoPeso;
                        break;
                    default:
                        break;
                }

            }
        }
    }
}
