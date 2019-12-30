using ExpectedObjects;
using Poker.TexasHoldem.Lib;
using Poker.TexasHoldem.Test._Builder;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public void DeveFinalizarRodadaDeApostasComApostasIguais()
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

        [Fact]
        public void DeveFinalizarDuasRodadasDeApostasComApostasIguaisEmAmbas()
        {
            var quantidadeDePotesEsperado = 1;
            var quantidadeDeFichasNoPote = 600;
            var dealer = new Dealer();
            var jogadores = new JogadorBuilder().Novo().CriarJogadores(3).AdicionarApostas(new int[] { 100, 100, 100 }).ObterJogadores();

            dealer.FinalizarRodada(jogadores);
            jogadores[0].Apostar(100);
            jogadores[1].Apostar(100);
            jogadores[2].Apostar(100);
            dealer.FinalizarRodada(jogadores);

            Assert.Equal(quantidadeDePotesEsperado, dealer.Potes.Count);
            Assert.Equal(quantidadeDeFichasNoPote, dealer.Potes[0].Fichas);
        }

        [Fact]
        public void DeveFinalizarRodadaDeApostasComApostasDiferentes()
        {
            var quantidadeDePotesEsperado = 2;
            var quantidadeDeFichasNoPote1 = 300;
            var quantidadeDeFichasNoPote2 = 100;
            var quantidadeDeFichasDoJogadorAposFinalizarRodada = 0;
            var dealer = new Dealer();
            var jogadores = new JogadorBuilder().Novo().CriarJogadores(3).AdicionarApostas(new int[] { 100, 200, 100 }).ObterJogadores();

            dealer.FinalizarRodada(jogadores);

            Assert.Equal(quantidadeDePotesEsperado, dealer.Potes.Count);
            Assert.Equal(quantidadeDeFichasNoPote1, dealer.Potes[0].Fichas);
            Assert.Equal(quantidadeDeFichasNoPote2, dealer.Potes[1].Fichas);
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

        private int _ultimaAposta;

        public void FinalizarRodada(List<Jogador> jogadores)
        {
            var apostasDosUltimosPotes = new Dictionary<int, int>();

            if (Potes.Any())
                apostasDosUltimosPotes.Add(Potes.Last().Id, _ultimaAposta);

            foreach (var jogador in jogadores.OrderBy(j => j.FichasApostadasNaRodada))
            {
                var fichasApostadas = jogador.FichasApostadasNaRodada;

                foreach (var apostas in apostasDosUltimosPotes)
                {
                    Potes.Where(p => p.Id == apostas.Key).First().AdicionarFichas(jogador.Id, apostas.Value);
                    fichasApostadas -= apostas.Value;
                    _ultimaAposta = apostas.Value;
                }

                if (apostasDosUltimosPotes.LastOrDefault().Value == 0 || fichasApostadas > 0)
                {
                    var idPote = Potes.Count() + 1;
                    var pote = new Pote(idPote);
                    pote.AdicionarFichas(jogador.Id, fichasApostadas);
                    Potes.Add(pote);
                    apostasDosUltimosPotes.Add(idPote, fichasApostadas);
                    _ultimaAposta = fichasApostadas;
                }

                jogador.ZerarFichasApostadasNaRodada();
            }
        }
    }
}
