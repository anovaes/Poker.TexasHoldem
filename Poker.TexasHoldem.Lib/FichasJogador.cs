using Poker.TexasHoldem.Lib._Base;
using System;

namespace Poker.TexasHoldem.Lib
{
    public class FichasJogador
    {
        public int IdJogador { get; private set; }
        public int Fichas { get; private set; }

        /// <summary>
        /// Cria instância da classe FichasJogador
        /// </summary>
        /// <param name="idJogador">Id do jogador</param>
        /// <param name="fichas">Quantidade de fichas do jogador</param>
        public FichasJogador(int idJogador, int fichas)
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
