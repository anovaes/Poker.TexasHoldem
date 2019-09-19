using System;
using Xunit;
using ExpectedObjects;
using System.Collections.Generic;
using Poker.TexasHoldem.Lib._Base;
using System.Linq;
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

        public class Pote
        {
            private List<FichasJogador> _fichasJogadores;

            public int Id { get; private set; }
            public int Fichas {
                get
                {
                    return _fichasJogadores.Sum(fj => fj.Fichas);
                }
            }
            public bool Aberto { get; private set; }

            /// <summary>
            /// Inicia a instância do Pote
            /// </summary>
            /// <param name="id">Identificador do Pote</param>
            /// <param name="fichasJogadores">Lista de jogadores e suas respectivas fichas vinculados ao pote</param>
            public Pote(int id, List<FichasJogador> fichasJogadores)
            {
                if (fichasJogadores == null || !fichasJogadores.Any())
                    throw new Exception(Ressource.PoteJogadoresNaoInformados);

                _fichasJogadores = fichasJogadores;

                Id = id;
                Aberto = true;
            }

            public void Fechar()
            {
                Aberto = false;
            }
        }
    }
}
