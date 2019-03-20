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
        /// <param name="listaMesaJogador"></param>
        public MesaBuilder(List<MesaJogador> listaMesaJogador)
        {
            Mesas = new List<Mesa>();

            foreach (var mesaJogador in listaMesaJogador)
            {
                if (mesaJogador.IdMesa == 0 || mesaJogador.QuantidadeJogadores < 0 || mesaJogador.QuantidadeJogadores > 9)
                    continue;

                Mesa mesa = new Mesa(mesaJogador.IdMesa);

                for (int i = 1; i <= mesaJogador.QuantidadeJogadores; i++)
                {
                    Jogador jogador = new Jogador(i, $"jogador{i}");
                    mesa.Jogadores.Add(jogador);
                }
                Mesas.Add(mesa);
            }
        }
    }

    public class MesaJogador
    {
        public int IdMesa { get; private set; }
        public int QuantidadeJogadores { get; private set; }

        /// <summary>
        /// Cria uma instância de relação id da mesa com a quantidade de jogadores da mesa
        /// </summary>
        public MesaJogador(int idMesa, int quantidadeJogadores)
        {
            IdMesa = idMesa;
            QuantidadeJogadores = quantidadeJogadores;
        }
    }
}
