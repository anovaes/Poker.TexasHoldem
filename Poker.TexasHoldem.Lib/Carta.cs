using System;
using System.Collections.Generic;
using System.Text;

namespace Poker.TexasHoldem.Lib
{
    public class Carta
    {
        public string Id { get; private set; }
        public Valor Valor { get; private set; }
        public Naipe Naipe { get; private set; }

        /// <summary>
        /// Inicia uma instância de carta.
        /// </summary>
        /// <param name="idCarta">Id da carta.</param>
        public Carta(string idCarta)
        {
            var arrayCarta = idCarta?.Split(';');

            if (!ValidaFormato(arrayCarta))
                throw new Exception("O formato da carta não é válido.");

            Id = idCarta;
            Valor = new Valor(arrayCarta[0].ToUpper());
            Naipe = new Naipe(arrayCarta[1].ToUpper());

        }

        /// <summary>
        /// Verifica se o formato da carta é válido
        /// </summary>
        /// <param name="arrayCarta">Array contendo o id do valor e o id do naipe</param>
        /// <returns>Se o formato da carta for válido retorna true, caso contrário falso</returns>
        private static bool ValidaFormato(string[] arrayCarta)
        {
            return arrayCarta?.Length == 2 && !string.IsNullOrWhiteSpace(arrayCarta[0]) && !string.IsNullOrWhiteSpace(arrayCarta[1]);
        }
    }
}
