using Poker.TexasHoldem.Lib._Base;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Poker.TexasHoldem.Lib
{
    public class Pote
    {
        private List<FichasJogador> _fichasJogadores;

        public int Id { get; private set; }
        public int Fichas
        {
            get
            {
                return _fichasJogadores.Sum(fj => fj.Fichas);
            }
        }
        public bool Aberto { get; private set; }

        public Pote(int id)
        {
            Id = id;
            _fichasJogadores = new List<FichasJogador>();
        }

        /// <summary>
        /// Inicia a instância do Pote
        /// </summary>
        /// <param name="id">Identificador do Pote</param>
        /// <param name="fichasJogadores">Lista de jogadores e suas respectivas fichas vinculados ao pote</param>
        public Pote(int id, List<FichasJogador> fichasJogadores)
        {
            if (fichasJogadores == null || !fichasJogadores.Any())
                throw new Exception(Ressource.PoteJogadoresNaoInformados);

            _fichasJogadores = fichasJogadores;

            Id = id;
            Aberto = true;
        }

        public void Fechar()
        {
            Aberto = false;
        }

        /// <summary>
        /// Pesquisar a quantidade de fichas do jogador no pote
        /// </summary>
        /// <param name="idJogadorPesquisado">Id do jogador a ser pesquisado</param>
        /// <returns>Fichas do jogador pesquisado</returns>
        internal int PesquisarFichasDoJogador(int idJogador)
        {
            return _fichasJogadores.Where(fj => fj.IdJogador == idJogador).FirstOrDefault().Fichas;
        }

        internal void AdicionarFichas(int idJogador, int fichas)
        {
            var fichasJogador = _fichasJogadores.Where(fj => fj.IdJogador == idJogador).FirstOrDefault();

            if (fichasJogador == null)
                _fichasJogadores.Add(new FichasJogador(idJogador, fichas));
            else
                fichasJogador.AdicionarFichas(fichas);
        }
    }
}
