using Poker.TexasHoldem.Lib;
using System;
using System.Collections.Generic;
using System.Text;

namespace Poker.TexasHoldem.Test._Builder
{
    public class MaoBuilder
    {
        public Mao Mao { get; private set; }

        /// <summary>
        /// Inicia uma instância da MaoBuilder. Apenas preenche as cartas do jogador
        /// </summary>
        /// <param name="cartasJogador">Sequência de cartas do jogador separadas por pipe (|)</param>
        public MaoBuilder(string cartasJogador)
        {
            var arrCartasJogador = cartasJogador.Split("|");
            var cartas = new List<Carta>();

            foreach (var carta in arrCartasJogador)
            {
                cartas.Add(new Carta(carta));
            }

            Mao = new Mao(cartas[0], cartas[1]);
        }
    }
}
