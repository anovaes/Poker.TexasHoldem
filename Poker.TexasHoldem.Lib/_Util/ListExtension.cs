using System.Collections.Generic;
using System.Linq;

namespace Poker.TexasHoldem.Lib._Util
{
    public static class ListExtension
    {
        /// <summary>
        /// Move o primeiro item da lista para o final
        /// </summary>
        /// <typeparam name="T">Tipo de objeto genérico</typeparam>
        /// <param name="list">lista de objetos</param>
        public static void MoveFirstItemToFinal<T>(this List<T> list)
        {
            if (list.Any())
            {
                var item = list.First();
                list.Remove(item);
                list.Add(item);
            }
        }
    }
}
