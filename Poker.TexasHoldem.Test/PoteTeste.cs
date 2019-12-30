using System;
using Xunit;
using ExpectedObjects;
using System.Collections.Generic;
using Poker.TexasHoldem.Lib._Base;
using Poker.TexasHoldem.Lib;

namespace Poker.TexasHoldem.Test
{
    public class PoteTeste
    {
        [Fact]
        public void DeveCriarPote()
        {
            var jogadoresFichas = new List<FichasJogador> {
                new FichasJogador(1, 100),
                new FichasJogador(2, 100),
                new FichasJogador(3, 150)
            };

            var poteEsperado = new
            {
                Id = 1,
                Fichas = 350,
                Aberto = true
            };

            var poteGerado = new Pote(1, jogadoresFichas);

            poteEsperado.ToExpectedObject().ShouldMatch(poteGerado);
        }

        [Fact]
        public void NaoDeveCriarPoteQuandoAListaDeIdsForNula()
        {
            List<FichasJogador> fichasJogadores = null;

            var mensagemDeErro = Assert.Throws<Exception>(() => new Pote(1, fichasJogadores)).Message;
            Assert.Equal(Ressource.PoteJogadoresNaoInformados, mensagemDeErro);
        }

        [Fact]
        public void NaoDeveCriarPoteQuandoAListaNaoConterIds()
        {
            List<FichasJogador> fichasJogadores = new List<FichasJogador>();

            var mensagemDeErro = Assert.Throws<Exception>(() => new Pote(1, fichasJogadores)).Message;
            Assert.Equal(Ressource.PoteJogadoresNaoInformados, mensagemDeErro);
        }

        [Fact]
        public void DeveFecharPote()
        {
            var jogadoresFichas = new List<FichasJogador> {
                new FichasJogador(1, 100),
                new FichasJogador(2, 100),
                new FichasJogador(3, 150)
            };

            var poteGerado = new Pote(1, jogadoresFichas);
            Assert.True(poteGerado.Aberto);

            poteGerado.Fechar();
            Assert.False(poteGerado.Aberto);
        }

        [Fact]
        public void DeveInformarQuantidadeDeFichasDoJogador()
        {
            var idJogadorPesquisado = 2;
            var fichasEsperadas = 150;

            var jogadoresFichas = new List<FichasJogador> {
                new FichasJogador(1, 100),
                new FichasJogador(idJogadorPesquisado, fichasEsperadas),
                new FichasJogador(3, 200)
            };
            var poteGerado = new Pote(1, jogadoresFichas);

            var fichasPesquisada = poteGerado.PesquisarFichasDoJogador(idJogadorPesquisado);

            Assert.Equal(fichasEsperadas, fichasPesquisada);
        }

        [Fact]
        public void DeveReceberFichasDoJogadorQueNaoEstaNoPote()
        {
            var idJogador = 1;
            var fichasApostadas = 100;
            var poteGerado = new Pote(1);

            poteGerado.AdicionarFichas(idJogador, fichasApostadas);

            Assert.Equal(fichasApostadas, poteGerado.PesquisarFichasDoJogador(idJogador));
        }

        [Fact]
        public void DeveReceberFichasDoJogadorQueJaEstaNoPote()
        {
            var idJogador = 1;
            var fichasPrimeiraAposta = 100;
            var fichasSegundaAposta = 150;
            var fichasEsperadas = 250;
            var jogadoresFichas = new List<FichasJogador> {
                new FichasJogador(idJogador, fichasPrimeiraAposta)
            };
            var poteGerado = new Pote(1, jogadoresFichas);

            poteGerado.AdicionarFichas(idJogador, fichasSegundaAposta);

            Assert.Equal(fichasEsperadas, poteGerado.PesquisarFichasDoJogador(idJogador));
        }
    }
}
