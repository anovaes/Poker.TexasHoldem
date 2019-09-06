using Poker.TexasHoldem.Lib._Base;
using System;

namespace Poker.TexasHoldem.Lib
{
    public class JogadorFicha
    {
        public int IdJogador { get; private set; }
        public int Fichas { get; private set; }

        public JogadorFicha(int idJogador, int fichas)
        {
            if (idJogador <= 0)
                throw new Exception(Ressource.JogadorFichaIdInvalido);

            if (fichas <= 0)
                throw new Exception(Ressource.JogadorFichaFichasInvalidas);

            IdJogador = idJogador;
            Fichas = fichas;
        }
    }
}
