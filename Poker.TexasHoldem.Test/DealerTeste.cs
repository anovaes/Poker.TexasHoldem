using ExpectedObjects;
using Poker.TexasHoldem.Lib;
using Poker.TexasHoldem.Lib._Enum;
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
            var contador = 0;
            var quantidadeDeFichasEmCadaPote = new int[] { 300 };
            var quantidadeDeFichasDoJogadorAposFinalizarRodada = 0;
            var dealer = new Dealer();
            var jogadores = new JogadorBuilder().Novo().CriarJogadores(3).AdicionarApostas(new int[] { 100, 100, 100 }).ObterJogadores();

            dealer.FinalizarRodada(jogadores);

            Assert.Equal(quantidadeDeFichasEmCadaPote.Length, dealer.Potes.Count);
            Assert.Equal(quantidadeDeFichasEmCadaPote[contador], dealer.Potes[contador].Fichas);
            jogadores.ForEach(j => Assert.Equal(quantidadeDeFichasDoJogadorAposFinalizarRodada, j.FichasApostadasNaRodada));
            Assert.True(dealer.Potes[0].Aberto);
        }

        [Fact]
        public void DeveFinalizarDuasRodadasDeApostasComApostasIguaisEmAmbas()
        {
            var contador = 0;
            var quantidadeDeFichasEmCadaPote = new int[] { 600 };
            var dealer = new Dealer();
            var jogadores = new JogadorBuilder().Novo().CriarJogadores(3).AdicionarApostas(new int[] { 100, 100, 100 }).ObterJogadores();

            dealer.FinalizarRodada(jogadores);
            jogadores[0].Apostar(100);
            jogadores[1].Apostar(100);
            jogadores[2].Apostar(100);
            dealer.FinalizarRodada(jogadores);

            Assert.Equal(quantidadeDeFichasEmCadaPote.Length, dealer.Potes.Count);
            Assert.Equal(quantidadeDeFichasEmCadaPote[contador], dealer.Potes[contador].Fichas);
            Assert.True(dealer.Potes[0].Aberto);
        }

        [Fact]
        public void DeveFinalizarRodadaDeApostasComApostasDiferentes()
        {
            var quantidadeDeFichasEmCadaPote = new int[] { 300, 100 };
            var idJogadoresEmCadaPote = new List<int[]>
            {
                new int[] {1, 2, 3 },
                new int[] { 2 }
            };
            var quantidadeDeFichasDoJogadorAposFinalizarRodada = 0;
            var dealer = new Dealer();
            var jogadores = new JogadorBuilder().Novo().CriarJogadores(3).AdicionarApostas(new int[] { 100, 200, 100 }).ObterJogadores();

            dealer.FinalizarRodada(jogadores);

            Assert.Equal(quantidadeDeFichasEmCadaPote.Length, dealer.Potes.Count);

            for (int i = 0; i < quantidadeDeFichasEmCadaPote.Length; i++)
            {
                Assert.Equal(quantidadeDeFichasEmCadaPote[i], dealer.Potes[i].Fichas);
                Assert.Equal(idJogadoresEmCadaPote[i].Count(), dealer.Potes[i].JogadoresNoPote.Count);

                for (int j = 0; j < idJogadoresEmCadaPote[i].Count(); j++)
                {
                    Assert.Equal(idJogadoresEmCadaPote[i][j], dealer.Potes[i].JogadoresNoPote.OrderBy(p => p).ToList()[j]);
                }
            }

            jogadores.ForEach(j => Assert.Equal(quantidadeDeFichasDoJogadorAposFinalizarRodada, j.FichasApostadasNaRodada));
            Assert.False(dealer.Potes[0].Aberto);
            Assert.True(dealer.Potes[1].Aberto);

        }

        [Fact]
        public void DeveFinalizarTresRodadaDeApostasComApostasDiferentes()
        {
            var quantidadeDeFichasEmCadaPote = new int[] { 600, 200, 750, 400, 150 };
            var idJogadoresEmCadaPote = new List<int[]>
            {
                new int[] {1, 2, 3, 4, 5, 6 },
                new int[] {2, 4, 5, 6 },
                new int[] {2, 4, 6 },
                new int[] {4, 6 },
                new int[] { 6 },
            };
            var dealer = new Dealer();
            var jogadores = new JogadorBuilder().Novo().CriarJogadores(6).AdicionarApostas(new int[] { 100, 200, 100, 200, 150, 200 }).ObterJogadores();

            dealer.FinalizarRodada(jogadores.Where(j => j.Status == StatusJogador.Ativo).ToList());
            jogadores[0].TrocarStatus(StatusJogador.AllIn);
            jogadores[1].Apostar(200);
            jogadores[2].TrocarStatus(StatusJogador.AllIn);
            jogadores[3].Apostar(250);
            jogadores[4].TrocarStatus(StatusJogador.AllIn);
            jogadores[5].Apostar(250);
            dealer.FinalizarRodada(jogadores.Where(j => j.Status == StatusJogador.Ativo).ToList());
            jogadores[1].TrocarStatus(StatusJogador.AllIn);
            jogadores[3].Apostar(150);
            jogadores[5].Apostar(300);
            dealer.FinalizarRodada(jogadores.Where(j => j.Status == StatusJogador.Ativo).ToList());

            Assert.Equal(quantidadeDeFichasEmCadaPote.Length, dealer.Potes.Count);

            for (int i = 0; i < quantidadeDeFichasEmCadaPote.Length; i++)
            {
                Assert.Equal(quantidadeDeFichasEmCadaPote[i], dealer.Potes[i].Fichas);
                Assert.Equal(idJogadoresEmCadaPote[i].Count(), dealer.Potes[i].JogadoresNoPote.Count);

                for (int j = 0; j < idJogadoresEmCadaPote[i].Count(); j++)
                {
                    Assert.Equal(idJogadoresEmCadaPote[i][j], dealer.Potes[i].JogadoresNoPote.OrderBy(p => p).ToList()[j]);
                }
            }

            for (int i = 0; i < dealer.Potes.Count - 1; i++)
            {
                Assert.False(dealer.Potes[i].Aberto);
            }

            Assert.True(dealer.Potes.Last().Aberto);
        }

        [Fact]
        public void DeveFinalizarTresRodadaDeApostasComApostasZeradas()
        {
            var quantidadeDeFichasEmCadaPote = new int[] { 400, 150 };
            var idJogadoresEmCadaPote = new List<int[]>
            {
                new int[] {1, 2, 3, 4 },
                new int[] {1, 2, 3 }
            };
            var dealer = new Dealer();
            var jogadores = new JogadorBuilder().Novo().CriarJogadores(4).AdicionarApostas(new int[] { 150, 150, 150, 100 }).ObterJogadores();

            dealer.FinalizarRodada(jogadores.Where(j => j.Status == StatusJogador.Ativo).ToList());
            jogadores[0].Apostar(0);
            jogadores[1].Apostar(0);
            jogadores[2].Apostar(0);
            jogadores[3].TrocarStatus(StatusJogador.AllIn);
            dealer.FinalizarRodada(jogadores.Where(j => j.Status == StatusJogador.Ativo).ToList());
            jogadores[0].Apostar(0);
            jogadores[1].Apostar(0);
            jogadores[2].Apostar(0);
            jogadores[3].TrocarStatus(StatusJogador.AllIn);
            dealer.FinalizarRodada(jogadores.Where(j => j.Status == StatusJogador.Ativo).ToList());

            Assert.Equal(quantidadeDeFichasEmCadaPote.Length, dealer.Potes.Count);

            for (int i = 0; i < quantidadeDeFichasEmCadaPote.Length; i++)
            {
                Assert.Equal(quantidadeDeFichasEmCadaPote[i], dealer.Potes[i].Fichas);
                Assert.Equal(idJogadoresEmCadaPote[i].Count(), dealer.Potes[i].JogadoresNoPote.Count);

                for (int j = 0; j < idJogadoresEmCadaPote[i].Count(); j++)
                {
                    Assert.Equal(idJogadoresEmCadaPote[i][j], dealer.Potes[i].JogadoresNoPote.OrderBy(p => p).ToList()[j]);
                }
            }

            for (int i = 0; i < dealer.Potes.Count - 1; i++)
            {
                Assert.False(dealer.Potes[i].Aberto);
            }

            Assert.True(dealer.Potes.Last().Aberto);
        }

        [Fact]
        public void DeveFinalizarTresRodadaDeApostasComNoveJogadoresNaMesa()
        {
            var quantidadeDeFichasEmCadaPote = new int[] { 450, 400, 350, 300, 500, 200, 300, 100, 50 };
            var idJogadoresEmCadaPote = new List<int[]>
            {
                new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 },
                new int[] { 2, 3, 4, 5, 6, 7, 8, 9 },
                new int[] { 3, 4, 5, 6, 7, 8, 9 },
                new int[] { 4, 5, 6, 7, 8, 9 },
                new int[] { 5, 6, 7, 8, 9 },
                new int[] { 6, 7, 8, 9 },
                new int[] { 7, 8, 9 },
                new int[] { 8, 9 },
                new int[] { 9 },
            };
            var dealer = new Dealer();
            var jogadores = new JogadorBuilder().Novo().CriarJogadores(9).AdicionarApostas(new int[] { 50, 100, 150, 200, 250, 250, 250, 250, 250 }).ObterJogadores();

            dealer.FinalizarRodada(jogadores.Where(j => j.Status == StatusJogador.Ativo).ToList());
            jogadores[0].TrocarStatus(StatusJogador.AllIn);
            jogadores[1].TrocarStatus(StatusJogador.AllIn);
            jogadores[2].TrocarStatus(StatusJogador.AllIn);
            jogadores[3].TrocarStatus(StatusJogador.AllIn);
            jogadores[4].Apostar(50);
            jogadores[5].Apostar(100);
            jogadores[6].Apostar(150);
            jogadores[7].Apostar(150);
            jogadores[8].Apostar(150);
            dealer.FinalizarRodada(jogadores.Where(j => j.Status == StatusJogador.Ativo).ToList());
            jogadores[4].TrocarStatus(StatusJogador.AllIn);
            jogadores[5].TrocarStatus(StatusJogador.AllIn);
            jogadores[6].Apostar(50);
            jogadores[7].Apostar(100);
            jogadores[8].Apostar(150);
            dealer.FinalizarRodada(jogadores.Where(j => j.Status == StatusJogador.Ativo).ToList());

            Assert.Equal(quantidadeDeFichasEmCadaPote.Length, dealer.Potes.Count);

            for (int i = 0; i < quantidadeDeFichasEmCadaPote.Length; i++)
            {
                Assert.Equal(quantidadeDeFichasEmCadaPote[i], dealer.Potes[i].Fichas);
                Assert.Equal(idJogadoresEmCadaPote[i].Count(), dealer.Potes[i].JogadoresNoPote.Count);

                for (int j = 0; j < idJogadoresEmCadaPote[i].Count(); j++)
                {
                    Assert.Equal(idJogadoresEmCadaPote[i][j], dealer.Potes[i].JogadoresNoPote[j]);
                }
            }

            for (int i = 0; i < dealer.Potes.Count - 1; i++)
            {
                Assert.False(dealer.Potes[i].Aberto);
            }

            Assert.True(dealer.Potes.Last().Aberto);
        }

        [Fact]
        public void DeveDistribuirFichasAposRodadasDeAposta()
        {
            var dealer = new Dealer();
            var montanteFinalDeCadaJogador = new int[] { 200, 50, 50, 400 };
            var jogadores = new JogadorBuilder().Novo()
                .CriarJogadores(4)
                .AdicionarFichas(new int[] { 200, 200, 200, 100 })
                .AdicionarPontuacoes(new int[] { 3, 2, 1, 4 })
                .AdicionarApostas(new int[] { 150, 150, 150, 100 })
                .ObterJogadores();

            dealer.FinalizarRodada(jogadores.Where(s => s.Status != StatusJogador.Eliminado && s.Status != StatusJogador.Fold).ToList());
            jogadores[0].Apostar(0);
            jogadores[1].Apostar(0);
            jogadores[2].Apostar(0);
            jogadores[3].TrocarStatus(StatusJogador.AllIn);
            dealer.FinalizarRodada(jogadores.Where(j => j.Status == StatusJogador.Ativo).ToList());
            jogadores[0].Apostar(0);
            jogadores[1].Apostar(0);
            jogadores[2].Apostar(0);
            jogadores[3].TrocarStatus(StatusJogador.AllIn);
            dealer.FinalizarRodada(jogadores.Where(j => j.Status == StatusJogador.Ativo).ToList());

            dealer.DistribuirPote(jogadores.OrderByDescending(j => j.Mao.Pontuacao).ToList());

            for (int i = 0; i < jogadores.Count; i++)
            {
                Assert.Equal(montanteFinalDeCadaJogador[i], jogadores[i].Fichas);
            }
        }
    }

    public class Dealer
    {
        public List<Pote> Potes { get; private set; }
        public Dealer()
        {
            Potes = new List<Pote>();
        }

        internal void FinalizarRodada(List<Jogador> jogadores)
        {
            var primeiroJogadorAnalisado = true;
            var apostasDosUltimosPotes = new Dictionary<int, int>();

            foreach (var jogador in jogadores.OrderBy(j => j.FichasApostadasNaRodada))
            {
                var fichasApostadas = jogador.FichasApostadasNaRodada;

                foreach (var apostas in apostasDosUltimosPotes)
                {
                    Potes.Where(p => p.Id == apostas.Key).First().AdicionarFichas(jogador.Id, apostas.Value);
                    fichasApostadas -= apostas.Value;
                }

                if (apostasDosUltimosPotes.Count == 0 || fichasApostadas > 0)
                {
                    Pote pote = Potes.LastOrDefault();

                    if (!primeiroJogadorAnalisado || pote == null)
                    {
                        pote?.Fechar();
                        pote = new Pote((pote?.Id ?? 0) + 1);
                        Potes.Add(pote);
                    }

                    apostasDosUltimosPotes.Add(pote.Id, fichasApostadas);
                    pote.AdicionarFichas(jogador.Id, fichasApostadas);
                }

                primeiroJogadorAnalisado = false;
                jogador.ZerarFichasApostadasNaRodada();
            }
        }

        internal void DistribuirPote(List<Jogador> jogadores)
        {
            var porcentagemDoPotePorPontuacao = jogadores
                .GroupBy(j => j.Mao.Pontuacao)
                .Select(g => new
                {
                    Pontuacao = g.Key,
                    Porcentagem = 1 / g.Count()
                })
                .ToList();

            foreach (var jogador in jogadores)
            {
                var fichasGanhas = 0;

                foreach (var pote in Potes.Where(p=> p.JogadoresNoPote.Any(j=> j == jogador.Id)))
                {
                    var porcentagem = porcentagemDoPotePorPontuacao.Where(p => p.Pontuacao == jogador.Mao.Pontuacao).First().Porcentagem;
                    fichasGanhas += pote.RetirarFichasDoPote(porcentagem);
                }

                jogador.EncerrarMao(fichasGanhas);
            }
        }
    }
}
