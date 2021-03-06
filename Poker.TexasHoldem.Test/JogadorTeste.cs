﻿using ExpectedObjects;
using Poker.TexasHoldem.Lib;
using Poker.TexasHoldem.Lib._Base;
using Poker.TexasHoldem.Lib._Enum;
using Poker.TexasHoldem.Test._Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Poker.TexasHoldem.Test
{
    public class JogadorTeste
    {
        private readonly int _idJogadorDefault = 1;
        private readonly string _nomeJogadorDefault = "Alexandre Teste";
        private readonly Jogador _jogadorDefault;

        public JogadorTeste()
        {
            _jogadorDefault = new Jogador(_idJogadorDefault, _nomeJogadorDefault);
        }

        [Fact]
        public void DeveGerarJogador()
        {
            var jogadorEsperado = new
            {
                Id = _idJogadorDefault,
                Nome = _nomeJogadorDefault,
                Fichas = Ressource.JogadorFichasInicial,
                Status = Ressource.JogadorStatusInicial
            };

            var jogadorGerado = new Jogador(_idJogadorDefault, _nomeJogadorDefault);

            jogadorEsperado.ToExpectedObject().ShouldMatch(jogadorGerado);
        }

        [Theory(DisplayName = "NaoDevePermitirJogadorComIdInvalido")]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-10)]
        public void NaoDevePermitirJogadorComIdInvalido(int idJogadorInvalido)
        {
            var mensagemDeErro = Assert.Throws<Exception>(() => new Jogador(idJogadorInvalido, _nomeJogadorDefault)).Message;
            Assert.Equal(Ressource.JogadorMsgIdInvalido, mensagemDeErro);
        }

        [Theory(DisplayName = "NaoDevePermitirJogadorComNuloOuEmBranco")]
        [InlineData("")]
        [InlineData("    ")]
        [InlineData(null)]
        public void NaoDevePermitirJogadorComNuloOuEmBranco(string nomeJogadorInvalido)
        {
            var mensagemDeErro = Assert.Throws<Exception>(() => new Jogador(_idJogadorDefault, nomeJogadorInvalido)).Message;
            Assert.Equal(Ressource.JogadorMsgNomeObrigatorio, mensagemDeErro);
        }

        [Theory(DisplayName = "NaoDevePermitirJogadorComNomeSuperiorAVInteCaracteres")]
        [InlineData("abcdefghijklmnopqrstuvwxyz")]
        [InlineData("1234567890123456789012")]
        public void NaoDevePermitirJogadorComNomeSuperiorAVInteCaracteres(string nomeJogadorLongo)
        {
            var mensagemDeErro = Assert.Throws<Exception>(() => new Jogador(_idJogadorDefault, nomeJogadorLongo)).Message;
            Assert.Equal(Ressource.JogadorMsgNomeSuperior20Caracteres, mensagemDeErro);
        }

        [Theory(DisplayName = "PermitirDebitarFichasDoJogador")]
        [InlineData(100, 900)]
        [InlineData(500, 500)]
        [InlineData(999, 1)]
        public void PermitirDebitarFichasDoJogador(int fichasApostadas, int fichasTotalEsperadas)
        {
            _jogadorDefault.Apostar(fichasApostadas);

            Assert.Equal(fichasTotalEsperadas, _jogadorDefault.Fichas);
        }

        [Theory(DisplayName = "PermitirDuasAcoesDeDebitarFichasDoJogador")]
        [InlineData(100, 100, 800)]
        [InlineData(250, 250, 500)]
        [InlineData(500, 500, 0)]
        public void PermitirDuasAcoesDeDebitarFichasDoJogador(int fichasApostadasAcao1, int fichasApostadasAcao2, int fichasTotalEsperadas)
        {
            _jogadorDefault.Apostar(fichasApostadasAcao1);
            _jogadorDefault.Apostar(fichasApostadasAcao2);

            Assert.Equal(fichasTotalEsperadas, _jogadorDefault.Fichas);
        }

        [Theory(DisplayName = "PermitirRetirarQuantidadeSuperiorAoMáximoDeFichasDoJogador")]
        [InlineData(1200, 0, 1000)]
        [InlineData(2000, 0, 1000)]
        [InlineData(3000, 0, 1000)]
        public void PermitirRetirarQuantidadeSuperiorAoMáximoDeFichasDoJogador(int fichasApostadas, int fichasTotaisAposAposta, int fichasApostadasEsperadas)
        {
            var fichasApostadasPeloJogador = _jogadorDefault.Apostar(fichasApostadas);

            Assert.Equal(fichasTotaisAposAposta, _jogadorDefault.Fichas);
            Assert.Equal(fichasApostadasEsperadas, fichasApostadasPeloJogador);
        }

        [Theory(DisplayName ="TrocarStatusdoJogadorParaAllInSeAQuantidadeDeFichasApostadasForMaiorQueAQUantidadeTotalDeFichas")]
        [InlineData(500, StatusJogador.Esperando)]
        [InlineData(1000, StatusJogador.AllIn)]
        [InlineData(2000, StatusJogador.AllIn)]
        public void TrocarStatusdoJogadorParaAllInSeAQuantidadeDeFichasApostadasForMaiorQueAQUantidadeTotalDeFichas(int fichasApostadas, StatusJogador statusEsperado)
        {
            var fichasRetiradasDoJogador = _jogadorDefault.Apostar(fichasApostadas);

            Assert.Equal(statusEsperado, _jogadorDefault.Status);
        }

        [Theory(DisplayName ="VerficarQuantidadeDeFichasApostadasAposTresApostas")]
        [InlineData(100, 200, 300, 600)]
        [InlineData(100, 300, 600, 1000)]
        [InlineData(300, 300, 800, 1000)]
        public void VerficarQuantidadeDeFichasApostadasAposTresApostas(int fichasAposta1, int fichasAposta2, int fichasAposta3, int totalDeFichasApostadasEsperado)
        {
            _jogadorDefault.Apostar(fichasAposta1);
            _jogadorDefault.Apostar(fichasAposta2);
            _jogadorDefault.Apostar(fichasAposta3);

            Assert.Equal(totalDeFichasApostadasEsperado, _jogadorDefault.FichasApostadasNaMao);
        }

        [Theory(DisplayName = "PermitirTrocarDeStatus")]
        [InlineData(StatusJogador.Ativo)]
        [InlineData(StatusJogador.Eliminado)]
        public void PermitirTrocarDeStatus(StatusJogador statusJogadorEsperado)
        {
            _jogadorDefault.TrocarStatus(statusJogadorEsperado);

            Assert.Equal(statusJogadorEsperado, _jogadorDefault.Status);
        }

        [Theory(DisplayName = "NaoTrocarStatusSeJogadorEstiverEliminado")]
        [InlineData(StatusJogador.Ativo)]
        [InlineData(StatusJogador.AllIn)]
        public void NaoTrocarStatusSeJogadorEstiverEliminado(StatusJogador novoStatusJogador)
        {
            var statusJogadorEsperado = StatusJogador.Eliminado;
            _jogadorDefault.TrocarStatus(statusJogadorEsperado);

            _jogadorDefault.TrocarStatus(novoStatusJogador);

            Assert.Equal(statusJogadorEsperado, _jogadorDefault.Status);
        }

        [Fact]
        public void DeveReceberCartas()
        {
            var cartaBuilder = new CartaBuilder("A;P|K;O");
            var quantidadeDeCartasEsperada = 2;

            _jogadorDefault.ReceberCarta(cartaBuilder.CartasJogador[0]);
            _jogadorDefault.ReceberCarta(cartaBuilder.CartasJogador[1]);

            Assert.True(_jogadorDefault.Mao.Cartas.Any());
            Assert.Equal(quantidadeDeCartasEsperada, _jogadorDefault.Mao.Cartas.Count());
            Assert.Equal(cartaBuilder.CartasJogador[0].Id, _jogadorDefault.Mao.Cartas[0].Id);
            Assert.Equal(cartaBuilder.CartasJogador[1].Id, _jogadorDefault.Mao.Cartas[1].Id);
        }

        [Fact]
        public void DeveEncerrarJogada()
        {
            var fichasGanhas = 500;
            var fichasTotalEsperadaComJogador = 1500;

            _jogadorDefault.EncerrarMao(fichasGanhas);

            Assert.Equal(fichasTotalEsperadaComJogador, _jogadorDefault.Fichas);
        }

        [Fact]
        public void DeveLimparAMaoDoJogadorAposEncerrarAJogada()
        {
            var cartaBuilder = new CartaBuilder("A;P|K;O");
            var fichasGanhas = 500;

            _jogadorDefault.ReceberCarta(cartaBuilder.CartasJogador[0]);
            _jogadorDefault.ReceberCarta(cartaBuilder.CartasJogador[1]);
            _jogadorDefault.EncerrarMao(fichasGanhas);

            Assert.Null(_jogadorDefault.Mao);
        }

        [Fact]
        public void DeveEliminarJogadorCasoStatusForAllInENaoTenhaRecebidoNenhumaFicha()
        {
            var fichasApostadas = 1000;
            var fichasGanhas = 0;
            var statusEsperado = StatusJogador.Eliminado;

            _jogadorDefault.Apostar(fichasApostadas);
            _jogadorDefault.EncerrarMao(fichasGanhas);

            Assert.Equal(statusEsperado, _jogadorDefault.Status);
        }

        [Fact]
        public void DeveAlterarStatusDoJogadorParaAtivoCasoStatusForAllInETenhaRecebidoFicha()
        {
            var fichasApostadas = 1000;
            var fichasGanhas = 2000;
            var statusEsperado = StatusJogador.Ativo;

            _jogadorDefault.Apostar(fichasApostadas);
            _jogadorDefault.EncerrarMao(fichasGanhas);

            Assert.Equal(statusEsperado, _jogadorDefault.Status);
        }

        [Fact]
        public void NaoDevePermitirEncerrarJogadaComFichasNegativas()
        {
            var fichasApostadas = 1000;
            var fichasGanhas = -1000;

            _jogadorDefault.Apostar(fichasApostadas);

            var MensagemDeErro = Assert.Throws<Exception>(() => _jogadorDefault.EncerrarMao(fichasGanhas)).Message;
            Assert.Equal(Ressource.JogadorMsgValorFichasGanhasInvalido, MensagemDeErro);
        }

        [Fact]
        public void DeveAumentarMontanteDeFichasApostadasNaRodada()
        {
            var fichasApostadas = 100;

            _jogadorDefault.Apostar(fichasApostadas);

            Assert.Equal(fichasApostadas, _jogadorDefault.FichasApostadasNaRodada);
        }

        [Fact]
        public void DevePermitirZerarFichasApostadasNaRodada()
        {
            var fichasApostadas = 100;
            var fichasApostadaNaRodadaEsperada = 0;
            _jogadorDefault.Apostar(fichasApostadas);

            _jogadorDefault.ZerarFichasApostadasNaRodada();

            Assert.Equal(fichasApostadaNaRodadaEsperada, _jogadorDefault.FichasApostadasNaRodada);
        }

        [Fact]
        public void DeveZerarMontanteDeFichasApostadasAoFinalDaMao()
        {
            var fichasApostadas = 1000;
            var fichasGanhas = 2000;
            var montanteFichasApostadasEsperado = 0;

            _jogadorDefault.Apostar(fichasApostadas);
            _jogadorDefault.EncerrarMao(fichasGanhas);

            Assert.Equal(montanteFichasApostadasEsperado, _jogadorDefault.FichasApostadasNaMao);
        }

        [Fact]
        public void NaoDeveDarCartasAJogadorEliminado()
        {
            var cartaBuilder = new CartaBuilder("A;P|K;O");
            _jogadorDefault.TrocarStatus(StatusJogador.Eliminado);

            _jogadorDefault.ReceberCarta(cartaBuilder.CartasJogador[0]);
            _jogadorDefault.ReceberCarta(cartaBuilder.CartasJogador[1]);

            Assert.Null(_jogadorDefault.Mao);
        }

        [Fact]
        public void DeveExecutarAcaoDeFold()
        {
            var statusJogadorEsperado = StatusJogador.Fold;
            var valorAposta = 10;
            var valorApostaEsperadorAposFold = 0;

            _jogadorDefault.Apostar(valorAposta);
            _jogadorDefault.Fold();

            Assert.Equal(statusJogadorEsperado, _jogadorDefault.Status);
            Assert.Equal(valorApostaEsperadorAposFold, _jogadorDefault.FichasApostadasNaMao);
            Assert.Equal(valorApostaEsperadorAposFold, _jogadorDefault.FichasApostadasNaRodada);
            Assert.Null(_jogadorDefault.Mao);

        }
    }
}
