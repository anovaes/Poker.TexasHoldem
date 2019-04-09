using Poker.TexasHoldem.Lib;
using System;
using System.Collections.Generic;
using System.Text;

namespace Poker.TexasHoldem.Test._Builder
{
    public class MesaBuilder
    {
        public List<Mesa> Mesas { get; private set; }

        /// <summary>
        /// Cria uma intância da MesaBuilder para uso dos testes unitários
        /// </summary>
        /// <param name="qtdJogadoresPorMesa"></param>
        public MesaBuilder(params int[] qtdJogadoresPorMesa)
        {
            Mesas = new List<Mesa>();
            var idMesa = 1;

            foreach (var quantidade in qtdJogadoresPorMesa)
            {
                if (quantidade < 0 || quantidade > 9)
                    continue;

                Mesa mesa = new Mesa(idMesa);

                for (int i = 1; i <= quantidade; i++)
                {
                    Jogador jogador = new Jogador(i, $"jogador{i}");
                    mesa.Jogadores.Add(jogador);
                }
                Mesas.Add(mesa);

                idMesa++;
            }
        }
    }
}
