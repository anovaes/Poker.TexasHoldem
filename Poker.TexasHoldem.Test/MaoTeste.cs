using ExpectedObjects;
using Poker.TexasHoldem.Test._Base;
using Poker.TexasHoldem.Test._Builder;
using Poker.TexasHoldem.Test._Util;
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
        [InlineData("A;E|K;E", "Q;E|J;E|10;E|3;O|7;C", 91413121110, "Royal Flush de ESPADAS")]
        [InlineData("A;O|K;O", "Q;O|J;O|10;O|9;O|8;O", 91413121110, "Royal Flush de OUROS")]
        [InlineData("7;O|2;E", "10;C|J;C|Q;C|K;C|A;C", 91413121110, "Royal Flush de COPAS")]
        [InlineData("A;P|10;P", "K;P|2;O|Q;P|3;C|J;P", 91413121110, "Royal Flush de PAUS")]
        [InlineData("9;C|8;C", "7;C|6;C|5;C|2;O|4;P", 80908070605, "Straight Flush de COPAS, CINCO a NOVE")]
        [InlineData("7;E|3;O", "8;E|5;C|4;E|6;E|5;E", 80807060504, "Straight Flush de ESPADAS, QUATRO a OITO")]
        [InlineData("A;O|10;O", "3;O|5;O|4;E|4;O|2;O", 80504030201, "Straight Flush de OUROS, ÁS a CINCO")]
        [InlineData("K;P|J;P", "6;P|4;P|5;P|3;P|2;P", 80605040302, "Straight Flush de PAUS, DOIS a SEIS")]
        [InlineData("A;O|A;P", "A;C|5;O|3;O|4;O|2;O", 80504030201, "Straight Flush de OUROS, ÁS a CINCO")]
        [InlineData("A;C|A;O", "A;P|A;E|8;P|2;O|4;P", 71414141408, "Quadra de ASES, kicker OITO")]
        [InlineData("A;C|K;O", "K;P|A;E|8;P|K;C|K;E", 71313131314, "Quadra de REIS, kicker ÁS")]
        [InlineData("Q;C|Q;O", "2;P|Q;E|8;P|Q;P|4;E", 71212121208, "Quadra de DAMAS, kicker OITO")]
        [InlineData("7;C|2;O", "7;P|4;E|7;O|Q;P|7;E", 70707070712, "Quadra de SETES, kicker DAMA")]
        [InlineData("K;C|K;O", "K;P|10;E|10;P|6;O|3;P", 61313131010, "Full House de REIS e DEZ")]
        [InlineData("6;C|9;O", "6;P|9;E|6;O|9;C|3;P", 60909090606, "Full House de NOVES e SEIS")]
        [InlineData("3;C|5;O", "7;P|7;E|3;E|7;O|5;P", 60707070505, "Full House de SETES e CINCOS")]
        [InlineData("A;C|K;O", "4;P|2;E|4;E|4;O|2;O", 60404040202, "Full House de QUATROS e DOIS")]
        [InlineData("Q;P|8;P", "6;P|4;P|2;P|6;O|3;C", 51208060402, "Flush de PAUS. DAMA, OITO, SEIS, QUATRO e DOIS")]
        [InlineData("8;C|3;C", "2;C|4;C|K;C|10;C|5;C", 51310080504, "Flush de COPAS. REI, DEZ, OITO, CINCO e QUATRO")]
        [InlineData("A;O|3;C", "8;O|3;O|K;P|10;O|7;O", 51410080703, "Flush de OUROS. ÁS, DEZ, OITO, SETE e TRÊS")]
        [InlineData("2;E|7;O", "9;E|3;E|6;C|Q;E|J;E", 51211090302, "Flush de ESPADAS. DAMA, VALETE, NOVE, TRÊS e DOIS")]
        [InlineData("2;O|3;P", "4;E|5;O|6;P|K;O|9;C", 40605040302, "Sequência de DOIS a SEIS")]
        [InlineData("4;C|10;E", "5;C|2;O|Q;P|A;O|3;E", 40504030201, "Sequência de ÁS a CINCO")]
        [InlineData("A;C|2;O", "K;C|10;O|Q;E|7;O|J;P", 41413121110, "Sequência de DEZ a ÁS")]
        [InlineData("A;C|A;O", "A;P|5;O|3;E|2;E|4;C", 40504030201, "Sequência de ÁS a CINCO")]
        [InlineData("5;C|K;O", "6;O|9;O|A;E|7;O|8;P", 40908070605, "Sequência de CINCO a NOVE")]
        [InlineData("J;C|J;O", "J;P|8;O|5;E|K;C|3;O", 31111111308, "Trinca de VALETES, kickers REI e OITO")]
        [InlineData("Q;C|8;E", "10;P|Q;P|5;O|3;P|Q;E", 31212121008, "Trinca de DAMAS, kickers DEZ e OITO")]
        [InlineData("3;C|A;E", "3;P|2;O|4;O|3;C|7;E", 30303031407, "Trinca de TRÊS, kickers ÁS e SETE")]
        [InlineData("10;P|10;C", "6;O|6;E|3;O|A;P|9;O", 21010060614, "Dois pares de DEZ e SEIS, kicker ÁS")]
        [InlineData("7;P|3;C", "J;O|6;E|3;O|J;P|7;O", 21111070706, "Dois pares de VALETES e SETES, kicker SEIS")]
        [InlineData("A;P|K;C", "J;O|J;E|2;O|2;P|A;O", 21414111113, "Dois pares de ASES e VALETES, kicker REI")]
        [InlineData("10;P|10;C", "7;O|8;E|8;O|7;P|2;O", 21010080807, "Dois pares de DEZ e OITOS, kicker SETE")]
        [InlineData("A;O|A;P", "4;P|8;E|3;C|7;P|9;O", 11414090807, "Par de ASES, kickers NOVE, OITO e SETE")]
        [InlineData("8;O|10;P", "8;P|A;E|3;C|5;P|J;O", 10808141110, "Par de OITOS, kickers ÁS, VALETE e DEZ")]
        [InlineData("K;C|J;O", "9;P|6;E|5;C|2;O|7;E", 1311090706, "Carta Alta de REI, kickers VALETE, NOVE, SETE e SEIS")]
        [InlineData("A;C|4;O", "7;P|Q;E|3;C|2;O|8;E", 1412080704, "Carta Alta de ÁS, kickers DAMA, OITO, SETE e QUATRO")]
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
                    return false;
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
            try
            {
                (string Id, string Nome) flush = AgruparNaipe(CartasShowdown);

                if (flush.Id == null)
                    return false;

                return IsSequencia(CartasShowdown.Where(c => c.Naipe.Id == flush.Id).ToList(), $"Straight Flush de {flush.Nome},", "8");
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
                (string Id, string Plural, string Peso) quadra = AgruparValor(CartasShowdown, 4, null);

                if (quadra.Id != null)
                {
                    (string Nome, string Peso) kicker = ObterKickers(CartasShowdown, quadra.Id, 1).First();
                    Classificacao = $"Quadra de {quadra.Plural}, kicker {kicker.Nome}";
                    Pontuacao = long.Parse($"7{Texto.Repeat(quadra.Peso, 4)}{kicker.Peso}");
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
                (string Id, string Plural, string Peso) trinca = AgruparValor(CartasShowdown, 3, null);

                if (trinca.Id == null)
                    return false;

                (string Id, string Plural, string Peso) par = AgruparValor(CartasShowdown, 2, trinca.Id);

                if (par.Id != null)
                {
                    Classificacao = $"Full House de {trinca.Plural} e {par.Plural}";
                    Pontuacao = long.Parse($"6{Texto.Repeat(trinca.Peso, 3)}{Texto.Repeat(par.Peso, 2)}");
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
            try
            {
                (string Id, string Nome) flush = AgruparNaipe(CartasShowdown);

                if (flush.Id == null)
                    return false;

                int contador = 1;
                string pontuacao = "";

                foreach (var carta in CartasShowdown.Where(c => c.Naipe.Id == flush.Id).Take(5))
                {
                    pontuacao += carta.Valor.PesoTexto;

                    if (contador == 1)
                        Classificacao = $"Flush de {flush.Nome}. {carta.Valor.Nome}";
                    else if (contador < 5)
                        Classificacao += $", {carta.Valor.Nome}";
                    else
                        Classificacao += $" e {carta.Valor.Nome}";

                    contador++;
                }

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
            try
            {
                return IsSequencia(CartasShowdown.Take(7).ToList(), "Sequência de", "4");
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
                (string Id, string Plural, string Peso) trinca = AgruparValor(CartasShowdown, 3, null);

                if (trinca.Id == null)
                    return false;

                (string Nome, string Peso)[] kickers = ObterKickers(CartasShowdown, trinca.Id, 2);

                Classificacao = $"Trinca de {trinca.Plural}, kickers {kickers[0].Nome} e {kickers[1].Nome}";
                Pontuacao = long.Parse($"3{Texto.Repeat(trinca.Peso, 3)}{string.Concat(kickers[0].Peso, kickers[1].Peso)}");
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
                (string Id, string Plural, string Peso) par1 = AgruparValor(CartasShowdown, 2, null);

                if (par1.Id == null)
                    return false;

                (string Id, string Plural, string Peso) par2 = AgruparValor(CartasShowdown, 2, par1.Id);

                if (par2.Id == null)
                    return false;

                var kicker = CartasShowdown
                    .Where(c => c.Valor.Id != par1.Id && c.Valor.Id != par2.Id)
                    .First();

                Classificacao = $"Dois pares de {par1.Plural} e {par2.Plural}, kicker {kicker.Valor.Nome}";
                Pontuacao = long.Parse($"2{Texto.Repeat(par1.Peso, 2)}{Texto.Repeat(par2.Peso, 2)}{kicker.Valor.PesoTexto}");
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
                (string Id, string Plural, string Peso) par = AgruparValor(CartasShowdown, 2, null);

                if (par.Id == null)
                    return false;

                (string Nome, string Peso)[] kickers = ObterKickers(CartasShowdown, par.Id, 3);

                Classificacao = $"Par de {par.Plural}, kickers {kickers[0].Nome}, {kickers[1].Nome} e {kickers[2].Nome}";
                Pontuacao = long.Parse($"1{Texto.Repeat(par.Peso, 2)}{string.Concat(kickers[0].Peso, kickers[1].Peso, kickers[2].Peso)}");
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
                    pontuacao += carta.Valor.PesoTexto;

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

        /// <summary>
        /// Realiza o agrupamento da quantidade de valores iguais informada
        /// </summary>
        /// <param name="cartas">Cartas disponíveis na jogada</param>
        /// <param name="quantidadeMinima">Quantidade de cartas mínima para gerar um grupo</param>
        /// <param name="idIgnorado">Id da carta que será ignorado no agrupamento</param>
        /// <returns>Tupla contendo Id, Nome no Plural e o Peso do valor que foi agrupado. Caso não seja formado um grupo com a quantidade mínima de cartas, 
        /// será retornado nulo</returns>
        private static (string Id, string Plural, string Peso) AgruparValor(List<Carta> cartas, int quantidadeMinima, string idIgnorado)
        {
            return cartas
                .GroupBy(c => new { c.Valor.Id, c.Valor.Plural, c.Valor.PesoTexto })
                .Where(g => (idIgnorado == null || g.Key.Id != idIgnorado) && g.Count() >= quantidadeMinima)
                .Select(c =>
                    (
                        Id: c.Key.Id, 
                        Plural: c.Key.Plural, 
                        Peso: c.Key.PesoTexto
                    )
                )
                .FirstOrDefault();
        }

        /// <summary>
        /// Realiza o agrupamento de cinco cartas com naipes iguais
        /// </summary>
        /// <param name="cartas">Cartas disponíveis na jogada</param>
        /// <returns>Tupla contendo Id e Nome do naipe que foi agrupado. Caso não seja formado um grupo com a quantidade de cinco cartas, 
        /// será retornado nulo</returns>
        private static (string Id, string Nome) AgruparNaipe(List<Carta> cartas)
        {
            return cartas
                .GroupBy(c => new { c.Naipe.Id, c.Naipe.Nome })
                .Where(g => g.Count() >= 5)
                .Select(c => 
                    (
                        Id: c.Key.Id, 
                        Nome: c.Key.Nome
                    )
                )
                .FirstOrDefault();
        }

        /// <summary>
        /// Obtêm as cartas kickers 
        /// </summary>
        /// <param name="cartas"> Cartas disponíveis na jogada</param>
        /// <param name="idIgnorado">Id da carta que será ignorado no agrupamento</param>
        /// <param name="quantidadeDeCartas">Quantidade de cartas utilizadas como kicker</param>
        /// <returns>Retorna uma lista de tupla contendo o Nome e o Peso das cartas kickers</returns>
        private (string Nome, string Peso)[] ObterKickers(List<Carta> cartas, string idIgnorado, int quantidadeDeCartas)
        {
            return CartasShowdown
                .Where(c => c.Valor.Id != idIgnorado)
                .Take(quantidadeDeCartas)
                .Select(c => (Nome: c.Valor.Nome, Peso: c.Valor.PesoTexto))
                .ToArray();
        }

        /// <summary>
        /// Realiza a verificação de sequência. Utilizada nos casos de Straight Flush e Sequêcia
        /// </summary>
        /// <param name="cartas">Cartas válidas</param>
        /// <param name="raizClassificacao">Raíz da mensagem de classificação</param>
        /// <param name="raizPontuacao">Raiz da pontuação</param>
        /// <returns>Se for uma sequência, retorna true. Caso contrário, false </returns>
        private bool IsSequencia(List<Carta> cartas, string raizClassificacao, string raizPontuacao)
        {
            string menorValor = null;
            string maiorValor = null;
            Carta cartaAnterior = null;
            string pontuacao = "";
            int intervalos = 0;
            int quantidadeDeCartas = 0;

            // Caso houver um Ás entre as cartas será adicionada uma nova carta, mas com o peso = 1
            if (cartas.Where(c => c.Valor.Id == "A").Any())
            {
                string naipe = "";
                cartas.ForEach(c =>
                {
                    if (c.Valor.Id == "A")
                        naipe = c.Naipe.Id;
                });

                Carta carta = new Carta($"A;{naipe}");
                carta.Valor.TrocarPeso();
                cartas.Add(carta);
                intervalos--;
            }

            foreach (var cartaAtual in cartas)
            {
                var diferencaDeValor = cartaAnterior?.Valor?.Peso - cartaAtual.Valor.Peso;
                if (diferencaDeValor > 1)
                {
                    pontuacao = "";
                    maiorValor = null;
                    quantidadeDeCartas = 0;

                    //Será incrementado um intervalo sermpre que houver uma diferença maior que 1 do peso entre as cartas.
                    //Caso exista Ás já foi retirado um do intervalo, pois ainda será analisado como Ás baixo
                    intervalos++;

                    //Com 3 intervalos maiores que 1 já não é mais possível realizar uma sequência
                    if (intervalos == 3)
                        break;
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
                Classificacao = $"{raizClassificacao} {menorValor} a {maiorValor}";
                Pontuacao = long.Parse($"{raizPontuacao}{pontuacao}");
                return true;
            }
            else
                return false;
        }
    }
}
