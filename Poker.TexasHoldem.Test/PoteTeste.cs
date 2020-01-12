using System;
using Xunit;
using ExpectedObjects;
using System.Collections.Generic;
using Poker.TexasHoldem.Lib._Base;
using Poker.TexasHoldem.Lib;
using System.Linq;

namespace Poker.TexasHoldem.Test
{
    public class PoteTeste
    {
        [Fact]
        public void DeveCriarPote()
        {
            var poteEsperado = new
            {
                Id = 1,
                Fichas = 0,
                FichasMinimaDoPote = 0,
                Aberto = true
            };

            var poteGerado = new Pote(1);

            poteEsperado.ToExpectedObject().ShouldMatch(poteGerado);
        }

        [Fact]
        public void DeveFecharPote()
        {
            var poteGerado = new Pote(1);
            Assert.True(poteGerado.Aberto);

            poteGerado.Fechar();
            Assert.False(poteGerado.Aberto);
        }

        [Fact]
        public void DeveReceberFichasDoJogadorQueNaoEstaNoPote()
        {
            var idJogador = 1;
            var fichasEsperadas = 100;
            var poteGerado = new Pote(1);

            poteGerado.AdicionarFichas(idJogador, fichasEsperadas);

            Assert.Equal(fichasEsperadas, poteGerado.Fichas);
            Assert.Equal(fichasEsperadas, poteGerado.FichasMinimaDoPote);
            Assert.Contains(poteGerado.JogadoresNoPote, j => j == idJogador);
        }

        [Fact]
        public void DeveReceberFichasDoJogadorQueJaEstaNoPote()
        {
            var idJogador = 1;
            var fichasPrimeiraAposta = 100;
            var fichasSegundaAposta = 150;
            var fichasEsperadas = fichasPrimeiraAposta + fichasSegundaAposta;
            var poteGerado = new Pote(1);
            poteGerado.AdicionarFichas(idJogador, fichasPrimeiraAposta);

            poteGerado.AdicionarFichas(idJogador, fichasSegundaAposta);

            Assert.Equal(fichasEsperadas, poteGerado.Fichas);
            Assert.Equal(fichasSegundaAposta, poteGerado.FichasMinimaDoPote);
            Assert.Contains(poteGerado.JogadoresNoPote, j => j == idJogador);
        }
    }
}
