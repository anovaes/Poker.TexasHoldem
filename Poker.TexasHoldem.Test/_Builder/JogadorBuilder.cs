using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities.Resources;
using Poker.TexasHoldem.Lib;
using Poker.TexasHoldem.Lib._Base;
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
        private int[] _pontuacoes;
        private int[] _montantesDeFichas;

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
        /// Caso seja informado menos status do que a quantidade de jogadores, os jogadores com status faltando ficarão com status Ativo.
        /// </summary>
        /// <param name="statusJogadores"></param>
        /// <returns></returns>
        public JogadorBuilder IndicarStatus(StatusJogador[] statusJogadores)
        {
            _statusJogadores = statusJogadores;
            return this;
        }

        /// <summary>
        /// Informa a pontuação dos jogadores através de um array de pontuacoes. Cada item do array significa a pontuação de cada jogador.
        /// Caso seja informado mais pontuações do que a quantidade de jogadores, as pontuações excedentes serão ignoradas.
        /// Caso seja informado menos pontuações do que a quantidade de jogadores, os jogadores com pontuações faltando ficarão com pontuação zerada.
        /// </summary>
        /// <param name="statusJogadores"></param>
        /// <returns></returns>
        public JogadorBuilder AdicionarPontuacoes(int[] pontuacoes)
        {
            _pontuacoes = pontuacoes;
            return this;
        }

        /// <summary>
        /// Informa as fichas dos jogadores através de um array de fichas. Cada item do array significa as fichas de cada jogador.
        /// Caso seja informado mais montantes de fichas do que a quantidade de jogadores, os montantes excedentes serão ignoradas.
        /// Caso seja informado menos montantes de fichas do que a quantidade de jogadores, os jogadores com montantes de fichas faltando ficarão com as fichas default.
        /// </summary>
        /// <param name="statusJogadores"></param>
        /// <returns></returns>
        public JogadorBuilder AdicionarFichas(int[] montantesDeFichas)
        {
            _montantesDeFichas = montantesDeFichas;
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
                var pontuacaoAtual = _pontuacoes?.Length > i ? _pontuacoes[i] : 0;
                var fichas = _montantesDeFichas?.Length > i ? _montantesDeFichas[i] : Ressource.JogadorFichasInicial;

                jogador.AlterarValorFichasApostadasNaRodada(apostaAtual);
                jogador.TrocarStatus(statusAtual);
                jogador.AtribuirFichas(fichas);

                if (_pontuacoes != null)
                    jogador.AtribuirPontuacao(pontuacaoAtual);

                jogadores.Add(jogador);
            }

            return jogadores;
        }
    }
}
