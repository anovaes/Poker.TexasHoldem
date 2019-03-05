using System;
using System.Collections.Generic;
using System.Text;

namespace Poker.TexasHoldem.Test._Util
{
    public static class Texto
    {
        /// <summary>
        /// Repete o texto na quantidade de vezes que foi informado
        /// </summary>
        /// <param name="texto">Texto a ser repetido</param>
        /// <param name="quantidade">Quantidade de repetições</param>
        /// <returns>Retorna o texto com as devidas repetições. Caso o texto fornecido seja nulo, será retornado string.Empty</returns>
        public static string Repeat(string texto, int quantidade)
        {
            return texto != null ? new StringBuilder().Insert(0, texto, quantidade).ToString() : string.Empty;
        }
    }
}
