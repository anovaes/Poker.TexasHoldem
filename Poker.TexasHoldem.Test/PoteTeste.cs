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
            var jogadores = new List<Jogador> {
                new Jogador(1, "teste"),
                new Jogador(2, "novo"),
                new Jogador(3, "foo")
            };

            jogadores[0].Apostar(100);
            jogadores[1].Apostar(100);
            jogadores[2].Apostar(100);

            var poteEsperado = new
            {
                Id = 1,
                Fichas = 300,
                Ativo = true
            };

            var poteGerado = new Pote(1, jogadores);

            poteEsperado.ToExpectedObject().ShouldMatch(poteGerado);
        }

        [Fact]
        public void NaoDeveCriarPoteQuandoAListaDeIdsForNula()
        {
            List<Jogador> jogadores = null;

            var mensagemDeErro = Assert.Throws<Exception>(() => new Pote(1, jogadores)).Message;
            Assert.Equal(Ressource.PoteJogadoresNaoInformados, mensagemDeErro);
        }

        [Fact]
        public void NaoDeveCriarPoteQuandoAListaNaoConterIds()
        {
            List<Jogador> jogadores = new List<Jogador>();

            var mensagemDeErro = Assert.Throws<Exception>(() => new Pote(1, jogadores)).Message;
            Assert.Equal(Ressource.PoteJogadoresNaoInformados, mensagemDeErro);
        }

        public class Pote
        {
            private List<JogadorFicha> _jogadores;

            public int Id { get; private set; }
            public int Fichas { get; private set; }
            public bool Ativo { get; private set; }

            /// <summary>
            /// Inicia a instância do Pote
            /// </summary>
            /// <param name="id">Identificador do Pote</param>
            /// <param name="jogadores">Lista de jogadores vinculados ao pote</param>
            public Pote(int id, List<Jogador> jogadores)
            {
                if (jogadores == null || !jogadores.Any())
                    throw new Exception(Ressource.PoteJogadoresNaoInformados);

                _jogadores = new List<JogadorFicha>();

                foreach (var jogador in jogadores)
                {
                    _jogadores.Add(new JogadorFicha(jogador.Id, jogador.FichasApostadasNaRodada));
                }

                Id = id;
                Fichas = _jogadores.Sum(j => j.Fichas);
                Ativo = true;
            }

            internal class JogadorFicha
            {
                public int IdJogador { get; private set; }
                public int Fichas { get; private set; }

                public JogadorFicha(int idJogador, int fichas)
                {
                    IdJogador = idJogador;
                    Fichas = fichas;
                }
            }
        }
    }
}
