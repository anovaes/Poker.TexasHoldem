using Poker.TexasHoldem.Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Poker.TexasHoldem.Test._Builder
{
    public class MesaBuilder
    {
        private int[] _quantidadeJogadoresPorMesa;
        private bool _deveIniciarPartida;
        private bool _deveIniciarMao;

        /// <summary>
        /// Cria uma intância da MesaBuilder. (Exclusivo para uso de testes unitários)
        /// </summary>
        public MesaBuilder()
        {

        }

        /// <summary>
        /// Inicia instância
        /// </summary>
        /// <returns>retorna instância MesaBuilder</returns>
        public static MesaBuilder Novo()
        {
            return new MesaBuilder();
        }

        /// <summary>
        /// Indica quantidade de jogadores por mesa
        /// </summary>
        /// <param name="qtdJogadoresPorMesa">Array com a quantidade de jogadores por mesa. Cada item do array é uma nova mesa que será gerada</param>
        /// <returns>Instância atual</returns>
        public MesaBuilder JogadoresPorMesa(params int[] quantidadeJogadoresPorMesa)
        {
            _quantidadeJogadoresPorMesa = quantidadeJogadoresPorMesa;
            return this;
        }

        /// <summary>
        /// Indica se deve ativar método IniciarPartida
        /// </summary>
        /// <returns>Instância atual</returns>
        public MesaBuilder DeveIniciarPartida()
        {
            _deveIniciarPartida = true;
            return this;
        }

        /// <summary>
        /// Indica se deve ativar método Iniciar Mao
        /// </summary>
        /// <returns>Instância atual</returns>
        public MesaBuilder DeveIniciarMao()
        {
            _deveIniciarMao = true;
            return this;
        }

        /// <summary>
        /// Monta a lista de mesas com base nas informações anteriores
        /// </summary>
        /// <returns>Lista de mesas geradas</returns>
        public List<Mesa> ObterListaDeMesas()
        {
            var mesas = new List<Mesa>();
            var idMesa = 1;

            foreach (var quantidade in _quantidadeJogadoresPorMesa)
            {
                if (quantidade < 0 || quantidade > 9)
                    continue;

                Mesa mesa = new Mesa(idMesa);

                for (int i = 1; i <= quantidade; i++)
                {
                    Jogador jogador = new Jogador(i, $"jogador{i}");
                    mesa.Jogadores.Add(jogador);
                }
                mesas.Add(mesa);

                idMesa++;
            }

            foreach (var mesa in mesas)
            {
                if (_deveIniciarPartida)
                    mesa.IniciarPartida();

                if (_deveIniciarMao)
                    mesa.IniciarMao();
            }

            return mesas;
        }

        /// <summary>
        /// Obtém a primeira mesa da lista
        /// </summary>
        /// <returns>Mesa gerada</returns>
        public Mesa ObterPrimeiraMesa()
        {
            return ObterListaDeMesas().FirstOrDefault();
        }
    }
}
