using ExpectedObjects;
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

            Assert.Equal(totalDeFichasApostadasEsperado, _jogadorDefault.FichasApostadas);
        }

        [Theory(DisplayName = "PermitirTrocarDeStatus")]
        [InlineData(StatusJogador.Ativo)]
        [InlineData(StatusJogador.EmAcao)]
        [InlineData(StatusJogador.Eliminado)]
        public void PermitirTrocarDeStatus(StatusJogador statusJogadorEsperado)
        {
            _jogadorDefault.TrocarStatus(statusJogadorEsperado);

            Assert.Equal(statusJogadorEsperado, _jogadorDefault.Status);
        }

        [Theory(DisplayName = "NaoTrocarStatusSeJogadorEstiverEliminado")]
        [InlineData(StatusJogador.Ativo)]
        [InlineData(StatusJogador.EmAcao)]
        [InlineData(StatusJogador.AllIn)]
        public void NaoTrocarStatusSeJogadorEstiverEliminado(StatusJogador novoStatusJogador)
        {
            var statusJogadorEsperado = StatusJogador.Eliminado;
            _jogadorDefault.TrocarStatus(StatusJogador.Eliminado);
            _jogadorDefault.TrocarStatus(novoStatusJogador);

            Assert.Equal(statusJogadorEsperado, _jogadorDefault.Status);
        }

        [Fact]
        public void DeveIniciarJogada()
        {
            var maoBuilder = new MaoBuilder("A;P|K;O");
            var quantidadeDeCartasEsperada = 2;

            _jogadorDefault.IniciarRodada(maoBuilder.CartasJogador[0], maoBuilder.CartasJogador[1]);

            Assert.True(_jogadorDefault.Mao.Cartas.Any());
            Assert.Equal(quantidadeDeCartasEsperada, _jogadorDefault.Mao.Cartas.Count());
            Assert.Equal(maoBuilder.CartasJogador[0].Id, _jogadorDefault.Mao.Cartas[0].Id);
            Assert.Equal(maoBuilder.CartasJogador[1].Id, _jogadorDefault.Mao.Cartas[1].Id);
        }

        [Fact]
        public void DeveEncerrarJogada()
        {
            var maoBuilder = new MaoBuilder("A;P|K;O");
            var fichasGanhas = 500;
            var fichasTotalEsperadaComJogador = 1500;

            _jogadorDefault.IniciarRodada(maoBuilder.CartasJogador[0], maoBuilder.CartasJogador[1]);
            _jogadorDefault.EncerrarRodada(fichasGanhas);

            Assert.Equal(fichasTotalEsperadaComJogador, _jogadorDefault.Fichas);
        }

        [Fact]
        public void DeveLimparAMaoDoJogadorAposEncerrarAJogada()
        {
            var maoBuilder = new MaoBuilder("A;P|K;O");
            var fichasGanhas = 500;

            _jogadorDefault.IniciarRodada(maoBuilder.CartasJogador[0], maoBuilder.CartasJogador[1]);
            _jogadorDefault.EncerrarRodada(fichasGanhas);

            Assert.Null(_jogadorDefault.Mao);
        }

        [Fact]
        public void DeveEliminarJogadorCasoStatusForAllInENaoTenhaRecebidoNenhumaFicha()
        {
            var maoBuilder = new MaoBuilder("A;P|K;O");
            var fichasApostadas = 1000;
            var fichasGanhas = 0;
            var statusEsperado = StatusJogador.Eliminado;

            _jogadorDefault.IniciarRodada(maoBuilder.CartasJogador[0], maoBuilder.CartasJogador[1]);
            _jogadorDefault.Apostar(fichasApostadas);
            _jogadorDefault.EncerrarRodada(fichasGanhas);

            Assert.Equal(statusEsperado, _jogadorDefault.Status);
        }

        [Fact]
        public void DeveAlterarStatusDoJogadorParaAtivoCasoStatusForAllInETenhaRecebidoFicha()
        {
            var maoBuilder = new MaoBuilder("A;P|K;O");
            var fichasApostadas = 1000;
            var fichasGanhas = 2000;
            var statusEsperado = StatusJogador.Ativo;

            _jogadorDefault.IniciarRodada(maoBuilder.CartasJogador[0], maoBuilder.CartasJogador[1]);
            _jogadorDefault.Apostar(fichasApostadas);
            _jogadorDefault.EncerrarRodada(fichasGanhas);

            Assert.Equal(statusEsperado, _jogadorDefault.Status);
        }

        [Fact]
        public void NaoDevePermitirEncerrarJogadaComFichasNegativas()
        {
            var maoBuilder = new MaoBuilder("A;P|K;O");
            var fichasApostadas = 1000;
            var fichasGanhas = -1000;

            _jogadorDefault.IniciarRodada(maoBuilder.CartasJogador[0], maoBuilder.CartasJogador[1]);
            _jogadorDefault.Apostar(fichasApostadas);

            var MensagemDeErro = Assert.Throws<Exception>(() => _jogadorDefault.EncerrarRodada(fichasGanhas)).Message;
            Assert.Equal(Ressource.JogadorMsgValorFichasGanhasInvalido, MensagemDeErro);
        }

        [Fact]
        public void DeveZerarMontanteDeFichasApostadasAoFinalDaRodada()
        {
            var maoBuilder = new MaoBuilder("A;P|K;O");
            var fichasApostadas = 1000;
            var fichasGanhas = 2000;
            var montanteFichasApostadasEsperado = 0;

            _jogadorDefault.IniciarRodada(maoBuilder.CartasJogador[0], maoBuilder.CartasJogador[1]);
            _jogadorDefault.Apostar(fichasApostadas);
            _jogadorDefault.EncerrarRodada(fichasGanhas);

            Assert.Equal(montanteFichasApostadasEsperado, _jogadorDefault.FichasApostadas);
        }

        [Fact]
        public void NaoDeveDarCartasAJogadorEliminado()
        {
            var maoBuilder = new MaoBuilder("A;P|K;O");
            _jogadorDefault.TrocarStatus(StatusJogador.Eliminado);

            _jogadorDefault.IniciarRodada(maoBuilder.CartasJogador[0], maoBuilder.CartasJogador[1]);

            Assert.Null(_jogadorDefault.Mao);
        }
    }

    public class Jogador
    {
        public int Id { get; private set; }
        public string Nome { get; private set; }
        public int Fichas { get; private set; }
        public int FichasApostadas { get; private set; }
        public StatusJogador Status { get; private set; }
        public Mao Mao { get; private set; }


        /// <summary>
        /// Inicia uma instância de jogador.
        /// </summary>
        /// <param name="id">Id do jogador</param>
        /// <param name="nome">Nome do jogador</param>
        public Jogador(int id, string nome)
        {
            if (id <= 0)
                throw new Exception(Ressource.JogadorMsgIdInvalido);

            if (string.IsNullOrEmpty(nome?.Trim()))
                throw new Exception(Ressource.JogadorMsgNomeObrigatorio);

            if (nome.Length > 20)
                throw new Exception(Ressource.JogadorMsgNomeSuperior20Caracteres);

            Id = id;
            Nome = nome;
            Fichas = Ressource.JogadorFichasInicial;
            Status = Ressource.JogadorStatusInicial;
        }

        /// <summary>
        /// Inicia a rodada do jogador, caso o jogador não esteja eliminado
        /// </summary>
        /// <param name="carta1">Primeira carta do jogador</param>
        /// <param name="carta2">Segunda carta do jogador</param>
        public void IniciarRodada(Carta carta1, Carta carta2)
        {
            if (Status != StatusJogador.Eliminado)
            {
                if (Status == StatusJogador.Esperando)
                    Status = StatusJogador.Ativo;

                Mao = new Mao(carta1, carta2);
            }
            else
                Mao = null;
        }

        /// <summary>
        /// Debita as fichas do jogador e retorna o valor. 
        /// </summary>
        /// <param name="fichasAposta">Fichas apostadas</param>
        /// <returns>Quantidade de fichas apostadas. Caso o valor apostado seja superior a quantidade máxima, retorna apenas a quantidade máxima e 
        /// troca o status do jogador para AllIn </returns>
        public int Apostar(int fichasAposta)
        {
            if (Fichas <= fichasAposta)
            {
                fichasAposta = Fichas;
                Fichas = 0;
                Status = StatusJogador.AllIn;
            }
            else
            {
                Fichas -= fichasAposta;
            }

            FichasApostadas += fichasAposta;
            return fichasAposta;
        }

        /// <summary>
        /// Troca o status do jogador, exceto quando este for igual a Eliminado
        /// </summary>
        /// <param name="statusJogador">Novo status do jogador</param>
        public void TrocarStatus(StatusJogador statusJogador)
        {
            if (Status != StatusJogador.Eliminado)
                Status = statusJogador;
        }

        /// <summary>
        /// Encerra a jogada e atribui as fichas ganhas ao montande do jogador.
        /// </summary>
        /// <param name="fichasGanhas">Fichas ganhas na rodada atual</param>
        internal void EncerrarRodada(int fichasGanhas)
        {
            if (fichasGanhas < 0)
                throw new Exception(Ressource.JogadorMsgValorFichasGanhasInvalido);

            Fichas += fichasGanhas;
            FichasApostadas = 0;
            Mao = null;

            if (Status == StatusJogador.AllIn)
                Status = fichasGanhas != 0 ? StatusJogador.Ativo : StatusJogador.Eliminado;
        }
    }
}
