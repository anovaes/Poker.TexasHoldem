﻿using Poker.TexasHoldem.Lib;
using System.Collections.Generic;

namespace Poker.TexasHoldem.Test._Builder
{
    internal class CartaBuilder
    {
        public List<Carta> CartasJogador { get; private set; }
        public List<Carta> CartasMesa { get; private set; }

        /// <summary>
        /// Inicia uma instância da CartaBuilder. Apenas preenche as cartas do jogador
        /// </summary>
        /// <param name="cartasJogador">Sequência de cartas do jogador separadas por pipe (|)</param>
        public CartaBuilder(string cartasJogador)
        {
            var arrCartasJogador = cartasJogador.Split("|");
            CartasJogador = new List<Carta>();

            foreach (var carta in arrCartasJogador)
            {
                CartasJogador.Add(new Carta(carta));
            }
        }

        /// <summary>
        /// Inicia uma instância de CartaBuilder
        /// </summary>
        /// <param name="cartasJogador">Sequência de cartas do jogador separadas por pipe (|)</param>
        /// <param name="cartasMesa">Sequência de cartas da mesa separadas por pipe (|)</param>
        public CartaBuilder(string cartasJogador, string cartasMesa)
        {
            var arrCartasJogador = cartasJogador.Split("|");
            var arrCartasMesa = cartasMesa.Split("|");

            CartasJogador = new List<Carta>();
            CartasMesa = new List<Carta>();

            foreach (var carta in arrCartasJogador)
            {
                CartasJogador.Add(new Carta(carta));
            }

            foreach (var carta in arrCartasMesa)
            {
                CartasMesa.Add(new Carta(carta));
            }
        }
    }
}
