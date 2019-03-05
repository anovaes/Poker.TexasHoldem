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
        [InlineData(1000, 0)]
        public void PermitirDebitarFichasDoJogador(int fichasRetiradas, int fichasEsperadas)
        {
            var jogadorGerado = new Jogador(_idJogadorDefault, _nomeJogadorDefault);

            jogadorGerado.DebitarFichas(fichasRetiradas);

            Assert.Equal(fichasEsperadas, jogadorGerado.Fichas);
        }

        [Theory(DisplayName = "PermitirDuasAcoesDeDebitarFichasDoJogador")]
        [InlineData(100, 100, 800)]
        [InlineData(250, 250, 500)]
        [InlineData(500, 500, 0)]
        public void PermitirDuasAcoesDeDebitarFichasDoJogador(int fichasRetiradasAcao1, int fichasRetiradasAcao2, int fichasEsperadas)
        {
            var jogadorGerado = new Jogador(_idJogadorDefault, _nomeJogadorDefault);

            jogadorGerado.DebitarFichas(fichasRetiradasAcao1);
            jogadorGerado.DebitarFichas(fichasRetiradasAcao2);

            Assert.Equal(fichasEsperadas, jogadorGerado.Fichas);
        }

        [Theory(DisplayName = "PermitirRetirarQuantidadeSuperiorAoMáximoDeFichasDoJogador")]
        [InlineData(1200, 0, 1000)]
        [InlineData(2000, 0, 1000)]
        [InlineData(3000, 0, 1000)]
        public void PermitirRetirarQuantidadeSuperiorAoMáximoDeFichasDoJogador(int fichasRetiradas, int fichasTotaisAposODebito, int fichasRetiradasEsperadas)
        {
            var jogadorGerado = new Jogador(_idJogadorDefault, _nomeJogadorDefault);

            var fichasRetiradasDoJogador = jogadorGerado.DebitarFichas(fichasRetiradas);

            Assert.Equal(fichasTotaisAposODebito, jogadorGerado.Fichas);
            Assert.Equal(fichasRetiradasEsperadas, fichasRetiradasDoJogador);
        }
    }

    public class Jogador
    {
        public int Id { get; private set; }
        public string Nome { get; private set; }
        public int Fichas { get; private set; }
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
        /// <param name="fichas">Fichas a serem debitadas</param>
        /// <returns>Quantidade de fichas debitadas. Caso o valor a ser debitado seja superior a quantidade máxima, retorna apenas a quantidade máxima </returns>
        public int DebitarFichas(int fichasDebito)
        {
            if (Fichas < fichasDebito)
            {
                fichasDebito = Fichas;
                Fichas = 0;
            }
            else
            {
                Fichas -= fichasDebito;
            }

            return fichasDebito;
        }
    }
}
