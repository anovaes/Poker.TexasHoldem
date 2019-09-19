using ExpectedObjects;
using Poker.TexasHoldem.Lib;
using Poker.TexasHoldem.Test._Builder;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Poker.TexasHoldem.Test
{
    public class DealerTeste
    {
        [Fact]
        public void DeveCriarDealer()
        {
            var dealerEsperado = new
            {
                Potes = new List<Pote>()
            };

            var dealerGerado = new Dealer();

            dealerEsperado.ToExpectedObject().ShouldMatch(dealerGerado);
        }

        [Fact]
        public void DeveFinalizarRodadaDeApostas()
        {
            var quantidadeDePotesEsperado = 1;
            var quantidadeDeFichasNoPote = 300;
            var quantidadeDeFichasDoJogadorAposFinalizarRodada = 0;
            var dealer = new Dealer();
            var jogadores = new JogadorBuilder().Novo().CriarJogadores(3).AdicionarApostas(new int[] { 100, 100, 100 }).ObterJogadores();

            dealer.FinalizarRodada(jogadores);

            Assert.Equal(quantidadeDePotesEsperado, dealer.Potes.Count);
            Assert.Equal(quantidadeDeFichasNoPote, dealer.Potes[0].Fichas);
            jogadores.ForEach(j => Assert.Equal(quantidadeDeFichasDoJogadorAposFinalizarRodada, j.FichasApostadasNaRodada));
        }
    }

    public class Dealer
    {
        public List<Pote> Potes { get; private set; }
        public Dealer()
        {
            Potes = new List<Pote>();
        }

        public void FinalizarRodada(List<Jogador> jogadores)
        {
            var fichasJogadores = new List<FichasJogador>();
            var idPoteAtual = Potes.Count + 1;
            foreach (var jogador in jogadores)
            {
                fichasJogadores.Add(new FichasJogador(jogador.Id, jogador.FichasApostadasNaRodada));
                jogador.ZerarFichasApostadasNaRodada();
            }

            Potes.Add(new Pote(idPoteAtual, fichasJogadores));
        }
    }
}
