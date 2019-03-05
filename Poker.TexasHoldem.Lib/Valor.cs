using Poker.TexasHoldem.Lib._Base;
using System;

namespace Poker.TexasHoldem.Lib
{
    public class Valor
    {
        public string Id { get; private set; }
        public string Nome { get; private set; }
        public string Plural { get; private set; }
        public int Peso { get; private set; }
        public string PesoTexto
        {
            get
            {
                return Peso.ToString().PadLeft(2, '0');
            }
        }

        /// <summary>
        /// Inicia uma instância de valor.
        /// </summary>
        /// <param name="id">Id do valor</param>
        public Valor(string id)
        {
            var deParaValor = new DeParaValor(id);

            if (deParaValor.Id == null)
                throw new Exception(Ressource.CartaValorInvalido);

            Id = deParaValor.Id;
            Nome = deParaValor.Nome;
            Plural = deParaValor.Plural;
            Peso = deParaValor.Peso;
        }

        /// <summary>
        /// Troca o peso do Ás
        /// </summary>
        public void TrocarPeso()
        {
            if (Id == "A")
                Peso = Peso == Ressource.ValorAsAltoPeso ? Ressource.ValorAsBaixoPeso : Ressource.ValorAsAltoPeso;
        }

        internal class DeParaValor
        {
            public string Id { get; private set; }
            public string Nome { get; private set; }
            public string Plural { get; private set; }
            public int Peso { get; private set; }

            /// <summary>
            /// Inicia um objeto do De/Para de Valor.
            /// </summary>
            /// <param name="id">Id do valor.</param>
            public DeParaValor(string id)
            {
                switch (id)
                {
                    case "2":
                        Id = id;
                        Nome = Ressource.ValorDoisNome;
                        Plural = Ressource.ValorDoisNome;
                        Peso = Ressource.ValorDoisPeso;
                        break;
                    case "3":
                        Id = id;
                        Nome = Ressource.ValorTresNome;
                        Plural = Ressource.ValorTresNome;
                        Peso = Ressource.ValorTresPeso;
                        break;
                    case "4":
                        Id = id;
                        Nome = Ressource.ValorQuatroNome;
                        Plural = Ressource.ValorQuatroPlural;
                        Peso = Ressource.ValorQuatroPeso;
                        break;
                    case "5":
                        Id = id;
                        Nome = Ressource.ValorCincoNome;
                        Plural = Ressource.ValorCincoPlural;
                        Peso = Ressource.ValorCincoPeso;
                        break;
                    case "6":
                        Id = id;
                        Nome = Ressource.ValorSeisNome;
                        Plural = Ressource.ValorSeisNome;
                        Peso = Ressource.ValorSeisPeso;
                        break;
                    case "7":
                        Id = id;
                        Nome = Ressource.ValorSeteNome;
                        Plural = Ressource.ValorSetePlural;
                        Peso = Ressource.ValorSetePeso;
                        break;
                    case "8":
                        Id = id;
                        Nome = Ressource.ValorOitoNome;
                        Plural = Ressource.ValorOitoPlural;
                        Peso = Ressource.ValorOitoPeso;
                        break;
                    case "9":
                        Id = id;
                        Nome = Ressource.ValorNoveNome;
                        Plural = Ressource.ValorNovePlural;
                        Peso = Ressource.ValorNovePeso;
                        break;
                    case "10":
                        Id = id;
                        Nome = Ressource.ValorDezNome;
                        Plural = Ressource.ValorDezNome;
                        Peso = Ressource.ValorDezPeso;
                        break;
                    case "J":
                        Id = id;
                        Nome = Ressource.ValorValeteNome;
                        Plural = Ressource.ValorValetePlural;
                        Peso = Ressource.ValorValetePeso;
                        break;
                    case "Q":
                        Id = id;
                        Nome = Ressource.ValorDamaNome;
                        Plural = Ressource.ValorDamaPlural;
                        Peso = Ressource.ValorDamaPeso;
                        break;
                    case "K":
                        Id = id;
                        Nome = Ressource.ValorReiNome;
                        Plural = Ressource.ValorReiPlural;
                        Peso = Ressource.ValorReiPeso;
                        break;
                    case "A":
                        Id = id;
                        Nome = Ressource.ValorAsNome;
                        Plural = Ressource.ValorAsPlural;
                        Peso = Ressource.ValorAsAltoPeso;
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
