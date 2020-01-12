using Poker.TexasHoldem.Lib._Base;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Poker.TexasHoldem.Lib
{
    public class Pote
    {
        

        public int Id { get; private set; }
        public int Fichas { get; private set; }
        public int FichasMinimaDoPote { get; private set; }
        public List<int> JogadoresNoPote { get; private set; }
        public bool Aberto { get; private set; }

        public Pote(int id)
        {
            Id = id;
            Aberto = true;
            JogadoresNoPote = new List<int>();
        }

        public void Fechar()
        {
            Aberto = false;
        }

        /// <summary>
        /// Adiciona fichas ao pote de um determinado jogador
        /// </summary>
        /// <param name="idJogador">Id do jogador</param>
        /// <param name="fichas">Quantidade de fichas do jogador</param>
        internal void AdicionarFichas(int idJogador, int fichas)
        {
            if (!JogadoresNoPote.Any(j => j == idJogador))
                JogadoresNoPote.Add(idJogador);

            FichasMinimaDoPote = fichas;
            Fichas += fichas;
        }

        internal int RetirarFichasDoPote(decimal porcentagem)
        {
            var fichasRetiradas = (int)Math.Floor(Fichas * porcentagem);
            Fichas -= fichasRetiradas;
            return fichasRetiradas;
        }
    }
}
