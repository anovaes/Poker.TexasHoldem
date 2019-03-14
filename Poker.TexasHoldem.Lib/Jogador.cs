using Poker.TexasHoldem.Lib._Base;
using Poker.TexasHoldem.Lib._Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace Poker.TexasHoldem.Lib
{
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
