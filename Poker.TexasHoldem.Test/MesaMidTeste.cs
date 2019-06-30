using Poker.TexasHoldem.Lib._Base;
using Poker.TexasHoldem.Lib._Enum;
using Poker.TexasHoldem.Lib._Util;
using Poker.TexasHoldem.Test._Builder;
using System;
using System.Linq;
using Xunit;

namespace Poker.TexasHoldem.Test
{
    public class MesaMidTeste
    {
        private readonly int _quantidadeMinimaDeJogadoresPermitidos = 2;
        private readonly int _quantidadeMaximaDeJogadoresPermitidos = 9;
        private readonly int _idMesaDefault = 1;
        private readonly string _nomeJogadorDefault = "Alexandre";
        private int _valorSmallBlindEsperado;
        private int _valorBigBlindEsperado;

        public MesaMidTeste()
        {
            _valorSmallBlindEsperado = Ressource.MesaValorInicialBlind / 2;
            _valorBigBlindEsperado = Ressource.MesaValorInicialBlind;
        }

        [Fact]
        public void DeveIniciarMao()
        {
            var mesaGerada = MesaBuilder.Novo().JogadoresPorMesa(_quantidadeMinimaDeJogadoresPermitidos).DeveIniciarPartida().ObterPrimeiraMesa();

            mesaGerada.IniciarMao();

            Assert.True(mesaGerada.PreFlopExecutado);
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
        public void NaoDeveIniciarMaoCasoNaoHajaAQuantidadeMinimaDeJogadoresAtivos()
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
            Assert.True(mesaGerada.FlopExecutado);
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
            Assert.Equal(Ressource.MesaMsgNaoPermitidoIniciarNovaRodadaSemApostasMinimas, mensagemDeErro);
        }

        [Fact]
        public void NaoDeveExecutarFlopAntesDoPreFlop()
        {
            var mesaGerada = MesaBuilder.Novo().JogadoresPorMesa(_quantidadeMinimaDeJogadoresPermitidos).DeveIniciarPartida().ObterPrimeiraMesa();

            var mensagemDeErro = Assert.Throws<Exception>(() => mesaGerada.Flop()).Message;
            Assert.Equal(Ressource.MesaMsgFlopDeveSerExecutadoAposPreFlop, mensagemDeErro);
        }

        [Fact]
        public void NaoDevePermitirQueOFlopSejaExecutadoMaisDeUmaVezNaRodada()
        {
            var mesaGerada = MesaBuilder.Novo().JogadoresPorMesa(_quantidadeMinimaDeJogadoresPermitidos).DeveIniciarPartida().DeveIniciarMao().DeveExecutarFlop().RealizarApostasAposPreFlop().ObterPrimeiraMesa();

            var mensagemDeErro = Assert.Throws<Exception>(() => mesaGerada.Flop()).Message;
            Assert.Equal(Ressource.MesaMsgFlopNaoDeveSerExecutadoMaisDeUmaVez, mensagemDeErro);
        }

        [Theory(DisplayName = "NaoDeveIniciarFlopCasoAlgumaJogadaIndevidaJaTenhaSidoAcionada")]
        [InlineData("turn")]
        [InlineData("river")]
        public void NaoDeveIniciarFlopCasoAlgumaJogadaIndevidaJaTenhaSidoAcionada(string jogada)
        {
            var mesaGerada = MesaBuilder.Novo().JogadoresPorMesa(_quantidadeMinimaDeJogadoresPermitidos).DeveIniciarPartida().DeveIniciarMao().AlterarStatusJogadaMesa(jogada, true).ObterPrimeiraMesa();

            var mensagemDeErro = Assert.Throws<Exception>(() => mesaGerada.Flop()).Message;
            Assert.Equal(Ressource.MesaMsgFlopDeveSerExecutadoAntesDoTurnERiver, mensagemDeErro);
        }

        [Fact]
        public void DeveIniciarTurn()
        {
            var quantidadeJogadores = 3;
            var quantidadeCartasMesaPosTurn = 4;
            var mesaGerada = MesaBuilder.Novo().JogadoresPorMesa(quantidadeJogadores).DeveIniciarPartida().DeveIniciarMao().RealizarApostasAposPreFlop().DeveExecutarFlop().ObterPrimeiraMesa();

            var retorno = mesaGerada.Turn();

            Assert.True(retorno);
            Assert.True(mesaGerada.TurnExecutado);
            Assert.Equal(quantidadeCartasMesaPosTurn, mesaGerada.Cartas.Count);
        }

        [Fact]
        public void NaoDeveExecutarTurnAntesDoPreFlopElop()
        {
            var mesaGerada = MesaBuilder.Novo().JogadoresPorMesa(_quantidadeMinimaDeJogadoresPermitidos).DeveIniciarPartida().ObterPrimeiraMesa();

            var mensagemDeErro = Assert.Throws<Exception>(() => mesaGerada.Turn()).Message;
            Assert.Equal(Ressource.MesaMsgTurnDeveSerExecutadoAposPreFlopEFlop, mensagemDeErro);
        }

        [Fact]
        public void NaoDevePermitirQueOTurnSejaExecutadoMaisDeUmaVezNaRodada()
        {
            var mesaGerada = MesaBuilder.Novo().JogadoresPorMesa(_quantidadeMinimaDeJogadoresPermitidos).DeveIniciarPartida().DeveIniciarMao().RealizarApostasAposPreFlop().DeveExecutarFlop().DeveExecutarTurn().ObterPrimeiraMesa();

            var mensagemDeErro = Assert.Throws<Exception>(() => mesaGerada.Turn()).Message;
            Assert.Equal(Ressource.MesaMsgTurnNaoDeveSerExecutadoMaisDeUmaVez, mensagemDeErro);
        }

        [Fact]
        public void NaoDeveIniciarTurnCasoORiverJaTenhaSidoAcionado()
        {
            var mesaGerada = MesaBuilder.Novo().JogadoresPorMesa(_quantidadeMinimaDeJogadoresPermitidos).DeveIniciarPartida().DeveIniciarMao().RealizarApostasAposPreFlop().DeveExecutarFlop().AlterarStatusJogadaMesa("river", true).ObterPrimeiraMesa();

            var mensagemDeErro = Assert.Throws<Exception>(() => mesaGerada.Turn()).Message;
            Assert.Equal(Ressource.MesaMsgTurnDeveSerExecutadoAntesDoRiver, mensagemDeErro);
        }

        [Fact]
        public void DeveIniciarRiver()
        {
            var quantidadeJogadores = 3;
            var quantidadeCartasMesaPosTurn = 5;
            var mesaGerada = MesaBuilder.Novo().JogadoresPorMesa(quantidadeJogadores).DeveIniciarPartida().DeveIniciarMao().RealizarApostasAposPreFlop().DeveExecutarFlop().DeveExecutarTurn().ObterPrimeiraMesa();

            var retorno = mesaGerada.River();

            Assert.True(retorno);
            Assert.True(mesaGerada.RiverExecutado);
            Assert.Equal(quantidadeCartasMesaPosTurn, mesaGerada.Cartas.Count);
        }

        [Fact]
        public void NaoDeveExecutarRiverPrimeiroQueDemaisJogadas()
        {
            var mesaGerada = MesaBuilder.Novo().JogadoresPorMesa(_quantidadeMinimaDeJogadoresPermitidos).DeveIniciarPartida().ObterPrimeiraMesa();

            var mensagemDeErro = Assert.Throws<Exception>(() => mesaGerada.River()).Message;
            Assert.Equal(Ressource.MesaMsgRiverDeveSerExecutadoAposDemaisRodadas, mensagemDeErro);
        }

        [Fact]
        public void NaoDevePermitirQueORiverSejaExecutadoMaisDeUmaVezNaRodada()
        {
            var mesaGerada = MesaBuilder.Novo().JogadoresPorMesa(_quantidadeMinimaDeJogadoresPermitidos).DeveIniciarPartida().DeveIniciarMao().RealizarApostasAposPreFlop().DeveExecutarFlop().DeveExecutarTurn().DeveExecutarRiver().ObterPrimeiraMesa();

            var mensagemDeErro = Assert.Throws<Exception>(() => mesaGerada.River()).Message;
            Assert.Equal(Ressource.MesaMsgRiverNaoDeveSerExecutadoMaisDeUmaVez, mensagemDeErro);
        }
    }
}