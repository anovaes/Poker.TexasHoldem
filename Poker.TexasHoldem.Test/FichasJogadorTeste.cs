using ExpectedObjects;
using Poker.TexasHoldem.Lib;
using Poker.TexasHoldem.Lib._Base;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Poker.TexasHoldem.Test
{
    public class FichasJogadorTeste
    {
        [Fact]
        public void DeveGerarFichasJogador()
        {
            var jogadorFichaEsperado = new
            {
                IdJogador = 1,
                Fichas = 100
            };

            var jogadorFichaGerado = new FichasJogador(jogadorFichaEsperado.IdJogador, jogadorFichaEsperado.Fichas);

            jogadorFichaEsperado.ToExpectedObject().ShouldMatch(jogadorFichaGerado);
        }

        [Theory(DisplayName = "NaoDeveGerarJogadorFichaQuandoIdForMenorOuIgualAZero")]
        [InlineData(0)]
        [InlineData(-1)]
        public void NaoDeveGerarJogadorFichaQuandoIdForMenorOuIgualAZero(int idJogadorInvalido)
        {
            var mensagem = Assert.Throws<Exception>(() => new FichasJogador(idJogadorInvalido, 100)).Message;
            Assert.Equal(Ressource.JogadorFichaIdInvalido, mensagem);
        }

        [Theory(DisplayName = "NaoDeveGerarJogadorFichaQuandoIdForMenorOuIgualAZero")]
        [InlineData(0)]
        [InlineData(-1)]
        public void NaoDeveGerarJogadorFichaQuandoQuantidadeDeFichasForMenorOuIgualAZero(int quantidadeDeFichasInvalida)
        {
            var mensagem = Assert.Throws<Exception>(() => new FichasJogador(1, quantidadeDeFichasInvalida)).Message;
            Assert.Equal(Ressource.JogadorFichaFichasInvalidas, mensagem);
        }

        [Fact]
        public void DeveAdicionarFichas()
        {
            var idJogador = 1;
            var fichasIniciais = 100;
            var fichasAdicionadas = 250;
            var fichasEsperadas = fichasIniciais + fichasAdicionadas;
            var objFichasJogador = new FichasJogador(idJogador, fichasIniciais);

            objFichasJogador.AdicionarFichas(fichasAdicionadas);

            Assert.Equal(fichasEsperadas, objFichasJogador.Fichas);
        }
    }
}
