using Poker.TexasHoldem.Lib._Base;
using System;

namespace Poker.TexasHoldem.Lib
{
    public class Naipe
    {
        public string Id { get; private set; }
        public string Nome { get; private set; }
        public string Simbolo { get; private set; }

        /// <summary>
        /// Inicia uma instância de naipe
        /// </summary>
        /// <param name="id">Id do Naipe</param>
        public Naipe(string id)
        {
            var deParaNaipe = new DeParaNaipe(id);

            if (deParaNaipe.Id == null)
                throw new Exception(Ressource.CartaNaipeInvalido);

            Id = deParaNaipe.Id;
            Nome = deParaNaipe.Nome;
            Simbolo = deParaNaipe.Simbolo;
        }

        internal class DeParaNaipe
        {
            public string Id { get; private set; }
            public string Nome { get; private set; }
            public string Simbolo { get; private set; }

            /// <summary>
            /// Inicia uma instância de De/Para de Naipe.
            /// </summary>
            /// <param name="id">Id do valor.</param>
            public DeParaNaipe(string id)
            {
                switch (id)
                {
                    case "P":
                        Id = id;
                        Nome = Ressource.NaipePausNome;
                        Simbolo = Ressource.NaipePausSimbolo;
                        break;
                    case "C":
                        Id = id;
                        Nome = Ressource.NaipeCopasNome;
                        Simbolo = Ressource.NaipeCopasSimbolo;
                        break;
                    case "E":
                        Id = id;
                        Nome = Ressource.NaipeEspadasNome;
                        Simbolo = Ressource.NaipeEspadasSimbolo;
                        break;
                    case "O":
                        Id = id;
                        Nome = Ressource.NaipeOurosNome;
                        Simbolo = Ressource.NaipeOurosSimbolo;
                        break;
                }
            }
        }
    }
}
