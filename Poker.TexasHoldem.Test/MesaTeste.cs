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

        public MesaTeste()
        {

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
            var mesaGerada = new MesaBuilder(_quantidadeMaximaDeJogadoresPermitidos).Mesas.FirstOrDefault();

            Assert.False(mesaGerada.IncluirJogador(_nomeJogadorDefault));
        }

        [Theory(DisplayName = "NaoDeveIncluirNovoJogadorSeOStatusDaMesaForDiferenteDeAguardando")]
        [InlineData(StatusMesa.Ativa)]
        [InlineData(StatusMesa.Finalizada)]
        public void NaoDeveIncluirNovoJogadorSeOStatusDaMesaForDiferenteDeAguardando(StatusMesa statusAtualDaMesa)
        {
            var mesaGerada = new MesaBuilder(_quantidadeMinimaDeJogadoresPermitidos).Mesas.FirstOrDefault();
            mesaGerada.AlterarStatus(statusAtualDaMesa);

            Assert.False(mesaGerada.IncluirJogador(_nomeJogadorDefault));
        }

        [Fact]
        public void DeveIniciarAMesa()
        {
            var statusEsperado = StatusMesa.Ativa;
            var mesaGerada = new MesaBuilder(_quantidadeMinimaDeJogadoresPermitidos).Mesas.FirstOrDefault();

            mesaGerada.IniciarPartida();

            Assert.Equal(statusEsperado, mesaGerada.Status);
        }

        [Fact]
        public void NaoDeveIniciarMesaCasoHajaMenosJogadoresDoQueOPermitido()
        {
            var mesaGerada = new MesaBuilder(_quantidadeMinimaDeJogadoresPermitidos - 1).Mesas.FirstOrDefault();

            var MensagemDeErro = Assert.Throws<Exception>(() => mesaGerada.IniciarPartida()).Message;
            Assert.Equal(Ressource.MesaMsgNaoPermitidoIniciarPartidaSemQuantidadeMinimaDeJogadores, MensagemDeErro);
        }

        [Theory(DisplayName = "NaoDeveIniciarAMesaCasoEstaJaEstejaAtiva")]
        [InlineData(StatusMesa.Ativa)]
        [InlineData(StatusMesa.Finalizada)]
        public void NaoDeveIniciarAMesaCasoEstaJaEstejaAtiva(StatusMesa statusAtualDaMesa)
        {
            var mesaGerada = new MesaBuilder(_quantidadeMinimaDeJogadoresPermitidos).Mesas.FirstOrDefault();
            mesaGerada.AlterarStatus(statusAtualDaMesa);

            mesaGerada.IniciarPartida();

            Assert.Equal(statusAtualDaMesa, mesaGerada.Status);
        }

        [Fact]
        public void DeveIniciarJogadaDistribuindoCartasAosJogadoresEOFlop()
        {
            var mesaGerada = new MesaBuilder(_quantidadeMinimaDeJogadoresPermitidos).Mesas.FirstOrDefault();
            var quantidadeCartasDoJogador = 2;
            var quantidadeCartasMesaPosFlop = 3;

            mesaGerada.IniciarPartida();
            mesaGerada.IniciarRodada();

            foreach (var jogador in mesaGerada.Jogadores)
            {
                Assert.Equal(quantidadeCartasDoJogador, jogador.Mao.Cartas.Count);
            }

            Assert.Equal(quantidadeCartasMesaPosFlop, mesaGerada.Cartas.Count);
        }

        [Fact]
        public void DeveIniciarJogadaDistribuindoCartasApenasAosJogadoresAtivos()
        {
            var mesaGerada = new MesaBuilder(4).Mesas.FirstOrDefault();
            var quantidadeCartasDoJogadorAtivo = 2;

            mesaGerada.Jogadores[2].TrocarStatus(StatusJogador.Eliminado);
            mesaGerada.Jogadores[3].TrocarStatus(StatusJogador.Eliminado);
            mesaGerada.IniciarPartida();
            mesaGerada.IniciarRodada();

            foreach (var jogadorAtivo in mesaGerada.JogadoresAtivos)
            {
                Assert.Equal(quantidadeCartasDoJogadorAtivo, jogadorAtivo.Mao.Cartas.Count);
            }

            foreach (var jogadorInativo in mesaGerada.Jogadores.Where(j=>j.Status == StatusJogador.Eliminado).ToList())
            {
                Assert.Null(jogadorInativo.Mao);
            }
        }

        [Fact]
        public void NaoDeveIniciarJogadaCasoANaoHajaAQuantidadeMinimaDeJogadoresAtivos()
        {
            var mesaGerada = new MesaBuilder(_quantidadeMinimaDeJogadoresPermitidos).Mesas.FirstOrDefault();

            mesaGerada.IniciarPartida();
            mesaGerada.Jogadores[1].TrocarStatus(StatusJogador.Eliminado);

            var mensagemDeErro = Assert.Throws<Exception>(() => mesaGerada.IniciarRodada()).Message;
            Assert.Equal(Ressource.MesaMsgNaoPermitidoIniciarRodadaSemQuantidadeMinimaDeJogadores, mensagemDeErro);
        }
    }

    public class Mesa
    {
        public int Id { get; private set; }
        public int Pote { get; private set; }
        public List<Jogador> Jogadores { get; private set; }
        public List<Jogador> JogadoresAtivos { get
            {
                return Jogadores.Where(s => s.Status != StatusJogador.Eliminado).ToList();
            }
        }
        public List<Carta> Cartas { get; private set; }
        public int IdJogadorDealer { get; private set; }
        public StatusMesa Status { get; private set; }
        public Baralho Baralho { get; private set; }

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
                IdJogadorDealer = Jogadores.OrderByDescending(j=> j.Id).First().Id;
            }
        }

        /// <summary>
        /// Inicia rodada enquanto há a quantidade mínima de jogadores ativos na mesa
        /// </summary>
        public void IniciarRodada()
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

            Cartas.Add(Baralho.DistribuirCarta());
            Cartas.Add(Baralho.DistribuirCarta());
            Cartas.Add(Baralho.DistribuirCarta());
        }

        private void OrdenarJogadores()
        {
            var jogadoresAtivos = Jogadores.Where(s => s.Status != StatusJogador.Eliminado);
            var ordemJogadores = new List<int>();
            var idJogador = IdJogadorDealer;

            do
            {
                ordemJogadores.Add(idJogador);

                if (jogadoresAtivos.Count() == ordemJogadores.Count())
                    break;

                idJogador++;
                while (!jogadoresAtivos.Where(j => j.Id == idJogador).Any())
                {
                    idJogador++;
                    if (idJogador > _quantidadeMaximaDeJogadoresPermitidos)
                        idJogador = 1;
                }

            } while (IdJogadorDealer != idJogador);
        }
    }
}