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

        [Theory(DisplayName = "DeveClassificarEGerarPontuacaoDaJogada")]
        [InlineData("A;E|K;E", "Q;E|J;E|10;E|3;O|7;C",  91413121110, "Royal Flush de ESPADAS")]
        [InlineData("9;C|8;C", "7;C|6;C|5;C|2;O|4;P",   80908070605, "Straight Flush de COPAS, CINCO a NOVE")]
        [InlineData("A;C|A;O", "A;P|A;E|8;P|2;O|4;P",   71414141408, "Quadra de ASES, kicker OITO")]
        [InlineData("K;C|K;O", "K;P|10;E|10;P|6;O|3;P", 61313131010, "Full House de REIS e DEZ")]
        [InlineData("Q;P|8;P", "6;P|4;P|2;P|6;O|3;C",   51208060402, "Flush de PAUS, maior carta DAMA")]
        [InlineData("2;O|3;P", "4;E|5;O|6;P|K;O|9;C",   40605040302, "Sequência de DOIS a SEIS")]
        [InlineData("J;C|J;O", "J;P|8;O|5;E|K;C|3;O",   31111111308, "Trinca de VALETES, kickers REI e OITO")]
        [InlineData("10;P|10;C", "6;O|6;E|3;O|A;P|9;O", 21010060614, "Dois pares de DEZ e SEIS, kicker ÁS")]
        [InlineData("A;O|A;P", "4;P|8;E|3;C|7;P|9;O",   11414090807, "Par de ASES, kickers NOVE, OITO e SETE")]
        [InlineData("K;C|J;O", "9;P|6;E|5;C|2;O|7;E",    1311090706, "Carta Alta de REI, kickers VALETE, NOVE, SETE e SEIS")]
        public void DeveClassificarEGerarPontuacaoDaJogada(string cartasJogador, string cartasMesa, long pontuacaoEsperada, string classificacaoEsperada)
        {
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
                VerificarFlush,
                VerificarSequencia,
                VerificarTrinca,
                VerificarDoisPares,
                VerificarPar,
                VerificarCartaAlta
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
                    Pontuacao = 91413121110;
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
                Pontuacao = long.Parse($"8{pontuacao}");
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
                    .GroupBy(c => new { c.Valor.Id, c.Valor.Plural, c.Valor.Peso })
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
                    Pontuacao = long.Parse($"7{stringQuadra}{kicker.Valor.Peso.ToString().PadLeft(2, '0')}");
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
                    .FirstOrDefault();

                if (par != null)
                {
                    var stringTrinca = new StringBuilder().Insert(0, trinca.Peso.ToString().PadLeft(2, '0'), 3).ToString();
                    var stringPar = new StringBuilder().Insert(0, par.Peso.ToString().PadLeft(2, '0'), 2).ToString();
                    Classificacao = $"Full House de {trinca.Plural} e {par.Plural}";
                    Pontuacao = long.Parse($"6{stringTrinca}{stringPar}");
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
            string maiorCarta = null;
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

                foreach (var carta in CartasShowdown.Where(c => c.Naipe.Id == flush.Id).Take(5))
                {
                    maiorCarta = maiorCarta ?? carta.Valor.Nome;
                    pontuacao += carta.Valor.Peso.ToString().PadLeft(2, '0');
                }

                Classificacao = $"Flush de {flush.Nome}, maior carta {maiorCarta}";
                Pontuacao = long.Parse($"5{pontuacao}");
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao Verificar Flush - {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Verifica se a mão possui uma Sequência
        /// </summary>
        /// <returns>Retorna true caso houver Sequência na jogada, caso contrário false</returns>
        private bool VerificarSequencia()
        {
            string menorValor = null;
            string maiorValor = null;
            Carta cartaAnterior = null;
            string pontuacao = "";
            int intervalos = 0;
            int quantidadeDeCartas = 0;

            try
            {
                foreach (var cartaAtual in CartasShowdown)
                {
                    var diferencaDeValor = cartaAnterior?.Valor?.Peso - cartaAtual.Valor.Peso;
                    if (diferencaDeValor > 1)
                    {
                        pontuacao = "";
                        maiorValor = null;
                        quantidadeDeCartas = 0;

                        //Com 3 intervalos maiores que 1 já não é mais possível realizar uma sequência
                        intervalos++;
                        if (intervalos == 3)
                            return false;
                    }
                    else if (diferencaDeValor == 0)
                        continue;

                    pontuacao += cartaAtual.Valor.Peso.ToString().PadLeft(2, '0');
                    maiorValor = maiorValor ?? cartaAtual.Valor.Nome;
                    menorValor = cartaAtual.Valor.Nome;
                    cartaAnterior = cartaAtual;

                    //Necessário ter cinco cartas na sequência para ocorrer a sequência
                    quantidadeDeCartas++;
                    if (quantidadeDeCartas == 5)
                        break;
                }

                if (quantidadeDeCartas == 5)
                {
                    Classificacao = $"Sequência de {menorValor} a {maiorValor}";
                    Pontuacao = long.Parse($"4{pontuacao}");
                    return true;
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao Verificar Sequência - {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Verifica se a mão possui uma Trinca
        /// </summary>
        /// <returns>Retorna true caso houver Trinca na jogada, caso contrário false</returns>
        private bool VerificarTrinca()
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

                var kickers = CartasShowdown
                    .Where(c => c.Valor.Id != trinca.Id)
                    .Take(2)
                    .ToList();

                var stringTrinca = new StringBuilder().Insert(0, trinca.Peso.ToString().PadLeft(2, '0'), 3).ToString();
                var stringKickers = $"{kickers[0].Valor.Peso.ToString().PadLeft(2, '0') }{ kickers[1].Valor.Peso.ToString().PadLeft(2, '0')}";
                Classificacao = $"Trinca de {trinca.Plural}, kickers {kickers[0].Valor.Nome} e {kickers[1].Valor.Nome}";
                Pontuacao = long.Parse($"3{stringTrinca}{stringKickers}");
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao Verificar Trinca - {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Verifica se a mão possui dois Pares
        /// </summary>
        /// <returns>Retorna true caso houver Dois Pares na jogada, caso contrário false</returns>
        private bool VerificarDoisPares()
        {
            try
            {
                var pares = CartasShowdown
                    .GroupBy(c => new { c.Valor.Id, c.Valor.Plural, c.Valor.Peso })
                    .Where(g => g.Count() == 2)
                    .Select(c => new
                    {
                        c.Key.Id,
                        c.Key.Plural,
                        c.Key.Peso
                    })
                    .Take(2)
                    .ToList();

                if (pares.Count() != 2)
                    return false;

                var kickers = CartasShowdown
                    .Where(c => c.Valor.Id != pares[0].Id && c.Valor.Id != pares[1].Id)
                    .Take(1)
                    .ToList();


                var stringPar1 = new StringBuilder().Insert(0, pares[0].Peso.ToString().PadLeft(2, '0'), 2).ToString();
                var stringPar2 = new StringBuilder().Insert(0, pares[1].Peso.ToString().PadLeft(2, '0'), 2).ToString();
                var stringKicker = kickers[0].Valor.Peso.ToString().PadLeft(2, '0');
                Classificacao = $"Dois pares de {pares[0].Plural} e {pares[1].Plural}, kicker {kickers[0].Valor.Nome}";
                Pontuacao = long.Parse($"2{stringPar1}{stringPar2}{stringKicker}");
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao Verificar Dois Pares - {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Verifica se a mão possui um Par
        /// </summary>
        /// <returns>Retorna true caso houver Par na jogada, caso contrário false</returns>
        private bool VerificarPar()
        {
            try
            {
                var par = CartasShowdown
                    .GroupBy(c => new { c.Valor.Id, c.Valor.Plural, c.Valor.Peso })
                    .Where(g => g.Count() == 2)
                    .Select(c => new
                    {
                        c.Key.Id,
                        c.Key.Plural,
                        c.Key.Peso
                    })
                    .FirstOrDefault();

                if (par == null)
                    return false;

                var kickers = CartasShowdown
                .Where(c => c.Valor.Id != par.Id)
                .Take(3)
                .ToList();


                var stringPar = new StringBuilder().Insert(0, par.Peso.ToString().PadLeft(2, '0'), 2).ToString();
                var stringKickers = $"{kickers[0].Valor.Peso.ToString().PadLeft(2, '0') }{ kickers[1].Valor.Peso.ToString().PadLeft(2, '0')}{ kickers[2].Valor.Peso.ToString().PadLeft(2, '0')}";
                Classificacao = $"Par de {par.Plural}, kickers {kickers[0].Valor.Nome}, {kickers[1].Valor.Nome} e {kickers[2].Valor.Nome}";
                Pontuacao = long.Parse($"1{stringPar}{stringKickers}");
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao Verificar Par - {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Verifica se a mão possui Carta Alta
        /// </summary>
        /// <returns>Retorna true caso houver Carta Alta na jogada, caso contrário false</returns>
        private bool VerificarCartaAlta()
        {
            try
            {
                var pontuacao = "";
                var contador = 1;

                foreach (var carta in CartasShowdown.Take(5).ToList())
                {
                    pontuacao += carta.Valor.Peso.ToString().PadLeft(2, '0');

                    if (contador == 1)
                        Classificacao = $"Carta Alta de {carta.Valor.Nome}, kickers ";
                    else if (contador == 2)
                        Classificacao += $"{carta.Valor.Nome}";
                    else if (contador < 5)
                        Classificacao += $", {carta.Valor.Nome}";
                    else
                        Classificacao += $" e {carta.Valor.Nome}";

                    contador++;
                } 

                Pontuacao = long.Parse(pontuacao);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao Verificar Carta Alta - {ex.Message}", ex);
            }
        }
    }
}
