using ExpectedObjects;
using Poker.TexasHoldem.Lib;
using Poker.TexasHoldem.Lib._Base;
using Poker.TexasHoldem.Lib._Enum;
using Poker.TexasHoldem.Test._Builder;
using System;
using System.Collections.Generic;
using Xunit;

namespace Poker.TexasHoldem.Test
{
    public class MesaPreTeste
    {
        private readonly int _quantidadeMinimaDeJogadoresPermitidos = 2;
        private readonly int _quantidadeMaximaDeJogadoresPermitidos = 9;
        private readonly int _idMesaDefault = 1;
        private readonly string _nomeJogadorDefault = "Alexandre";
        private int _valorSmallBlindEsperado;
        private int _valorBigBlindEsperado;

        public MesaPreTeste()
        {
            _valorSmallBlindEsperado = Ressource.MesaValorInicialBlind / 2;
            _valorBigBlindEsperado = Ressource.MesaValorInicialBlind;
        }

        [Fact]
        public void DeveGerarMesa()
        {
            var idMesa = 1;
            var mesaEsperada = new
            {
                Id = idMesa,
                Pote = 0,
                Jogadores = new List<Jogador>(),
                Cartas = new List<Carta>(),
                Status = StatusMesa.Aguardando
            };

            var mesaGerada = new Mesa(idMesa);

            mesaEsperada.ToExpectedObject().ShouldMatch(mesaGerada);
        }

        [Theory(DisplayName = "NaoDevePermitirMesaComIdInvalido")]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-10)]
        public void NaoDevePermitirMesaComIdInvalido(int idMesaInvalido)
        {
            var mensagemDeErro = Assert.Throws<Exception>(() => new Mesa(idMesaInvalido)).Message;
            Assert.Equal(Ressource.MesaMsgIdInvalido, mensagemDeErro);
        }

        [Fact]
        public void DevePermitirTrocaDeStatusDaMesa()
        {
            var mesaGerada = new Mesa(_idMesaDefault);
            var statusEsperado = StatusMesa.Ativa;

            mesaGerada.AlterarStatus(statusEsperado);

            Assert.Equal(statusEsperado, mesaGerada.Status);
        }

        [Fact]
        public void NaoDevePermitirTrocaDeStatusParaAguardandoCasoAMesaJaEstejaAtiva()
        {
            var mesaGerada = new Mesa(_idMesaDefault);
            var statusNaoPermitido = StatusMesa.Aguardando;
            StatusMesa statusAtual = StatusMesa.Ativa;
            mesaGerada.AlterarStatus(statusAtual);

            var mensagemDeErro = Assert.Throws<Exception>(() => mesaGerada.AlterarStatus(statusNaoPermitido)).Message;

            Assert.Equal(Ressource.MesaMsgNaoPermitidoMudarStatusDeAtivaParaAguardando, mensagemDeErro);
        }

        [Fact]
        public void NaoDevePermitirTrocaDeStatusParaAguardandoCasoAMesaJaEstejaFinalizada()
        {
            var mesaGerada = new Mesa(_idMesaDefault);
            var statusNaoPermitido = StatusMesa.Aguardando;
            StatusMesa statusAtual = StatusMesa.Finalizada;
            mesaGerada.AlterarStatus(statusAtual);

            var mensagemDeErro = Assert.Throws<Exception>(() => mesaGerada.AlterarStatus(statusNaoPermitido)).Message;

            Assert.Equal(Ressource.MesaMsgNaoPermitidoMudarStatusDeFinalizadaParaAguardando, mensagemDeErro);
        }

        [Fact]
        public void NaoDevePermitirTrocaDeStatusParaAtivaCasoAMesaJaEstejaFinalizada()
        {
            var mesaGerada = new Mesa(_idMesaDefault);
            var statusNaoPermitido = StatusMesa.Ativa;
            var statusAtual = StatusMesa.Finalizada;
            mesaGerada.AlterarStatus(statusAtual);

            var mensagemDeErro = Assert.Throws<Exception>(() => mesaGerada.AlterarStatus(statusNaoPermitido)).Message;

            Assert.Equal(Ressource.MesaMsgNaoPermitidoMudarStatusDeFinalizadaParaAtiva, mensagemDeErro);
        }

        [Fact]
        public void DeveIncluirNovoJogador()
        {
            var idJogadorEsperado = 1;
            var novoJogadorEsperado = new Jogador(idJogadorEsperado, _nomeJogadorDefault);
            var mesaGerada = new Mesa(_idMesaDefault);

            bool jogadorIncluidoComSucesso = mesaGerada.IncluirJogador(_nomeJogadorDefault);

            Assert.True(jogadorIncluidoComSucesso);
        }

        [Fact]
        public void NaoDeveIncluirNovoJogadorSeAMesaJaPossuirAQuantidadeMaximaDeJogadores()
        {
            var mesaGerada = MesaBuilder.Novo().JogadoresPorMesa(_quantidadeMaximaDeJogadoresPermitidos).ObterPrimeiraMesa();

            Assert.False(mesaGerada.IncluirJogador(_nomeJogadorDefault));
        }

        [Theory(DisplayName = "NaoDeveIncluirNovoJogadorSeOStatusDaMesaForDiferenteDeAguardando")]
        [InlineData(StatusMesa.Ativa)]
        [InlineData(StatusMesa.Finalizada)]
        public void NaoDeveIncluirNovoJogadorSeOStatusDaMesaForDiferenteDeAguardando(StatusMesa statusAtualDaMesa)
        {
            var mesaGerada = MesaBuilder.Novo().JogadoresPorMesa(_quantidadeMinimaDeJogadoresPermitidos).ObterPrimeiraMesa();
            mesaGerada.AlterarStatus(statusAtualDaMesa);

            Assert.False(mesaGerada.IncluirJogador(_nomeJogadorDefault));
        }

        [Fact]
        public void DeveIniciarAMesa()
        {
            var statusEsperado = StatusMesa.Ativa;
            var mesaGerada = MesaBuilder.Novo().JogadoresPorMesa(_quantidadeMinimaDeJogadoresPermitidos).DeveIniciarPartida().ObterPrimeiraMesa();

            Assert.Equal(statusEsperado, mesaGerada.Status);
        }

        [Theory(DisplayName = "NaoDeveIniciarMaoCasoAlgumaJogadaDaMesaTenhaSidoAcionada")]
        [InlineData("pre")]
        [InlineData("flop")]
        [InlineData("turn")]
        [InlineData("river")]
        public void NaoDeveIniciarMaoCasoAlgumaJogadaDaMesaJaTenhaSidoAcionada(string jogada)
        {
            var mesaGerada = MesaBuilder.Novo().JogadoresPorMesa(_quantidadeMinimaDeJogadoresPermitidos).DeveIniciarPartida().AlterarStatusJogadaMesa(jogada, true).ObterPrimeiraMesa();

            var mensagemDeErro = Assert.Throws<Exception>(() => mesaGerada.IniciarMao()).Message;
            Assert.Equal(Ressource.MesaMsgPreFlopExecutadoAposOutraJogadaDeMesa, mensagemDeErro);
        }

        [Fact]
        public void NaoDeveIniciarMesaCasoHajaMenosJogadoresDoQueOPermitido()
        {
            var mesaGerada = MesaBuilder.Novo().JogadoresPorMesa(_quantidadeMinimaDeJogadoresPermitidos - 1).ObterPrimeiraMesa();

            var MensagemDeErro = Assert.Throws<Exception>(() => mesaGerada.IniciarPartida()).Message;
            Assert.Equal(Ressource.MesaMsgNaoPermitidoIniciarPartidaSemQuantidadeMinimaDeJogadores, MensagemDeErro);
        }

        [Fact]
        public void DevePreencherOValorDoBlindAoIniciarPartida()
        {
            var mesaGerada = MesaBuilder.Novo().JogadoresPorMesa(_quantidadeMinimaDeJogadoresPermitidos).DeveIniciarPartida().ObterPrimeiraMesa();

            Assert.Equal(Ressource.MesaValorInicialBlind, mesaGerada.ValorBlind);

        }

        [Theory(DisplayName = "NaoDeveIniciarAMesaCasoEstaJaEstejaAtiva")]
        [InlineData(StatusMesa.Ativa)]
        [InlineData(StatusMesa.Finalizada)]
        public void NaoDeveIniciarAMesaCasoEstaJaEstejaAtiva(StatusMesa statusAtualDaMesa)
        {
            var mesaGerada = MesaBuilder.Novo().JogadoresPorMesa(_quantidadeMinimaDeJogadoresPermitidos).ObterPrimeiraMesa();
            mesaGerada.AlterarStatus(statusAtualDaMesa);

            mesaGerada.IniciarPartida();

            Assert.Equal(statusAtualDaMesa, mesaGerada.Status);
        }
    }
}