using ExpectedObjects;
using Poker.TexasHoldem.Lib;
using Poker.TexasHoldem.Lib._Base;
using Poker.TexasHoldem.Lib._Enum;
using Poker.TexasHoldem.Lib._Util;
using Poker.TexasHoldem.Test._Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Poker.TexasHoldem.Test
{
    public class MesaPosTeste
    {
        private readonly int _quantidadeMinimaDeJogadoresPermitidos = 2;
        private readonly int _quantidadeMaximaDeJogadoresPermitidos = 9;
        private readonly int _idMesaDefault = 1;
        private readonly string _nomeJogadorDefault = "Alexandre";
        private int _valorSmallBlindEsperado;
        private int _valorBigBlindEsperado;

        public MesaPosTeste()
        {
            _valorSmallBlindEsperado = Ressource.MesaValorInicialBlind / 2;
            _valorBigBlindEsperado = Ressource.MesaValorInicialBlind;
        }
    }
}