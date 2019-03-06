using ExpectedObjects;
using Poker.TexasHoldem.Lib;
using Poker.TexasHoldem.Lib._Base;
using Poker.TexasHoldem.Lib._Enum;
using System;
using System.Collections.Generic;
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

        [Theory(DisplayName ="PermitirCreditoDeFichas")]
        [InlineData(200, 1200)]
        [InlineData(500, 1500)]
        [InlineData(1000, 2000)]
        public void PermitirCreditoDeFichas(int fichas, int totalDeFichasEsperado)
        {
            _jogadorDefault.AdicionarFichas(fichas);

            Assert.Equal(totalDeFichasEsperado, _jogadorDefault.Fichas);
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
        /// Adiciona as fichas ao montante do jogador
        /// </summary>
        /// <param name="fichas"></param>
        public void AdicionarFichas(int fichas)
        {
            Fichas += fichas;
        }

        public void TrocarStatus(StatusJogador statusJogador)
        {
            if (Status != StatusJogador.Eliminado)
                Status = statusJogador;
        }
    }
}
