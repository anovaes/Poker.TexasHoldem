using ExpectedObjects;
using Poker.TexasHoldem.Test._Base;
using Poker.TexasHoldem.Test._Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Poker.TexasHoldem.Test
{

    public class MaoTeste
    {
        private readonly Carta _primeiraCarta;
        private readonly Carta _segundaCarta;

        public MaoTeste()
        {
            _primeiraCarta = new Carta("A;C");
            _segundaCarta = new Carta("K;P");
        }

        [Fact]
        public void DeveGerarMao()
        {
            var cartas = new List<Carta>
                {
                    _primeiraCarta,
                    _segundaCarta
                };

            var maoEsperada = new
            {
                Cartas = cartas,
                Classificacao = "",
                Pontuacao = Int64.Parse("0")
            };

            var maoGerada = new Mao(_primeiraCarta, _segundaCarta);

            maoEsperada.ToExpectedObject().ShouldMatch(maoGerada);
        }

        [Fact]
        public void NaoDevePermitirQueAlgumaDasCartaSejaNula()
        {
            Carta primeiraCartaNula = null;
            Carta segundaCartaNula = null;
            var menssagemDeErro = Assert.Throws<Exception>(() => new Mao(primeiraCartaNula, segundaCartaNula)).Message;
            Assert.Equal(Ressource.MaoCartaInvalida, menssagemDeErro);
        }

        [Fact]
        public void NaoDevePermitirCartasDuplicadas()
        {
            Carta primeiraCartaNula = new Carta("A;E");
            Carta segundaCartaNula = new Carta("A;E");
            var menssagemDeErro = Assert.Throws<Exception>(() => new Mao(primeiraCartaNula, segundaCartaNula)).Message;
            Assert.Equal(Ressource.MaoCartasDuplicadas, menssagemDeErro);
        }

        [Fact]
        public void ClassificarJogadaComoRoyalFlush()
        {
            var cartasJogador = "A;E|K;E";
            var cartasMesa = "Q;E|J;E|10;E|3;O|7;C";
            var pontuacaoEsperada = 101413121110;
            var classificacaoEsperada = "Royal Flush de ESPADAS";
            var maoBuilderGerada = new MaoBuilder(cartasJogador, cartasMesa);
            var maoGerada = new Mao(maoBuilderGerada.CartasJogador[0], maoBuilderGerada.CartasJogador[1]);

            maoGerada.Classificar(maoBuilderGerada.CartasMesa);

            Assert.Equal(classificacaoEsperada, maoGerada.Classificacao);
            Assert.Equal(pontuacaoEsperada, maoGerada.Pontuacao);
        }

        [Fact]
        public void ClassificarJogadaComoStraightFlush()
        {
            var cartasJogador = "9;C|8;C";
            var cartasMesa = "7;C|6;C|5;C|2;O|4;P";
            var pontuacaoEsperada = 90908070605;
            var classificacaoEsperada = "Straight Flush de COPAS, CINCO a NOVE";
            var maoBuilderGerada = new MaoBuilder(cartasJogador, cartasMesa);
            var maoGerada = new Mao(maoBuilderGerada.CartasJogador[0], maoBuilderGerada.CartasJogador[1]);

            maoGerada.Classificar(maoBuilderGerada.CartasMesa);

            Assert.Equal(classificacaoEsperada, maoGerada.Classificacao);
            Assert.Equal(pontuacaoEsperada, maoGerada.Pontuacao);
        }

        [Fact]
        public void ClassificarJogadaComoQuadra()
        {
            var cartasJogador = "A;C|A;O";
            var cartasMesa = "A;P|A;E|8;P|2;O|4;P";
            var pontuacaoEsperada = 81414141408;
            var classificacaoEsperada = "Quadra de ASES, kicker OITO";
            var maoBuilderGerada = new MaoBuilder(cartasJogador, cartasMesa);
            var maoGerada = new Mao(maoBuilderGerada.CartasJogador[0], maoBuilderGerada.CartasJogador[1]);

            maoGerada.Classificar(maoBuilderGerada.CartasMesa);

            Assert.Equal(classificacaoEsperada, maoGerada.Classificacao);
            Assert.Equal(pontuacaoEsperada, maoGerada.Pontuacao);
        }

        [Fact]
        public void ClassificarJogadaComoFullHouse()
        {
            var cartasJogador = "K;C|K;O";
            var cartasMesa = "K;P|10;E|10;P|6;O|3;P";
            var pontuacaoEsperada = 71313131010;
            var classificacaoEsperada = "Full House de REIS e DEZ";
            var maoBuilderGerada = new MaoBuilder(cartasJogador, cartasMesa);
            var maoGerada = new Mao(maoBuilderGerada.CartasJogador[0], maoBuilderGerada.CartasJogador[1]);

            maoGerada.Classificar(maoBuilderGerada.CartasMesa);

            Assert.Equal(classificacaoEsperada, maoGerada.Classificacao);
            Assert.Equal(pontuacaoEsperada, maoGerada.Pontuacao);
        }

        [Fact]
        public void ClassificarJogadaComoFlush()
        {
            var cartasJogador = "Q;P|8;P";
            var cartasMesa = "6;P|4;P|2;P|6;O|3;C";
            var pontuacaoEsperada = 61208060402;
            var classificacaoEsperada = "Flush de Paus, maior carta DAMA";
            var maoBuilderGerada = new MaoBuilder(cartasJogador, cartasMesa);
            var maoGerada = new Mao(maoBuilderGerada.CartasJogador[0], maoBuilderGerada.CartasJogador[1]);

            maoGerada.Classificar(maoBuilderGerada.CartasMesa);

            Assert.Equal(classificacaoEsperada, maoGerada.Classificacao);
            Assert.Equal(pontuacaoEsperada, maoGerada.Pontuacao);
        }
    }

    public class Mao
    {
        public List<Carta> Cartas { get; private set; }
        public List<Carta> CartasShowdown { get; private set; }
        public string Classificacao { get; private set; }
        public long Pontuacao { get; private set; }

        private delegate bool MetodoVerificacao();

        private readonly List<MetodoVerificacao> _listVerificacao;

        /// <summary>
        /// Inicia uma instância de mão
        /// </summary>
        /// <param name="primeiraCarta">Primeira Carta</param>
        /// <param name="segundaCarta">Segunda Carta</param>
        public Mao(Carta primeiraCarta, Carta segundaCarta)
        {
            if (primeiraCarta == null || segundaCarta == null)
                throw new Exception(Ressource.MaoCartaInvalida);

            if (primeiraCarta.Id == segundaCarta.Id)
                throw new Exception(Ressource.MaoCartasDuplicadas);

            Classificacao = "";
            Cartas = new List<Carta>
            {
                primeiraCarta,
                segundaCarta
            };

            _listVerificacao = new List<MetodoVerificacao>
            {
                VerificarRoyalFlush,
                VerificarStraightFlush,
                VerificarQuadra,
                VerificarFullHouse,
                VerificarFlush
            };
        }

        /// <summary>
        /// Classifica e gera pontuação a jogada com base nas cartas do jogador e as cartas da mesa
        /// </summary>
        /// <param name="cartasMesa">Cartas da mesa. É esperado receber uma lista contendo cinco cartas</param>
        public void Classificar(List<Carta> cartasMesa)
        {
            CartasShowdown = MontarShowdown(cartasMesa).OrderByDescending(carta => carta.Valor.Peso).ToList();

            foreach (var verificar in _listVerificacao)
            {
                if (verificar())
                    break;
            }
        }

        /// <summary>
        /// Monta uma lista de cartas juntando as cartas do jogador e da mesa
        /// </summary>
        /// <param name="cartasMesa">Cartas da Mesa</param>
        /// <returns>Retorna a lista de cartas do showdown</returns>
        private List<Carta> MontarShowdown(List<Carta> cartasMesa)
        {
            List<Carta> cartasDaJogada = new List<Carta>();

            foreach (var carta in Cartas)
            {
                cartasDaJogada.Add(carta);
            }

            foreach (var carta in cartasMesa)
            {
                cartasDaJogada.Add(carta);
            }

            return cartasDaJogada;
        }

        /// <summary>
        /// Verifica se a mão possui um RoyalFlush
        /// </summary>
        /// <returns>Retorna true caso houver Royal Flush na jogada, caso contrário false</returns>
        private bool VerificarRoyalFlush()
        {
            try
            {
                var grupo = CartasShowdown.Take(5).GroupBy(carta => carta.Naipe.Nome);

                if (CartasShowdown[0].Valor.Peso == 14 &&
                    CartasShowdown[1].Valor.Peso == 13 &&
                    CartasShowdown[2].Valor.Peso == 12 &&
                    CartasShowdown[3].Valor.Peso == 11 &&
                    CartasShowdown[4].Valor.Peso == 10 &&
                    grupo.Count() == 1)
                {
                    Classificacao = $"Royal Flush de {grupo.First().Key}";
                    Pontuacao = 101413121110;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao Verificar Royal Flush - {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Verifica se a mão possui um Straight Flush
        /// </summary>
        /// <returns>Retorna true caso houver Straight Flush na jogada, caso contrário false</returns>
        private bool VerificarStraightFlush()
        {
            string menorValor = null;
            string maiorValor = null;
            Carta cartaAnterior = null;
            string pontuacao = "";

            try
            {
                var flush = CartasShowdown
                    .GroupBy(c => new { c.Naipe.Id, c.Naipe.Nome })
                    .Where(g => g.Count() >= 5)
                    .Select(c => new
                    {
                        c.Key.Id,
                        c.Key.Nome
                    })
                    .FirstOrDefault();

                if (flush == null)
                    return false;

                foreach (var cartaAtual in CartasShowdown.Where(c => c.Naipe.Id == flush.Id))
                {
                    var diferencaDeValor = cartaAnterior?.Valor?.Peso - cartaAtual.Valor.Peso;
                    if (cartaAnterior != null && diferencaDeValor != 1)
                        return false;

                    pontuacao += cartaAtual.Valor.Peso.ToString().PadLeft(2, '0');
                    maiorValor = maiorValor ?? cartaAtual.Valor.Nome;
                    menorValor = cartaAtual.Valor.Nome;
                    cartaAnterior = cartaAtual;
                }

                Classificacao = $"Straight Flush de {flush.Nome}, {menorValor} a {maiorValor}";
                Pontuacao = long.Parse($"09{pontuacao}");
                return true;
        }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao Verificar Straight Flush - {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Verifica se a mão possui uma Quadra
        /// </summary>
        /// <returns>Retorna true caso houver Quadra na jogada, caso contrário false</returns>
        private bool VerificarQuadra()
        {
            try
            {
                var quadra = CartasShowdown
                    .GroupBy(c => new { c.Valor.Id, c.Valor.Plural, c.Valor.Peso})
                    .Where(g => g.Count() == 4)
                    .Select(c => new
                    {
                        c.Key.Id,
                        c.Key.Plural,
                        c.Key.Peso
                    })
                    .FirstOrDefault();

                if (quadra != null)
                {
                    var kicker = CartasShowdown
                        .Where(c => c.Valor.Id != quadra.Id)
                        .First();

                    var stringQuadra = new StringBuilder().Insert(0, quadra.Peso.ToString().PadLeft(2, '0'), 4).ToString();
                    Classificacao = $"Quadra de {quadra.Plural}, kicker {kicker.Valor.Nome}";
                    Pontuacao = long.Parse($"08{stringQuadra}{kicker.Valor.Peso.ToString().PadLeft(2,'0')}");
                    return true;
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao Verificar Quadra - {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Verifica se a mão possui um FullHouse
        /// </summary>
        /// <returns>Retorna true caso houver Full House na jogada, caso contrário false</returns>
        private bool VerificarFullHouse()
        {
            try
            {
                var trinca = CartasShowdown
                    .GroupBy(c => new { c.Valor.Id, c.Valor.Plural, c.Valor.Peso })
                    .Where(g => g.Count() == 3)
                    .Select(c => new
                    {
                        c.Key.Id,
                        c.Key.Plural,
                        c.Key.Peso
                    })
                    .FirstOrDefault();

                if (trinca == null)
                    return false;

                var par = CartasShowdown
                    .GroupBy(c => new { c.Valor.Id, c.Valor.Plural, c.Valor.Peso })
                    .Where(g => g.Key.Id != trinca.Id && g.Count() == 2)
                    .Select(c => new
                    {
                        c.Key.Id,
                        c.Key.Plural,
                        c.Key.Peso
                    })
                    .First();

                if (par != null)
                {
                    var stringTrinca = new StringBuilder().Insert(0, trinca.Peso.ToString().PadLeft(2, '0'), 3).ToString();
                    var stringPar = new StringBuilder().Insert(0, par.Peso.ToString().PadLeft(2, '0'), 2).ToString();
                    Classificacao = $"Full House de {trinca.Plural} e {par.Plural}";
                    Pontuacao = long.Parse($"07{stringTrinca}{stringPar}");
                    return true;
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao Verificar Full House - {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Verifica se a mão possui um Flush
        /// </summary>
        /// <returns>Retorna true caso houver Flush na jogada, caso contrário false</returns>
        private bool VerificarFlush()
        {
            var classificacao = "";
            var pontuacao = "";

            try
            {
                var flush = CartasShowdown
                    .GroupBy(c => new { c.Naipe.Id, c.Naipe.Nome })
                    .Where(g => g.Count() >= 5)
                    .Select(c => new
                    {
                        c.Key.Id,
                        c.Key.Nome
                    })
                    .FirstOrDefault();

                if (flush == null)
                    return false;

                foreach (var carta in CartasShowdown.Where(c => c.Naipe.Id == flush.Id))
                {
                    classificacao += carta.Valor.Nome;
                    pontuacao += carta.Valor.Peso.ToString().PadLeft(2, '0');
                }

                Classificacao = $"Flush de {flush.Nome}, {classificacao}";
                Pontuacao = long.Parse($"06{pontuacao}");
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao Verificar Flush - {ex.Message}", ex);
            }
        }
    }
}
