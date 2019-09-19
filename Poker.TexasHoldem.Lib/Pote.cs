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
    }
}
