using Poker.TexasHoldem.Lib._Base;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Poker.TexasHoldem.Lib
{
    public class Baralho
    {
        public List<Carta> Cartas { get; private set; }

        /// <summary>
        /// Inicia uma instância de baralho
        /// </summary>
        public Baralho()
        {
            Embaralhar(Ressource.BaralhoIdValores, Ressource.BaralhoIdNaipes);
        }

        /// <summary>
        ///     Inicia uma instância de baralho. Construtor utilizado apenas para teste
        /// </summary>
        /// <param name="valores">Todos os valores separados por ponto e vírgula (;)</param>
        /// <param name="naipes">Todos os naipes separados por ponto e vírgula (;)</param>
        internal Baralho(string valores, string naipes)
        {
            Embaralhar(valores, naipes);
        }

        /// <summary>
        /// Realiza o embaralhamento das cartas
        /// </summary>
        /// <param name="valores">Todos os valores separados por ponto e vírgula (;)</param>
        /// <param name="naipes">Todos os naipes separados por ponto e vírgula (;)</param>
        private void Embaralhar(string valores, string naipes)
        {
            var arrCartas = new Carta[52];
            var listNumerosRandom = new List<int>();
            var random = new Random();
            var arrValores = (valores ?? "").Split(";");
            var arrNaipes = (naipes ?? "").Split(";");

            if (arrValores.Length != 13)
                throw new Exception(Ressource.BaralhoMsgQuantidadeDeValoresInvalido);

            if (arrNaipes.Length != 4)
                throw new Exception(Ressource.BaralhoMsgQuantidadeDeNaipesInvalido);

            if (arrValores.GroupBy(valor => valor).Where(grupo => grupo.Count() > 1).Count() > 0)
                throw new Exception(Ressource.BaralhoMsgContemIdsDeValoresDuplicados);

            if (arrNaipes.GroupBy(valor => valor).Where(grupo => grupo.Count() > 1).Count() > 0)
                throw new Exception(Ressource.BaralhoMsgContemIdsDeNaipesDuplicados);

            foreach (var valor in arrValores)
            {
                foreach (var naipe in arrNaipes)
                {
                    var numRandom = random.Next(0, 51);

                    while (listNumerosRandom.Where(num => num == numRandom).Any())
                    {
                        numRandom++;
                        if (numRandom > 51)
                            numRandom = 0;
                    }

                    arrCartas[numRandom] = new Carta($"{valor};{naipe}");

                    listNumerosRandom.Add(numRandom);
                }
            }

            Cartas = arrCartas.ToList();
        }

        /// <summary>
        /// Retira a primeira carta do baralho e a retorna.
        /// </summary>
        /// <returns>Retorna a primeira carta do baralho</returns>
        public Carta DistribuirCarta()
        {
            var carta = Cartas.First();
            Cartas.Remove(carta);
            return carta;
        }
    }
}
