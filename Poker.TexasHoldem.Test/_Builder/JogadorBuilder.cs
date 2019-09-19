using Poker.TexasHoldem.Lib;
using Poker.TexasHoldem.Lib._Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace Poker.TexasHoldem.Test._Builder
{
    public class JogadorBuilder
    {
        private int _quantidadeJogadores;
        private int[] _apostas;
        private StatusJogador[] _statusJogadores;

        /// <summary>
        /// Deve criar instância da JogadorBuilder. (Exclusivo para uso de testes unitários)
        /// </summary>
        public JogadorBuilder()
        {

        }

        /// <summary>
        /// Iniciar instância
        /// </summary>
        /// <returns>retorna instância JogadorBuilder</returns>
        public JogadorBuilder Novo()
        {
            return new JogadorBuilder();
        }

        /// <summary>
        /// Indica a quantidade de jogadores que serão criados
        /// </summary>
        /// <param name="quantidadeDeJogadores">quantidade de jogadores</param>
        /// <returns>retorna instância JogadorBuilder</returns>
        public JogadorBuilder CriarJogadores(int quantidadeDeJogadores)
        {
            _quantidadeJogadores = quantidadeDeJogadores;
            return this;
        }

        /// <summary>
        /// Informa as apostas dos jogadores através de um array de inteiro. Cada item do array significa a aposta para um jogador diferente.
        /// Caso seja informado mais apostas do que a quantidade de jogadores, as apostas excedentes serão ignoradas.
        /// Caso seja informado menos apostas do que a quantidade de jogadores, os jogadores com a aposta faltando ficarão zerados.
        /// </summary>
        /// <param name="apostas">Array contendo as apostas dos jogadores cadastrados na mesa</param>
        /// <returns>retorna instância JogadorBuilder</returns>
        public JogadorBuilder AdicionarApostas(int[] apostas)
        {
            _apostas = apostas;
            return this;
        }

        /// <summary>
        /// Informa os status dos jogadores através de um array de status. Cada item do array significa o status de cada jogador.
        /// Caso seja informado mais status do que a quantidade de jogadores, os status excedentes serão ignorados.
        /// Caso seja informado menos apostas do que a quantidade de jogadores, os jogadores com status faltando ficarão com status Ativo.
        /// </summary>
        /// <param name="statusJogadores"></param>
        /// <returns></returns>
        public JogadorBuilder IndicarStatus(StatusJogador[] statusJogadores)
        {
            _statusJogadores = statusJogadores;
            return this;
        }

        /// <summary>
        /// Monta a lista de jogadores com base nas informações anteriores
        /// </summary>
        /// <returns></returns>
        public List<Jogador> ObterJogadores()
        {
            var jogadores = new List<Jogador>();

            for (int i = 0; i < _quantidadeJogadores; i++)
            {
                var id = i + 1;
                var jogador = new Jogador(id, $"jogador{id}");
                var apostaAtual = _apostas?.Length > i ? _apostas[i] : 0;
                var statusAtual = _statusJogadores?.Length > i ? _statusJogadores[i] : StatusJogador.Ativo;

                jogador.AlterarValorFichasApostadasNaRodada(apostaAtual);
                jogador.TrocarStatus(statusAtual);

                jogadores.Add(jogador);
            }

            return jogadores;
        }
    }
}
