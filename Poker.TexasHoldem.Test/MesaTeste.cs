using ExpectedObjects;
using Poker.TexasHoldem.Lib;
using Poker.TexasHoldem.Lib._Base;
using Poker.TexasHoldem.Lib._Enum;
using Poker.TexasHoldem.Lib._Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Poker.TexasHoldem.Test
{
    public class MesaTeste
    {
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
                IdJogadorDealer = 1,
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
        public void DeveIncluirNovoJogador()
        {
            var idMesaGerada = 1;
            var idJogadorEsperado = 1;
            var nomeJogadorEsperado = "Alexandre";
            var novoJogadorEsperado = new Jogador(idJogadorEsperado, nomeJogadorEsperado);
            var mesaGerada = new Mesa(idMesaGerada);

            bool jogadorIncluidoComSucesso = mesaGerada.IncluirJogador(nomeJogadorEsperado);

            Assert.True(jogadorIncluidoComSucesso);
        }

        [Fact]
        public void DeveIniciarAMesa()
        {

        }
    }

    public class Mesa
    {
        public int Id { get; private set; }
        public int Pote { get; private set; }
        public List<Jogador> Jogadores { get; private set; }
        public List<Carta> Cartas { get; private set; }
        public int IdJogadorDealer { get; private set; }
        public StatusMesa Status { get; private set; }

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
            IdJogadorDealer = 1;
            Status = StatusMesa.Aguardando;
        }

        /// <summary>
        /// Inclui um novo jogador se houver espaço na mesa. Cada mesa pode ter no máximo nove jogadores
        /// </summary>
        /// <param name="nomeJogadorEsperado">Nome do jogador que será incluído</param>
        /// <returns>Retorna true caso o jogador tenha sido incluído com sucesso. Caso contrário, false  </returns>
        public bool IncluirJogador(string nomeJogador)
        {
            int idNovoJogador = (Jogadores.OrderByDescending(j => j.Id).FirstOrDefault()?.Id ?? 0) + 1;
            Jogadores.Add(new Jogador(idNovoJogador, nomeJogador));
            return true;
        }
    }
}