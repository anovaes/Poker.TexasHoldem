using System;
using Xunit;
using ExpectedObjects;
using System.Collections.Generic;
using Poker.TexasHoldem.Lib._Base;
using System.Linq;

namespace Poker.TexasHoldem.Test
{
    public class PoteTeste
    {
        [Fact]
        public void DeveCriarPote()
        {
            var idsJogadores = new List<int> { 1, 2, 3 };
            var poteEsperado = new
            {
                Fichas = 0,
                IdsJogadores = idsJogadores,
                Ativo = true
            };

            var poteGerado = new Pote(idsJogadores);

            poteEsperado.ToExpectedObject().ShouldMatch(poteGerado);
        }

        [Fact]
        public void NaoDeveCriarPoteQuandoAListaDeIdsForNula()
        {
            List<int> idsJogadores = null;

            var mensagemDeErro = Assert.Throws<Exception>(() => new Pote(idsJogadores)).Message;
            Assert.Equal(Ressource.PoteJogadoresNaoInformados, mensagemDeErro);
        }

        [Fact]
        public void NaoDeveCriarPoteQuandoAListaNaoConterIds()
        {
            List<int> idsJogadores = new List<int>();

            var mensagemDeErro = Assert.Throws<Exception>(() => new Pote(idsJogadores)).Message;
            Assert.Equal(Ressource.PoteJogadoresNaoInformados, mensagemDeErro);
        }

        public class Pote
        {
            public int Fichas { get; private set; }
            public List<int> IdsJogadores { get; private set; }
            public bool Ativo { get; private set; }

            /// <summary>
            /// Inicia a instância do Pote
            /// </summary>
            /// <param name="idsJogadores">Lista de id dos jogadores vinculados ao pote</param>
            public Pote(List<int> idsJogadores)
            {
                if (idsJogadores == null || !idsJogadores.Any())
                    throw new Exception(Ressource.PoteJogadoresNaoInformados);

                IdsJogadores = idsJogadores;
                Ativo = true;
            }
        }
    }
}
