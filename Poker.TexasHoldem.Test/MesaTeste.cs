using ExpectedObjects;
using Poker.TexasHoldem.Lib;
using Poker.TexasHoldem.Lib._Base;
using Poker.TexasHoldem.Lib._Enum;
using Poker.TexasHoldem.Lib._Util;
using Poker.TexasHoldem.Test._Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Poker.TexasHoldem.Test
{
    public class MesaTeste
    {
        private readonly int _quantidadeMinimaDeJogadoresPermitidos = 2;
        private readonly int _quantidadeMaximaDeJogadoresPermitidos = 9;
        private readonly int _idMesaDefault = 1;
        private readonly string _nomeJogadorDefault = "Alexandre";
        private int _valorSmallBlindEsperado;
        private int _valorBigBlindEsperado;

        public MesaTeste()
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

        [Fact]
        public void DeveIniciarMaoDistribuindoCartasAosJogadores()
        {
            var quantidadeCartasDoJogador = 2;
            var quantidadeCartasMesa = 0;

            var mesaGerada = MesaBuilder.Novo().JogadoresPorMesa(_quantidadeMinimaDeJogadoresPermitidos).DeveIniciarPartida().DeveIniciarMao().ObterPrimeiraMesa();

            foreach (var jogador in mesaGerada.Jogadores)
            {
                Assert.Equal(quantidadeCartasDoJogador, jogador.Mao.Cartas.Count);
            }

            Assert.Equal(quantidadeCartasMesa, mesaGerada.Cartas.Count);
        }

        [Fact]
        public void DeveIniciarMaoDistribuindoCartasApenasAosJogadoresAtivos()
        {
            var quantidadeJogadores = 4;
            var mesaGerada = MesaBuilder.Novo().JogadoresPorMesa(quantidadeJogadores).ObterPrimeiraMesa();
            var quantidadeCartasDoJogadorAtivo = 2;
            mesaGerada.Jogadores[2].TrocarStatus(StatusJogador.Eliminado);
            mesaGerada.Jogadores[3].TrocarStatus(StatusJogador.Eliminado);

            mesaGerada.IniciarPartida();
            mesaGerada.IniciarMao();

            foreach (var jogadorAtivo in mesaGerada.JogadoresAtivos)
            {
                Assert.Equal(quantidadeCartasDoJogadorAtivo, jogadorAtivo.Mao.Cartas.Count);
            }

            foreach (var jogadorInativo in mesaGerada.Jogadores.Where(j => j.Status == StatusJogador.Eliminado).ToList())
            {
                Assert.Null(jogadorInativo.Mao);
            }
        }

        [Fact]
        public void NaoDeveIniciarJogadaCasoNaoHajaAQuantidadeMinimaDeJogadoresAtivos()
        {
            var mesaGerada = MesaBuilder.Novo().JogadoresPorMesa(_quantidadeMinimaDeJogadoresPermitidos).DeveIniciarPartida().ObterPrimeiraMesa();
            mesaGerada.Jogadores[1].TrocarStatus(StatusJogador.Eliminado);

            var mensagemDeErro = Assert.Throws<Exception>(() => mesaGerada.IniciarMao()).Message;
            Assert.Equal(Ressource.MesaMsgNaoPermitidoIniciarRodadaSemQuantidadeMinimaDeJogadores, mensagemDeErro);
        }

        [Theory(DisplayName = "DeveIndicarJogadorSmallBlindAoIniciarRodada")]
        [InlineData(5, 1, 0)]
        [InlineData(2, 1, 2)]
        [InlineData(6, 6, 5)]
        [InlineData(9, 1, 9)]
        [InlineData(9, 2, 1)]
        public void DeveIndicarJogadorSmallBlindAoIniciarMao(int quantidadeJogadoresNaMesa, int idJogadorSmallBlindEsperado, int idJogadorSmallBlindJogadaAnterior)
        {
            var mesaGerada = MesaBuilder.Novo().JogadoresPorMesa(quantidadeJogadoresNaMesa).DeveIniciarPartida().ObterPrimeiraMesa();
            mesaGerada.AlterarIdJogadorSmallBlind(idJogadorSmallBlindJogadaAnterior);

            mesaGerada.IniciarMao();

            Assert.Equal(idJogadorSmallBlindEsperado, mesaGerada.IdJogadorSmallBlind);
        }

        [Theory(DisplayName = "DeveIndicarJogadorUTGAoIniciarRodada")]
        [InlineData(2, 1)]
        [InlineData(3, 3)]
        [InlineData(6, 3)]
        [InlineData(9, 3)]
        public void DeveIndicarJogadorUTGAoIniciarMao(int quantidadeJogadoresNaMesa, int idJogadorUTGEsperado)
        {
            var mesaGerada = MesaBuilder.Novo().JogadoresPorMesa(quantidadeJogadoresNaMesa).DeveIniciarPartida().DeveIniciarMao().ObterPrimeiraMesa();

            Assert.Equal(idJogadorUTGEsperado, mesaGerada.IdJogadorUTG);
            Assert.Equal(idJogadorUTGEsperado, mesaGerada.JogadoresAtivos[0].Id);
        }

        [Theory(DisplayName = "DeveIndicarJogadorUTGAoIniciarRodadaDePartidaEmAndamento")]
        [InlineData(2, 2, 1)]
        [InlineData(5, 5, 3)]
        [InlineData(9, 6, 9)]
        [InlineData(3, 2, 2)]
        public void DeveIndicarJogadorUTGAoIniciarRodadaDePartidaEmAndamento(int quantidadeJogadoresNaMesa, int idJogadorSmallBlindJogadaAnterior, int idJogadorUTGEsperado)
        {
            var mesaGerada = MesaBuilder.Novo().JogadoresPorMesa(quantidadeJogadoresNaMesa).DeveIniciarPartida().ObterPrimeiraMesa();
            mesaGerada.AlterarIdJogadorSmallBlind(idJogadorSmallBlindJogadaAnterior);

            mesaGerada.IniciarMao();

            Assert.Equal(idJogadorUTGEsperado, mesaGerada.IdJogadorUTG);
        }

        [Fact]
        public void DeveJogadorAtualEstarNuloAoIniciarMao()
        {
            var mesaGerada = MesaBuilder.Novo().JogadoresPorMesa(_quantidadeMinimaDeJogadoresPermitidos).DeveIniciarPartida().DeveIniciarMao().ObterPrimeiraMesa();

            Assert.Null(mesaGerada.JogadorAtual);
        }

        [Fact]
        public void DeveApostaAtualConterOValorDoBigBlindAoIniciarMao()
        {
            var apostaAtualEsperada = Ressource.MesaValorInicialBlind;
            var mesaGerada = MesaBuilder.Novo().JogadoresPorMesa(_quantidadeMinimaDeJogadoresPermitidos).DeveIniciarPartida().DeveIniciarMao().ObterPrimeiraMesa();

            Assert.Equal(apostaAtualEsperada, mesaGerada.ApostaAtual);
        }

        [Fact]
        public void DeveColetarBlindDosJogadoresAoIniciarRodada()
        {
            var quantidadeJogadoresNaMesa = 3;
            var valorPoteEsperado = _valorBigBlindEsperado + _valorSmallBlindEsperado;
            var quantidadeFichasJogadorSmallBlindEsperada = Ressource.JogadorFichasInicial - _valorSmallBlindEsperado;
            var quantidadeFichasJogadorBigBlindEsperada = Ressource.JogadorFichasInicial - _valorBigBlindEsperado;
            var mesaGerada = MesaBuilder.Novo().JogadoresPorMesa(quantidadeJogadoresNaMesa).DeveIniciarPartida().ObterPrimeiraMesa();
            var mensagemSmallBlind = Mensagem.Gerar(Ressource.MesaAcaoBlind, mesaGerada.Jogadores[0].Nome, "small", _valorSmallBlindEsperado.ToString());
            var mensagemBigBlind = Mensagem.Gerar(Ressource.MesaAcaoBlind, mesaGerada.Jogadores[1].Nome, "big", _valorBigBlindEsperado.ToString());
            var mensagemEsperada = $"{mensagemSmallBlind}\r\n{mensagemBigBlind}";

            var mensagemAtual = mesaGerada.IniciarMao();
            var indexJogadorSmallBlind = mesaGerada.JogadoresAtivos.FindIndex(j => j.Id == mesaGerada.IdJogadorSmallBlind);

            Assert.Equal(mensagemEsperada, mensagemAtual);
            Assert.Equal(quantidadeFichasJogadorSmallBlindEsperada, mesaGerada.JogadoresAtivos[indexJogadorSmallBlind].Fichas);
            Assert.Equal(quantidadeFichasJogadorBigBlindEsperada, mesaGerada.JogadoresAtivos[indexJogadorSmallBlind + 1].Fichas);
            Assert.Equal(valorPoteEsperado, mesaGerada.Pote);
        }

        [Fact]
        public void DeveApresentarApenasJogadoresAtivosNaMao()
        {
            var quantidadeDeJogadoresAtivosNaMao = 6;
            var mesaGerada = MesaBuilder.Novo().JogadoresPorMesa(_quantidadeMaximaDeJogadoresPermitidos).DeveIniciarPartida().DeveIniciarMao().ObterPrimeiraMesa();

            mesaGerada.Jogadores[0].TrocarStatus(StatusJogador.Eliminado);
            mesaGerada.Jogadores[1].TrocarStatus(StatusJogador.Fold);
            mesaGerada.Jogadores[2].TrocarStatus(StatusJogador.Fold);

            Assert.Equal(quantidadeDeJogadoresAtivosNaMao, mesaGerada.JogadoresAtivos.Count());
        }

        [Theory(DisplayName = "DeveIndicarProximoJogador")]
        [InlineData(3, 1, true, 3)]
        [InlineData(2, 2, true, 2)]
        [InlineData(2, 3, false, null)]
        [InlineData(6, 6, true, 2)]
        [InlineData(7, 5, true, 7)]
        [InlineData(9, 9, true, 2)]
        [InlineData(9, 10, false, null)]
        [InlineData(9, 11, true, 3)] //Este cenário apesar de válido para o método, não pode ser considerado válido para o jogo. A classe de cima precisa compreender que todos os jogadores já tiveram ação na rodada.
        public void DeveIndicarProximoJogador(int quantidadeJogadoresNaMesa, int quantidadeIteracoes, bool existeProximoJogadorEsperado, int? idJogadorAtualEsperado)
        {
            var existeProximoJogadorAtual = false;
            var mesaGerada = MesaBuilder.Novo().JogadoresPorMesa(quantidadeJogadoresNaMesa).DeveIniciarPartida().DeveIniciarMao().ObterPrimeiraMesa();

            for (int i = 0; i < quantidadeIteracoes; i++)
            {
                existeProximoJogadorAtual = mesaGerada.TentarIndicarProximoJogador();
            }

            Assert.Equal(existeProximoJogadorEsperado, existeProximoJogadorAtual);
            Assert.Equal(idJogadorAtualEsperado, mesaGerada.JogadorAtual?.Id);
        }

        [Fact]
        public void DeveReceberApostaDoJogador()
        {
            var valorAposta = 1;
            var valorPoteEsperado = _valorBigBlindEsperado + _valorSmallBlindEsperado + valorAposta;
            var mesaGerada = MesaBuilder.Novo().JogadoresPorMesa(_quantidadeMinimaDeJogadoresPermitidos).DeveIniciarPartida().DeveIniciarMao().ObterPrimeiraMesa();

            mesaGerada.TentarIndicarProximoJogador();
            var statusJogadorAtualEsperado = mesaGerada.JogadorAtual.Status;
            var mensagemEsperada = Mensagem.Gerar(Ressource.MesaAcaoPagar, mesaGerada.JogadorAtual.Nome, valorAposta.ToString());
            var mensagemAtual = mesaGerada.ReceberAposta(valorAposta);

            Assert.Equal(mensagemEsperada, mensagemAtual);
            Assert.Equal(statusJogadorAtualEsperado, mesaGerada.JogadorAtual.Status);
            Assert.Equal(valorPoteEsperado, mesaGerada.Pote);
        }

        /// <summary>
        /// Quando o jogador enviar no método ReceberAcao aposta igual a 0. Será necessário que o método entenda a situação em que foi passado a aposta zerada.
        /// Caso o valor de suas apostas na rodada sejam iguais a da mesa, a ação deve ser encarada como um CHECK. Caso o valor de suas apostas na rodada sejam menores que a da mesa, a ação deve ser encarada como FOLD
        /// </summary>
        [Fact]
        public void DeveCompreenderAcaoDeFoldDoJogadorAtravesDeApostaZerada()
        {
            var valorAposta = 0;
            var statusJogadorEsperado = StatusJogador.Fold;
            var valorPodeEsperado = _valorBigBlindEsperado + _valorSmallBlindEsperado;
            var mesaGerada = MesaBuilder.Novo().JogadoresPorMesa(_quantidadeMinimaDeJogadoresPermitidos + 1).DeveIniciarPartida().DeveIniciarMao().ObterPrimeiraMesa();
            mesaGerada.TentarIndicarProximoJogador();
            var mensagemEsperada = Mensagem.Gerar(Ressource.MesaAcaoFold, mesaGerada.JogadorAtual.Nome);

            var mensagemAtual = mesaGerada.ReceberAposta(valorAposta);

            Assert.Equal(mensagemEsperada, mensagemAtual);
            Assert.Equal(valorPodeEsperado, mesaGerada.Pote);
            Assert.Equal(statusJogadorEsperado, mesaGerada.JogadorAtual.Status);
            Assert.Null(mesaGerada.JogadorAtual.Mao);
        }

        [Fact]
        public void DeveCompreenderAcaoDeCheckDoJogadorAtravesDeApostaZerada()
        {
            var valorApostaJogadorSmallBlind = 1;
            var valorAposta = 0;
            var valorPodeEsperado = _valorBigBlindEsperado + _valorSmallBlindEsperado + valorApostaJogadorSmallBlind;
            var mesaGerada = MesaBuilder.Novo().JogadoresPorMesa(_quantidadeMinimaDeJogadoresPermitidos).DeveIniciarPartida().DeveIniciarMao().ObterPrimeiraMesa();

            //Jogador smallblind
            mesaGerada.TentarIndicarProximoJogador();
            mesaGerada.ReceberAposta(valorApostaJogadorSmallBlind);

            //Jogador bigblind
            mesaGerada.TentarIndicarProximoJogador();
            var mensagemEsperada = Mensagem.Gerar(Ressource.MesaAcaoCheck, mesaGerada.JogadorAtual.Nome);
            var statusJogadorEsperado = mesaGerada.JogadorAtual.Status;
            var mensagemAtual = mesaGerada.ReceberAposta(valorAposta);

            Assert.Equal(mensagemEsperada, mensagemAtual);
            Assert.Equal(valorPodeEsperado, mesaGerada.Pote);
            Assert.Equal(statusJogadorEsperado, mesaGerada.JogadorAtual.Status);
            Assert.NotNull(mesaGerada.JogadorAtual.Mao);
        }

        [Fact]
        public void DeveReordenarListaDeJogadoresAposRaise()
        {
            var quantidadeJogadores = 3;
            var valorApostaUTG = 2;
            var valorApostaRaise = 3;
            var idPrimeiroJogadorListaAntesDoRaiseEsperado = 3;
            var idPrimeiroJogadorListaDepoisDoRaiseEsperado = 1;
            var valorApostaAtualAposRaise = 4;
            var mesaGerada = MesaBuilder.Novo().JogadoresPorMesa(quantidadeJogadores).DeveIniciarPartida().DeveIniciarMao().ObterPrimeiraMesa();

            Assert.Equal(idPrimeiroJogadorListaAntesDoRaiseEsperado, mesaGerada.Jogadores.First().Id);

            mesaGerada.TentarIndicarProximoJogador();
            mesaGerada.ReceberAposta(valorApostaUTG);
            mesaGerada.TentarIndicarProximoJogador();
            mesaGerada.ReceberAposta(valorApostaRaise);

            Assert.Equal(idPrimeiroJogadorListaDepoisDoRaiseEsperado, mesaGerada.Jogadores.First().Id);
            Assert.Equal(valorApostaAtualAposRaise, mesaGerada.ApostaAtual);
        }

        [Fact]
        public void DeveReordenarListaDeJogadoresAposDoisRaisesEmSequencia()
        {
            var quantidadeJogadores = 3;
            var valorApostaUTG = 2;
            var valorApostaRaiseSmallBlind = 3;
            var valorApostaRaiseBigBlind = 4;
            var idPrimeiroJogadorListaAntesDoRaiseEsperado = 3;
            var idPrimeiroJogadorListaDepoisDoPrimeiroRaiseEsperado = 1;
            var idPrimeiroJogadorListaDepoisDoSegundoRaiseEsperado = 2;
            var valorApostaAtualAposPrimeiroRaise = 4;
            var valorApostaAtualAposSegundoRaise = 6;
            var mesaGerada = MesaBuilder.Novo().JogadoresPorMesa(quantidadeJogadores).DeveIniciarPartida().DeveIniciarMao().ObterPrimeiraMesa();

            Assert.Equal(idPrimeiroJogadorListaAntesDoRaiseEsperado, mesaGerada.Jogadores.First().Id);

            mesaGerada.TentarIndicarProximoJogador();
            mesaGerada.ReceberAposta(valorApostaUTG);
            mesaGerada.TentarIndicarProximoJogador();
            mesaGerada.ReceberAposta(valorApostaRaiseSmallBlind);

            Assert.Equal(idPrimeiroJogadorListaDepoisDoPrimeiroRaiseEsperado, mesaGerada.Jogadores.First().Id);
            Assert.Equal(valorApostaAtualAposPrimeiroRaise, mesaGerada.ApostaAtual);

            mesaGerada.TentarIndicarProximoJogador();
            mesaGerada.ReceberAposta(valorApostaRaiseBigBlind);

            Assert.Equal(idPrimeiroJogadorListaDepoisDoSegundoRaiseEsperado, mesaGerada.Jogadores.First().Id);
            Assert.Equal(valorApostaAtualAposSegundoRaise, mesaGerada.ApostaAtual);
        }

        [Fact]
        public void NaoDevePermitirValorDeApostaNegativo()
        {
            var valorApostaInvalido = -10;
            var mesaGerada = MesaBuilder.Novo().JogadoresPorMesa(_quantidadeMinimaDeJogadoresPermitidos).DeveIniciarPartida().DeveIniciarMao().ObterPrimeiraMesa();
            mesaGerada.TentarIndicarProximoJogador();

            var mensagemAtual = Assert.Throws<Exception>(() => mesaGerada.ReceberAposta(valorApostaInvalido)).Message;
            Assert.Equal(Ressource.MesaMsgNaoPermitidoApostaComValorNegativo, mensagemAtual);
        }

        [Fact]
        public void DeveIniciarFlop()
        {
            var quantidadeJogadores = 3;
            var apostas = new int[] { 2, 1, 0 };
            var quantidadeCartasMesaPosFlop = 3;
            var indice = 0;
            var valorApostaAtualEsperado = 0;
            var fichasApostadasNaRodadaEsperado = 0;
            var idJogadorPrimeiroDaLista = 1;
            var mesaGerada = MesaBuilder.Novo().JogadoresPorMesa(quantidadeJogadores).DeveIniciarPartida().DeveIniciarMao().ObterPrimeiraMesa();

            while (mesaGerada.TentarIndicarProximoJogador())
            {
                mesaGerada.ReceberAposta(apostas[indice]);
                indice++;
            }

            var retorno = mesaGerada.Flop();

            Assert.True(retorno);
            Assert.Equal(quantidadeCartasMesaPosFlop, mesaGerada.Cartas.Count);
            Assert.Equal(idJogadorPrimeiroDaLista, mesaGerada.JogadoresAtivos.First().Id);
            Assert.Equal(valorApostaAtualEsperado, mesaGerada.ApostaAtual);

            foreach (var jogador in mesaGerada.JogadoresAtivos)
            {
                Assert.Equal(fichasApostadasNaRodadaEsperado, jogador.FichasApostadasNaRodada);
            }
        }

        [Fact]
        public void DeveRetornarFalsePorFaltaDeJogadoresAtivosAoIniciarFlop()
        {
            var quantidadeJogadores = 3;
            var apostaDefault = 0;
            var mesaGerada = MesaBuilder.Novo().JogadoresPorMesa(quantidadeJogadores).DeveIniciarPartida().DeveIniciarMao().ObterPrimeiraMesa();

            while (mesaGerada.TentarIndicarProximoJogador())
            {
                mesaGerada.ReceberAposta(apostaDefault);
            }

            var retorno = mesaGerada.Flop();

            Assert.False(retorno);
        }

        [Fact]
        public void NaoDevePermitirIniciarFlopSemQueTodosJogadoresAtivosTenhamRealizadoApostas()
        {
            var quantidadeJogadores = 3;
            var mesaGerada = MesaBuilder.Novo().JogadoresPorMesa(quantidadeJogadores).DeveIniciarPartida().DeveIniciarMao().ObterPrimeiraMesa();

            var mensagemDeErro = Assert.Throws<Exception>(() => mesaGerada.Flop()).Message;
            Assert.Equal(Ressource.MesaMsgNaoPermitidoIniciarFlopSemApostasMinimas, mensagemDeErro);
        }

        [Fact]
        public void DeveIniciarTurn()
        {
            var quantidadeJogadores = 3;
            var apostas = new int[] { 2, 1, 0 };
            var quantidadeCartasMesaPosTurn = 4;
            var indice = 0;
            var mesaGerada = MesaBuilder.Novo().JogadoresPorMesa(quantidadeJogadores).DeveIniciarPartida().DeveIniciarMao().ObterPrimeiraMesa();

            while (mesaGerada.TentarIndicarProximoJogador())
            {
                mesaGerada.ReceberAposta(apostas[indice]);
                indice++;
            }

            mesaGerada.Flop();
            var retorno = mesaGerada.Turn();

            Assert.True(retorno);
            Assert.Equal(quantidadeCartasMesaPosTurn, mesaGerada.Cartas.Count);
        }

        [Fact]
        public void DeveIniciarRiver()
        {
            var quantidadeJogadores = 3;
            var apostas = new int[] { 2, 1, 0 };
            var quantidadeCartasMesaPosTurn = 5;
            var indice = 0;
            var mesaGerada = MesaBuilder.Novo().JogadoresPorMesa(quantidadeJogadores).DeveIniciarPartida().DeveIniciarMao().ObterPrimeiraMesa();

            while (mesaGerada.TentarIndicarProximoJogador())
            {
                mesaGerada.ReceberAposta(apostas[indice]);
                indice++;
            }

            mesaGerada.Flop();
            mesaGerada.Turn();
            var retorno = mesaGerada.River();

            Assert.True(retorno);
            Assert.Equal(quantidadeCartasMesaPosTurn, mesaGerada.Cartas.Count);
        }
    }

    public class Mesa
    {
        public int Id { get; private set; }
        public int Pote { get; private set; }
        public List<Jogador> Jogadores { get; private set; }
        public List<Jogador> JogadoresAtivos
        {
            get
            {
                return Jogadores.Where(s => s.Status != StatusJogador.Eliminado && s.Status != StatusJogador.Fold).ToList();
            }
        }
        public Jogador JogadorAtual { get; private set; }
        public List<Carta> Cartas { get; private set; }
        public int IdJogadorSmallBlind { get; private set; }
        public int IdJogadorUTG { get; private set; }
        public StatusMesa Status { get; private set; }
        public Baralho Baralho { get; private set; }
        public int ValorBlind { get; private set; }
        public int ApostaAtual { get; private set; }

        private readonly int _quantidadeMinimaDeJogadoresPermitidos = 2;
        private readonly int _quantidadeMaximaDeJogadoresPermitidos = 9;

        /// <summary>
        /// Inicia uma instância de mesa.
        /// </summary>
        /// <param name="id">Identificador da mesa</param>
        public Mesa(int id)
        {
            if (id <= 0)
                throw new Exception(Ressource.MesaMsgIdInvalido);

            Id = id;
            Pote = 0;
            Jogadores = new List<Jogador>();
            Cartas = new List<Carta>();
            Status = StatusMesa.Aguardando;
        }

        /// <summary>
        /// Altera o status da mesa
        /// </summary>
        /// <param name="status">Status para qual será mudado</param>
        public void AlterarStatus(StatusMesa status)
        {
            if (status == StatusMesa.Aguardando && Status == StatusMesa.Ativa)
                throw new Exception(Ressource.MesaMsgNaoPermitidoMudarStatusDeAtivaParaAguardando);

            if (status == StatusMesa.Aguardando && Status == StatusMesa.Finalizada)
                throw new Exception(Ressource.MesaMsgNaoPermitidoMudarStatusDeFinalizadaParaAguardando);

            if (status == StatusMesa.Ativa && Status == StatusMesa.Finalizada)
                throw new Exception(Ressource.MesaMsgNaoPermitidoMudarStatusDeFinalizadaParaAtiva);

            Status = status;
        }

        /// <summary>
        /// Inclui um novo jogador se houver espaço na mesa. Cada mesa pode ter no máximo nove jogadores
        /// </summary>
        /// <param name="nomeJogadorEsperado">Nome do jogador que será incluído</param>
        /// <returns>Retorna true caso o jogador tenha sido incluído com sucesso. Caso contrário, false  </returns>
        public bool IncluirJogador(string nomeJogador)
        {
            if (Jogadores.Count < _quantidadeMaximaDeJogadoresPermitidos && Status == StatusMesa.Aguardando)
            {
                int idNovoJogador = (Jogadores.OrderByDescending(j => j.Id).FirstOrDefault()?.Id ?? 0) + 1;
                Jogadores.Add(new Jogador(idNovoJogador, nomeJogador));
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// Inicia a partida caso esteja com status aguardando e contenha a quantidade mínima de jogadores
        /// </summary>
        public void IniciarPartida()
        {
            if (Jogadores.Count < _quantidadeMinimaDeJogadoresPermitidos)
                throw new Exception(Ressource.MesaMsgNaoPermitidoIniciarPartidaSemQuantidadeMinimaDeJogadores);

            if (Status == StatusMesa.Aguardando)
            {
                AlterarStatus(StatusMesa.Ativa);
                ValorBlind = Ressource.MesaValorInicialBlind;
            }
        }

        /// <summary>
        /// Inicia rodada enquanto há a quantidade mínima de jogadores ativos na mesa
        /// </summary>
        public string IniciarMao()
        {
            if (JogadoresAtivos.Count() < _quantidadeMinimaDeJogadoresPermitidos)
                throw new Exception(Ressource.MesaMsgNaoPermitidoIniciarRodadaSemQuantidadeMinimaDeJogadores);

            Baralho = new Baralho();

            for (int i = 0; i < 2; i++)
            {
                foreach (var jogador in JogadoresAtivos)
                {
                    jogador.ReceberCarta(Baralho.DistribuirCarta());
                }
            }

            if (IdJogadorSmallBlind != 0)
            {
                OrdenarJogadores(IdJogadorSmallBlind);
                Jogadores.MoveFirstItemToFinal<Jogador>();
            }

            IdJogadorSmallBlind = JogadoresAtivos.First().Id;

            var mensagem = "";

            //Receber Small Blind
            TentarIndicarProximoJogador();
            mensagem = ReceberAposta(ValorBlind / 2, "small");

            //Receber Big Blind
            TentarIndicarProximoJogador();
            mensagem += $"\r\n{ReceberAposta(ValorBlind, "big")}";

            ApostaAtual = ValorBlind;

            // Se houver mais do que dois jogadores pega a terceira posição da lista, caso contrário a segunda posição
            IdJogadorUTG = JogadoresAtivos[JogadoresAtivos.Count > 2 ? 2 : 0].Id;

            // Posiciona o UTG como primeiro da lista
            OrdenarJogadores(IdJogadorUTG);
            JogadorAtual = null;

            return mensagem;
        }

        /// <summary>
        /// Verifica se há algum jogador ainda para jogar na rodada. 
        /// Caso haja jogador pendente de ação, a propriedade JogadorAtual será preenchida com o objeto deste jogador, caso contrário, a propriedade será preenchida com null. 
        /// </summary>
        /// <returns>Retorna true enquanto há jogadores pendentes, caso contrário false.</returns>
        public bool TentarIndicarProximoJogador()
        {
            if (JogadorAtual == null)
            {
                JogadorAtual = JogadoresAtivos[0];
                return true;
            }
            else
            {
                var indiceProximoJogador = JogadoresAtivos.FindIndex(j => j.Id == JogadorAtual.Id) + 1;

                if (indiceProximoJogador < JogadoresAtivos.Count())
                {
                    JogadorAtual = JogadoresAtivos[indiceProximoJogador];
                    return true;
                }
                else
                {
                    JogadorAtual = null;
                    return false;
                }
            }
        }

        /// <summary>
        /// Receber aposta do jogador e integrar ao Pote da Mesa
        /// </summary>
        /// <param name="fichas">Quantidade de fichas apostadas</param>
        /// <param name="blind">Indica se a aposta é proveniente do blind</param>
        public string ReceberAposta(int fichas, string blind = null)
        {
            if (fichas < 0)
                throw new Exception(Ressource.MesaMsgNaoPermitidoApostaComValorNegativo);

            var aposta = JogadorAtual.FichasApostadasNaRodada + fichas;
            fichas = JogadorAtual.Apostar(fichas);
            Pote += fichas;

            var mensagem = "";

            if (fichas == 0)
            {
                if (aposta < ApostaAtual)
                {
                    JogadorAtual.Fold();
                    mensagem = Mensagem.Gerar(Ressource.MesaAcaoFold, JogadorAtual.Nome);
                }
                else
                    mensagem = Mensagem.Gerar(Ressource.MesaAcaoCheck, JogadorAtual.Nome);
            }
            else
            {
                if (!string.IsNullOrEmpty(blind))
                    mensagem = Mensagem.Gerar(Ressource.MesaAcaoBlind, JogadorAtual.Nome, blind, fichas.ToString());
                else if (aposta > ApostaAtual)
                {
                    ApostaAtual = aposta;
                    OrdenarJogadores(JogadorAtual.Id);
                    mensagem = Mensagem.Gerar(Ressource.MesaAcaoRaise, JogadorAtual.Nome, fichas.ToString());
                }
                else
                    mensagem = Mensagem.Gerar(Ressource.MesaAcaoPagar, JogadorAtual.Nome, fichas.ToString());
            }


            return mensagem;
        }

        /// <summary>
        /// Abre o flop e reordena a lista de jogadores
        /// </summary>
        /// <returns>Retorna true caso haja jogadores suficientes para realizar o flop. Caso contrário, false</returns>
        public bool Flop()
        {
            return RenovarRodadaDeApostas(3);
        }

        /// <summary>
        /// Abre o turn e reordena a lista de jogadores
        /// </summary>
        /// <returns>Retorna true caso haja jogadores suficientes para realizar o turn. Caso contrário, false</returns>
        public bool Turn()
        {
            return RenovarRodadaDeApostas(1);
        }

        /// <summary>
        /// Abre o river e reordena a lista de jogadores
        /// </summary>
        /// <returns>Retorna true caso haja jogadores suficientes para realizar o river. Caso contrário, false</returns>
        public bool River()
        {
            return RenovarRodadaDeApostas(1);
        }

        /// <summary>
        /// Abre cartas comunitárias e reordena a lista de jogadores
        /// </summary>
        /// <param name="quantidadeCartasComunitarias">Quantidade de cartas comunitárias abertas na mesa</param>
        /// <returns>Retorna true caso haja jogadores suficientes para realizar a rodada. Caso contrário, false</returns>
        private bool RenovarRodadaDeApostas(int quantidadeCartasComunitarias)
        {
            if (JogadoresAtivos.Count < _quantidadeMinimaDeJogadoresPermitidos)
                return false;

            foreach (var jogador in JogadoresAtivos)
            {
                if (jogador.FichasApostadasNaRodada < ApostaAtual)
                    throw new Exception(Ressource.MesaMsgNaoPermitidoIniciarFlopSemApostasMinimas);
                else
                    jogador.ZerarFichasApostadasNaRodada();
            }

            OrdenarJogadores(IdJogadorSmallBlind);

            //Queimar Carta
            Baralho.DistribuirCarta();

            //Montar Flop
            for (int i = 0; i < quantidadeCartasComunitarias; i++)
            {
                Cartas.Add(Baralho.DistribuirCarta());
            }

            ApostaAtual = 0;

            return true;
        }

        private void OrdenarJogadores(int idJogador)
        {
            while (Jogadores.First().Id != idJogador)
            {
                Jogadores.MoveFirstItemToFinal<Jogador>();
            }
        }

        /// <summary>
        /// Altera o Id do jogador na posição de Small Blind
        /// </summary>
        /// <param name="idJogadorSmallBlind">id do jogador na posição de small blind</param>
        internal void AlterarIdJogadorSmallBlind(int idJogadorSmallBlind)
        {
            IdJogadorSmallBlind = idJogadorSmallBlind;
        }
    }
}