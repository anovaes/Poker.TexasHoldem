using ExpectedObjects;
using Poker.TexasHoldem.Lib;
using Poker.TexasHoldem.Lib._Base;
using Poker.TexasHoldem.Test._Builder;
using System;
using System.Collections.Generic;
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
            Assert.Equal(Ressource.MaoCartasJogadorDuplicadas, menssagemDeErro);
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
        [InlineData("3;C|A;E", "3;P|2;O|4;O|3;O|7;E", 30303031407, "Trinca de TRÊS, kickers ÁS e SETE")]
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

        [Fact]
        public void NaoDeveGerarClassificacaoCasoHajaCartasDuplicadas()
        {
            var maoBuilderGerada = new MaoBuilder("A;O|K;C", "Q;C|10;C|K;C|8;P|A;O");
            var maoGerada = new Mao(maoBuilderGerada.CartasJogador[0], maoBuilderGerada.CartasJogador[1]);

            string mensagemDeErro = Assert.Throws<Exception>(() => maoGerada.Classificar(maoBuilderGerada.CartasMesa)).Message;
            Assert.Equal($"{Ressource.MaoCartasClassificacaoDuplicadas} A;O|K;C", mensagemDeErro);
        }
    }
}
