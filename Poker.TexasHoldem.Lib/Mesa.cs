using Poker.TexasHoldem.Lib._Base;
using Poker.TexasHoldem.Lib._Enum;
using Poker.TexasHoldem.Lib._Util;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Poker.TexasHoldem.Lib
{
    public class Mesa
    {
        public int Id { get; private set; }
        public int Pote { get; private set; } //Criar classe PoteGeral que irá controlar o pote de fichas da mesa
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
        public bool PreFlopExecutado { get; private set; }
        public bool FlopExecutado { get; private set; }
        public bool TurnExecutado { get; set; }
        public bool RiverExecutado { get; set; }


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
            if (PreFlopExecutado || FlopExecutado || TurnExecutado || RiverExecutado)
                throw new Exception(Ressource.MesaMsgPreFlopExecutadoAposOutraJogadaDeMesa);

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
            PreFlopExecutado = true;

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
                {
                    mensagem = Mensagem.Gerar(Ressource.MesaAcaoBlind, JogadorAtual.Nome, blind, fichas.ToString());
                }
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
            if (!PreFlopExecutado)
                throw new Exception(Ressource.MesaMsgFlopDeveSerExecutadoAposPreFlop);

            if (FlopExecutado)
                throw new Exception(Ressource.MesaMsgFlopNaoDeveSerExecutadoMaisDeUmaVez);

            if (TurnExecutado || RiverExecutado)
                throw new Exception(Ressource.MesaMsgFlopDeveSerExecutadoAntesDoTurnERiver);

            return FlopExecutado = RenovarRodadaDeApostas(3);
        }

        /// <summary>
        /// Abre o turn e reordena a lista de jogadores
        /// </summary>
        /// <returns>Retorna true caso haja jogadores suficientes para realizar o turn. Caso contrário, false</returns>
        public bool Turn()
        {
            if (!PreFlopExecutado || !FlopExecutado)
                throw new Exception(Ressource.MesaMsgTurnDeveSerExecutadoAposPreFlopEFlop);

            if (TurnExecutado)
                throw new Exception(Ressource.MesaMsgTurnNaoDeveSerExecutadoMaisDeUmaVez);

            if (RiverExecutado)
                throw new Exception(Ressource.MesaMsgTurnDeveSerExecutadoAntesDoRiver);

            return TurnExecutado = RenovarRodadaDeApostas(1);
        }

        /// <summary>
        /// Abre o river e reordena a lista de jogadores
        /// </summary>
        /// <returns>Retorna true caso haja jogadores suficientes para realizar o river. Caso contrário, false</returns>
        public bool River()
        {
            if (!PreFlopExecutado || !FlopExecutado || !TurnExecutado)
                throw new Exception(Ressource.MesaMsgRiverDeveSerExecutadoAposDemaisRodadas);

            if (RiverExecutado)
                throw new Exception(Ressource.MesaMsgRiverNaoDeveSerExecutadoMaisDeUmaVez);

            return RiverExecutado = RenovarRodadaDeApostas(1);
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
                    throw new Exception(Ressource.MesaMsgNaoPermitidoIniciarNovaRodadaSemApostasMinimas);
                else
                    jogador.ZerarFichasApostadasNaRodada();
            }

            OrdenarJogadores(IdJogadorSmallBlind);

            //Queimar Carta
            Baralho.DistribuirCarta();

            //Adicionar cartas na mesa
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

        /// <summary>
        /// Método utilizado apenas em classes de teste para realizar a simulação de uma jogada executada
        /// </summary>
        /// <param name="jogada">nome da jogada (pre, flop, turn, river)</param>
        /// <param name="status">valor booleano do status da jogada</param>
        internal void AlterarExecucaoDeJogadas(string jogada, bool status)
        {
            switch (jogada)
            {
                case "pre":
                    PreFlopExecutado = status;
                    break;
                case "flop":
                    FlopExecutado = status;
                    break;
                case "turn":
                    TurnExecutado = status;
                    break;
                case "river":
                    RiverExecutado = status;
                    break;
                default:
                    break;
            }
        }
    }
}